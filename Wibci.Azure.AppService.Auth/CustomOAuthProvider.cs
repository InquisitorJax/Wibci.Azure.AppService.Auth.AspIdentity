using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Wibci.Azure.AppService.Auth.DataAccess;
using Wibci.Azure.AppService.Auth.DataObjects;
using Wibci.Azure.AppService.Auth.Util;

namespace Wibci.Azure.AppService.Auth
{
    public class CustomOAuthProvider : OAuthAuthorizationServerProvider
    {
        private const string userIdKey = "as:client_id";

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary[userIdKey];
            var currentClient = context.OwinContext.Get<string>(userIdKey);

            // enforce client binding of refresh token
            if (originalClient != currentClient)
            {
                context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
                return Task.FromResult(default(int));
            }

            // chance to change authentication ticket for refresh token requests
            var newId = new ClaimsIdentity(context.Ticket.Identity);
            newId.AddClaim(new Claim("newClaim", "newValue"));

            var newTicket = new AuthenticationTicket(newId, context.Ticket.Properties);
            context.Validated(newTicket);
            return Task.FromResult(default(int));
            //fter this method executes successfully, the flow for the code will hit method “CreateAsync”
            //in class “CustomRefreshTokenProvider” and a new refresh token is generated and returned in the response along with the new access token.
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //ammended to support refresh tokens
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");

            if (allowedOrigin == null) allowedOrigin = "*";

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            var userManager = context.OwinContext.GetUserManager<AppUserManager>();

            ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "User name or password is incorrect.");
                return;
            }

            if (!user.EmailConfirmed)
            {
                context.SetError("invalid_grant", "Email address is not confirmed.");
                return;
            }

            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, "JWT");

            //add the user Id to the response
            var props = new AuthenticationProperties(new Dictionary<string, string>
            {
                 {
                     "as:client_id", context.ClientId ?? string.Empty
                 },
                 {
                     "userName", context.UserName
                 },
                 {
                     "guid", user.Id
                 }
            });

            var ticket = new AuthenticationTicket(oAuthIdentity, props);

            context.Validated(ticket);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            //NOTE: part of refresh token implementation

            string clientId = string.Empty;
            string clientSecret = string.Empty;
            AuthApiClient client = null;

            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (context.ClientId == null)
            {
                //Remove the comments from the below line context.SetError, and invalidate context
                //if you want to force sending clientId/secrets once obtain access tokens.
                context.Validated();
                //context.SetError("invalid_clientId", "ClientId should be sent.");
                return Task.FromResult<object>(null);
            }

            using (IAuthDataAccess repo = CC.IoC.Resolve<IAuthDataAccess>())
            {
                client = repo.FindApiClient(context.ClientId);
            }

            if (client == null)
            {
                context.SetError("invalid_clientId", $"Client {context.ClientId} is not registered in the system.");
                return Task.FromResult<object>(null);
            }

            if (client.ApplicationType == AuthApplicationTypes.NativeConfidential)
            {
                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    context.SetError("invalid_clientId", "Client secret should be sent.");
                    return Task.FromResult<object>(null);
                }
                else
                {
                    if (client.Secret != CryptoHelper.Hash(clientSecret))
                    {
                        context.SetError("invalid_clientId", "Client secret is invalid.");
                        return Task.FromResult<object>(null);
                    }
                }
            }

            if (!client.Active)
            {
                context.SetError("invalid_clientId", "Client is inactive.");
                return Task.FromResult<object>(null);
            }

            context.OwinContext.Set<string>("as:client_id", clientId); //make client_id available for later security checks
            context.OwinContext.Set<string>("as:clientAllowedOrigin", client.AllowedOrigin);
            context.OwinContext.Set<string>("as:clientRefreshTokenLifeTime", client.RefreshTokenLifeTime.ToString());

            context.Validated();
            return Task.FromResult<object>(null);
        }
    }
}
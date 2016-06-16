using Microsoft.Azure.Mobile.Server.Login;
using Microsoft.Owin.Security;
using System;
using System.Configuration;
using System.IdentityModel.Tokens;

namespace Wibci.Azure.AppService.Auth
{
    public class CustomZumoTokenFormat : ISecureDataFormat<AuthenticationTicket>
    {
        public string Protect(AuthenticationTicket data)
        {
            //original implementation here: http://bitoftech.net/2015/02/16/implement-oauth-json-web-tokens-authentication-in-asp-net-web-api-and-identity-2/
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            string audience = GetHost();
            string issuer = GetHost();

            string appSignKey = GetSigningKey();

            JwtSecurityToken token = AppServiceLoginHandler.CreateToken(
                data.Identity.Claims,
                appSignKey,
                audience,
                issuer,
                TimeSpan.FromHours(24)
                );

            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.WriteToken(token);

            return jwt;
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }

        private static string GetHost()
        {
            const string hostVariableName = "%WEBSITE_SITE_NAME%";
            string hostKey = Environment.ExpandEnvironmentVariables(hostVariableName);
            string host = string.Format("https://{0}.azurewebsites.net/", hostKey.ToLower());

            if (string.IsNullOrWhiteSpace(hostKey) || hostKey == hostVariableName)
                host = ConfigurationManager.AppSettings["ValidAudience"];

            return host;
        }

        private static string GetSigningKey()
        {
            const string keyVariable = "WEBSITE_AUTH_SIGNING_KEY";
            string key = Environment.GetEnvironmentVariable(keyVariable);

            if (string.IsNullOrWhiteSpace(key) || key == keyVariable)
                key = ConfigurationManager.AppSettings["SigningKey"];

            return key;
        }
    }
}
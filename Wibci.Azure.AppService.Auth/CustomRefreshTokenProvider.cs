using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Threading.Tasks;
using Wibci.Azure.AppService.Auth.DataAccess;
using Wibci.Azure.AppService.Auth.DataObjects;
using Wibci.Azure.AppService.Auth.Util;
using Wibci.Core;

namespace Wibci.Azure.AppService.Auth
{
    public class CustomRefreshTokenProvider : IAuthenticationTokenProvider
    {
        public void Create(AuthenticationTokenCreateContext context)
        {
            AsyncHelper.RunSync(() => CreateAsync(context));
        }

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var clientid = context.Ticket.Properties.Dictionary["as:client_id"];

            if (string.IsNullOrEmpty(clientid))
            {
                return;
            }

            var refreshTokenId = Guid.NewGuid().ToString("n");

            using (IAuthDataAccess repo = CC.IoC.Resolve<IAuthDataAccess>())
            {
                var refreshTokenLifeTime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime");

                var token = new AuthRefreshToken()
                {
                    Id = CryptoHelper.Hash(refreshTokenId),
                    ClientId = clientid,
                    Subject = context.Ticket.Identity.Name,
                    IssuedUtc = DateTime.UtcNow,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime))
                };

                context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
                context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;

                token.ProtectedTicket = context.SerializeTicket();

                var result = await repo.AddRefreshTokenAsync(token);

                if (result)
                {
                    context.SetToken(refreshTokenId);
                }
            }
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            AsyncHelper.RunSync(() => ReceiveAsync(context));
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            string hashedTokenId = CryptoHelper.Hash(context.Token);

            using (IAuthDataAccess repo = CC.IoC.Resolve<IAuthDataAccess>())
            {
                var refreshToken = await repo.FindRefreshTokenAsync(hashedTokenId);

                if (refreshToken != null)
                {
                    //Get protectedTicket from refreshToken class
                    context.DeserializeTicket(refreshToken.ProtectedTicket);
                    var result = await repo.RemoveRefreshTokenAsync(hashedTokenId);
                }
            }
        }
    }
}
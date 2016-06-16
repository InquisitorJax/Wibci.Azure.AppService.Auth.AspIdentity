using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wibci.Azure.AppService.Auth.DataObjects;

namespace Wibci.Azure.AppService.Auth.DataAccess
{
    public interface IAuthDataAccess : IDisposable
    {
        Task<bool> AddApiClientAsync(AuthApiClient apiClient);

        Task<bool> AddRefreshTokenAsync(AuthRefreshToken token);

        AuthApiClient FindApiClient(string clientId);

        Task<AuthRefreshToken> FindRefreshTokenAsync(string refreshTokenId);

        List<AuthRefreshToken> GetAllRefreshTokens();

        Task<bool> RemoveRefreshTokenAsync(AuthRefreshToken refreshToken);

        Task<bool> RemoveRefreshTokenAsync(string refreshTokenId);
    }

    /// <summary>
    /// from http://bitoftech.net/2014/07/16/enable-oauth-refresh-tokens-angularjs-app-using-asp-net-web-api-2-owin/&safe=active&as_qdr=all
    /// </summary>
    public class AuthDataAccess : EntityDataAccessBase, IAuthDataAccess
    {
        public async Task<bool> AddApiClientAsync(AuthApiClient apiClient)
        {
            AuthApiClient existingClient = Context.ApiClients.FirstOrDefault(x => x.Name == apiClient.Name);

            if (existingClient != null)
            {
                Context.ApiClients.Remove(existingClient);
                await Context.SaveChangesAsync();
            }

            Context.ApiClients.Add(apiClient);
            bool success = await Context.SaveChangesAsync() > 0;

            if (!success)
            {
                //GetLogger().Error($"Could not add ApiClient {apiClient.Name}");
            }

            return success;
        }

        public async Task<bool> AddRefreshTokenAsync(AuthRefreshToken token)
        {
            var existingToken = Context.RefreshTokens.Where(r => r.Subject == token.Subject && r.ClientId == token.ClientId).SingleOrDefault();

            if (existingToken != null)
            {
                var result = await RemoveRefreshTokenAsync(existingToken);
            }

            Context.RefreshTokens.Add(token);

            bool success = await Context.SaveChangesAsync() > 0;

            if (!success)
            {
                //Logger().Error($"Could not add Refresh Token for {token.Subject} {token.ClientId}");
            }

            return success;
        }

        public AuthApiClient FindApiClient(string clientId)
        {
            var client = Context.ApiClients.Find(clientId);

            if (client == null)
            {
                //GetLogger().Info($"Could not find Api Client {clientId}");
            }

            return client;
        }

        public async Task<AuthRefreshToken> FindRefreshTokenAsync(string refreshTokenId)
        {
            var refreshToken = await Context.RefreshTokens.FindAsync(refreshTokenId);

            if (refreshToken == null)
            {
                //GetLogger().Info($"Could not find RefreshToken for id {refreshTokenId}");
            }

            return refreshToken;
        }

        public List<AuthRefreshToken> GetAllRefreshTokens()
        {
            return Context.RefreshTokens.ToList();
        }

        public async Task<bool> RemoveRefreshTokenAsync(string refreshTokenId)
        {
            var refreshToken = await Context.RefreshTokens.FindAsync(refreshTokenId);

            if (refreshToken != null)
            {
                Context.RefreshTokens.Remove(refreshToken);
                return await Context.SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<bool> RemoveRefreshTokenAsync(AuthRefreshToken refreshToken)
        {
            Context.RefreshTokens.Remove(refreshToken);
            return await Context.SaveChangesAsync() > 0;
        }
    }
}
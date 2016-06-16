using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using Wibci.Azure.AppService.Auth.DataObjects;

namespace Wibci.Azure.AppService.Auth
{
    public class AuthIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public AuthIdentityDbContext(string connection) : base(connection)
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<AuthApiClient> ApiClients { get; set; }
        public DbSet<AuthRefreshToken> RefreshTokens { get; set; }
    }
}
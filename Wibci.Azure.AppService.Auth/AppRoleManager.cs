using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System.Data.Entity;

namespace Wibci.Azure.AppService.Auth
{
    public class AppRoleManager : RoleManager<IdentityRole>
    {
        public AppRoleManager(IRoleStore<IdentityRole, string> roleStore) : base(roleStore)
        {
        }

        public static AppRoleManager Create(IdentityFactoryOptions<AppRoleManager> options, IOwinContext context)
        {
            DbContext dbContext = context.Get<AuthIdentityDbContext>();

            var appRoleManager = new AppRoleManager(new RoleStore<IdentityRole>(dbContext));

            return appRoleManager;
        }
    }
}
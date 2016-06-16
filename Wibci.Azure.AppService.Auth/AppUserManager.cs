using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using Wibci.Azure.AppService.Auth.Util;

namespace Wibci.Azure.AppService.Auth
{
    public class AppUserManager : UserManager<ApplicationUser>
    {
        public AppUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static AppUserManager Create(IdentityFactoryOptions<AppUserManager> options, IOwinContext context)
        {
            var appDbContext = context.Get<AuthIdentityDbContext>();
            var appUserManager = new AppUserManager(new UserStore<ApplicationUser>(appDbContext));

            SetupIdentityEmailService(appUserManager, options);

            //Configure validation logic for usernames
            appUserManager.UserValidator = new UserValidator<ApplicationUser>(appUserManager)
            {
                AllowOnlyAlphanumericUserNames = false, //NOTE: uses the email as the user name
                RequireUniqueEmail = true
            };

            //Configure validation logic for passwords
            appUserManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = false,
                RequireLowercase = true,
                //RequireUppercase = true,
            };

            return appUserManager;
        }

        private static void SetupIdentityEmailService(AppUserManager appUserManager, IdentityFactoryOptions<AppUserManager> options)
        {
            appUserManager.EmailService = CC.IoC.Resolve<IIdentityMessageService>();
            //appUserManager.SmsService = new IdentitySmsMessageService();

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                appUserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"))
                {
                    //Code for email confirmation and reset password life time
                    TokenLifespan = TimeSpan.FromHours(6)
                };
            }
        }
    }
}
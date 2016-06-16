using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Wibci.Azure.AppService.Auth
{
    public class ApplicationUser : IdentityUser
    {
        //see: http://bitoftech.net/2015/01/21/asp-net-identity-2-with-asp-net-web-api-2-accounts-management/

        public string FirstName { get; set; }

        public DateTime JoinDate { get; set; }
        public string LastName { get; set; }

        public byte[] ProfilePictureThumb { get; set; }
        public string ProfilePictureUri { get; set; }
        public string Provider { get; set; }

        //ExternalAuthSId? TODO: Add SId of  external auth provider like google

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, UserName));
            userIdentity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, Email));
            userIdentity.AddClaim(new Claim(ClaimTypes.Name, UserName));

            return userIdentity;
        }
    }
}
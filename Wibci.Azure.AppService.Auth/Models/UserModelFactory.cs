using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Routing;

namespace Wibci.Azure.AppService.Auth.Models
{
    public class UserModelFactory
    {
        private AppUserManager _AppUserManager;
        private UrlHelper _UrlHelper;

        public UserModelFactory(HttpRequestMessage request, AppUserManager appUserManager)
        {
            _UrlHelper = new UrlHelper(request);
            _AppUserManager = appUserManager;
        }

        public UserReturnModel Create(ApplicationUser appUser)
        {
            return new UserReturnModel
            {
                Url = _UrlHelper.Link("GetUserById", new { id = appUser.Id }),
                Id = appUser.Id,
                UserName = appUser.UserName,
                FullName = string.Format("{0} {1}", appUser.FirstName, appUser.LastName),
                Email = appUser.Email,
                EmailConfirmed = appUser.EmailConfirmed,
                JoinDate = appUser.JoinDate,
                Roles = _AppUserManager.GetRolesAsync(appUser.Id).Result,
                Claims = _AppUserManager.GetClaimsAsync(appUser.Id).Result
            };
        }
    }

    public class UserReturnModel
    {
        public IList<System.Security.Claims.Claim> Claims { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string FullName { get; set; }
        public string Id { get; set; }
        public DateTime JoinDate { get; set; }
        public int Level { get; set; }
        public IList<string> Roles { get; set; }
        public string Url { get; set; }
        public string UserName { get; set; }
    }
}
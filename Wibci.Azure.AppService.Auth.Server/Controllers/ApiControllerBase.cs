using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Azure.Mobile.Server.Config;
using System.Net.Http;
using System.Web.Http;
using Wibci.Azure.AppService.Auth.Models;
using Wibci.Core;

namespace Wibci.Azure.AppService.Auth.Server.Controllers
{
    [MobileAppController]
    public class ApiControllerBase : ApiController
    {
        private AppRoleManager _appRoleManager = null;
        private AppUserManager _appUserManager = null;
        private UserModelFactory _modelFactory;

        public ApiControllerBase()
        {
        }

        protected AppRoleManager AppRoleManager
        {
            get
            {
                return _appRoleManager = _appRoleManager ?? Request.GetOwinContext().GetUserManager<AppRoleManager>();
            }
        }

        protected AppUserManager AppUserManager
        {
            get
            {
                return _appUserManager = _appUserManager ?? Request.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        protected ApplicationUser CurrentUser
        {
            get { return AppUserManager.FindById(CurrentUserId); }
        }

        protected string CurrentUserId
        {
            get { return User.Identity.GetUserId(); }
        }

        protected UserModelFactory TheModelFactory
        {
            get
            {
                if (_modelFactory == null)
                {
                    _modelFactory = new UserModelFactory(Request, AppUserManager);
                }
                return _modelFactory;
            }
        }

        protected IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    //No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        protected IHttpActionResult GetHttpActionResultError(Notification notification)
        {
            IHttpActionResult retResult = null;
            if (!notification.IsValid())
            {
                foreach (var error in notification.Items)
                {
                    ModelState.AddModelError(error.Reference ?? "", error.Message);
                }

                if (ModelState.IsValid)
                {
                    //No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return retResult;
        }
    }
}
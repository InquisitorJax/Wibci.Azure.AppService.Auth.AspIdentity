using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Wibci.Core;
using Wibci.Core.Commands;

namespace Wibci.Azure.AppService.Auth
{
    public interface IAssignRoleToUserCommand : IAsyncLogicCommand<AssignRoleRequest, AssignRoleResult>
    {
    }

    public class AssignRoleRequest
    {
        public string Role { get; set; }
        public string UserId { get; set; }
    }

    public class AssignRoleResult : CommandResult
    {
    }

    public class AssignRoleToUserCommand : AsyncLogicCommand<AssignRoleRequest, AssignRoleResult>, IAssignRoleToUserCommand
    {
        //see: http://bitoftech.net/2015/03/11/asp-net-identity-2-1-roles-based-authorization-authentication-asp-net-web-api/

        private AppRoleManager _appRoleManager = null;
        private AppUserManager _appUserManager = null;

        protected AppRoleManager AppRoleManager
        {
            get
            {
                return _appRoleManager = _appRoleManager ?? HttpContext.Current.Request.GetOwinContext().GetUserManager<AppRoleManager>();
            }
        }

        protected AppUserManager AppUserManager
        {
            get
            {
                return _appUserManager = _appUserManager ?? HttpContext.Current.Request.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        public override async Task<AssignRoleResult> ExecuteAsync(AssignRoleRequest request)
        {
            AssignRoleResult retResult = new AssignRoleResult();

            var appUser = await AppUserManager.FindByIdAsync(request.UserId);

            if (appUser == null)
            {
                retResult.Notification.Add("Add Role failed. Could not find the requested user");
                return retResult;
            }

            var currentRoles = await AppUserManager.GetRolesAsync(appUser.Id);

            bool systemHasRole = AppRoleManager.Roles.FirstOrDefault(x => x.Name == request.Role) != null;

            if (!systemHasRole)
            {
                retResult.Notification.Add($"Role {request.Role} does not exist in the system");
                return retResult;
            }

            if (!currentRoles.Contains(request.Role))
            {
                IdentityResult addResult = await AppUserManager.AddToRolesAsync(appUser.Id, request.Role);

                if (!addResult.Succeeded)
                {
                    retResult.Notification.Add($"Failed to add user {appUser.UserName} to role {request.Role}");

                    return retResult;
                }
            }
            else
            {
                retResult.Notification.Add(new NotificationItem($"AssingUserRole: User {appUser.Email} already assigned to role {request.Role}"));
            }

            return retResult;
        }
    }
}
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Threading.Tasks;
using System.Web;
using Validation;
using Wibci.Azure.AppService.Auth.Models;
using Wibci.Azure.AppService.Auth.Util;
using Wibci.Core;
using Wibci.Core.Commands;

namespace Wibci.Azure.AppService.Auth.Commands
{
    public interface IRegisterUserCommand : IAsyncLogicCommand<RegisterUserRequest, RegisterUserResult>
    {
    }

    public class RegisterUserCommand : AsyncLogicCommand<RegisterUserRequest, RegisterUserResult>, IRegisterUserCommand
    {
        private readonly IAssignRoleToUserCommand _assignUserToRoleCommand;

        private AppUserManager _appUserManager;

        public RegisterUserCommand(IAssignRoleToUserCommand assignUserToRoleCommand)
        {
            Requires.NotNull(assignUserToRoleCommand, nameof(assignUserToRoleCommand));

            _assignUserToRoleCommand = assignUserToRoleCommand;
        }

        protected AppUserManager AppUserManager
        {
            get
            {
                return _appUserManager = _appUserManager ?? HttpContext.Current.Request.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        public override async Task<RegisterUserResult> ExecuteAsync(RegisterUserRequest request)
        {
            RegisterUserResult retResult = new RegisterUserResult();

            CreateUserRequest userRequest = request.CreateUser;

            //TODO: Validation

            request.Provider = request.Provider ?? "Custom";

            var user = AppUserManager.FindByEmail(request.CreateUser.Email);
            if (user == null)
            {
                user = new ApplicationUser()
                {
                    UserName = userRequest.Email,
                    Email = userRequest.Email,
                    FirstName = userRequest.FirstName,
                    LastName = userRequest.LastName,
                    JoinDate = DateTime.Now.Date,
                    Provider = request.Provider
                };

                retResult.IdentityResult = await AppUserManager.CreateAsync(user, userRequest.Password);
                retResult.Notification = retResult.IdentityResult.AsNotification();
            }
            else
            {
                retResult.Notification.Add(new NotificationItem($"RegisterUser: User {userRequest.Email} already exists", NotificationSeverity.Warning));
            }

            if (retResult.IdentityResult != null && !retResult.IdentityResult.Succeeded)
            {
                return retResult;
            }
            else
            {
                retResult.User = user;

                AssignRoleRequest roleRequest = new AssignRoleRequest { Role = request.CreateUser.RoleName, UserId = user.Id };
                AssignRoleResult roleResult = await _assignUserToRoleCommand.ExecuteAsync(roleRequest);
                retResult.Notification.AddRange(roleResult.Notification);
            }

            if (retResult.IsValid())
            {
                //send confirmation email (TODO: Use Domain Event?)
                retResult.EmailConfirmationCode = await AppUserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            }

            return retResult;
        }
    }

    public class RegisterUserRequest
    {
        public CreateUserRequest CreateUser { get; set; }
        public string Provider { get; set; }
    }

    public class RegisterUserResult : CommandResult
    {
        private IdentityResult _identityResult;

        public string EmailConfirmationCode { get; set; }

        public IdentityResult IdentityResult
        {
            get { return _identityResult; }
            set
            {
                _identityResult = value;
                Notification = _identityResult.AsNotification();
            }
        }

        public ApplicationUser User { get; set; }
    }
}
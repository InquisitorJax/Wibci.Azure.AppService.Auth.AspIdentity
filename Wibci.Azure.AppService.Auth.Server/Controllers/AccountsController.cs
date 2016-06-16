using Microsoft.AspNet.Identity;
using Microsoft.Azure.Mobile.Server.Config;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Wibci.Azure.AppService.Auth.Commands;
using Wibci.Azure.AppService.Auth.Models;
using Wibci.Azure.AppService.Auth.Server.Core;
using Wibci.Azure.AppService.Auth.Util;

namespace Wibci.Azure.AppService.Auth.Server.Controllers
{
    [MobileAppController]
    [RoutePrefix("api/accounts")]
    public class AccountsController : ApiControllerBase
    {
        private IRegisterUserCommand RegisterUserCommand
        {
            get { return CC.IoC.Resolve<IRegisterUserCommand>(); }
        }

        [Route("ChangePassword")]
        [Authorize]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await AppUserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        [HttpGet]
        [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> ConfirmEmail(string userId = "", string code = "")
        {
            //TODO: update to POST, and request password as well, since user could have incorrectly typed email address

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("", "User Id and Code are required");
                return BadRequest(ModelState);
            }

            IdentityResult result = await AppUserManager.ConfirmEmailAsync(userId, code);

            if (result.Succeeded)
            {
                return Ok("You have been registered. Welcome to Holl3r Services!");
            }
            else
            {
                return GetErrorResult(result);
            }
        }

        [Route("user/{id:guid}")]
        [HttpDelete]
        //[Authorize(Roles = UserRoles.AllAdmins)] //TODO: Implement
        public async Task<IHttpActionResult> DeleteUser(string id)
        {
            //TODO: Admin cannot delete registeradmin
            //      System Admin cannot be deleted
            //      Cannot delete users that belong to other service providers

            var appUser = await AppUserManager.FindByIdAsync(id);

            if (appUser != null)
            {
                IdentityResult result = await AppUserManager.DeleteAsync(appUser);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }

                return Ok();
            }

            return NotFound();
        }

        [Route("user/{id:guid}", Name = "GetUserById")]
        //[Authorize(Roles = UserRoles.SystemAdmin)] //TODO: Implement
        public async Task<IHttpActionResult> GetUser(string Id)
        {
            var userId = User.Identity.GetUserId();
            var user = await AppUserManager.FindByIdAsync(Id);

            if (user != null)
            {
                if (user.Id != userId)
                    BadRequest("You cannot get other users information");
                else
                    return Ok(TheModelFactory.Create(user));
            }

            return NotFound();
        }

        [Route("user/{username}")]
        //[Authorize(Roles = UserRoles.SystemAdmin)] //TODO: Implement
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            var user = await this.AppUserManager.FindByNameAsync(username);

            if (user != null)
            {
                return Ok(TheModelFactory.Create(user));
            }

            return NotFound();
        }

        [Route("users")]
        //[Authorize(Roles = UserRoles.SystemAdmin)] //TODO: Implement
        public IHttpActionResult GetUsers()
        {
            return Ok(AppUserManager.Users.ToList().Select(u => this.TheModelFactory.Create(u)));
        }

        [Route("user/{id:guid}")]
        [HttpPut]
        [Authorize]
        public async Task<IHttpActionResult> PutUser(UpdateUserRequestDto request)
        {
            ApplicationUser currentUser = CurrentUser;

            currentUser.FirstName = request.FirstName;
            currentUser.LastName = request.LastName;
            currentUser.ProfilePictureUri = request.ProfileImageUri;

            //TODO: save request.ProfileImage to blob storage

            IdentityResult result = await AppUserManager.UpdateAsync(currentUser);

            if (!result.Succeeded)
            {
                //TODO: Translate errors returned to client
                return GetErrorResult(result);
            }

            Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = CurrentUserId }));

            return Created(locationHeader, TheModelFactory.Create(currentUser));
        }

        [Route("register")]
        [AllowAnonymous]
        [HttpPost]
        [RequireHttps]
        //[ValidateModelState(typeof(CreateUserRequest), "createUserModel")] //TODO: Implement
        public async Task<IHttpActionResult> Register(CreateUserRequest createUserModel)
        {
            RegisterUserRequest registerRequest = new RegisterUserRequest { CreateUser = createUserModel };

            RegisterUserResult registerResult = await RegisterUserCommand.ExecuteAsync(registerRequest);

            if (!registerResult.IdentityResult.Succeeded)
            {
                //TODO: Translate errors returned to client
                return GetErrorResult(registerResult.IdentityResult);
            }

            //send confirmation email (TODO: Use Domain Event)
            string code = registerResult.EmailConfirmationCode;
            ApplicationUser user = registerResult.User;

            var callbackUrl = new Uri(Url.Link("ConfirmEmailRoute", new { userId = user.Id, code = code }) + @"&zumo-api-version=2.0.0");

            await AppUserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

            Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));

            return Created(locationHeader, TheModelFactory.Create(user));
        }
    }
}
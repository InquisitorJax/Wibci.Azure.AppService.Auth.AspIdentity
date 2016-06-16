using Microsoft.Azure.Mobile.Server.Config;
using System.Threading.Tasks;
using System.Web.Http;
using Wibci.Azure.AppService.Auth.DataAccess;
using Wibci.Azure.AppService.Auth.Util;

namespace Wibci.Azure.AppService.Auth.Server.Controllers
{
    [MobileAppController]
    [RoutePrefix("api/RefreshTokens")]
    public class RefreshTokensController : ApiController
    {
        private IAuthDataAccess _repo = null;

        public RefreshTokensController()
        {
            _repo = CC.IoC.Resolve<IAuthDataAccess>();
        }

        [AllowAnonymous] //so user can try revoke own token (tokenId is hashed value)
        [Route("")]
        public async Task<IHttpActionResult> Delete(string tokenId)
        {
            var result = await _repo.RemoveRefreshTokenAsync(tokenId);
            if (result)
            {
                return Ok();
            }
            return BadRequest("Token Id does not exist");
        }

        //[Authorize(Roles = UserRoles.SystemAdmin)] //TODO: Implement
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(_repo.GetAllRefreshTokens());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repo.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
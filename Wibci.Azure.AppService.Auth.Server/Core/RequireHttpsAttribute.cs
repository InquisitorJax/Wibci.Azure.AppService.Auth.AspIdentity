using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Wibci.Azure.AppService.Auth.Server.Core
{
    public class RequireHttpsAttribute : AuthorizationFilterAttribute
    {
        //from: http://www.asp.net/web-api/overview/security/working-with-ssl-in-web-api

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.RequestUri.Scheme != Uri.UriSchemeHttps)
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                {
                    ReasonPhrase = "HTTPS Required"
                };
            }
            else
            {
                base.OnAuthorization(actionContext);
            }
        }
    }
}
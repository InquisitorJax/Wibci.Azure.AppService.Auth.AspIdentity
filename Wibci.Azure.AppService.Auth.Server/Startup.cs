using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Wibci.Azure.AppService.Auth.Server.Startup))]

namespace Wibci.Azure.AppService.Auth.Server
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}
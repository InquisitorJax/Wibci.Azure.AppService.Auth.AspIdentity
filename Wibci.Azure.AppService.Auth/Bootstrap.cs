using Wibci.Azure.AppService.Auth.Services;
using Wibci.Azure.AppService.Auth.Util;

namespace Wibci.Azure.AppService.Auth
{
    public static class Bootstrap
    {
        public static void Initialize(IDependencyContainer container, IDbContextProvider dbContext, ISendEmailCommand emailCommand)
        {
            //TODO: Initialize IoC with external dependencies
        }
    }
}
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using Wibci.Azure.AppService.Auth.Util;

namespace Wibci.Azure.AppService.Auth.Services
{
    public class IdentityEmailMessageService : IIdentityMessageService
    {
        private ISendEmailCommand Command
        {
            get { return CC.IoC.Resolve<ISendEmailCommand>(); }
        }

        public async Task SendAsync(IdentityMessage message)
        {
            const string from = "support@holl3r.com";
            const string fromName = "Holl3r Services";
            SendEmailRequest emailRequest = new SendEmailRequest(message.Body, message.Subject, message.Destination, from, fromName);

            //send an email
            await Command.ExecuteAsync(emailRequest);
        }
    }
}
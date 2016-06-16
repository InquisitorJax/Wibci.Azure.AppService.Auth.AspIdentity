using Microsoft.AspNet.Identity;
using Wibci.Core;

namespace Wibci.Azure.AppService.Auth.Util
{
    public static class IdentityResultExtensions
    {
        public static Notification AsNotification(this IdentityResult result)
        {
            Notification retNotification = Notification.Success();

            if (result == null)
            {
                retNotification.Add("Ah Snap! We could not process your identity!");
                return retNotification;
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        retNotification.Add(error);
                    }
                }

                if (retNotification.IsValid())
                {
                    //No ModelState errors are available to send, so just return an empty BadRequest.
                    retNotification.Add("What HO! Identity malfunction discovered. AAARRRGH!");
                }
            }

            return retNotification;
        }
    }
}
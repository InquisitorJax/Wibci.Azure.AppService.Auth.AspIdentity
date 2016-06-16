using Microsoft.Azure.Mobile.Server;

namespace Wibci.Azure.AppService.Auth.Server.DataObjects
{
    public class TodoItem : EntityData
    {
        public string Text { get; set; }

        public bool Complete { get; set; }
    }
}
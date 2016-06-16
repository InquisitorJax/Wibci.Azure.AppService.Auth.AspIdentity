namespace Wibci.Core.Commands
{
    public class CommandResult
    {
        public CommandResult()
        {
            Notification = Notification.Success();
        }

        public Notification Notification { get; set; }

        public virtual bool IsValid(NotificationSeverity severity = NotificationSeverity.Error)
        {
            return Notification.IsValid(severity);
        }
    }
}
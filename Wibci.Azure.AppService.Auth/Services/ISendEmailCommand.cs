using System.Threading.Tasks;
using Validation;
using Wibci.Core;
using Wibci.Core.Commands;

namespace Wibci.Azure.AppService.Auth.Services
{
    /// <summary>
    /// Use a service like mailchimp or sendgrid to send an email
    /// </summary>
    public interface ISendEmailCommand : IAsyncLogicCommand<SendEmailRequest, CommandResult>
    {
    }

    public class SendEmailRequest
    {
        public SendEmailRequest(string body, string subject, string destination, string fromEmail, string fromName)
        {
            Requires.NotNull(fromEmail, nameof(fromEmail));
            Requires.NotNull(body, nameof(body));
            Requires.NotNull(destination, nameof(destination));
            Requires.NotNull(subject, nameof(subject));

            FromEmail = fromEmail;
            FromName = fromName;
            Body = body;
            Destination = destination;
            Subject = subject;
        }

        public string Body { get; set; }

        public string Destination { get; set; }

        public byte[] EmbedImage { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public string ImageName { get; set; }
        public string Subject { get; set; }
    }

    internal class DefaultSmtpEmailCommand : AsyncLogicCommand<SendEmailRequest, CommandResult>, ISendEmailCommand
    {
        public override Task<CommandResult> ExecuteAsync(SendEmailRequest request)
        {
            //TODO: Implement default SMPT implementation
            return Task.FromResult(new CommandResult());
        }
    }
}
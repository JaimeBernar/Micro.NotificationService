namespace Micro.NotificationService.Services.Email
{
    using FluentResults;
    using MailKit.Net.Smtp;
    using Microsoft.Extensions.Options;
    using MimeKit;
    using Micro.NotificationService.Models;
    using Micro.NotificationService.Options;

    public class EmailService : IEmailService, IDisposable
    {
        private readonly SmtpClient smtpClient;

        private readonly ILogger<EmailService> logger;

        private readonly EmailServerOptions emailServerOptions;

        public EmailService(ILogger<EmailService> logger, IOptions<EmailServerOptions> options)
        {
            this.smtpClient = new SmtpClient();
            this.logger = logger;
            this.emailServerOptions = options.Value;
        }

        public async Task<Result> SendEmailNotification(IEnumerable<Notification> notifications)
        {
            var emailNotifications = notifications.Where(x => x.Channel == Common.Enums.NotificationChannel.Email);

            if (notifications.Any(x => x.Channel != Common.Enums.NotificationChannel.Email))
            {
                this.logger.LogWarning("Notifications that should NOT produce email notifications are being passed to {name}", nameof(EmailService));
            }

            var messages = CreateMessages(emailNotifications);

            try
            {
                var messagesList = messages.ToList();

                if (!messagesList.Any())
                {
                    logger.LogWarning("No messages were created for the specified Notifications");
                    return Result.Ok();
                }

                this.logger.LogInformation("Sending {count} messages", messagesList.Count);

                await this.smtpClient.ConnectAsync(this.emailServerOptions.Host, this.emailServerOptions.Port);

                var tasks = messagesList.Select(m => this.smtpClient.SendAsync(m));

                await Task.WhenAll(tasks);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                var message = string.Format("An error ocurred while sending the Email. {error}", ex);
                this.logger.LogError(message);
                return Result.Fail(message);
            }
            finally
            {
                await this.smtpClient.DisconnectAsync(true);
            }
        }

        private IEnumerable<MimeMessage> CreateMessages(IEnumerable<Notification> notifications)
        {
            var messages = new List<MimeMessage>();

            foreach (var notification in notifications)
            {
                var message = new MimeMessage();

                if (this.emailServerOptions.UseSenderInfo)
                {
                    message.From.Add(new MailboxAddress(this.emailServerOptions.EmailSenderName, this.emailServerOptions.EmailSenderAddress));
                }

                var receiverName = notification.ReceiverName ?? "Receiver";

                message.To.Add(new MailboxAddress(receiverName, notification.EmailAddress));

                message.Subject = notification.Header;
                message.Body = new TextPart()
                {
                    Text = notification.Body
                };

                message.Date = notification.CreatedAt;

                messages.Add(message);
            }

            return messages;
        }

        public void Dispose()
        {
            this.smtpClient.Dispose();
        }
    }
}

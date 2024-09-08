namespace NotificationService.Services
{
    using FluentResults;
    using MailKit.Net.Smtp;
    using Microsoft.Extensions.Options;
    using MimeKit;
    using NotificationService.Common.DTOs;
    using NotificationService.Models;
    using NotificationService.Options;

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

        public Task<Result> SendEmail(Dictionary<NotificationMessage, IEnumerable<Subscription>> groupedSubscriptions)
        {
            var messages = this.CreateMessages(groupedSubscriptions);
            return this.SendEmails(messages);
        }

        public Task<Result> SendEmail(IEnumerable<DirectNotificationMessage> directNotifications)
        {
            var messages = this.CreateMessages(directNotifications);
            return this.SendEmails(messages);
        }

        private async Task<Result> SendEmails(IEnumerable<MimeMessage> messages)
        {
            try
            {
                var messagesList = messages.ToList();
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

        private IEnumerable<MimeMessage> CreateMessages(Dictionary<NotificationMessage, IEnumerable<Subscription>> groupedSubscriptions)
        {
            var messages = new List<MimeMessage>();

            foreach(var group in groupedSubscriptions)
            {
                var notification = group.Key;
                var subscriptions = group.Value;

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("NotificationService", "notification@service.com"));

                foreach(var subscription in subscriptions)
                {
                    message.To.Add(new MailboxAddress("Receiver", subscription.EmailAddress));
                }
                                
                message.Subject = notification.Header;
                message.Body = new TextPart()
                {
                    Text = notification.Body
                };

                messages.Add(message);
            }

            return messages;
        }

        private IEnumerable<MimeMessage> CreateMessages(IEnumerable<DirectNotificationMessage> directNotifications)
        {
            var messages = new List<MimeMessage>();

            foreach (var notification in directNotifications)
            {
                var message = new MimeMessage();

                if (this.emailServerOptions.UseSenderInfo)
                {
                    message.From.Add(new MailboxAddress(this.emailServerOptions.EmailSenderName, this.emailServerOptions.EmailSenderAddress));
                }

                message.To.Add(new MailboxAddress("Receiver", notification.EmailAddress));

                message.Subject = notification.Header;
                message.Body = new TextPart()
                {
                    Text = notification.Body
                };

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

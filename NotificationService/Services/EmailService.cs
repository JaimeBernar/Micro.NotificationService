namespace NotificationService.Services
{
    using MailKit.Net.Smtp;
    using MimeKit;
    using NotificationService.Common.DTOs;
    using NotificationService.Models;

    public class EmailService : IEmailService
    {
        private readonly SmtpClient smtpClient;

        private readonly ILogger<EmailService> logger;

        public EmailService(ILogger<EmailService> logger)
        {
            this.smtpClient = new SmtpClient();
            this.logger = logger;
        }

        public async Task SendEmail(Dictionary<IncomingNotificationDto, IEnumerable<Subscription>> groupedSubscriptions)
        {
            try
            {
                await this.smtpClient.ConnectAsync("host", 2020);

                var messages = this.CreateMessages(groupedSubscriptions);              

                var tasks = messages.Select(m => this.smtpClient.SendAsync(m));

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                this.logger.LogError("An error ocurred while sending the Email. {error}", ex);
            }
        }

        private IEnumerable<MimeMessage> CreateMessages(Dictionary<IncomingNotificationDto, IEnumerable<Subscription>> groupedSubscriptions)
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

        public void Dispose()
        {
            this.smtpClient.Dispose();
        }
    }
}

namespace NotificationService.Services
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using NotificationService.Common.DTOs;
    using NotificationService.Common.Enums;
    using NotificationService.Data;
    using NotificationService.Models;
    using NotificationService.Options;

    public class NotificationOrchestrator : INotificationOrchestrator
    {
        private readonly Context context;

        private readonly IEmailService emailService;

        private readonly SettingsOptions settings;

        public NotificationOrchestrator(Context context, IEmailService emailService, IOptions<SettingsOptions> options)
        {
            this.context = context;
            this.emailService = emailService;
            this.settings = options.Value;
        }

        public Task ProcessNotification(NotificationMessage notification)
        {
            return this.ProcessNotifications([notification]);
        }

        public Task ProcessDirectNotification(DirectNotificationMessage notification)
        {
            return this.ProcessDirectNotifications([notification]);
        }

        public async Task ProcessNotifications(IEnumerable<NotificationMessage> notifications)
        {
            // Check if a subscription for that notification type exist
            var groupedSubscriptions = await this.GroupSubscriptionByNotification(notifications);

            var emailSubscriptions = groupedSubscriptions.Where(x => x.Key.Channel == NotificationChannel.Email).ToDictionary();
            var webSubscriptions = groupedSubscriptions.Where(x => x.Key.Channel == NotificationChannel.Email).ToDictionary();

            if (this.settings.EmailNotificationsActive)
            {
                await this.emailService.SendEmail(emailSubscriptions);
            }

            if (this.settings.WebNotificationsActive)
            {
                await this.emailService.SendEmail(webSubscriptions);
            }
        }

        public async Task ProcessDirectNotifications(IEnumerable<DirectNotificationMessage> notifications)
        {
            var emailSubscriptions = notifications.Where(x => x.Channel == NotificationChannel.Email);
            var webSubscriptions = notifications.Where(x => x.Channel == NotificationChannel.Email);

            if (this.settings.EmailNotificationsActive)
            {
                await this.emailService.SendEmail(emailSubscriptions);
            }

            if (this.settings.WebNotificationsActive)
            {
                await this.emailService.SendEmail(webSubscriptions);
            }
        }

        private async Task<Dictionary<NotificationMessage, IEnumerable<Subscription>>> GroupSubscriptionByNotification(IEnumerable<NotificationMessage> notifications)
        {
            // Select distinct pairs of NotificationType and Channel to avoid duplicate queries
            var notificationPairs = notifications
                .Select(n => new { n.NotificationType, n.Channel })
                .Distinct()
                .ToList();

            // Query subscriptions in a single batch query
            var subscriptions = await this.context.Subscriptions
                .Where(s => notificationPairs.Contains(new { s.NotificationType, s.Channel }))
                .ToListAsync();

            // Group subscriptions by notification
            var groupedSubscriptions = notifications
                .GroupBy(n => n, n => subscriptions.Where(s => s.NotificationType == n.NotificationType && s.Channel == n.Channel))
                .ToDictionary(g => g.Key, g => g.First());

            return groupedSubscriptions;
        }
    }
}

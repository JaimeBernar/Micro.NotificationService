namespace NotificationService.Services
{
    using Microsoft.EntityFrameworkCore;
    using NotificationService.Common.DTOs;
    using NotificationService.Common.Enums;
    using NotificationService.Data;
    using NotificationService.Models;

    public class NotificationOrchestrator : INotificationOrchestrator
    {
        private readonly Context context;

        private readonly IEmailService emailService;

        public NotificationOrchestrator(Context context, IEmailService emailService)
        {
            this.context = context;
            this.emailService = emailService;
        }

        public Task ProcessNotification(IncomingNotificationDto notification)
        {
            return this.ProcessNotifications([notification]);
        }

        public Task ProcessDirectNotification(DirectNotificationDto notification)
        {
            return this.ProcessDirectNotifications([notification]);
        }

        public async Task ProcessNotifications(IEnumerable<IncomingNotificationDto> notifications)
        {
            // Check if a subscription for that notification type exist
            var groupedSubscriptions = await this.GroupSubscriptionByNotification(notifications);

            var emailSubscriptions = groupedSubscriptions.Where(x => x.Key.Channel == NotificationChannel.Email);
            var webSubscriptions = groupedSubscriptions.Where(x => x.Key.Channel == NotificationChannel.Email);

            this.emailService.
        }

        public async Task ProcessDirectNotifications(IEnumerable<DirectNotificationDto> notifications)
        {
            foreach(var notification in notifications)
            {

            }
        }

        private async Task<Dictionary<IncomingNotificationDto, IEnumerable<Subscription>>> GroupSubscriptionByNotification(IEnumerable<IncomingNotificationDto> notifications)
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

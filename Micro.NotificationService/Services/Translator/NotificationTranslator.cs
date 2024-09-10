namespace Micro.NotificationService.Services.Translator
{
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Data;
    using Micro.NotificationService.Models;
    using Microsoft.EntityFrameworkCore;

    public class NotificationTranslator : INotificationTranslator
    {
        private readonly Context context;

        public NotificationTranslator(Context context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Notification>> ComputeNotifications(IEnumerable<NotificationMessage> notificationMessages)
        {
            var (containValues, groupedByNotifications) = await GroupSubscriptionByNotification(notificationMessages);

            if (!containValues)
            {
                return [];
            }

            var result = new List<Notification>();

            foreach (var group in groupedByNotifications)
            {
                var notificationMessage = group.Key;
                var subscriptions = group.Value;

                foreach (var subscription in subscriptions)
                {
                    var userId = subscription.UserId;

                    var notification = new Notification
                    {
                        UserId = userId,
                        ReceiverName = subscription.SubscriberName,
                        EmailAddress = subscription.EmailAddress,
                        NotificationType = subscription.NotificationType,
                        Channel = subscription.Channel,
                        Header = notificationMessage.Header,
                        Body = notificationMessage.Body,
                    };

                    result.Add(notification);
                }
            }

            return result;
        }

        public IEnumerable<Notification> ComputeNotifications(IEnumerable<DirectNotificationMessage> direcNotificationMessages)
        {
            var result = new List<Notification>();

            foreach (var directNotification in direcNotificationMessages)
            {
                var notification = new Notification
                {
                    UserId = directNotification.UserId,
                    ReceiverName = directNotification.ReceiverName,
                    EmailAddress = directNotification.EmailAddress,
                    NotificationType = directNotification.NotificationType,
                    Channel = directNotification.Channel,
                    Header = directNotification.Header,
                    Body = directNotification.Body,
                    IsDirect = true,
                };

                result.Add(notification);
            }

            return result;
        }

        private async Task<(bool containValues, Dictionary<NotificationMessage, IEnumerable<Subscription>> values)> GroupSubscriptionByNotification(IEnumerable<NotificationMessage> notifications)
        {
            var subscriptions = await context.Subscriptions.AsNoTracking().ToListAsync();

            if (!subscriptions.Any())
            {
                return (false, []);
            }

            // Group subscriptions by notification
            var groupedSubscriptions = notifications
                .GroupBy(n => n, n => subscriptions.Where(s => s.NotificationType == n.NotificationType))
                .ToDictionary(g => g.Key, g => g.First());

            if (!groupedSubscriptions.Any())
            {
                return (false, []);
            }

            var containValues = groupedSubscriptions.All(kvp => kvp.Value.Any());

            return (containValues, groupedSubscriptions);
        }
    }
}

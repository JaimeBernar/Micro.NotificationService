namespace Micro.NotificationService.Services
{
    using FluentResults;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Common.Enums;
    using Micro.NotificationService.Data;
    using Micro.NotificationService.Models;
    using Micro.NotificationService.Options;

    using Notification = Micro.NotificationService.Models.Notification;

    public class NotificationOrchestrator : INotificationOrchestrator
    {
        private readonly Context context;

        private readonly IEmailService emailService;

        private readonly SettingsOptions settings;

        private readonly ILogger<NotificationOrchestrator> logger;

        public NotificationOrchestrator(Context context, IEmailService emailService, IOptions<SettingsOptions> options,
            ILogger<NotificationOrchestrator> logger)
        {
            this.context = context;
            this.emailService = emailService;
            this.settings = options.Value;
            this.logger = logger;
        }

        public async Task<Result<IEnumerable<Notification>>> GetUserNotifications(Guid userId)
        {
            try
            {
                var result = await this.context.Notifications.Where(x => x.UserId == userId).ToArrayAsync();
                return Result.Ok<IEnumerable<Notification>>(result);
            }
            catch (Exception ex)
            {
                var message = string.Format("An Error ocurred while getting user notifications. {error}", ex);
                this.logger.LogError(message);
                return Result.Fail(message);
            }
        }

        public Task<Result> ProcessNotification(NotificationMessage notification)
        {
            return this.ProcessNotifications([notification]);
        }

        public Task<Result> ProcessDirectNotification(DirectNotificationMessage notification)
        {
            return this.ProcessDirectNotifications([notification]);
        }

        public async Task<Result> ProcessNotifications(IEnumerable<NotificationMessage> notifications)
        {
            // Check if a subscription for that notification type exist
            var groupedSubscriptions = await this.GroupSubscriptionByNotification(notifications);

            var emailSubscriptions = groupedSubscriptions.Where(x => x.Key.Channel == NotificationChannel.Email).ToDictionary();
            var webSubscriptions = groupedSubscriptions.Where(x => x.Key.Channel == NotificationChannel.Email).ToDictionary();

            if (this.settings.EmailNotificationsActive)
            {
                var emailResult = await this.emailService.SendEmail(emailSubscriptions);

                if (emailResult.IsFailed)
                {
                    return emailResult;
                }
            }

            if (this.settings.WebNotificationsActive)
            {
                var webResult = await this.emailService.SendEmail(webSubscriptions);

                if (webResult.IsFailed)
                {
                    return webResult;
                }
            }

            return Result.Ok();
        }

        public async Task<Result> ProcessDirectNotifications(IEnumerable<DirectNotificationMessage> notifications)
        {
            var emailSubscriptions = notifications.Where(x => x.Channel == NotificationChannel.Email);
            var webSubscriptions = notifications.Where(x => x.Channel == NotificationChannel.Email);

            if (this.settings.EmailNotificationsActive)
            {
                var emailResult = await this.emailService.SendEmail(emailSubscriptions);

                if (emailResult.IsFailed)
                {
                    return emailResult;
                }
            }

            if (this.settings.WebNotificationsActive)
            {
                var webResult = await this.emailService.SendEmail(webSubscriptions);

                if (webResult.IsFailed)
                {
                    return webResult;
                }
            }

            return Result.Ok();
        }

        private async Task<Dictionary<NotificationMessage, IEnumerable<Subscription>>> GroupSubscriptionByNotification(IEnumerable<NotificationMessage> notifications)
        {
            // Query subscriptions in a single batch query
            var subscriptions = await this.context.Subscriptions.ToListAsync();

            // Group subscriptions by notification
            var groupedSubscriptions = notifications
                .GroupBy(n => n, n => subscriptions.Where(s => s.NotificationType == n.NotificationType && s.Channel == n.Channel))
                .ToDictionary(g => g.Key, g => g.First());

            return groupedSubscriptions;
        }
    }
}

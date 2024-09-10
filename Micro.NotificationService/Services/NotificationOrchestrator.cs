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

    public class NotificationOrchestrator : INotificationOrchestrator
    {
        private readonly Context context;

        private readonly INotificationTranslator translator;

        private readonly IEmailService emailService;

        private readonly IWebService webService;

        private readonly SettingsOptions settings;

        private readonly ILogger<NotificationOrchestrator> logger;

        public NotificationOrchestrator(Context context, INotificationTranslator notificationTranslator, IEmailService emailService, 
            IWebService webService, IOptions<SettingsOptions> options, ILogger<NotificationOrchestrator> logger)
        {
            this.context = context;
            this.translator = notificationTranslator;
            this.emailService = emailService;
            this.webService = webService;
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

        public async Task<Result> ProcessNotifications(IEnumerable<NotificationMessage> notificationMessages)
        {
            var notifications = await this.translator.ComputeNotifications(notificationMessages);
            return await this.ProcessAndSaveNotifications(notifications);
        }

        public Task<Result> ProcessDirectNotifications(IEnumerable<DirectNotificationMessage> notificationMessages)
        {
            var notifications = this.translator.ComputeNotifications(notificationMessages);
            return this.ProcessAndSaveNotifications(notifications);
        }

        private async Task<Result> ProcessAndSaveNotifications(IEnumerable<Notification> notifications)
        {
            if (!notifications.Any())
            {
                return Result.Fail("No Notifications to process");
            }

            var emailNotifications = notifications.Where(x => x.Channel == NotificationChannel.Email);
            var webNotifications = notifications.Where(x => x.Channel == NotificationChannel.Web);

            this.context.Notifications.AddRange(notifications);
            var savedNotifications = await this.context.SaveChangesAsync();

            if(notifications.ToList().Count != savedNotifications)
            {
                return Result.Fail("the number of saved notifications is not equal to the number of notifications to process");
            }

            if (this.settings.EmailNotificationsActive && emailNotifications.Any())
            {
                var emailResult = await this.emailService.SendEmailNotification(emailNotifications);

                if (emailResult.IsFailed)
                {
                    return emailResult;
                }
            }

            if (this.settings.WebNotificationsActive && webNotifications.Any())
            {
                var webResult = await this.webService.SendWebNotifications(webNotifications);

                if (webResult.IsFailed)
                {
                    return webResult;
                }
            }

            return Result.Ok();
        }
    }
}

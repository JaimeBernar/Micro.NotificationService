namespace Micro.NotificationService.Services.Orchestrators
{
    using FluentResults;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Common.Enums;
    using Micro.NotificationService.Data;
    using Micro.NotificationService.Models;
    using Micro.NotificationService.Options;
    using Micro.NotificationService.Extensions;
    using System.Net;
    using Micro.NotificationService.Services.Email;
    using Micro.NotificationService.Services.Web;
    using Micro.NotificationService.Services.Translator;

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
            translator = notificationTranslator;
            this.emailService = emailService;
            this.webService = webService;
            settings = options.Value;
            this.logger = logger;
        }

        public async Task<Result<IEnumerable<Notification>>> GetUserNotifications(Guid userId)
        {
            try
            {
                var result = await context.Notifications.Where(x => x.UserId == userId && x.IsReaded == false).ToArrayAsync();
                return Result.Ok<IEnumerable<Notification>>(result);
            }
            catch (Exception ex)
            {
                var message = string.Format("An Error ocurred while getting user notifications. {error}", ex);
                logger.LogError(message);
                return Result.Fail(message);
            }
        }

        public async Task<Result> ProcessNotifications(IEnumerable<NotificationMessage> notificationMessages)
        {
            var notifications = await translator.ComputeNotifications(notificationMessages);
            return await ProcessAndSaveNotifications(notifications);
        }

        public Task<Result> ProcessDirectNotifications(IEnumerable<DirectNotificationMessage> notificationMessages)
        {
            var notifications = translator.ComputeNotifications(notificationMessages);
            return ProcessAndSaveNotifications(notifications);
        }

        private async Task<Result> ProcessAndSaveNotifications(IEnumerable<Notification> notifications)
        {
            if (!notifications.Any())
            {
                return Result.Fail("No Notifications to process");
            }

            var emailNotifications = notifications.Where(x => x.Channel == NotificationChannel.Email);
            var webNotifications = notifications.Where(x => x.Channel == NotificationChannel.Web);

            if (settings.EmailNotificationsActive && emailNotifications.Any())
            {
                var emailResult = await emailService.SendEmailNotification(emailNotifications);

                if (emailResult.IsFailed)
                {
                    return emailResult;
                }
            }

            if (settings.WebNotificationsActive && webNotifications.Any())
            {
                //Web notifications are saved as they need to be retrieved
                context.Notifications.AddRange(webNotifications);
                var savedNotifications = await context.SaveChangesAsync();

                var webResult = await webService.SendWebNotifications(webNotifications);

                if (webResult.IsFailed)
                {
                    return webResult;
                }
            }

            return Result.Ok();
        }

        public async Task<Result> DeleteNotifications(IEnumerable<Guid> ids)
        {
            try
            {
                var notifications = await context.Notifications.Where(x => ids.Contains(x.Id)).ToListAsync();

                if (notifications.Any())
                {
                    notifications.ForEach(x => x.IsReaded = true);
                    await context.SaveChangesAsync();
                    return Result.Ok();
                }

                return Result.Fail("No existing notifications for the specified ids").WithStatusCode(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
    }
}

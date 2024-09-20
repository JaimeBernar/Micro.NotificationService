namespace Micro.NotificationService.Services.Orchestrators
{
    using FluentResults;
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Common.Enums;
    using Micro.NotificationService.Extensions;
    using Micro.NotificationService.Models;
    using Micro.NotificationService.Options;
    using Micro.NotificationService.Services.Data;
    using Micro.NotificationService.Services.Email;
    using Micro.NotificationService.Services.Translator;
    using Micro.NotificationService.Services.Web;
    using Microsoft.Extensions.Options;
    using System.Net;

    public class NotificationOrchestrator : INotificationOrchestrator
    {
        private readonly IDataService dataService;

        private readonly INotificationTranslator translator;

        private readonly IEmailService emailService;

        private readonly IWebService webService;

        private readonly SettingsOptions settings;

        private readonly ILogger<NotificationOrchestrator> logger;

        public NotificationOrchestrator(IDataService dataService, INotificationTranslator notificationTranslator, IEmailService emailService,
            IWebService webService, IOptions<SettingsOptions> options, ILogger<NotificationOrchestrator> logger)
        {
            this.dataService = dataService;
            this.translator = notificationTranslator;
            this.emailService = emailService;
            this.webService = webService;
            this.settings = options.Value;
            this.logger = logger;
        }

        public Result<IEnumerable<Notification>> GetUserNotifications(string userId)
        {
            try
            {
                var result = this.dataService.Notifications.Query()
                                             .Where(x => x.UserId == userId && x.IsReaded == false)
                                             .ToEnumerable();

                return Result.Ok(result);
            }
            catch (Exception ex)
            {
                var message = string.Format("An Error ocurred while getting user notifications. {error}", ex);
                this.logger.LogError(message);
                return Result.Fail(message);
            }
        }

        public Task<Result> ProcessNotifications(IEnumerable<NotificationMessage> notificationMessages)
        {
            var notifications = this.translator.ComputeNotifications(notificationMessages);
            return ProcessAndSaveNotifications(notifications);
        }

        public Task<Result> ProcessDirectNotifications(IEnumerable<DirectNotificationMessage> notificationMessages)
        {
            var notifications = this.translator.ComputeNotifications(notificationMessages);
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
                //Web notifications are saved as they need to be retrieved
                this.dataService.Notifications.InsertBulk(webNotifications);

                var webResult = await this.webService.SendWebNotifications(webNotifications);

                if (webResult.IsFailed)
                {
                    return webResult;
                }
            }

            return Result.Ok();
        }

        public Result DeleteNotifications(IEnumerable<int> ids)
        {
            try
            {
                var notifications = this.dataService.Notifications.Query()
                                        .Where(x => ids.Contains(x.Id)).
                                        ToList();

                if (notifications.Any())
                {
                    notifications.ForEach(x => x.IsReaded = true);
                    this.dataService.Notifications.Update(notifications);
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

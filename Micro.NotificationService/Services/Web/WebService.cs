namespace Micro.NotificationService.Services.Web
{
    using FluentResults;
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Common.Enums;
    using Micro.NotificationService.Models;
    using System.Threading.Tasks;

    public class WebService : IWebService
    {
        private readonly ILogger<WebService> logger;

        private WebNotificationBatcher batcher;

        public WebService(ILogger<WebService> logger, WebNotificationBatcher batcher)
        {
            this.logger = logger;
            this.batcher = batcher;
        }

        public async Task<Result> SendWebNotifications(IEnumerable<Notification> notifications)
        {
            try
            {
                var webNotifications = notifications.Where(x => x.Channel == NotificationChannel.Web);

                if (notifications.Any(x => x.Channel != NotificationChannel.Web))
                {
                    this.logger.LogWarning("Notifications that should NOT produce web notifications are being passed to {name}", nameof(WebService));
                }

                var tasks = new List<Task>();

                foreach (var notification in webNotifications)
                {
                    var userId = notification.UserId;

                    var outNotification = new OutNotification
                    {
                        Id = notification.Id,
                        Body = notification.Body,
                        Header = notification.Header,
                        CreatedAt = notification.CreatedAt,
                    };

                    this.batcher.AddBatchedNotification(userId.ToString(), outNotification);
                }

                if (this.batcher.ShouldProcessBatchedNotifications())
                {
                    await this.batcher.ProcessBatchedNotifications();
                }

                return Result.Ok();
            }
            catch (Exception ex)
            {
                var message = string.Format("An error ocurred while sending the Web Notifications. {0}", ex);
                this.logger.LogError(message);
                return Result.Fail(message);
            }
        }
    }
}

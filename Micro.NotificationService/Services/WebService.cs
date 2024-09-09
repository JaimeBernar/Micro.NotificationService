namespace Micro.NotificationService.Services
{
    using FluentResults;
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Models;
    using Microsoft.AspNetCore.SignalR;
    using System.Threading.Tasks;
    using Notification = Micro.NotificationService.Common.DTOs.Notification;

    public class WebService : IWebService
    {
        private readonly ILogger<WebService> logger;

        private IHubContext<NotificationsHub> hub;

        private WebNotificationBatcher batcher;

        public WebService(ILogger<WebService> logger, WebNotificationBatcher batcher)
        {
            this.logger = logger;
            this.batcher = batcher;
        }

        public async Task<Result> SendWebNotification(Dictionary<NotificationMessage, IEnumerable<Subscription>> groupedSubscriptions)
        {
            try
            {
                var tasks = new List<Task>();

                foreach (var group in groupedSubscriptions)
                {
                    var notificationMessage = group.Key;
                    var subscriptions = group.Value;

                    var notification = new Notification
                    {
                        Body = notificationMessage.Body,
                        Header = notificationMessage.Header,
                    };

                    foreach (var subscription in subscriptions)
                    {
                        var userId = subscription.UserId.ToString();                        
                        this.batcher.AddBatchedNotification(userId, notification);
                    }
                }
                
                if(this.batcher.ShouldProcessBatchedNotifications())
                {
                    await this.batcher.ProcessBatchedNotifications();
                }

                return Result.Ok();
            }
            catch (Exception ex)
            {
                var message = string.Format("An error ocurred while sending the Web Notification. {error}", ex);
                this.logger.LogError(message);
                return Result.Fail(message);
            }
        }

        public async Task<Result> SendWebNotification(IEnumerable<DirectNotificationMessage> directNotifications)
        {
            try
            {
                var tasks = new List<Task>();

                foreach (var directNotification in directNotifications.Where(x => x.Channel == Common.Enums.NotificationChannel.Web))
                {
                    var userId = directNotification.UserId.ToString();

                    var notification = new Notification
                    {
                        Body = directNotification.Body,
                        Header = directNotification.Header,
                    };

                    this.batcher.AddBatchedNotification(userId, notification);
                }

                if (this.batcher.ShouldProcessBatchedNotifications())
                {
                    await this.batcher.ProcessBatchedNotifications();
                }

                return Result.Ok();
            }
            catch (Exception ex)
            {
                var message = string.Format("An error ocurred while sending the Web Notification. {error}", ex);
                this.logger.LogError(message);
                return Result.Fail(message);
            }
        }
    }
}

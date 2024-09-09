namespace Micro.NotificationService.Services
{
    using FluentResults;
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Common.SignalR;
    using Micro.NotificationService.Models;
    using Microsoft.AspNetCore.SignalR;

    using Notification = Micro.NotificationService.Common.DTOs.Notification;

    public class WebService : Hub, IWebService
    {
        private readonly ILogger<WebService> logger;

        private IHubContext<NotificationsHub> hub;

        public WebService(ILogger<WebService> logger, IHubContext<NotificationsHub> hub)
        {
            this.logger = logger;
            this.hub = hub;
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
                        tasks.Add(this.hub.Clients?.All.SendAsync(NotificationMethodNames.Receive, notification));
                    }
                }

                await Task.WhenAll(tasks);
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

                    tasks.Add(this.hub.Clients?.All.SendAsync(NotificationMethodNames.Receive, notification));
                }

                await Task.WhenAll(tasks);
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

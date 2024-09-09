namespace Micro.NotificationService.Services
{
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Common.SignalR;
    using Microsoft.AspNetCore.SignalR;
    using System.Collections.Concurrent;

    public sealed class WebNotificationBatcher
    {
        private readonly IHubContext<NotificationsHub> hub;

        private ConcurrentDictionary<string, List<Notification>> batchedNotifications = [];

        private Timer timer;

        private int bactchMaxSize = 100;

        public WebNotificationBatcher(IHubContext<NotificationsHub> hub)
        {
            this.hub = hub;
            this.timer = new Timer(async _ => await this.ProcessBatchedNotifications(true), null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
        }

        public void AddBatchedNotification(string userId, Notification notification)
        {
            if (this.batchedNotifications.TryGetValue(userId, out var notifications))
            {
                notifications.Add(notification);
            }
            else
            {
                this.batchedNotifications.TryAdd(userId, [notification]);
            }
        }

        public bool ShouldProcessBatchedNotifications()
        {
            return this.batchedNotifications.Values.SelectMany(x => x).ToList().Count > this.bactchMaxSize;
        }

        public async Task ProcessBatchedNotifications(bool calledByTimer = false)
        {
            //If the batched notifications are still not enough wait till the next call
            if (!calledByTimer && !this.ShouldProcessBatchedNotifications())
            {
                return;
            }

            var tasks = new List<Task>();

            foreach (var (userId, notifications) in this.batchedNotifications)
            {
                //TODO: Use group key to send only the interested user
                //tasks.Add(this.hub.Clients?.User(group.Key).SendAsync(NotificationMethodNames.Receive, group.Value));
                tasks.Add(this.hub.Clients?.All.SendAsync(NotificationMethodNames.Receive, notifications));
            }

            await Task.WhenAll(tasks);

            this.batchedNotifications.Clear();
        }
    }
}

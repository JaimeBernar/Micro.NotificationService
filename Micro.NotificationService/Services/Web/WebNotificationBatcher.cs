namespace Micro.NotificationService.Services.Web
{
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Common.SignalR;
    using Micro.NotificationService.Options;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Options;
    using System.Collections.Concurrent;

    public sealed class WebNotificationBatcher
    {
        private readonly IHubContext<NotificationsHub> hub;

        private ConcurrentDictionary<string, List<OutNotification>> batchedNotifications = [];

        private Timer timer;

        private int bactchMaxSize;

        public WebNotificationBatcher(IHubContext<NotificationsHub> hub, IOptions<SettingsOptions> settings)
        {
            var settingsValue = settings.Value;
            this.bactchMaxSize = settingsValue.BatchSize;
            this.hub = hub;
            this.timer = new Timer(async _ => await ProcessBatchedNotifications(true), null, TimeSpan.Zero, TimeSpan.FromSeconds(settingsValue.BatchTime));
        }

        public void AddBatchedNotification(string userId, OutNotification notification)
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
            if (!calledByTimer && !ShouldProcessBatchedNotifications())
            {
                return;
            }

            var tasks = new List<Task>();

            foreach (var (userId, notifications) in batchedNotifications)
            {
                //TODO: Use group key to send only the interested user
                //tasks.Add(this.hub.Clients?.User(group.Key).SendAsync(NotificationMethodNames.Receive, group.Value));
                tasks.Add(hub.Clients.All.SendAsync(NotificationMethodNames.Receive, notifications));
            }

            await Task.WhenAll(tasks);

            this.batchedNotifications.Clear();
        }
    }
}

namespace Micro.NotificationService.Services
{
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Common.SignalR;
    using Microsoft.AspNetCore.SignalR;

    public class NotificationsHub : Hub
    {
        public Task SendNotification(Notification notification, string userId)
        {
            return this.Clients?.User(userId).SendAsync(NotificationMethodNames.Receive, notification);
        }
    }
}

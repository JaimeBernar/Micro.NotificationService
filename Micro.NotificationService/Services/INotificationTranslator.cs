namespace Micro.NotificationService.Services
{
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Models;

    public interface INotificationTranslator
    {
        Task<IEnumerable<Notification>> ComputeNotifications(IEnumerable<NotificationMessage> notificationMessages);

        IEnumerable<Notification> ComputeNotifications(IEnumerable<DirectNotificationMessage> direcNotificationMessages);
    }
}

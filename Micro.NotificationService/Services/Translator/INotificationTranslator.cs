namespace Micro.NotificationService.Services.Translator
{
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Models;

    public interface INotificationTranslator
    {
        IEnumerable<Notification> ComputeNotifications(IEnumerable<NotificationMessage> notificationMessages);

        IEnumerable<Notification> ComputeNotifications(IEnumerable<DirectNotificationMessage> direcNotificationMessages);
    }
}

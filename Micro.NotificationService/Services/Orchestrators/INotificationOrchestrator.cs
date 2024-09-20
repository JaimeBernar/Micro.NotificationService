namespace Micro.NotificationService.Services.Orchestrators
{
    using FluentResults;
    using Micro.NotificationService.Common.DTOs;

    using Notification = Models.Notification;

    public interface INotificationOrchestrator
    {
        Result<IEnumerable<Notification>> GetUserNotifications(string userId);

        Task<Result> ProcessNotifications(IEnumerable<NotificationMessage> notificationMessages);

        Task<Result> ProcessDirectNotifications(IEnumerable<DirectNotificationMessage> notificationMessages);

        Result DeleteNotifications(IEnumerable<int> ids);
    }
}

namespace Micro.NotificationService.Services.Orchestrators
{
    using FluentResults;
    using Micro.NotificationService.Common.DTOs;

    using Notification = Models.Notification;

    public interface INotificationOrchestrator
    {
        Task<Result<IEnumerable<Notification>>> GetUserNotifications(Guid userId);

        Task<Result> ProcessNotifications(IEnumerable<NotificationMessage> notificationMessages);

        Task<Result> ProcessDirectNotifications(IEnumerable<DirectNotificationMessage> notificationMessages);

        Task<Result> DeleteNotifications(IEnumerable<Guid> ids);
    }
}

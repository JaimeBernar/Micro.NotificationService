namespace Micro.NotificationService.Services
{
    using FluentResults;
    using Micro.NotificationService.Common.DTOs;

    using Notification = Micro.NotificationService.Models.Notification;

    public interface INotificationOrchestrator
    {
        Task<Result<IEnumerable<Notification>>> GetUserNotifications(Guid userId);

        Task<Result> ProcessNotification(NotificationMessage notification);

        Task<Result> ProcessDirectNotification(DirectNotificationMessage notification);

        Task<Result> DeleteNotifications(IEnumerable<Guid> ids);
    }
}

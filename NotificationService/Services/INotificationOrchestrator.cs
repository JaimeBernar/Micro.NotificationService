namespace NotificationService.Services
{
    using FluentResults;
    using NotificationService.Common.DTOs;

    using Notification = NotificationService.Models.Notification;

    public interface INotificationOrchestrator
    {
        Task<Result<IEnumerable<Notification>>> GetUserNotifications(Guid userId);

        Task<Result> ProcessNotification(NotificationMessage notification);

        Task<Result> ProcessDirectNotification(DirectNotificationMessage notification);
    }
}

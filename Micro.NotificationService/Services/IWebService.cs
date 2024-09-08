namespace Micro.NotificationService.Services
{
    using FluentResults;
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Models;

    public interface IWebService
    {
        Task<Result> SendWebNotification(Dictionary<NotificationMessage, IEnumerable<Subscription>> groupedSubscriptions);

        Task<Result> SendWebNotification(IEnumerable<DirectNotificationMessage> directNotifications);
    }
}

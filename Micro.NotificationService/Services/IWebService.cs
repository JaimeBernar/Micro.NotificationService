namespace Micro.NotificationService.Services
{
    using FluentResults;
    using Micro.NotificationService.Models;

    public interface IWebService
    {
        Task<Result> SendWebNotifications(IEnumerable<Notification> notifications);
    }
}

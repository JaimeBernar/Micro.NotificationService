namespace Micro.NotificationService.Services.Web
{
    using FluentResults;
    using Micro.NotificationService.Models;

    public interface IWebService
    {
        Task<Result> SendWebNotifications(IEnumerable<Notification> notifications);
    }
}

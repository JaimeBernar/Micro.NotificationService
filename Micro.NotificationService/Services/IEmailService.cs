namespace Micro.NotificationService.Services
{
    using FluentResults;
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Models;

    public interface IEmailService 
    {
        Task<Result> SendEmail(Dictionary<NotificationMessage, IEnumerable<Subscription>> groupedSubscriptions);

        Task<Result> SendEmail(IEnumerable<DirectNotificationMessage> directNotifications);
    }
}

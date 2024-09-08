namespace NotificationService.Services
{
    using FluentResults;
    using NotificationService.Common.DTOs;
    using NotificationService.Models;

    public interface IEmailService 
    {
        Task<Result> SendEmail(Dictionary<NotificationMessage, IEnumerable<Subscription>> groupedSubscriptions);

        Task<Result> SendEmail(IEnumerable<DirectNotificationMessage> directNotifications);
    }
}

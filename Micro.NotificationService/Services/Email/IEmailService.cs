namespace Micro.NotificationService.Services.Email
{
    using FluentResults;
    using Micro.NotificationService.Models;

    public interface IEmailService
    {
        Task<Result> SendEmailNotification(IEnumerable<Notification> notifications);
    }
}

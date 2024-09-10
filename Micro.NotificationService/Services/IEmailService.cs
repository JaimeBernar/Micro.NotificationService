namespace Micro.NotificationService.Services
{
    using FluentResults;
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Models;

    public interface IEmailService 
    {
        Task<Result> SendEmailNotification(IEnumerable<Notification> notifications);
    }
}

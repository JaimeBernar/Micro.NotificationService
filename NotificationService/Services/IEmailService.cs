namespace NotificationService.Services
{
    using NotificationService.Common.DTOs;
    using NotificationService.Models;

    public interface IEmailService 
    {
        Task SendEmail(Dictionary<NotificationMessage, IEnumerable<Subscription>> groupedSubscriptions);

        Task SendEmail(IEnumerable<DirectNotificationMessage> directNotifications);
    }
}

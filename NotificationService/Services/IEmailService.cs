namespace NotificationService.Services
{
    using NotificationService.Common.DTOs;
    using NotificationService.Models;

    public interface IEmailService 
    {
        Task SendEmail(Dictionary<IncomingNotificationDto, IEnumerable<Subscription>> groupedSubscriptions);

        Task SendEmail(IEnumerable<DirectNotificationDto> directNotifications);
    }
}

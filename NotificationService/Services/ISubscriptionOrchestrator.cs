namespace NotificationService.Services
{
    using NotificationService.Common.DTOs;

    public interface ISubscriptionOrchestrator
    {
        Task ProcessSubscription(SubscriptionDto subscription);
    }
}

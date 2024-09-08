namespace NotificationService.Services
{
    using FluentResults;
    using NotificationService.Common.DTOs;
    using NotificationService.Models;

    public interface ISubscriptionOrchestrator
    {
        Task<Result<IEnumerable<Subscription>>> GetUserSubscriptions(Guid userId);

        Task<Result<Subscription>> ProcessSubscription(SubscriptionMessage subscription);

        Task<Result> DeleteSubscription(Guid subscriptionId);
    }
}

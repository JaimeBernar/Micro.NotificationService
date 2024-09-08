namespace Micro.NotificationService.Services
{
    using FluentResults;
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Models;

    public interface ISubscriptionOrchestrator
    {
        Task<Result<IEnumerable<Subscription>>> GetUserSubscriptions(Guid userId);

        Task<Result<Subscription>> ProcessSubscription(SubscriptionMessage subscription);

        Task<Result> DeleteSubscription(Guid subscriptionId);
    }
}

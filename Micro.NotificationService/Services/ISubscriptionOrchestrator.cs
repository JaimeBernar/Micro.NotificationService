namespace Micro.NotificationService.Services
{
    using FluentResults;
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Models;

    public interface ISubscriptionOrchestrator
    {
        Task<Result<IEnumerable<Subscription>>> GetUserSubscriptions(Guid userId);

        Task<Result<IEnumerable<Subscription>>> ProcessSubscriptions(IEnumerable<SubscriptionMessage> subscriptions);

        Task<Result> DeleteSubscriptions(IEnumerable<Guid> subscriptionIds);
    }
}

namespace Micro.NotificationService.Services.Orchestrators
{
    using FluentResults;
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Models;

    public interface ISubscriptionOrchestrator
    {
        Result<IEnumerable<Subscription>> GetUserSubscriptions(string userId);

        Result<IEnumerable<Subscription>> ProcessSubscriptions(IEnumerable<SubscriptionMessage> subscriptions);

        Result DeleteSubscriptions(IEnumerable<int> subscriptionIds);
    }
}

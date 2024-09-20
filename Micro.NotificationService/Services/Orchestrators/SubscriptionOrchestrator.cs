namespace Micro.NotificationService.Services.Orchestrators
{
    using FluentResults;
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Extensions;
    using Micro.NotificationService.Models;
    using Micro.NotificationService.Services.Data;
    using System.Net;

    public class SubscriptionOrchestrator : ISubscriptionOrchestrator
    {
        private readonly IDataService dataService;

        private readonly ILogger<SubscriptionOrchestrator> logger;

        public SubscriptionOrchestrator(IDataService dataService, ILogger<SubscriptionOrchestrator> logger)
        {
            this.dataService = dataService;
            this.logger = logger;
        }

        public Result<IEnumerable<Subscription>> GetUserSubscriptions(string userId)
        {
            try
            {
                var result = this.dataService.Subscriptions.Query()
                                 .Where(x => x.UserId == userId)
                                 .ToEnumerable();

                return Result.Ok(result);
            }
            catch (Exception ex)
            {
                var message = string.Format("An Error ocurred while getting user subscriptions. {0}", ex);
                this.logger.LogError(message);
                return Result.Fail(message);
            }
        }

        public Result<IEnumerable<Subscription>> ProcessSubscriptions(IEnumerable<SubscriptionMessage> subscriptions)
        {
            try
            {
                var result = new List<Subscription>();

                // Create a list of anonymous objects that combine the three fields for comparison
                var subscriptionKeys = subscriptions
                    .Select(s => new { s.UserId, s.NotificationType, s.Channel })
                    .ToList();

                // Fetch all existing subscriptions where the combination of UserId, NotificationType, and Channel match
                var existingSubscriptions = this.dataService.Subscriptions.Query()
                    .Where(s => subscriptionKeys.Contains(new { s.UserId, s.NotificationType, s.Channel }))
                    .ToList();

                // Update existing subscriptions
                foreach (var existingSubscription in existingSubscriptions)
                {
                    var subscription = subscriptions.FirstOrDefault(sm => sm.UserId == existingSubscription.UserId &&
                                                                          sm.NotificationType == existingSubscription.NotificationType &&
                                                                          sm.Channel == existingSubscription.Channel);

                    if (subscription != null)
                    {
                        existingSubscription.IsSubscribed = subscription.IsSubscribed;
                        result.Add(existingSubscription);
                    }
                }

                // Handle new subscriptions that do not exist in the database
                var newSubscriptions = subscriptions.Where(sm => !existingSubscriptions.Any(es => 
                                                                                            es.UserId == sm.UserId &&
                                                                                            es.NotificationType == sm.NotificationType &&
                                                                                            es.Channel == sm.Channel))
                                                                                            .ToList();

                foreach (var newSubscription in newSubscriptions)
                {
                    var model = newSubscription.ToModel();
                    result.Add(model);
                    this.dataService.Subscriptions.Insert(model);
                }

                return Result.Ok(result.AsEnumerable());
            }
            catch (Exception ex)
            {
                var message = string.Format("An Error ocurred while processing the subscription. {error}", ex);
                this.logger.LogError(message);
                return Result.Fail(message);
            }
        }

        public Result DeleteSubscriptions(IEnumerable<int> subscriptionIds)
        {
            try
            {
                var subscriptions = this.dataService.Subscriptions.Query()
                                        .Where(s => subscriptionIds.Contains(s.Id))
                                        .ToList();

                if (subscriptions.Any())
                {
                    subscriptions.ForEach(s => s.IsSubscribed = false);
                    return Result.Ok();
                }

                return Result.Fail("No existing subscriptions for the specified ids").WithStatusCode(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                var message = string.Format("An Error ocurred while deleting the subscription. {error}", ex);
                this.logger.LogError(message);
                return Result.Fail(message);
            }
        }
    }
}

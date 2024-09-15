namespace Micro.NotificationService.Services.Orchestrators
{
    using FluentResults;
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Data;
    using Micro.NotificationService.Extensions;
    using Micro.NotificationService.Models;
    using Microsoft.EntityFrameworkCore;
    using System.Net;

    public class SubscriptionOrchestrator : ISubscriptionOrchestrator
    {
        private readonly Context context;

        private readonly ILogger<SubscriptionOrchestrator> logger;

        public SubscriptionOrchestrator(Context context, ILogger<SubscriptionOrchestrator> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<Result<IEnumerable<Subscription>>> GetUserSubscriptions(Guid userId)
        {
            try
            {
                var result = await this.context.Subscriptions.AsNoTracking()
                                               .Where(x => x.UserId == userId)
                                               .ToArrayAsync();

                return Result.Ok<IEnumerable<Subscription>>(result);
            }
            catch (Exception ex)
            {
                var message = string.Format("An Error ocurred while getting user subscriptions. {error}", ex);
                this.logger.LogError(message);
                return Result.Fail(message);
            }
        }

        public async Task<Result<IEnumerable<Subscription>>> ProcessSubscriptions(IEnumerable<SubscriptionMessage> subscriptions)
        {
            try
            {
                var result = new List<Subscription>();

                //var existingSubscriptions = await this.context.Subscriptions.Where(s => 
                //                            subscriptions.Any(sm => sm.UserId == s.UserId && 
                //                                              sm.NotificationType == s.NotificationType &&
                //                                              sm.Channel == s.Channel)).ToArrayAsync();

                //foreach(var existingSubscription in existingSubscriptions)
                //{
                //    existingSubscription.IsSubscribed = true;
                //    result.Add(existingSubscription);
                //}

                // Create a list of anonymous objects that combine the three fields for comparison
                var subscriptionKeys = subscriptions
                    .Select(s => new { s.UserId, s.NotificationType, s.Channel })
                    .ToList();

                var whereClause = string.Join(" OR ", subscriptions.Select(s => $"(UserId={s.UserId} AND NotificationType='{s.NotificationType}' AND Channel='{s.Channel}')"));

                // Fetch all existing subscriptions where the combination of UserId, NotificationType, and Channel match
                var existingSubscriptions = await this.context.Subscriptions.FromSqlRaw("SELECT * FROM Subscriptions WHERE " + whereClause).ToListAsync();

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
                    es.UserId == sm.UserId && es.NotificationType == sm.NotificationType && es.Channel == sm.Channel)).ToList();

                foreach (var newSubscription in newSubscriptions)
                {
                    var model = newSubscription.ToModel();
                    result.Add(model);
                    this.context.Subscriptions.Add(model);
                }

                //foreach (var subscription in subscriptions)
                //{
                //    // Check if the subscription already exist
                //    var existingSubscription = await this.context.Subscriptions.FirstOrDefaultAsync(p =>
                //                                          p.UserId == subscription.UserId &&
                //                                          p.NotificationType == subscription.NotificationType &&
                //                                          p.Channel == subscription.Channel);

                //    if (existingSubscription != null)
                //    {
                //        existingSubscription.IsSubscribed = subscription.IsSubscribed;
                //        result.Add(existingSubscription);
                //    }
                //    else
                //    {
                //        var model = subscription.ToModel();
                //        result.Add(model);
                //        this.context.Subscriptions.Add(model);
                //    }
                //}

                await this.context.SaveChangesAsync();
                return Result.Ok(result.AsEnumerable());
            }
            catch (Exception ex)
            {
                var message = string.Format("An Error ocurred while processing the subscription. {error}", ex);
                this.logger.LogError(message);
                return Result.Fail(message);
            }
        }

        public async Task<Result> DeleteSubscriptions(IEnumerable<Guid> subscriptionIds)
        {
            try
            {
                var subscriptions = await this.context.Subscriptions.Where(s => subscriptionIds.Contains(s.Id)).ToListAsync();

                if (subscriptions.Any())
                {
                    subscriptions.ForEach(s => s.IsSubscribed = false);
                    await context.SaveChangesAsync();
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

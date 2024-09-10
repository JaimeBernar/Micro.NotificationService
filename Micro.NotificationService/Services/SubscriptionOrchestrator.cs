namespace Micro.NotificationService.Services
{
    using FluentResults;
    using Microsoft.EntityFrameworkCore;
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Data;
    using Micro.NotificationService.Extensions;
    using Micro.NotificationService.Models;
    using System.Net;
    using System.Linq.Expressions;

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
                var result = await this.context.Subscriptions.Where(x => x.UserId == userId).ToArrayAsync();
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

                foreach (var subscription in subscriptions)
                {
                    // Check if the subscription already exist
                    var existingSubscription = await this.context.Subscriptions.FirstOrDefaultAsync(p =>
                                                          p.UserId == subscription.UserId &&
                                                          p.NotificationType == subscription.NotificationType &&
                                                          p.Channel == subscription.Channel);

                    if (existingSubscription != null)
                    {
                        existingSubscription.IsSubscribed = subscription.IsSubscribed;
                        result.Add(existingSubscription);
                    }
                    else
                    {
                        var model = subscription.ToModel();
                        result.Add(model);
                        this.context.Subscriptions.Add(model);
                    }
                }

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
                    await this.context.SaveChangesAsync();
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

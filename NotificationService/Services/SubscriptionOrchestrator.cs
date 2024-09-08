namespace NotificationService.Services
{
    using FluentResults;
    using Microsoft.EntityFrameworkCore;
    using NotificationService.Common.DTOs;
    using NotificationService.Data;
    using NotificationService.Extensions;
    using NotificationService.Models;

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

        public async Task<Result<Subscription>> ProcessSubscription(SubscriptionMessage subscription)
        {
            try
            {
                // Check if the subscription already exist
                var existingSubscription = await this.context.Subscriptions.FirstOrDefaultAsync(p =>
                                                      p.UserId == subscription.UserId &&
                                                      p.NotificationType == subscription.NotificationType &&
                                                      p.Channel == subscription.Channel);

                if (existingSubscription != null)
                {
                    existingSubscription.IsSubscribed = subscription.IsSubscribed;
                    await this.context.SaveChangesAsync();
                    return Result.Ok(existingSubscription);
                }

                var model = subscription.ToModel();
                this.context.Subscriptions.Add(model);
                await this.context.SaveChangesAsync();
                return Result.Ok(model);

            }
            catch (Exception ex)
            {
                var message = string.Format("An Error ocurred while processing the subscription. {error}", ex);
                this.logger.LogError(message);
                return Result.Fail(message);
            }
        }
    }
}

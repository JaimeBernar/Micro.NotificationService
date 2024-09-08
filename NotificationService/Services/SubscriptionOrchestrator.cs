namespace NotificationService.Services
{
    using Microsoft.EntityFrameworkCore;
    using NotificationService.Common.DTOs;
    using NotificationService.Data;
    using NotificationService.Extensions;

    public class SubscriptionOrchestrator : ISubscriptionOrchestrator
    {
        private readonly Context context;

        public SubscriptionOrchestrator(Context context)
        {
            this.context = context;
        }

        public async Task ProcessSubscription(SubscriptionMessage subscription)
        {
            // Check if the subscription already exist
            var existingSubscription = await this.context.Subscriptions.FirstOrDefaultAsync(p => 
                                                  p.UserId == subscription.UserId &&
                                                  p.NotificationType == subscription.NotificationType &&
                                                  p.Channel == subscription.Channel);

            if(existingSubscription != null)
            {
                existingSubscription.IsSubscribed = subscription.IsSubscribed;
                await this.context.SaveChangesAsync();
                return;
            }

            this.context.Subscriptions.Add(subscription.ToModel());
            await this.context.SaveChangesAsync();
        }
    }
}

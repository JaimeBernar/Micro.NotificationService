namespace Micro.NotificationService.Extensions
{
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Models;

    public static class DTOExtensions
    {
        public static Subscription ToModel(this SubscriptionMessage dto)
        {
            return new Subscription
            {
                UserId = dto.UserId,
                NotificationType = dto.NotificationType,
                Channel = dto.Channel,
                IsSubscribed = dto.IsSubscribed,
                EmailAddress = dto.EmailAddress,
                SubscriberName = dto.SubscriberName
            };
        }
    }
}

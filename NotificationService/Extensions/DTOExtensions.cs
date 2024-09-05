using NotificationService.Common.DTOs;
using NotificationService.Models;

namespace NotificationService.Extensions
{
    public static class DTOExtensions
    {
        public static Subscription ToModel(this SubscriptionDto dto)
        {
            return new Subscription
            {
                UserId = dto.UserId,
                NotificationType = dto.NotificationType,
                Channel = dto.Channel,
                IsSubscribed = dto.IsSubscribed,
                EmailAddress = dto.EmailAddress,
            };
        }
    }
}

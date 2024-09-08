using NotificationService.Common.DTOs;
using NotificationService.Models;

namespace NotificationService.Extensions
{
    public static class DTOExtensions
    {
        public static Models.Subscription ToModel(this Common.DTOs.SubscriptionMessage dto)
        {
            return new Models.Subscription
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

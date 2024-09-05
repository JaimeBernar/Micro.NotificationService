namespace NotificationService.Common.DTOs
{
    using System;
    using NotificationService.Common.Enums;

    public class SubscriptionDto
    {
        public Guid UserId { get; set; }

        public string NotificationType { get; set; }

        public NotificationChannel Channel { get; set; }

        public bool IsSubscribed { get; set; }
    }
}

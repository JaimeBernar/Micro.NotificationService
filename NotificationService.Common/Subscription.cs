namespace NotificationService.Common
{
    using System;

    public class Subscription
    {
        public Guid UserId { get; set; }

        public string NotificationType { get; set; }
    }
}

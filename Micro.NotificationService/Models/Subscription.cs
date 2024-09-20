namespace Micro.NotificationService.Models
{
    using Micro.NotificationService.Common.Enums;
    using System;

    public class Subscription 
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string NotificationType { get; set; }

        public string EmailAddress { get; set; }

        public string SubscriberName { get; set; }

        public NotificationChannel Channel { get; set; }

        public bool IsSubscribed { get; set; }
    }
}

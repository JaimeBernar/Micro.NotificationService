namespace NotificationService.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using NotificationService.Common.Enums;

    [Table(nameof(Subscription))]
    public class Subscription : Entity
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string NotificationType { get; set; }

        [Required]
        public string EmailAddress { get; set; }

        public string SubscriberName { get; set; }

        public NotificationChannel Channel { get; set; }

        public bool IsSubscribed { get; set; }
    }
}

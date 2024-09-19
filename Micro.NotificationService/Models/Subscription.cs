namespace Micro.NotificationService.Models
{
    using Micro.NotificationService.Common.Enums;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics.CodeAnalysis;

    [Table(nameof(Subscription))]
    public class Subscription : Entity
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [NotNull]
        public string NotificationType { get; set; }

        [Required]
        [NotNull]
        public string EmailAddress { get; set; }

        public string SubscriberName { get; set; }

        public NotificationChannel Channel { get; set; }

        public bool IsSubscribed { get; set; }
    }
}

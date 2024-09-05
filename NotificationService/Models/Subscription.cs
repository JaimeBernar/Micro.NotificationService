namespace NotificationService.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using NotificationService.Common.Enums;

    public class Subscription
    {
        [Required]
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string NotificationType { get; set; }

        public NotificationChannel Channel { get; set; }

        public bool IsSubscribed { get; set; }
    }
}

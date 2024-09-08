namespace NotificationService.Models
{
    using NotificationService.Common.Enums;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class DirectNotification
    {
        [Required]
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }
                
        public string EmailAddress { get; set; }

        public string Header { get; set; }

        public string Body { get; set; }

        public NotificationChannel Channel { get; set; }
    }
}

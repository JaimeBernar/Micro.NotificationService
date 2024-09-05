namespace NotificationService.Models
{
    using NotificationService.Common.Enums;
    using System.ComponentModel.DataAnnotations;

    public class Notification
    {
        [Required]
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string NotificationType { get; set; }

        public NotificationChannel Channel { get; set; }

        public string Header { get; set; }

        public string Body { get; set; }
    }
}

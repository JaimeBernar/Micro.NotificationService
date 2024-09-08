namespace NotificationService.Models
{
    using NotificationService.Common.Enums;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table(nameof(Notification))]
    public class Notification : Entity
    {
        public string NotificationType { get; set; }

        public NotificationChannel Channel { get; set; }

        public string Header { get; set; }

        public string Body { get; set; }
    }
}

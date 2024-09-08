namespace Micro.NotificationService.Models
{
    using Micro.NotificationService.Common.Enums;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table(nameof(Notification))]
    public class Notification : Entity
    {
        public Guid UserId { get; set; }

        public string NotificationType { get; set; }

        public NotificationChannel Channel { get; set; }

        public string Header { get; set; }

        public string Body { get; set; }
    }
}

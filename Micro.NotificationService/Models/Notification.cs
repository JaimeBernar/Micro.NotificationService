namespace Micro.NotificationService.Models
{
    using Micro.NotificationService.Common.Enums;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics.CodeAnalysis;

    [Table(nameof(Notification))]
    public class Notification : Entity
    {
        [Required]
        public Guid UserId { get; set; }

        public string? ReceiverName { get; set; }

        public string? EmailAddress { get; set; }

        public string? NotificationType { get; set; }

        public NotificationChannel Channel { get; set; }

        public string? Header { get; set; }

        [NotNull]
        public string Body { get; set; } = null!;

        public DateTime CreatedAt { get; } = DateTime.UtcNow;

        public bool IsDirect { get; set; }

        public bool IsReaded { get; set; }
    }
}

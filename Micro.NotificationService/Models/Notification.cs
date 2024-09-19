namespace Micro.NotificationService.Models
{
    using Micro.NotificationService.Common.Enums;

    public class Notification 
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string? ReceiverName { get; set; }

        public string? EmailAddress { get; set; }

        public string? NotificationType { get; set; }

        public NotificationChannel Channel { get; set; }

        public string? Header { get; set; }

        public string Body { get; set; } = null!;

        public DateTime CreatedAt { get; } = DateTime.UtcNow;

        public bool IsDirect { get; set; }

        public bool IsReaded { get; set; }
    }
}

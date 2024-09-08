namespace NotificationService.Common.DTOs
{
    using System;

    public class Notification
    {
        public Guid Id { get; internal set; }

        public string Header { get; set; }

        public string Body { get; set; }

        public DateTime CreatedAt { get; } = DateTime.UtcNow;
    }
}

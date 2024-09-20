namespace Micro.NotificationService.Common.DTOs
{
    using System;

    public class OutNotification
    {
        public int Id { get; set; }

        public string Header { get; set; }

        public string Body { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

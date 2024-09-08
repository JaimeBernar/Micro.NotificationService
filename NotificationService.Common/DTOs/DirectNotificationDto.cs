namespace NotificationService.Common.DTOs
{
    using System;
    using NotificationService.Common.Enums;

    public class DirectNotificationDto
    {
        public Guid UserId { get; set; }

        public string EmailAddress { get; set; }

        public string Header { get; set; }

        public string Body { get; set; }

        public NotificationChannel Channel { get; set; }
    }
}

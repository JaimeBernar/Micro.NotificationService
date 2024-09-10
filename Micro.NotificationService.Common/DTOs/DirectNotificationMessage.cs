namespace Micro.NotificationService.Common.DTOs
{
    using System;
    using Micro.NotificationService.Common.Enums;

    public class DirectNotificationMessage
    {
        public Guid UserId { get; set; }

        public string ReceiverName { get; set; }

        public string EmailAddress { get; set; }

        public string Header { get; set; }

        public string Body { get; set; }

        public string NotificationType { get; set; }

        public NotificationChannel Channel { get; set; }
    }
}

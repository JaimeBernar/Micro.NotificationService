namespace NotificationService.Common
{
    using System;

    public class DirectNotification
    {
        public Guid UserId { get; set; }

        public string EmailAddress { get; set; }

        public string Header { get; set; }

        public string Body { get; set; }

        public NotificationChannel Channel { get; set; }
    }
}

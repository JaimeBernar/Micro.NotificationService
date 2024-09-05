namespace NotificationService.Common.DTOs
{
    using NotificationService.Common.Enums;

    public class NotificationDto
    {
        public string NotificationType { get; set; }

        public NotificationChannel Channel { get; set; }

        public string Header { get; set; }

        public string Body { get; set; }
    }
}

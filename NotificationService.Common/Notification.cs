namespace NotificationService.Common
{
    public class Notification
    {
        public string NotificationType { get; set; }

        public NotificationChannel Channel { get; set; }

        public string Header { get; set; }

        public string Body { get; set; }
    }
}

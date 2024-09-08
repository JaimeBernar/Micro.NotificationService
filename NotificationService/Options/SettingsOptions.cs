namespace NotificationService.Options
{
    public class SettingsOptions
    {
        public const string Settings = "Settings";

        public bool EmailNotificationsActive { get; set; }

        public bool WebNotificationsActive { get; set; }
    }
}

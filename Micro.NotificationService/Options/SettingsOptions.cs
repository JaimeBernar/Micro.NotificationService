namespace Micro.NotificationService.Options
{
    public class SettingsOptions
    {
        public const string Settings = "Settings";

        public bool EmailNotificationsActive { get; set; }

        public bool WebNotificationsActive { get; set; }

        public int BatchSize { get; set; }

        public int BatchTime { get; set; }
    }
}

namespace NotificationService.Options
{
    public class EmailServerOptions
    {
        public const string EmailServer = "EmailServer";

        public string Host { get; set; }

        public int Port { get; set; }

        public bool UseSenderInfo { get; set; }

        public string EmailSenderName { get; set; }

        public string EmailSenderAddress { get; set; }
    }
}

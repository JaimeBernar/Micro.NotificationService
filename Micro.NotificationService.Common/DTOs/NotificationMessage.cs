namespace Micro.NotificationService.Common.DTOs
{
    public class NotificationMessage
    {
        public string NotificationType { get; set; }

        public string Header { get; set; }

        public string Body { get; set; }

        
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is NotificationMessage other))
            {
                return false;
            }
                                    
            return string.Equals(this.NotificationType, other.NotificationType);
        }
                
        public override int GetHashCode()
        {
            return this.NotificationType.GetHashCode();
        }
    }
}

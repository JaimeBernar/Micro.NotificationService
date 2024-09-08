namespace NotificationService.Common.DTOs
{
    using System;
    using NotificationService.Common.Enums;

    public class SubscriptionDto
    {
        public Guid UserId { get; set; }

        public string EmailAddress { get; set; }

        public string NotificationType { get; set; }

        public NotificationChannel Channel { get; set; }

        public bool IsSubscribed { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is SubscriptionDto other))
            {
                return false;
            }

            return string.Equals(this.NotificationType, other.NotificationType) && 
                   this.Channel == other.Channel &&
                   this.UserId == other.UserId && 
                   string.Equals(this.EmailAddress, other.EmailAddress);
        }

        public override int GetHashCode()
        {
            return this.NotificationType.GetHashCode() * this.Channel.GetHashCode() * this.UserId.GetHashCode();
        }
    }
}

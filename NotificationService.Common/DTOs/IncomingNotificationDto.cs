﻿namespace NotificationService.Common.DTOs
{
    using NotificationService.Common.Enums;

    public class IncomingNotificationDto
    {
        public string NotificationType { get; set; }

        public NotificationChannel Channel { get; set; }

        public string Header { get; set; }

        public string Body { get; set; }

        
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is IncomingNotificationDto other))
            {
                return false;
            }
                                    
            return string.Equals(this.NotificationType, other.NotificationType) && this.Channel == other.Channel;
        }
                
        public override int GetHashCode()
        {
            return this.NotificationType.GetHashCode() * this.Channel.GetHashCode();
        }
    }
}

namespace NotificationService.Models
{
    using NotificationService.Common.Enums;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table(nameof(DirectNotification))]
    public class DirectNotification : Entity
    {
        public Guid UserId { get; set; }

        public string ReceiverName { get; set; }
                
        public string EmailAddress { get; set; }

        public string Header { get; set; }

        public string Body { get; set; }

        public NotificationChannel Channel { get; set; }
    }
}

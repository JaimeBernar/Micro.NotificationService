namespace Micro.NotificationService.Services.Data
{
    using LiteDB;
    using Micro.NotificationService.Models;
    using Micro.NotificationService.Options;
    using Microsoft.Extensions.Options;

    public class DataService : IDataService
    {       
        public DataService(IOptions<SettingsOptions> settings) : this(settings.Value.DatatabasePathAndName)
        {
        }

        public DataService(string databasePathAndName)
        {
            this.Database = new LiteDatabase(databasePathAndName);

            //Make sure that the collections exist
            this.Notifications = this.Database.GetCollection<Notification>();
            this.Notifications.EnsureIndex(x => x.Id);
            this.Notifications.EnsureIndex(x => x.NotificationType);
            this.Notifications.EnsureIndex(x => x.Channel);
            this.Notifications.EnsureIndex(x => x.UserId);
            this.Notifications.EnsureIndex(x => x.EmailAddress);

            this.Subscriptions = this.Database.GetCollection<Subscription>();
            this.Subscriptions.EnsureIndex(x => x.Id);
            this.Subscriptions.EnsureIndex(x => x.NotificationType);
            this.Subscriptions.EnsureIndex(x => x.Channel);
            this.Subscriptions.EnsureIndex(x => x.UserId);
            this.Subscriptions.EnsureIndex(x => x.EmailAddress);
        }

        public LiteDatabase Database { get; set; }

        public ILiteCollection<Notification> Notifications { get; }

        public ILiteCollection<Subscription> Subscriptions { get; }

        public void Dispose()
        {            
            this.Database?.Dispose();
        }
    }
}

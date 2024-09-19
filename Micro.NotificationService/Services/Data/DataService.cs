namespace Micro.NotificationService.Services.Data
{
    using LiteDB;
    using Micro.NotificationService.Models;

    public class DataService : IDataService
    {       
        public DataService()
        {
            var path = Path.Combine("..", "..", "Data", "Database.db");
            this.Database = new LiteDatabase(path);

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

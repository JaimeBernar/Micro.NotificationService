namespace Micro.NotificationService.Services.Data
{
    using LiteDB;
    using Micro.NotificationService.Models;

    public interface IDataService : IDisposable
    {
        LiteDatabase Database { get; set; }

        ILiteCollection<Notification> Notifications { get; }

        ILiteCollection<Subscription> Subscriptions { get; }
    }
}

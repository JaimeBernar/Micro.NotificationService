namespace Micro.NotificationService.Data
{
    using Microsoft.EntityFrameworkCore;
    using Micro.NotificationService.Models;

    public class Context(DbContextOptions<Context> options) : DbContext(options)
    {
        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }
    }
}

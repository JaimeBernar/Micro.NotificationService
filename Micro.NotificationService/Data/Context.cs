namespace Micro.NotificationService.Data
{
    using Micro.NotificationService.Models;
    using Microsoft.EntityFrameworkCore;

    public class Context(DbContextOptions<Context> options) : DbContext(options)
    {
        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }
    }
}

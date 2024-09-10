namespace Micro.NotificationService.Data
{
    using Microsoft.EntityFrameworkCore;
    using Micro.NotificationService.Models;

    public class Context(DbContextOptions<Context> options) : DbContext(options)
    {
        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var path = Path.Combine("Data", "database.db");
            optionsBuilder.UseSqlite($"Data Source={path}");
        }
    }
}

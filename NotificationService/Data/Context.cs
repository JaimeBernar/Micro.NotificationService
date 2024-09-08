namespace NotificationService.Data
{
    using Microsoft.EntityFrameworkCore;
    using NotificationService.Models;

    public class Context : DbContext
    {
        public DbSet<DirectNotification> DirectNotifications { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var path = Path.Combine("Data", "database.db");
            optionsBuilder.UseSqlite($"Data Source={path}");
        }
    }
}

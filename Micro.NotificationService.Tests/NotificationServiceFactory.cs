namespace Micro.NotificationService.Tests
{
    using FluentResults;
    using Micro.NotificationService.Models;
    using Micro.NotificationService.Services.Email;
    using Micro.NotificationService.Services.Web;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Data.Sqlite;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;

    public class NotificationServiceFactory : WebApplicationFactory<Program>
    {
        private SqliteConnection connection;

        public NotificationServiceFactory()
        {
            var dbConn = new SqliteConnection("Filename=:memory:");
            dbConn.Open();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");

            builder.ConfigureServices(services =>
            {
                //// Remove the existing DbContext registration
                //var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(Context));

                //if (descriptor != null)
                //{
                //    services.Remove(descriptor);
                //}

                //// Re-register the DbContext with SQLite in-memory database
                //services.AddDbContext<Context>(options => options.UseSqlite(this.connection));

                //// Build the service provider
                //var sp = services.BuildServiceProvider();

                //// Ensure database schema is applied
                //using var scope = sp.CreateScope();
                //var scopedServices = scope.ServiceProvider;
                //var db = scopedServices.GetRequiredService<Context>();

                //db.Database.EnsureCreated();

                var emailServiceMock = new Mock<IEmailService>();
                emailServiceMock.Setup(x => x.SendEmailNotification(It.IsAny<IEnumerable<Notification>>())).ReturnsAsync(Result.Ok());
                services.AddScoped(_ => emailServiceMock.Object);

                var webServiceMock = new Mock<IWebService>();
                webServiceMock.Setup(x => x.SendWebNotifications(It.IsAny<IEnumerable<Notification>>())).ReturnsAsync(Result.Ok());
                services.AddScoped(_ => webServiceMock.Object);
            });
        }
    }
}

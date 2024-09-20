namespace Micro.NotificationService.Tests
{
    using FluentResults;
    using Micro.NotificationService.Models;
    using Micro.NotificationService.Services.Email;
    using Micro.NotificationService.Services.Web;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;

    public class NotificationServiceFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");

            builder.ConfigureServices(services =>
            {
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

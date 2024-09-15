namespace Micro.NotificationService.Tests.Tests
{
    using FluentResults;
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Data;
    using Micro.NotificationService.Models;
    using Micro.NotificationService.Options;
    using Micro.NotificationService.Services.Email;
    using Micro.NotificationService.Services.Orchestrators;
    using Micro.NotificationService.Services.Translator;
    using Micro.NotificationService.Services.Web;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;

    public class NotificationOrchestratorTestFixture
    {
        private Context context;
        private NotificationOrchestrator orchestrator;
        private Mock<INotificationTranslator> translator;
        private Mock<IEmailService> emailService;
        private Mock<IWebService> webService;
        private Mock<IOptions<SettingsOptions>> options;
        private Mock<ILogger<NotificationOrchestrator>> logger;
        private Notification notification;
        private Guid userId;

        public NotificationOrchestratorTestFixture()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            this.context = new Context(options);
            this.context.Database.OpenConnection();
            this.context.Database.EnsureCreated();

            this.userId = Guid.NewGuid();
            this.translator = new Mock<INotificationTranslator>();
            this.emailService = new Mock<IEmailService>();
            this.emailService.Setup(x => x.SendEmailNotification(It.IsAny<IEnumerable<Notification>>())).ReturnsAsync(Result.Ok());
            this.webService = new Mock<IWebService>();
            this.webService.Setup(x => x.SendWebNotifications(It.IsAny<IEnumerable<Notification>>())).ReturnsAsync(Result.Ok());
            this.options = new Mock<IOptions<SettingsOptions>>();

            var settings = new SettingsOptions
            {
                EmailNotificationsActive = true,
                WebNotificationsActive = true,
            };

            this.options.Setup(x => x.Value).Returns(settings);
            this.logger = new Mock<ILogger<NotificationOrchestrator>>();

            this.orchestrator = new NotificationOrchestrator(this.context,
                                                             this.translator.Object,
                                                             this.emailService.Object,
                                                             this.webService.Object,
                                                             this.options.Object,
                                                             this.logger.Object);
        }

        [Fact]
        public async Task VerifyGetUserNotifications()
        {
            await this.VerifyProcessNotifications();

            var result = await this.orchestrator.GetUserNotifications(this.userId);

            Assert.True(result.IsSuccess);

            var values = result.Value;

            Assert.Multiple(() =>
            {
                Assert.Single(values);
                Assert.Equal(this.notification, values.First());
            });
        }

        [Fact]
        public async Task VerifyProcessNotifications()
        {
            this.notification = new Notification()
            {
                Id = Guid.NewGuid(),
                UserId = this.userId,
                Header = "Hey",
                Body = "It's my Spiderman",
                NotificationType = "Hello",
                EmailAddress = "spiderman@avengers.com",
                ReceiverName = "Peter Parker",
                Channel = Common.Enums.NotificationChannel.Email,
            };

            this.translator.Setup(x => x.ComputeNotifications(It.IsAny<IEnumerable<NotificationMessage>>())).ReturnsAsync([this.notification]);

            var result = await this.orchestrator.ProcessNotifications([new NotificationMessage()]);

            Assert.True(result.IsSuccess);

            this.emailService.Verify(x => x.SendEmailNotification(It.IsAny<IEnumerable<Notification>>()), Times.Once());

            this.notification.Channel = Common.Enums.NotificationChannel.Web;

            result = await this.orchestrator.ProcessNotifications([new NotificationMessage()]);

            Assert.True(result.IsSuccess);

            this.webService.Verify(x => x.SendWebNotifications(It.IsAny<IEnumerable<Notification>>()), Times.Once());
            this.context.Notifications.Contains(this.notification);
        }

        [Fact]
        public async Task VerifyProcessDirectNotifications()
        {
            this.notification = new Notification()
            {
                Id = Guid.NewGuid(),
                UserId = this.userId,
                Header = "Hey",
                Body = "It's my Spiderman",
                NotificationType = "Hello",
                EmailAddress = "spiderman@avengers.com",
                ReceiverName = "Peter Parker",
                Channel = Common.Enums.NotificationChannel.Email,
                IsDirect = true,
            };

            this.translator.Setup(x => x.ComputeNotifications(It.IsAny<IEnumerable<DirectNotificationMessage>>())).Returns([this.notification]);

            var result = await this.orchestrator.ProcessDirectNotifications([new DirectNotificationMessage()]);

            Assert.True(result.IsSuccess);

            this.emailService.Verify(x => x.SendEmailNotification(It.IsAny<IEnumerable<Notification>>()), Times.Once());

            this.notification.Channel = Common.Enums.NotificationChannel.Web;

            result = await this.orchestrator.ProcessDirectNotifications([new DirectNotificationMessage()]);

            Assert.True(result.IsSuccess);

            this.webService.Verify(x => x.SendWebNotifications(It.IsAny<IEnumerable<Notification>>()), Times.Once());
            this.context.Notifications.Contains(notification);
        }

        [Fact]
        public async Task VerifyDeleteNotifications()
        {
            await this.VerifyGetUserNotifications();

            await this.orchestrator.DeleteNotifications([this.notification.Id]);

            var result = await this.orchestrator.GetUserNotifications(this.userId);

            Assert.True(result.IsSuccess);

            var values = result.Value;

            Assert.Empty(values);
        }
    }
}

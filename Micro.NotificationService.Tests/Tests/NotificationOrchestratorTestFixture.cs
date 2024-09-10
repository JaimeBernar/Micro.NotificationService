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
    using NUnit.Framework;

    [TestFixture]
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

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            this.context = new Context(options);
            this.context.Database.OpenConnection(); // Open connection to the SQLite in-memory DB
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

        [Test]
        public async Task VerifyGetUserNotifications()
        {
            await this.VerifyProcessNotifications();

            var result = await this.orchestrator.GetUserNotifications(this.userId);

            Assert.That(result.IsSuccess, Is.True);

            var values = result.Value;

            Assert.Multiple(() =>
            {
                Assert.That(values.Count(), Is.EqualTo(1));
                Assert.That(values.First(), Is.EqualTo(this.notification));
            });
        }

        [Test]
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

            Assert.That(result.IsSuccess, Is.True);

            this.emailService.Verify(x => x.SendEmailNotification(It.IsAny<IEnumerable<Notification>>()), Times.Once());

            this.notification.Channel = Common.Enums.NotificationChannel.Web;

            result = await this.orchestrator.ProcessNotifications([new NotificationMessage()]);

            Assert.That(result.IsSuccess, Is.True);

            this.webService.Verify(x => x.SendWebNotifications(It.IsAny<IEnumerable<Notification>>()), Times.Once());
            this.context.Notifications.Contains(this.notification);
        }

        [Test]
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

            Assert.That(result.IsSuccess, Is.True);

            this.emailService.Verify(x => x.SendEmailNotification(It.IsAny<IEnumerable<Notification>>()), Times.Once());

            this.notification.Channel = Common.Enums.NotificationChannel.Web;

            result = await this.orchestrator.ProcessDirectNotifications([new DirectNotificationMessage()]);

            Assert.That(result.IsSuccess, Is.True);

            this.webService.Verify(x => x.SendWebNotifications(It.IsAny<IEnumerable<Notification>>()), Times.Once());
            this.context.Notifications.Contains(notification);
        }

        [Test]
        public async Task VerifyDeleteNotifications()
        {
            await this.VerifyGetUserNotifications();

            await this.orchestrator.DeleteNotifications([this.notification.Id]);

            var result = await this.orchestrator.GetUserNotifications(this.userId);

            Assert.That(result.IsSuccess, Is.True);

            var values = result.Value;

            Assert.That(values.Count(), Is.EqualTo(0));
        }
    }
}

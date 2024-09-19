namespace Micro.NotificationService.Tests.Tests
{
    using FluentResults;
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Common.Enums;
    using Micro.NotificationService.Models;
    using Micro.NotificationService.Options;
    using Micro.NotificationService.Services.Data;
    using Micro.NotificationService.Services.Email;
    using Micro.NotificationService.Services.Orchestrators;
    using Micro.NotificationService.Services.Translator;
    using Micro.NotificationService.Services.Web;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;

    public class NotificationOrchestratorTestFixture : IDisposable
    {
        private DataService dataService;
        private NotificationOrchestrator orchestrator;
        private Mock<INotificationTranslator> translator;
        private Mock<IEmailService> emailService;
        private Mock<IWebService> webService;
        private Mock<IOptions<SettingsOptions>> options;
        private Mock<ILogger<NotificationOrchestrator>> logger;
        private Notification notification;
        private string userId;
        private int webNotificationId = 1;
        private int emailNotificationId = 2;

        public NotificationOrchestratorTestFixture()
        {
            this.dataService = new DataService(Path.Combine(Directory.GetCurrentDirectory(), "notification-tests.db"));
            this.dataService.Notifications.DeleteAll();
            this.dataService.Subscriptions.DeleteAll();
            this.userId = Guid.NewGuid().ToString();
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

            this.orchestrator = new NotificationOrchestrator(this.dataService,
                                                             this.translator.Object,
                                                             this.emailService.Object,
                                                             this.webService.Object,
                                                             this.options.Object,
                                                             this.logger.Object);
        }

        public void Dispose()
        {
            this.dataService.Dispose();
        }

        [Fact]
        public async Task VerifyGetUserNotifications()
        {
            await this.VerifyProcessNotifications();

            var result = this.orchestrator.GetUserNotifications(this.userId);

            Assert.True(result.IsSuccess);

            var values = result.Value;

            var value = values.First();

            Assert.Multiple(() =>
            {
                Assert.Single(values);
                Assert.Equal(this.notification.Id, value.Id);
                Assert.Equal(this.notification.Header, value.Header);
                Assert.Equal(this.notification.Body, value.Body);
                Assert.Equal(this.notification.ReceiverName, value.ReceiverName);
                Assert.Equal(this.notification.Channel, value.Channel);
                Assert.Equal(this.notification.IsReaded, value.IsReaded);
                Assert.Equal(this.notification.EmailAddress, value.EmailAddress);
            });
        }

        [Fact]
        public async Task VerifyProcessNotifications()
        {
            this.notification = new Notification()
            {
                Id = emailNotificationId,
                UserId = this.userId,
                Header = "Hey",
                Body = "It's me Spiderman",
                NotificationType = "Hello",
                EmailAddress = "spiderman@avengers.com",
                ReceiverName = "Peter Parker",
                Channel = NotificationChannel.Email,
            };

            this.translator.Setup(x => x.ComputeNotifications(It.IsAny<IEnumerable<NotificationMessage>>())).Returns([this.notification]);

            var result = await this.orchestrator.ProcessNotifications([new NotificationMessage()]);

            Assert.True(result.IsSuccess);

            this.emailService.Verify(x => x.SendEmailNotification(It.IsAny<IEnumerable<Notification>>()), Times.Once());

            this.notification.Channel = NotificationChannel.Web;
            this.notification.Id = webNotificationId;

            result = await this.orchestrator.ProcessNotifications([new NotificationMessage()]);

            Assert.True(result.IsSuccess);

            this.webService.Verify(x => x.SendWebNotifications(It.IsAny<IEnumerable<Notification>>()), Times.Once());
            this.dataService.Notifications.Query().ToList().Contains(this.notification);
        }

        [Fact]
        public async Task VerifyProcessDirectNotifications()
        {
            this.notification = new Notification()
            {
                Id = emailNotificationId,
                UserId = this.userId,
                Header = "Hey",
                Body = "It's me Spiderman",
                NotificationType = "Hello",
                EmailAddress = "spiderman@avengers.com",
                ReceiverName = "Peter Parker",
                Channel = NotificationChannel.Email,
                IsDirect = true,
            };

            this.translator.Setup(x => x.ComputeNotifications(It.IsAny<IEnumerable<DirectNotificationMessage>>())).Returns([this.notification]);

            var result = await this.orchestrator.ProcessDirectNotifications([new DirectNotificationMessage()]);

            Assert.True(result.IsSuccess);

            this.emailService.Verify(x => x.SendEmailNotification(It.IsAny<IEnumerable<Notification>>()), Times.Once());

            this.notification.Channel = NotificationChannel.Web;
            this.notification.Id = webNotificationId;

            result = await this.orchestrator.ProcessDirectNotifications([new DirectNotificationMessage()]);

            Assert.True(result.IsSuccess);

            this.webService.Verify(x => x.SendWebNotifications(It.IsAny<IEnumerable<Notification>>()), Times.Once());
            this.dataService.Notifications.Query().ToList().Contains(notification);
        }

        [Fact]
        public async Task VerifyDeleteNotifications()
        {
            await this.VerifyGetUserNotifications();

            this.orchestrator.DeleteNotifications([emailNotificationId, webNotificationId]);

            var result = this.orchestrator.GetUserNotifications(this.userId);

            Assert.True(result.IsSuccess);

            var values = result.Value;

            Assert.Empty(values);
        }
    }
}

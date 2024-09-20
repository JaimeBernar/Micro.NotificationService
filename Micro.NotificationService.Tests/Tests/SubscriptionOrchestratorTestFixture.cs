namespace Micro.NotificationService.Tests.Tests
{
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Models;
    using Micro.NotificationService.Services.Data;
    using Micro.NotificationService.Services.Orchestrators;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class SubscriptionOrchestratorTestFixture : IDisposable
    {
        private DataService dataService;
        private SubscriptionOrchestrator orchestrator;
        private Mock<ILogger<SubscriptionOrchestrator>> logger;
        private Subscription subscription;
        private string userId;

        public SubscriptionOrchestratorTestFixture()
        {
            this.dataService = new DataService(Path.Combine(Directory.GetCurrentDirectory(), "subscription-tests.db"));
            this.dataService.Notifications.DeleteAll();
            this.dataService.Subscriptions.DeleteAll();

            this.userId = Guid.NewGuid().ToString();

            this.logger = new Mock<ILogger<SubscriptionOrchestrator>>();

            this.orchestrator = new SubscriptionOrchestrator(this.dataService, this.logger.Object);
        }

        public void Dispose()
        {
            this.dataService.Dispose();
        }

        [Fact]
        public void VerifyGetUserSubscriptions()
        {
            this.VerifyProcessSubscriptions();

            var result = this.orchestrator.GetUserSubscriptions(this.userId);

            Assert.True(result.IsSuccess);

            var values = result.Value;

            Assert.Equal(2, values.Count());
        }

        [Fact]
        public void VerifyProcessSubscriptions()
        {
            var message = new SubscriptionMessage()
            {
                UserId = this.userId,
                NotificationType = "Hello",
                EmailAddress = "spiderman@avengers.com",
                SubscriberName = "Peter Parker",
                Channel = Common.Enums.NotificationChannel.Email,
            };

            var result = this.orchestrator.ProcessSubscriptions([message]);

            Assert.Multiple(() =>
            {
                Assert.True(result.IsSuccess);
                Assert.Equal(1, this.dataService.Subscriptions.Count());
            });

            this.subscription = result.Value.First();

            message.Channel = Common.Enums.NotificationChannel.Web;

            result = this.orchestrator.ProcessSubscriptions([message]);

            Assert.Multiple(() =>
            {
                Assert.True(result.IsSuccess);
                Assert.Equal(2, this.dataService.Subscriptions.Count());
            });
        }

        [Fact]
        public void VerifyDeleteSubscriptions()
        {
            this.VerifyGetUserSubscriptions();

            this.orchestrator.DeleteSubscriptions([this.subscription.Id]);

            var result = this.orchestrator.GetUserSubscriptions(this.userId);

            Assert.True(result.IsSuccess);

            var values = result.Value;

            Assert.Equal(2, values.Count());
        }
    }
}

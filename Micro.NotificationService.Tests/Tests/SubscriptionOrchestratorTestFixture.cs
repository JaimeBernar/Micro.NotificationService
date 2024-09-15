namespace Micro.NotificationService.Tests.Tests
{
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Data;
    using Micro.NotificationService.Models;
    using Micro.NotificationService.Services.Orchestrators;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class SubscriptionOrchestratorTestFixture
    {
        private Context context;
        private SubscriptionOrchestrator orchestrator;
        private Mock<ILogger<SubscriptionOrchestrator>> logger;
        private Subscription subscription;
        private Guid userId;

        public SubscriptionOrchestratorTestFixture()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            this.context = new Context(options);
            this.context.Database.OpenConnection(); // Open connection to the SQLite in-memory DB
            this.context.Database.EnsureCreated();

            this.userId = Guid.NewGuid();

            this.logger = new Mock<ILogger<SubscriptionOrchestrator>>();

            this.orchestrator = new SubscriptionOrchestrator(this.context, this.logger.Object);
        }

        [Fact]
        public async Task VerifyGetUserSubscriptions()
        {
            await this.VerifyProcessSubscriptions();

            var result = await this.orchestrator.GetUserSubscriptions(this.userId);

            Assert.True(result.IsSuccess);

            var values = result.Value;

            Assert.Equal(2, values.Count());
        }

        [Fact]
        public async Task VerifyProcessSubscriptions()
        {
            var message = new SubscriptionMessage()
            {
                UserId = this.userId,
                NotificationType = "Hello",
                EmailAddress = "spiderman@avengers.com",
                SubscriberName = "Peter Parker",
                Channel = Common.Enums.NotificationChannel.Email,
            };

            var result = await this.orchestrator.ProcessSubscriptions([message]);

            Assert.Multiple(() =>
            {
                Assert.True(result.IsSuccess);
                Assert.Equal(1, this.context.Subscriptions.Count());
            });

            this.subscription = result.Value.First();

            message.Channel = Common.Enums.NotificationChannel.Web;

            result = await this.orchestrator.ProcessSubscriptions([message]);

            Assert.Multiple(() =>
            {
                Assert.True(result.IsSuccess);
                Assert.Equal(2, this.context.Subscriptions.Count());
            });
        }

        [Fact]
        public async Task VerifyDeleteSubscriptions()
        {
            await this.VerifyGetUserSubscriptions();

            await this.orchestrator.DeleteSubscriptions([this.subscription.Id]);

            var result = await this.orchestrator.GetUserSubscriptions(this.userId);

            Assert.True(result.IsSuccess);

            var values = result.Value;

            Assert.Equal(2, values.Count());
        }
    }
}

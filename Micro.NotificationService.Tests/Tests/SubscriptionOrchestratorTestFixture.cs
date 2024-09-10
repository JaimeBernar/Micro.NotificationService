namespace Micro.NotificationService.Tests.Tests
{
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Data;
    using Micro.NotificationService.Models;
    using Micro.NotificationService.Services.Orchestrators;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class SubscriptionOrchestratorTestFixture
    {
        private Context context;
        private SubscriptionOrchestrator orchestrator;
        private Mock<ILogger<SubscriptionOrchestrator>> logger;
        private Subscription subscription;
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

            this.logger = new Mock<ILogger<SubscriptionOrchestrator>>();

            this.orchestrator = new SubscriptionOrchestrator(this.context, this.logger.Object);
        }

        [Test]
        public async Task VerifyGetUserSubscriptions()
        {
            await this.VerifyProcessSubscriptions();

            var result = await this.orchestrator.GetUserSubscriptions(this.userId);

            Assert.That(result.IsSuccess, Is.True);

            var values = result.Value;

            Assert.That(values.Count(), Is.EqualTo(2));
        }

        [Test]
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
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(this.context.Subscriptions.Count(), Is.EqualTo(1));
            });

            this.subscription = result.Value.First();

            message.Channel = Common.Enums.NotificationChannel.Web;

            result = await this.orchestrator.ProcessSubscriptions([message]);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(this.context.Subscriptions.Count(), Is.EqualTo(2));
            });
        }

        [Test]
        public async Task VerifyDeleteSubscriptions()
        {
            await this.VerifyGetUserSubscriptions();

            await this.orchestrator.DeleteSubscriptions([this.subscription.Id]);

            var result = await this.orchestrator.GetUserSubscriptions(this.userId);

            Assert.That(result.IsSuccess, Is.True);

            var values = result.Value;

            Assert.That(values.Count(), Is.EqualTo(2));
        }
    }
}

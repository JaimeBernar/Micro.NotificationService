namespace Micro.NotificationService.Tests.IntegrationTests
{
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Common.Enums;
    using Micro.NotificationService.Models;
    using System.Net;
    using System.Text;
    using System.Text.Json;
    using Xunit;

    public class NotificationModuleIntegrationTests : IClassFixture<NotificationServiceFactory>
    {
        private readonly HttpClient client;
        private readonly string userId = Guid.NewGuid().ToString();

        public NotificationModuleIntegrationTests(NotificationServiceFactory factory)
        {
            this.client = factory.CreateClient();
        }

        private async Task PostSubscription()
        {
            //1.Create subscription
            var subscription = new SubscriptionMessage
            {
                Channel = NotificationChannel.Web,
                EmailAddress = "spiderman@avengers.com",
                NotificationType = "Type",
                SubscriberName = "Peter Parker",
                UserId = this.userId,
            };

            var json = JsonSerializer.Serialize(subscription);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var subscriptionResult = await this.client.PostAsync("api/v1/subscriptions", content);

            Assert.Multiple(() =>
            {
                Assert.True(subscriptionResult.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.OK, subscriptionResult.StatusCode);
            });
        }

        [Fact]
        public async Task VerifyGetUserNotifications()
        {
            await this.PostSubscription();

            //1.Post notification
            var notification = new NotificationMessage
            {
                Body = "Which type am I?",
                Header = "Hey",
                NotificationType = "Type"
            };

            var json = JsonSerializer.Serialize(notification);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var result = await this.client.PostAsync("api/v1/notifications", content);

            Assert.Multiple(() =>
            {
                Assert.True(result.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            });

            //2.Retrieve notifications produced 
            var notificationsResult = await this.client.GetAsync($"api/v1/notifications/{userId}");

            Assert.True(notificationsResult.IsSuccessStatusCode);

            var notifications = await JsonSerializer.DeserializeAsync<IEnumerable<Notification>>(await notificationsResult.Content.ReadAsStreamAsync());

            Assert.Multiple(() =>
            {
                Assert.NotEmpty(notifications);
                Assert.Equivalent(notification, notifications.First());
            });
        }

        [Fact]
        public async Task VerifyPostNewNotification()
        {
            await this.PostSubscription();

            //Notification without type can't be processed
            var notification = new NotificationMessage
            {
                Body = "Which type am I?",
                Header = "Hey"
            };

            var json = JsonSerializer.Serialize(notification);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var result = await this.client.PostAsync("api/v1/notifications", content);

            Assert.Multiple(() =>
            {
                Assert.False(result.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.UnprocessableEntity, result.StatusCode);
            });

            //add the type
            notification.NotificationType = "Type";

            json = JsonSerializer.Serialize(notification);
            content = new StringContent(json, Encoding.UTF8, "application/json");

            result = await this.client.PostAsync("api/v1/notifications", content);

            Assert.Multiple(() =>
            {
                Assert.True(result.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            });
        }

        [Fact]
        public async Task VerifyPostNewDirectNotification()
        {
            //Notification without type and other mandatory fields can't be processed
            var notification = new DirectNotificationMessage
            {
                Body = "Which type am I?",
                Header = "Hey"
            };

            var json = JsonSerializer.Serialize(notification);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var result = await this.client.PostAsync("api/v1/direct-notifications", content);

            Assert.Multiple(() =>
            {
                Assert.False(result.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.UnprocessableEntity, result.StatusCode);
            });

            //add the type and other mandatory fields
            notification.NotificationType = "Type";
            notification.EmailAddress = "spiderman@avengers.com";

            json = JsonSerializer.Serialize(notification);
            content = new StringContent(json, Encoding.UTF8, "application/json");

            result = await this.client.PostAsync("api/v1/direct-notifications", content);

            Assert.Multiple(() =>
            {
                Assert.True(result.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            });
        }

        [Fact]
        public async Task VerifyPostNewNotifications()
        {
            await this.PostSubscription();

            //Notification without type can't be processed
            var notification = new NotificationMessage
            {
                Body = "Which type am I?",
                Header = "Hey"
            };

            List<NotificationMessage> messages = [];

            messages.Add(notification);
            var json = JsonSerializer.Serialize(messages);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var result = await this.client.PostAsync("api/v1/batch/notifications", content);

            Assert.Multiple(() =>
            {
                Assert.False(result.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.UnprocessableEntity, result.StatusCode);
            });

            //add the type
            notification.NotificationType = "Type";

            json = JsonSerializer.Serialize(messages);
            content = new StringContent(json, Encoding.UTF8, "application/json");

            result = await this.client.PostAsync("api/v1/batch/notifications", content);

            Assert.Multiple(() =>
            {
                Assert.True(result.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            });
        }

        [Fact]
        public async Task VerifyPostNewDirectNotifications()
        {
            //Notification without type can't be processed
            var notification = new DirectNotificationMessage
            {
                Body = "Which type am I?",
                Header = "Hey"
            };

            List<DirectNotificationMessage> messages = [];

            messages.Add(notification);
            var json = JsonSerializer.Serialize(messages);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var result = await this.client.PostAsync("api/v1/batch/notifications", content);

            Assert.Multiple(() =>
            {
                Assert.False(result.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.UnprocessableEntity, result.StatusCode);
            });

            //add the type
            notification.NotificationType = "Type";
            notification.EmailAddress = "spiderman@avengers.com";

            json = JsonSerializer.Serialize(messages);
            content = new StringContent(json, Encoding.UTF8, "application/json");

            result = await this.client.PostAsync("api/v1/batch/notifications", content);

            Assert.Multiple(() =>
            {
                Assert.True(result.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            });
        }
    }
}

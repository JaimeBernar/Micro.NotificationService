using Micro.NotificationService.Common.DTOs;
using Micro.NotificationService.Common.SignalR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Radzen;
using System.Text.Json;

namespace Micro.NotificationService.Web.Pages
{
    public partial class Home
    {
        [Inject]
        public HttpClient HttpClient { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IConfiguration Configuration { get; set; }

        private HubConnection HubConnection { get; set; }

        private List<OutNotification> notifications = [];

        private string apiBaseUrl;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            this.apiBaseUrl = this.Configuration["NotificationServiceSettings:ApiBaseUrl"];
            var hubUrl = this.Configuration["NotificationServiceSettings:NotificationsHubPath"];

            this.HubConnection = new HubConnectionBuilder()
                .WithUrl($"{this.apiBaseUrl}{hubUrl}")
                .WithAutomaticReconnect()
                .Build();

            this.HubConnection.On<IEnumerable<OutNotification>>(NotificationMethodNames.Receive, async (notifications) =>
            {
                this.notifications.AddRange(notifications);
                await this.InvokeAsync(this.StateHasChanged);
            });

            await this.HubConnection.StartAsync();
            await this.Refresh();
        }

        private async Task Refresh()
        {
            var response = await this.HttpClient.GetAsync($"{this.apiBaseUrl}/api/v1/notifications/3fa85f64-5717-4562-b3fc-2c963f66afa6");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var notifications = JsonSerializer.Deserialize<IEnumerable<OutNotification>>(content);

                if (notifications.Any())
                {
                    this.notifications.Clear();
                    this.notifications.AddRange(notifications);
                    await this.InvokeAsync(this.StateHasChanged);
                }
            }
        }

        private async Task DeleteNotification(OutNotification notification)
        {
            //1.Send message to API to delete from database
            var response = await this.HttpClient.DeleteAsync($"{this.apiBaseUrl}/api/v1/notifications/{notification.Id}");

            if (response.IsSuccessStatusCode)
            {
                //2.Remove it from list
                this.notifications.Remove(notification);
                await this.InvokeAsync(this.StateHasChanged);
            }
        }

        private async Task DeleteAll()
        {
            var json = JsonSerializer.Serialize(this.notifications.Select(x => x.Id));

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{this.apiBaseUrl}/api/v1/notifications"),
                Method = HttpMethod.Delete,
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"),
            };

            //1.Send message to API to delete from database
            var response = await this.HttpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                //2.Remove it from list
                this.notifications.Clear();
                await this.InvokeAsync(this.StateHasChanged);
            }
        }

        private async Task OnConnectedClicked()
        {
            if (this.HubConnection.State == HubConnectionState.Connected)
            {
                await this.HubConnection.StopAsync();
            }
            else if (this.HubConnection.State == HubConnectionState.Disconnected)
            {
                await this.HubConnection.StartAsync();
            }
        }

        private string GetIconColor()
        {
            return this.HubConnection.State switch
            {
                HubConnectionState.Disconnected => Colors.Secondary,
                HubConnectionState.Connected => Colors.Success,
                HubConnectionState.Connecting => Colors.Warning,
                HubConnectionState.Reconnecting => Colors.Warning,
                _ => Colors.Secondary,
            };
        }
    }
}

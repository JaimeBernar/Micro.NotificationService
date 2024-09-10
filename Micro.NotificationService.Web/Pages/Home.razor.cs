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

        private HubConnection HubConnection { get; set; }

        private List<OutNotification> notifications = [];

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            this.HubConnection = new HubConnectionBuilder()
                .WithUrl($"https://localhost:7070{NotificationMethodNames.HubUrl}")
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
            var response = await this.HttpClient.GetAsync("https://localhost:7070/api/v1/notifications/3fa85f64-5717-4562-b3fc-2c963f66afa6");

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

        private async Task DeleteAll()
        {
            var json = JsonSerializer.Serialize(this.notifications.Select(x => x.Id));

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri("https://localhost:7070/api/v1/notifications"),
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
            if(this.HubConnection.State == HubConnectionState.Connected)
            {
                await this.HubConnection.StopAsync();
            }
            else if(this.HubConnection.State == HubConnectionState.Disconnected)
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

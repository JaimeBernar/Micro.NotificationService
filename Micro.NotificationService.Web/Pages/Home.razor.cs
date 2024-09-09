using Micro.NotificationService.Common.DTOs;
using Micro.NotificationService.Common.SignalR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Radzen;

namespace Micro.NotificationService.Web.Pages
{
    public partial class Home
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        private HubConnection HubConnection { get; set; }

        private List<Notification> notifications = [];

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            this.HubConnection = new HubConnectionBuilder()
                .WithUrl($"https://localhost:7070{NotificationMethodNames.HubUrl}")
                .WithAutomaticReconnect()
                .Build();

            this.HubConnection.On<IEnumerable<Notification>>(NotificationMethodNames.Receive, async (notifications) =>
            {
                this.notifications.AddRange(notifications);
                await this.InvokeAsync(this.StateHasChanged);
            });

            await this.HubConnection.StartAsync();
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

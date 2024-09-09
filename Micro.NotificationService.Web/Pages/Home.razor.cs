using Micro.NotificationService.Common.DTOs;
using Micro.NotificationService.Common.SignalR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

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

            this.HubConnection.On<Notification>(NotificationMethodNames.Receive, (notification) =>
            {
                this.notifications.Add(notification);
                this.InvokeAsync(this.StateHasChanged);
            });

            await this.HubConnection.StartAsync();
        }
    }
}

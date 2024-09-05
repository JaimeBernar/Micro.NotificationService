namespace NotificationService.Modules
{
    using Carter;
    using Microsoft.AspNetCore.Mvc;
    using NotificationService.Services;

    public class SubscriptionsModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v1/notifications/{userId:guid}", this.GetUserNotifications);
            app.MapPost("api/v1/notifications", this.PostNewNotification);
        }

        public async Task GetUserNotifications(HttpContext context, Guid userId, [FromServices] INotificationOrchestrator orchestrator)
        {

        }

        public async Task PostNewNotification(HttpContext context, [FromServices] INotificationOrchestrator orchestrator)
        {

        }
    }
}

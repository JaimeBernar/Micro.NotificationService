namespace NotificationService.Modules
{
    using Carter;
    using Carter.ModelBinding;
    using Carter.Response;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Routing;
    using NotificationService.Common.DTOs;
    using NotificationService.Services;

    public class NotificationsModule : ICarterModule
    {
        private readonly ILogger<NotificationsModule> logger;

        public NotificationsModule(ILogger<NotificationsModule> logger)
        {
            this.logger = logger;
        }

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v1/notifications/{userId:guid}", this.GetUserNotifications);
            app.MapPost("api/v1/notifications", this.PostNewNotification);
        }

        public async Task GetUserNotifications(HttpContext context, Guid userId, [FromServices] INotificationOrchestrator orchestrator)
        {
            try
            {

            }
            catch (Exception ex)
            {
                this.logger.LogError("An error ocurred while getting user notifications. {error}", ex);
                await context.Response.Negotiate(ex);
            }
        }

        public async Task PostNewNotification(HttpContext context, NotificationMessage notification, [FromServices] INotificationOrchestrator orchestrator) 
        {
            try
            {                

            }
            catch (Exception ex)
            {
                this.logger.LogError("An error ocurred while posting a new notification. {error}", ex);
                await context.Response.Negotiate(ex);
            }
        }
    }
}

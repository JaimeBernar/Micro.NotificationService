namespace Micro.NotificationService.Modules
{
    using Carter;
    using Carter.ModelBinding;
    using Carter.Response;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Routing;
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Services;
    using System.Net;

    public class NotificationsModule : ICarterModule
    {
        private readonly ILogger<NotificationsModule> logger;

        public NotificationsModule(ILogger<NotificationsModule> logger)
        {
            this.logger = logger;
        }

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v1/notifications/{userId:guid}", this.GetUserNotifications)
               .Produces((int)HttpStatusCode.OK)
               .Produces((int)HttpStatusCode.InternalServerError);

            app.MapPost("api/v1/notifications", this.PostNewNotification)
               .Produces((int)HttpStatusCode.OK)
               .Produces((int)HttpStatusCode.InternalServerError);

            app.MapPost("api/v1/direct-notifications", this.PostNewDirectNotification)
               .Produces((int)HttpStatusCode.OK)
               .Produces((int)HttpStatusCode.InternalServerError);

            app.MapPost("api/v1/batch/notifications", this.PostNewNotifications)
               .Produces((int)HttpStatusCode.OK)
               .Produces((int)HttpStatusCode.InternalServerError);

            app.MapPost("api/v1/batch/direct-notifications", this.PostNewDirectNotifications)
               .Produces((int)HttpStatusCode.OK)
               .Produces((int)HttpStatusCode.InternalServerError);

            app.MapDelete("api/v1/notifications/{id:guid}", this.DeleteNotification)
               .Produces((int)HttpStatusCode.OK)
               .Produces((int)HttpStatusCode.InternalServerError);

            app.MapDelete("api/v1/notifications", this.DeleteNotifications)
               .Produces((int)HttpStatusCode.OK)
               .Produces((int)HttpStatusCode.InternalServerError);
        }

        public async Task GetUserNotifications(HttpContext context, Guid userId, [FromServices] INotificationOrchestrator orchestrator)
        {
            try
            {
                var result = await orchestrator.GetUserNotifications(userId);

                if (result.IsFailed)
                {
                    await context.Response.Negotiate(result);
                    return;
                }

                await context.Response.Negotiate(result.Value);
            }
            catch (Exception ex)
            {
                this.logger.LogError("An error ocurred while getting user notifications. {error}", ex);
                await context.Response.Negotiate(ex);
            }
        }

        public async Task PostNewNotification(HttpContext context, [FromBody] NotificationMessage notification, [FromServices] INotificationOrchestrator orchestrator) 
        {
            try
            {
                var validationResult = context.Request.Validate(notification);

                if (!validationResult.IsValid)
                {
                    await context.Response.Negotiate(validationResult);
                    return;
                }

                var result = await orchestrator.ProcessNotifications([notification]);
                await context.Response.Negotiate(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError("An error ocurred while posting a new notification. {error}", ex);
                await context.Response.Negotiate(ex);
            }
        }

        public async Task PostNewDirectNotification(HttpContext context, [FromBody] DirectNotificationMessage notification, [FromServices] INotificationOrchestrator orchestrator)
        {
            try
            {
                var validationResult = context.Request.Validate(notification);

                if (!validationResult.IsValid)
                {
                    await context.Response.Negotiate(validationResult);
                    return;
                }

                var result = await orchestrator.ProcessDirectNotifications([notification]);
                await context.Response.Negotiate(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError("An error ocurred while posting a new notification. {error}", ex);
                await context.Response.Negotiate(ex);
            }
        }

        public async Task PostNewNotifications(HttpContext context, [FromBody] IEnumerable<NotificationMessage> notifications, [FromServices] INotificationOrchestrator orchestrator)
        {
            try
            {
                var validationResult = context.Request.Validate(notifications);

                if (!validationResult.IsValid)
                {
                    await context.Response.Negotiate(validationResult);
                    return;
                }

                var result = await orchestrator.ProcessNotifications(notifications);
                await context.Response.Negotiate(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError("An error ocurred while posting a new notification. {error}", ex);
                await context.Response.Negotiate(ex);
            }
        }

        public async Task PostNewDirectNotifications(HttpContext context, [FromBody] IEnumerable<DirectNotificationMessage> notifications, [FromServices] INotificationOrchestrator orchestrator)
        {
            try
            {
                var validationResult = context.Request.Validate(notifications);

                if (!validationResult.IsValid)
                {
                    await context.Response.Negotiate(validationResult);
                    return;
                }

                var result = await orchestrator.ProcessDirectNotifications(notifications);
                await context.Response.Negotiate(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError("An error ocurred while posting a new notification. {error}", ex);
                await context.Response.Negotiate(ex);
            }
        }

        public async Task DeleteNotification(HttpContext context, Guid id, [FromServices] INotificationOrchestrator orchestrator)
        {
            try
            {
                var result = await orchestrator.DeleteNotifications([id]);
                await context.Response.Negotiate(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError("An error ocurred while deleting the notification. {error}", ex);
                await context.Response.Negotiate(ex);
            }
        }

        public async Task DeleteNotifications(HttpContext context, [FromBody] IEnumerable<Guid> ids, [FromServices] INotificationOrchestrator orchestrator)
        {
            try
            {
                var result = await orchestrator.DeleteNotifications(ids);
                await context.Response.Negotiate(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError("An error ocurred while deleting the notifications. {error}", ex);
                await context.Response.Negotiate(ex);
            }
        }
    }
}

namespace NotificationService.Modules
{
    using Carter;
    using Carter.ModelBinding;
    using Carter.Response;
    using Microsoft.AspNetCore.Mvc;
    using NotificationService.Common.DTOs;
    using NotificationService.Services;
    using System.Net;

    public class SubscriptionsModule : ICarterModule
    {
        private readonly ILogger<SubscriptionsModule> logger;

        public SubscriptionsModule(ILogger<SubscriptionsModule> logger)
        {
            this.logger = logger;
        }

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v1/subscriptions/{userId:guid}", this.GetUserSubscriptions)
               .Produces((int)HttpStatusCode.OK)
               .Produces((int)HttpStatusCode.InternalServerError);

            app.MapPost("api/v1/subscriptions", this.PostNewSubscription)
               .Produces((int)HttpStatusCode.OK)
               .Produces((int)HttpStatusCode.InternalServerError);

            app.MapDelete("api/v1/subscriptions/{subscriptionId:guid}", this.DeleteSubscription)
               .Produces((int)HttpStatusCode.OK)
               .Produces((int)HttpStatusCode.NotFound)
               .Produces((int)HttpStatusCode.InternalServerError);                
        }

        public async Task GetUserSubscriptions(HttpContext context, Guid userId, [FromServices] ISubscriptionOrchestrator orchestrator)
        {
            try
            {
                var result = await orchestrator.GetUserSubscriptions(userId);

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

        public async Task PostNewSubscription(HttpContext context, [FromBody] SubscriptionMessage subscription, [FromServices] ISubscriptionOrchestrator orchestrator)
        {
            try
            {
                var validationResult = context.Request.Validate(subscription);

                if (!validationResult.IsValid)
                {
                    await context.Response.Negotiate(validationResult);
                    return;
                }

                var result = await orchestrator.ProcessSubscription(subscription);

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

        public async Task DeleteSubscription(HttpContext context, Guid subscriptionId, [FromServices] ISubscriptionOrchestrator orchestrator)
        {
            try
            {
                var result = await orchestrator.DeleteSubscription(subscriptionId);

                if (result.IsFailed)
                {
                    await context.Response.Negotiate(result);
                    return;
                }

                await context.Response.Negotiate(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError("An error ocurred while getting user notifications. {error}", ex);
                await context.Response.Negotiate(ex);
            }
        }
    }
}

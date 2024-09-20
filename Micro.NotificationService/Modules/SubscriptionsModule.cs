namespace Micro.NotificationService.Modules
{
    using Carter;
    using Carter.ModelBinding;
    using Carter.Response;
    using Micro.NotificationService.Common.DTOs;
    using Micro.NotificationService.Models;
    using Micro.NotificationService.Services.Orchestrators;
    using Microsoft.AspNetCore.Mvc;
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
            app.MapGet("api/v1/subscriptions/{userId}", this.GetUserSubscriptions)
               .Produces((int)HttpStatusCode.OK)
               .Produces((int)HttpStatusCode.InternalServerError)
               .Produces<IEnumerable<Subscription>>();

            app.MapPost("api/v1/subscriptions", this.PostNewSubscription)
               .Accepts<SubscriptionMessage>("application/json")
               .Produces((int)HttpStatusCode.OK)
               .Produces((int)HttpStatusCode.InternalServerError);

            app.MapPost("api/v1/batch/subscriptions", this.PostNewSubscriptions)
               .Accepts<IEnumerable<SubscriptionMessage>>("application/json")
               .Produces((int)HttpStatusCode.OK)
               .Produces((int)HttpStatusCode.InternalServerError);

            app.MapDelete("api/v1/subscriptions/{id:int}", this.DeleteSubscription)
               .Produces((int)HttpStatusCode.OK)
               .Produces((int)HttpStatusCode.NotFound)
               .Produces((int)HttpStatusCode.InternalServerError);

            app.MapDelete("api/v1/subscriptions", this.DeleteSubscriptions)
               .Accepts<IEnumerable<int>>("application/json")
               .Produces((int)HttpStatusCode.OK)
               .Produces((int)HttpStatusCode.NotFound)
               .Produces((int)HttpStatusCode.InternalServerError);
        }

        public async Task GetUserSubscriptions(HttpContext context, string userId, [FromServices] ISubscriptionOrchestrator orchestrator)
        {
            try
            {
                var result = orchestrator.GetUserSubscriptions(userId);

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

                var result = orchestrator.ProcessSubscriptions([subscription]);

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

        public async Task PostNewSubscriptions(HttpContext context, [FromBody] IEnumerable<SubscriptionMessage> subscriptions, [FromServices] ISubscriptionOrchestrator orchestrator)
        {
            try
            {
                var validationResult = context.Request.Validate(subscriptions);

                if (!validationResult.IsValid)
                {
                    await context.Response.Negotiate(validationResult);
                    return;
                }

                var result = orchestrator.ProcessSubscriptions(subscriptions);

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

        public async Task DeleteSubscription(HttpContext context, int id, [FromServices] ISubscriptionOrchestrator orchestrator)
        {
            try
            {
                var result = orchestrator.DeleteSubscriptions([id]);

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

        public async Task DeleteSubscriptions(HttpContext context, [FromBody] IEnumerable<int> ids, [FromServices] ISubscriptionOrchestrator orchestrator)
        {
            try
            {
                var result = orchestrator.DeleteSubscriptions(ids);

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

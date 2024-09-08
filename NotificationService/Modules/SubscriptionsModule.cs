namespace NotificationService.Modules
{
    using Carter;
    using Carter.Response;
    using Microsoft.AspNetCore.Mvc;
    using NotificationService.Services;

    public class SubscriptionsModule : ICarterModule
    {
        private readonly ILogger<SubscriptionsModule> logger;

        public SubscriptionsModule(ILogger<SubscriptionsModule> logger)
        {
            this.logger = logger;
        }

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("api/v1/subscriptions/{userId:guid}", this.GetUserSubscriptions);
            app.MapPost("api/v1/subscription", this.PostNewSubscription);
        }

        public async Task GetUserSubscriptions(HttpContext context, Guid userId, [FromServices] ISubscriptionOrchestrator orchestrator)
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

        public async Task PostNewSubscription(HttpContext context, [FromServices] ISubscriptionOrchestrator orchestrator)
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
    }
}

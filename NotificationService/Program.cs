namespace NotificationService
{
    using Carter;
    using NotificationService.Data;
    using NotificationService.Modules;
    using NotificationService.Services;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<Context>();
            builder.Services.AddCarter(configurator: c =>
            {
                c.WithModule<NotificationsModule>();
                c.WithModule<SubscriptionsModule>();
            });

            builder.Services.AddScoped<INotificationOrchestrator, NotificationOrchestrator>();
            builder.Services.AddScoped<ISubscriptionOrchestrator, SubscriptionOrchestrator>();
            builder.Services.AddScoped<IEmailService, EmailService>();

            var app = builder.Build();
            app.MapCarter();            

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.Run();
        }
    }
}


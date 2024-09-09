namespace Micro.NotificationService.Extensions
{
    using Carter;
    using Micro.NotificationService.Data;
    using Micro.NotificationService.Modules;
    using Micro.NotificationService.Options;
    using Micro.NotificationService.Services;
    using Micro.NotificationService.Validators;
    using Microsoft.AspNetCore.ResponseCompression;
    using System.Text;

    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddDbContext<Context>();
            serviceCollection.AddLogging();
            serviceCollection.AddCarter(configurator: c =>
            {
                c.WithModule<NotificationsModule>();
                c.WithModule<SubscriptionsModule>();
                c.WithValidator<SubscriptionMessageValidator>();
                c.WithValidator<NotificationMessageValidator>();
                c.WithValidator<DirectNotificationMessageValidator>();
            });

            serviceCollection.AddScoped<INotificationOrchestrator, NotificationOrchestrator>();
            serviceCollection.AddScoped<ISubscriptionOrchestrator, SubscriptionOrchestrator>();
            serviceCollection.AddScoped<IEmailService, EmailService>();
            serviceCollection.AddScoped<IWebService, WebService>();
            serviceCollection.AddSignalR();

            serviceCollection.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/octet-stream"]);
            });

            serviceCollection.AddCors(options =>
            {
                options.AddPolicy("Cors", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public static void AddOptions(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<EmailServerOptions>(configuration.GetSection(EmailServerOptions.EmailServer));
            serviceCollection.Configure<SettingsOptions>(configuration.GetSection(SettingsOptions.Settings));
        }
    }
}

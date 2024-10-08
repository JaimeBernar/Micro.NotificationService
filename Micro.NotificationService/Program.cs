namespace Micro.NotificationService
{
    using Carter;
    using Micro.NotificationService.Extensions;
    using Micro.NotificationService.Options;
    using Micro.NotificationService.Services.Hub;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using Serilog;

    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddOptions(builder.Configuration);
            builder.Services.AddServices();
            builder.Services.AddSerilog((context, configuration) =>
            {
                configuration.ReadFrom.Configuration(builder.Configuration);
            });

            ILogger<Program> logger = null!;

            try
            {
                var app = builder.Build();
                if (!app.Environment.IsDevelopment())
                {
                    //Needs to be immediately after building the app
                    app.UseResponseCompression();
                }

                logger = app.Services.GetRequiredService<ILogger<Program>>();
                var settings = app.Services.GetRequiredService<IOptions<SettingsOptions>>().Value;
                var serverOptions = app.Services.GetRequiredService<IOptions<EmailServerOptions>>().Value;

                logger.LogInformation("Logging Configuration");
                logger.LogInformation("Settings:");
                logger.LogInformation($"{nameof(SettingsOptions.EmailNotificationsActive)}={settings.EmailNotificationsActive}");
                logger.LogInformation($"{nameof(SettingsOptions.WebNotificationsActive)}={settings.WebNotificationsActive}");

                logger.LogInformation("EmailServer:");
                logger.LogInformation($"{nameof(EmailServerOptions.Host)}={serverOptions.Host}");
                logger.LogInformation($"{nameof(EmailServerOptions.Port)}={serverOptions.Port}");
                logger.LogInformation($"{nameof(EmailServerOptions.UseSenderInfo)}={serverOptions.UseSenderInfo}");
                logger.LogInformation($"{nameof(EmailServerOptions.EmailSenderName)}={serverOptions.EmailSenderName}");
                logger.LogInformation($"{nameof(EmailServerOptions.EmailSenderAddress)}={serverOptions.EmailSenderAddress}");

                app.UseCors("Cors");
                app.MapCarter();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                app.MapHub<NotificationsHub>(settings.NotificationsHubPath);
                app.Run();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error ocurred while executing the app");
            }
        }
    }
}


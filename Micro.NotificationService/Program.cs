namespace Micro.NotificationService
{
    using Carter;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Hosting;
    using Micro.NotificationService.Data;
    using Micro.NotificationService.Extensions;
    using Microsoft.Extensions.Options;
    using Micro.NotificationService.Options;
    using Serilog;

    public class Program
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

            var app = builder.Build();

            var logger = app.Services.GetRequiredService<ILogger<Program>>();
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

            EnsureCreatedAndApplyMigrations(app);
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

        private static void EnsureCreatedAndApplyMigrations(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<Context>();
                //dbContext.Database.EnsureCreated();  
                dbContext.Database.Migrate();        
            }
        }
    }
}


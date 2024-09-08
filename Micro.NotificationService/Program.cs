namespace Micro.NotificationService
{
    using Carter;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Hosting;
    using Micro.NotificationService.Data;
    using Micro.NotificationService.Extensions;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddOptions(builder.Configuration);
            builder.Services.AddServices();

            var app = builder.Build();
            app.EnsureCreatedAndApplyMigrations();
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

        private static void EnsureCreatedAndApplyMigrations(this WebApplication app)
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


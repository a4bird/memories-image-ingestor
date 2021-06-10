using System;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace Memories.Image.Ingestor.Lambda
{
    public class Startup
    {
        public static ServiceProvider Initialise()
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            var logger = CreateLogger();
            var configuration = CreateConfiguration();
            return ConfigureDependencyInjection(logger, configuration);
        }

        private static ILogger CreateLogger()
        {            

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", Constants.ApplicationName)
                .WriteTo.Console(new JsonFormatter())
                .CreateLogger();

            return Log.Logger;
        }

        private static IConfiguration CreateConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("Environment");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json")
                .AddJsonFile($"appsettings.{environment}.json", true, false)
                .AddEnvironmentVariables();

            return configuration.Build();
        }

        private static ServiceProvider ConfigureDependencyInjection(ILogger logger, IConfiguration configuration)
        {
            var services = new ServiceCollection();
            services.AddSingleton(logger);
            services.AddSingleton<MessageAttributeHelper>();
            services.AddTransient<MessageHandler>();

            return services.BuildServiceProvider();
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using Amazon.DynamoDBv2;
using Amazon.S3;
using Memories.Image.Remover.Lambda.Common;
using Memories.Image.Remover.Lambda.Data.Commands;
using Memories.Image.Remover.Lambda.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace Memories.Image.Remover.Lambda
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
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json")
                .AddJsonFile($"appsettings.{environment}.json", true, false)
                .AddEnvironmentVariables();

            return configuration.Build();
        }

        private static ServiceProvider ConfigureDependencyInjection(ILogger logger, IConfiguration configuration)
        {
            var services = new ServiceCollection();

            services.Configure<DynamoDbOptions>(configuration.GetSection("DynamoDb"));

            services.AddSingleton(logger);
            services.AddTransient<MessageHandler>();
            services.AddTransient<ICloudStorage, CloudStorage>();
            services.AddTransient<IRemoveImageObjectsQuery, RemoveImageObjectsQuery>();


            var awsOptions = configuration.GetAWSOptions();
            services.AddDefaultAWSOptions(awsOptions);
            
            if (Environment.GetEnvironmentVariable("Environment") != Constants.EnvironmentName.Local)
            {
                services.AddAWSService<IAmazonS3>();
                services.AddAWSService<IAmazonDynamoDB>();
            }
            else
            {
                Log.Warning("Connecting to LocalStack");

                services.AddSingleton<IAmazonDynamoDB>(new AmazonDynamoDBClient(
                    new AmazonDynamoDBConfig
                    {
                        ServiceURL = configuration["dynamoDbServiceUrl"],
                    }));
            }

            return services.BuildServiceProvider();
        }
    }
}

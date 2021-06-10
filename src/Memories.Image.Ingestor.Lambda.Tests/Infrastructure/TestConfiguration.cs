using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Memories.Image.Ingestor.Tests.Infrastructure
{
    public static class TestConfiguration
    {
        private static readonly IConfigurationRoot _configuration;

        static TestConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("IntegrationTestEnvironment");
            _configuration = new ConfigurationBuilder()
                .SetBasePath($"{Directory.GetCurrentDirectory()}/Integration")
                .AddJsonFile("appSettings.json")
                .AddJsonFile($"appSettings.{environment}.json", true, false)
                .Build();
        }

        public static IConfiguration Config => _configuration;
    }
}

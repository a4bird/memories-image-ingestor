using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Memories.Image.Ingestor.Lambda
{
    public class Function
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public Function()
        {
            _serviceProvider = Startup.Initialise();
            _logger = _serviceProvider.GetRequiredService<ILogger>();
        }

        public async Task Execute(S3Event s3Event)
        {
            var messageHandler = _serviceProvider.GetRequiredService<MessageHandler>();
            _logger.Information("Beginning to process {Count} records.", s3Event.Records.Count);

            var tasks = s3Event.Records.Select(messageHandler.Handle);

            await Task.WhenAll(tasks);
        }
    }
}

using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
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

        public async Task Execute(SQSEvent sqsEvent)
        {
            var messageHandler = _serviceProvider.GetRequiredService<MessageHandler>();
            _logger.Information("Beginning to process {Count} records.", sqsEvent.Records.Count);

            var tasks = sqsEvent.Records.Select(messageHandler.Handle);

            await Task.WhenAll(tasks);
        }
    }
}

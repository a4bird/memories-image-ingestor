using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Serilog;
using static Amazon.Lambda.SQSEvents.SQSEvent;
using static Amazon.S3.Util.S3EventNotification;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Memories.Image.Remover.Lambda
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
            
            _logger.Information("Beginning to process {Count} records.", sqsEvent.Records.Count);

            var tasks = sqsEvent.Records.Select(HandleSqsMessage);

            await Task.WhenAll(tasks);
        }

        async Task HandleSqsMessage(SQSMessage sqsMessage) {

            var messageHandler = _serviceProvider.GetRequiredService<MessageHandler>();
            _logger.Information("Handling {@sqsMessage} Record", sqsMessage);

            var s3Event = JsonConvert.DeserializeObject<S3Event>(sqsMessage.Body);

            var tasks = s3Event.Records.Select(messageHandler.Handle);

            await Task.WhenAll(tasks);
        }
    }
}

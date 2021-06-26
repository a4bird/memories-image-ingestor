using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Serilog;
using static Amazon.Lambda.DynamoDBEvents.DynamoDBEvent;

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

        public async Task Execute(DynamoDBEvent dynamodbEvent)
        {
            
            _logger.Information("Beginning to process {Count} records.", dynamodbEvent.Records.Count);

            var tasks = dynamodbEvent.Records.Select(HandleDynamodbStreamRecord);

            await Task.WhenAll(tasks);
        }

        Task HandleDynamodbStreamRecord(DynamodbStreamRecord record) {

            var messageHandler = _serviceProvider.GetRequiredService<MessageHandler>();

            _logger.Information("DynamoDb Record: {@record}", record);
            var streamRecordJson = Document.FromAttributeMap(record.Dynamodb.NewImage).ToJson();
            _logger.Information("DynamoDb New Image Record Json: {@streamRecordJson}", streamRecordJson);

            //var dataItem = JsonConvert.DeserializeObject<>(streamRecordJson);

            //var tasks = s3Event.Records.Select(messageHandler.Handle);

            //await Task.WhenAll(tasks);

            return Task.CompletedTask;
        }
    }
}

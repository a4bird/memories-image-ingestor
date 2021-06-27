using System.Threading.Tasks;
using Amazon.Lambda.CloudWatchLogsEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Serilog;

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

        public async Task Execute(CloudWatchLogsEvent cloudWatchLogsEvent)
        {
            var messageHandler = _serviceProvider.GetRequiredService<MessageHandler>();

            LambdaLogger.Log($"CloudWatchLogsEvent:{JsonConvert.SerializeObject(cloudWatchLogsEvent)}");

        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.SQSEvents;
using Newtonsoft.Json;
using Memories.Image.Ingestor.Lambda;

namespace Memories.Image.Ingestor.Tests.Infrastructure
{
    public class Lambda
    {
        private readonly Function _function;

        public Lambda()
        {
            _function = new Function();
        }

        public Task ExecuteWithEvent(object body, string type)
        {
            var sqsEvent = new SQSEvent
            {
                Records = new List<SQSEvent.SQSMessage>
                {
                    new SQSEvent.SQSMessage
                    {
                        Body = JsonConvert.SerializeObject(body),
                        MessageAttributes = new Dictionary<string, SQSEvent.MessageAttribute>
                        {
                            { "Type", new SQSEvent.MessageAttribute { StringValue = type, DataType = "String" } },
                            { "TraceParent", new SQSEvent.MessageAttribute { StringValue = "00-f5704e5528e55b45893f6f5a30e5bcf0-d19ec0dad3f5e744-00", DataType = "String" } },
                            { "MessageId", new SQSEvent.MessageAttribute { StringValue = "UniqueMessageId", DataType = "String" } },
                            { "CreatedTime", new SQSEvent.MessageAttribute { StringValue = "2020-05-01T03:11:58Z", DataType = "String" } },
                            { "SequenceId", new SQSEvent.MessageAttribute { StringValue = "2", DataType = "String" } }
                        }
                    }
                }
            };

            return _function.Execute(sqsEvent);
        }
    }
}

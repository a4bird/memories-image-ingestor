using System;
using static Amazon.Lambda.SQSEvents.SQSEvent;

namespace Memories.Image.Ingestor.Lambda
{
    public class MessageAttributeHelper
    {
        public MessageAttributes Extract(SQSMessage sqsMessage)
        {
            return new MessageAttributes
            {
                Type = sqsMessage.MessageAttributes["Type"].StringValue,
                CreateTime = DateTime.Parse(sqsMessage.MessageAttributes["CreatedTime"].StringValue),
                SequenceId = sqsMessage.MessageAttributes["SequenceId"].StringValue,
                MessageId = sqsMessage.MessageAttributes["MessageId"].StringValue,
                TraceParent = sqsMessage.MessageAttributes["TraceParent"].StringValue
            };
        }
    }
}

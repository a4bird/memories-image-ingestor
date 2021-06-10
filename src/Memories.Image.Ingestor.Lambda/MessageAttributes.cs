using System;

namespace Memories.Image.Ingestor.Lambda
{
    public class MessageAttributes
    {
        public string Type { get; set; }
        public string MessageId { get; set; }
        public string TraceParent { get; set; }
        public string SequenceId { get; set; }
        public DateTime CreateTime { get; set; }
    }
}

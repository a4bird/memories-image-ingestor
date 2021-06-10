using System;

namespace Memories.Image.Ingestor.Lambda
{
    public class MessageAttributes
    {
        public string Key { get; set; }
        public long Size { get; set; }
        public string ETag { get; set; }
    }
}

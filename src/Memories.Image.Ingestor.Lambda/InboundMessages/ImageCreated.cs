using System;

namespace Memories.Image.Ingestor.Lambda.InboundMessages
{
    public class ImageCreated
    {
        public string AccountEmailAddress { get; set; }
        public string AlbumName { get; set; }
        public string FileName { get; set; }
        public DateTime DateCreated { get; set; }
    }
}

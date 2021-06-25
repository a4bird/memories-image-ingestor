using System;

namespace Memories.Image.Ingestor.Lambda.Results
{
    public class ObjectMetadataResult
    {
        public string Account { get; set; }
        public string Album { get; set; }
        public string Filename { get; set; }
        public DateTime UploadDateUtc { get; set; }
    }
}

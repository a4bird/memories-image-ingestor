using System;

namespace Memories.Image.Remover.Lambda.Requests
{
    public class RemoveImageRequest
    {
        public string Account { get; set; }
        public string Album { get; set; }
        public string Filename { get; set; }
        public string ObjectKey { get; set; }
        public DateTime UploadDate { get; set; }
    }
}

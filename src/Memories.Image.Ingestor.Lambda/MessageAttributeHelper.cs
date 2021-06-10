using System.Text;
using static Amazon.S3.Util.S3EventNotification;

namespace Memories.Image.Ingestor.Lambda
{
    public class MessageAttributeHelper
    {
        public MessageAttributes Extract(S3EventNotificationRecord s3EventNotification)
        {
            return new MessageAttributes
            {
                Key =  s3EventNotification.S3.Object.Key,
                ETag = s3EventNotification.S3.Object.ETag,
                Size = s3EventNotification.S3.Object.Size,
            };
        }
    }
}

using System;
using System.Threading.Tasks;
using Serilog;
using static Amazon.S3.Util.S3EventNotification;

namespace Memories.Image.Ingestor.Lambda
{
    public class MessageHandler
    {
        private readonly MessageAttributeHelper _messageAttributeHelper;
        private readonly ILogger _logger;

        public MessageHandler(MessageAttributeHelper messageAttributeHelper, ILogger logger)
        {
            _messageAttributeHelper = messageAttributeHelper;
            _logger = logger;
        }

        public async Task Handle(S3EventNotificationRecord s3EventNotification)
        {

            try
            {
                var messageAttributes = _messageAttributeHelper.Extract(s3EventNotification);
                using var trace = new Trace(messageAttributes);

                _logger.Information("Processing file {@fileKey}", messageAttributes.Key);

            }
            catch (Exception e)
            {
                _logger.Error(e, "Error occurred while processing SQS message {@message}", s3EventNotification);
                throw;
            }
        }
    }
}

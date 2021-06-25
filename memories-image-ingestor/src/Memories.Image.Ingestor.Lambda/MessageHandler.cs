using System;
using System.Threading.Tasks;
using Memories.Image.Ingestor.Lambda.Data.Commands;
using Memories.Image.Ingestor.Lambda.Services;
using Serilog;
using Memories.Image.Ingestor.Lambda.Data.Requests;
using static Amazon.S3.Util.S3EventNotification;

namespace Memories.Image.Ingestor.Lambda
{
    public class MessageHandler
    {
        private readonly MessageAttributeHelper _messageAttributeHelper;
        private readonly ICloudStorage _cloudStorage;
        private readonly ICreateImageObjectCommand _createImageObjectCommand;
        private readonly ILogger _logger;

        public MessageHandler(MessageAttributeHelper messageAttributeHelper, ICloudStorage cloudStorage,
            ICreateImageObjectCommand createImageObjectCommand, ILogger logger)
        {
            _messageAttributeHelper = messageAttributeHelper;
            _cloudStorage = cloudStorage;
            _createImageObjectCommand = createImageObjectCommand;
            _logger = logger;
        }

        public async Task Handle(S3EventNotificationRecord s3EventNotification)
        {

            try
            {
                _logger.Information("Handling {@S3EventNotification} Record", s3EventNotification);
                var messageAttributes = _messageAttributeHelper.Extract(s3EventNotification);
                using var trace = new Trace(messageAttributes);

                _logger.Information("Processing file {@fileKey}", messageAttributes.Key);

                var objectMetadataResult = await _cloudStorage.GetObjectMetadata(Constants.BucketName, messageAttributes.Key);
                if (!objectMetadataResult.IsSuccess) {
                    _logger.Information("Failed to retrieve Metadata for object key {@key}", messageAttributes.Key);
                    return;
                }

                var createObjectResult = await _createImageObjectCommand.CreateImageObject(new CreateImageRequest
                {
                    Account = objectMetadataResult.Model.Account,
                    Album = objectMetadataResult.Model.Album,
                    Filename = objectMetadataResult.Model.Filename,
                    ObjectKey = messageAttributes.Key,
                    UploadDate = objectMetadataResult.Model.UploadDateUtc,
                });

                if (!createObjectResult.IsSuccess)
                {
                    _logger.Information("Failed to add object for key {@key}", messageAttributes.Key);
                    return;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error occurred while processing message {@message}", s3EventNotification);
                throw;
            }
        }
    }
}

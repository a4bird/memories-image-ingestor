using System;
using System.Threading.Tasks;
using Memories.Image.Ingestor.Lambda.Commands;
using Memories.Image.Ingestor.Lambda.Services;
using Serilog;
using Memories.Image.Ingestor.Lambda.Requests;
using static Amazon.S3.Util.S3EventNotification;
using System.Net;

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

                var decodedKey = WebUtility.UrlDecode(messageAttributes.Key);
                _logger.Information("Processing decodedKey {@decodedKey}", decodedKey);

                var objectMetadataResult = await _cloudStorage.GetObjectMetadata(Constants.BucketName, decodedKey);
                if (!objectMetadataResult.IsSuccess) {
                    // TODO Possibly throw if you want to process from Dlq
                    _logger.Information("Failed to retrieve Metadata for object key {@key}", decodedKey);
                    return;
                }

                var createObjectResult = await _createImageObjectCommand.CreateImageObject(new CreateImageRequest
                {
                    Account = objectMetadataResult.Model.Account,
                    Album = objectMetadataResult.Model.Album,
                    Filename = objectMetadataResult.Model.Filename,
                    ObjectKey = decodedKey,
                    UploadDate = objectMetadataResult.Model.UploadDateUtc,
                });

                if (!createObjectResult.IsSuccess)
                {
                    // TODO Possibly throw if you want to process from Dlq
                    _logger.Information("Failed to add object for key {@key}", decodedKey);
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

using System;
using System.Threading.Tasks;
using Memories.Image.Remover.Lambda.Data.Commands;
using Memories.Image.Remover.Lambda.Services;
using Serilog;
using System.Net;

namespace Memories.Image.Remover.Lambda
{
    public class MessageHandler
    {
        private readonly ICloudStorage _cloudStorage;
        private readonly IRemoveImageObjectCommand _createImageObjectCommand;
        private readonly ILogger _logger;

        public MessageHandler(ICloudStorage cloudStorage,
            IRemoveImageObjectCommand createImageObjectCommand, ILogger logger)
        {
            _cloudStorage = cloudStorage;
            _createImageObjectCommand = createImageObjectCommand;
            _logger = logger;
        }

        public async Task Handle(string s3ObjectKey)
        {

            try
            {
                _logger.Information("Handling {@s3ObjectKey}", s3ObjectKey);

                var decodedKey = WebUtility.UrlDecode(s3ObjectKey);
                _logger.Information("Processing decodedKey {@decodedKey}", decodedKey);

                var objectMetadataResult = await _cloudStorage.GetObjectMetadata(Constants.BucketName, decodedKey);
                if (!objectMetadataResult.IsSuccess)
                {
                    _logger.Information("Failed to retrieve Metadata for decodedKey object key {@key}", decodedKey);
                    return;
                }


            }
            catch (Exception e)
            {
                _logger.Error(e, "Error occurred while processing message {@message}", s3ObjectKey);
                throw;
            }
        }
    }
}

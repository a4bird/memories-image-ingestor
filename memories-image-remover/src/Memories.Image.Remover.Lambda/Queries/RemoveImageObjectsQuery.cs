using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Memories.Image.Remover.Lambda.Common;
using Memories.Image.Remover.Lambda.Common.Types;
using Memories.Image.Remover.Lambda.Queries;
using Memories.Image.Remover.Lambda.Requests;
using Microsoft.Extensions.Options;
using Serilog;

namespace Memories.Image.Remover.Lambda.Data.Commands
{
    public interface IRemoveImageObjectsQuery
    {
        Task<Result> RemoveImageObject(RemoveImageRequest request);
    }

    public class RemoveImageObjectsQuery : QueryBase, IRemoveImageObjectsQuery
    {
        private readonly ILogger _logger;
        public RemoveImageObjectsQuery(IAmazonDynamoDB dynamoDbClient, IOptions<DynamoDbOptions> dynamoDbOptions,
            ILogger logger) : base(dynamoDbClient, dynamoDbOptions.Value)
        {
            _logger = logger;
        }

        public async Task<Result> RemoveImageObject(RemoveImageRequest request)
        {

            _logger.Information("Handling DynamoDb Request {@request}", request);
            var imageObjectItem = new MemoriesModel
            {
                Account = request.Account,
                Album = request.Album,
                Filename = request.Filename,
                ObjectKey = request.ObjectKey,
                UploadDate = request.UploadDate
            }.ToAttributeValueMap();

            try
            {
                // TODO: Remove Item, Remove Batch Item
                var response = await DynamoDbClient.PutItemAsync(TableName, imageObjectItem);

                return Result.Ok;
                
            }
            catch (Exception e)
            {
                _logger.Information("Failed to add item to dynamo db for key {Key} created on {Date} \n Exception Message: {Exception}"
                    , request.ObjectKey, request.UploadDate, e.Message);

                throw e;
            }
        }
    }
}

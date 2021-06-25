using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Memories.Image.Remover.Lambda.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Memories.Image.Remover.Lambda.Commands
{
    public abstract class CommandBase
    {
        protected readonly IAmazonDynamoDB DynamoDbClient;

        protected readonly string TableName;

        protected CommandBase(IAmazonDynamoDB dynamoDbClient, DynamoDbOptions settings)
        {
            DynamoDbClient = dynamoDbClient;
            TableName = settings?.TableName;
        }

        protected async Task<Dictionary<string, AttributeValue>> GetOriginalItem(string account, string album,
            string attributeName)
        {
            var originalMessageItem = await DynamoDbClient.GetItemAsync(new GetItemRequest(
                TableName,
                new Dictionary<string, AttributeValue>
                {
                    {"Account", new AttributeValue(account)},
                    {"Album", new AttributeValue(album)}
                },
                true));

            if (!originalMessageItem.IsItemSet)
            {
                throw new Exception("Exception calling database endpoint. Item was not returned from call.");
            }

            if (!originalMessageItem.Item.ContainsKey(attributeName))
            {
                throw new Exception($"{attributeName} was not found. Possible Data corruption.");
            }

            return originalMessageItem.Item;
        }
    }
}

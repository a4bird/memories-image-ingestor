using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Memories.Image.Remover.Lambda.Data.Extensions
{
    public static class DictionaryExtensions
    {
        public static void AddDynamoAttribute(this Dictionary<string, AttributeValue> item, string key, object value)
        {
            if (value is string valueString)
            {
                item.Add(key, !string.IsNullOrWhiteSpace(valueString) ? new AttributeValue(valueString) : new AttributeValue { NULL = true });
            }
            else if (value is DateTime valueDateTime)
            {
                item.Add(key, (valueDateTime != DateTime.MinValue) ? new AttributeValue(valueDateTime.ToString("O")) : new AttributeValue { NULL = true });
            }
        }
    }
}

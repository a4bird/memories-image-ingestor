using System;
using System.Collections.Generic;
using Memories.Image.Ingestor.Lambda.Data.Extensions;
using Amazon.DynamoDBv2.Model;
using System.Globalization;

namespace Memories.Image.Ingestor.Lambda.Data
{
    public class MemoriesModel
    {
        private readonly Dictionary<string, AttributeValue> _attributeValuesMap;
        public MemoriesModel()
        {
            _attributeValuesMap = new Dictionary<string, AttributeValue>();
        }

        public string PK
        {
            get {
                return $"{Album}+{Account}";
            }
        }
        public string Account { get; set; }
        public string Album { get; set; }
        public string Filename { get; set; }
        public string ObjectKey { get; set; }
        public DateTime UploadDate { get; set; }

        public Dictionary<string, AttributeValue> ToAttributeValueMap()
        {
            _attributeValuesMap.Clear();
            _attributeValuesMap.Add(nameof(PK), new AttributeValue(PK));
            _attributeValuesMap.Add(nameof(Account), new AttributeValue(Account));
            _attributeValuesMap.Add(nameof(Album), new AttributeValue(Album));
            _attributeValuesMap.Add(nameof(Filename), new AttributeValue(Filename));
            _attributeValuesMap.Add(nameof(ObjectKey), new AttributeValue(ObjectKey));
            _attributeValuesMap.AddDynamoAttribute(nameof(UploadDate), UploadDate.ToString("O"));
            return _attributeValuesMap;
        }

        public MemoriesModel FromAttributeValueMap(Dictionary<string, AttributeValue> item)
        {
            return new MemoriesModel
            {
                Account = item[nameof(Account)].S,
                Album = item[nameof(Album)].S,
                Filename = item[nameof(Filename)].S,
                ObjectKey = item[nameof(ObjectKey)].S,
                UploadDate = DateTime.Parse(item[nameof(UploadDate)].S, null, DateTimeStyles.RoundtripKind),
            };
        }

    }
}

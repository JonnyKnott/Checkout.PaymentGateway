using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;
using Checkout.PaymentGateway.Core.Interfaces;
using Checkout.PaymentGateway.Models.ServiceModels;
using Xunit;

namespace Checkout.PaymentGateway.Data.Test.Models
{
    public class TestEntity : IDynamoDbSerializable
    {
        public string PartitionKey { get; set; }
        public string SortKey { get; set; }
        public string AdditionalAttribute1 { get; set; }
        public Dictionary<string, AttributeValue> ToAttributeMap()
        {
            var attributeMap = new Dictionary<string, AttributeValue>
            {
                { nameof(PartitionKey), new AttributeValue(PartitionKey) },
                { nameof(SortKey), new AttributeValue(SortKey) }
            };
            
            if(AdditionalAttribute1 != null)
                attributeMap.Add(nameof(AdditionalAttribute1), new AttributeValue(AdditionalAttribute1));

            return attributeMap;
        }

        public void FromAttributeMap(Dictionary<string, AttributeValue> attributeMap)
        {
            PartitionKey = attributeMap[nameof(PartitionKey)].S;
            SortKey = attributeMap[nameof(SortKey)].S;
            AdditionalAttribute1 = attributeMap[nameof(AdditionalAttribute1)].S;
        }
    }
}
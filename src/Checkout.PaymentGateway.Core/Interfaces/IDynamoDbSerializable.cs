using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;

namespace Checkout.PaymentGateway.Core.Interfaces
{
    public interface IDynamoDbSerializable
    {
        Dictionary<string, AttributeValue> ToAttributeMap();
        void FromAttributeMap(Dictionary<string, AttributeValue> attributeMap);
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using Amazon.DynamoDBv2.Model;
using Checkout.PaymentGateway.Core.Interfaces;
using Checkout.PaymentGateway.Models.ServiceModels.Enums;

namespace Checkout.PaymentGateway.Models.ServiceModels
{
    public class PaymentResult : IDynamoDbSerializable
    {
        public Guid Id { get; set; }
        public string PaymentIdentifier { get; set; }
        public string CardNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public PaymentStatus Status { get; set; }
        public Dictionary<string, AttributeValue> ToAttributeMap()
        {
            return new Dictionary<string, AttributeValue>
            {
                { nameof(PaymentIdentifier), new AttributeValue(PaymentIdentifier) },
                { nameof(CardNumber), new AttributeValue(CardNumber) },
                { nameof(Amount), new AttributeValue { N = Amount.ToString(CultureInfo.InvariantCulture) } },
                { nameof(Timestamp), new AttributeValue{ S = Timestamp.ToString("G") } },
                { nameof(Status), new AttributeValue(Status.ToString()) }
            };
        }

        public void FromAttributeMap(Dictionary<string, AttributeValue> attributeMap)
        {
            PaymentIdentifier = attributeMap[nameof(PaymentIdentifier)].S;
            CardNumber = attributeMap[nameof(CardNumber)].S;

            if (!decimal.TryParse(attributeMap[nameof(Amount)].N, out var amountValue))
                throw new ArgumentException("Attribute map contains invalid data", nameof(Amount));
            
            if (!DateTime.TryParse(attributeMap[nameof(Timestamp)].S, out var timestampValue))
                throw new ArgumentException("Attribute map contains invalid data", nameof(Timestamp));
            
            if (!Enum.TryParse<PaymentStatus>(attributeMap[nameof(Status)].S, out var statusValue))
                throw new ArgumentException("Attribute map contains invalid data", nameof(Status));
            
            Amount = amountValue;
            Timestamp = timestampValue;
            Status = statusValue;
        }
    }
}
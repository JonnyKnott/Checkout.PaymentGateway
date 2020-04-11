namespace Checkout.PaymentGateway.Models
{
    public class Constants
    {
        public static class DynamoDb
        {
            public const string PaymentResultTableName = "PaymentResult";
            public const string PaymentResultPartitionKey = "PaymentIdentifier";
            public const string PaymentResultSortKey = "Timestamp";
        }
    }
}
namespace Checkout.PaymentGateway.Models.Configuration
{
    public class DynamoDbConfiguration<TDataModel>
    where TDataModel: class
    {
        public string TableName { get; set; }
        public string PartitionKey { get; set; }
        public string SortKey { get; set; }
    }
}
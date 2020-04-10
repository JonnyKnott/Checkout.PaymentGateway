namespace Checkout.PaymentGateway.Models.External
{
    public class BankPaymentResponse
    {
        public string PaymentIdentifier { get; set; }
        public string Status { get; set; }
    }
}
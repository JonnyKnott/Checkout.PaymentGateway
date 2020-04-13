namespace Checkout.PaymentGateway.Models.ApiModels.Payment
{
    public class PaymentRequest
    {
        public string CardNumber { get; set; }
        public string Currency { get; set; }
        public string Cvv { get; set; }
        public string ExpiryDate { get; set; }
        public decimal Amount { get; set; }
    }
}
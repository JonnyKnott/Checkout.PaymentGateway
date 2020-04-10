using System.Collections.Generic;

namespace Checkout.PaymentGateway.Models.ApiModels.Payment
{
    public class PaymentResponse
    {
        public string PaymentReference { get; set; }
        public PaymentStatus Status { get; set; }
    }

    public class PaymentStatus
    {
        public string Status { get; set; }
        public ICollection<string> Errors { get; set; }
    }
}
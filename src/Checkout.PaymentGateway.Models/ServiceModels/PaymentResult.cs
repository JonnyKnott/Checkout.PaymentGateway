using System;
using Checkout.PaymentGateway.Models.ServiceModels.Enums;

namespace Checkout.PaymentGateway.Models.ServiceModels
{
    public class PaymentResult
    {
        public Guid Id { get; set; }
        public string PaymentIdentifier { get; set; }
        public string CardNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public PaymentStatus Status { get; set; }
    }
}
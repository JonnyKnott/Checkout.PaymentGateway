using System;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Models.ApiModels.Payment;
using Checkout.PaymentGateway.Models.ServiceModels;

namespace Checkout.PaymentGateway.Services.Payment
{
    public class PaymentExecutionService : IPaymentExecutionService
    {
        public Task<ServiceObjectResult<PaymentResponse>> ExecutePayment(PaymentRequest paymentRequest)
        {
            throw new NotImplementedException();
        }
    }
}
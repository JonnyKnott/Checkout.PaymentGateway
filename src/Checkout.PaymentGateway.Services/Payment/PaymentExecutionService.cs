using System;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Data.Repository;
using Checkout.PaymentGateway.Models.ApiModels.Payment;
using Checkout.PaymentGateway.Models.ServiceModels;
using Checkout.PaymentGateway.Services.External.Clients;
using PaymentStatus = Checkout.PaymentGateway.Models.ServiceModels.Enums.PaymentStatus;

namespace Checkout.PaymentGateway.Services.Payment
{
    
    public class PaymentExecutionService : IPaymentExecutionService
    {
        private readonly IBankRequestClient _bankRequestClient;

        public PaymentExecutionService(IBankRequestClient bankRequestClient)
        {
            _bankRequestClient = bankRequestClient;
        }

        public async Task<ServiceObjectResult<PaymentResult>> ExecutePayment(PaymentRequest paymentRequest)
        {
            var sendPaymentResponse = await _bankRequestClient.SendPayment(paymentRequest);

            if (!sendPaymentResponse.Success && sendPaymentResponse.Result?.PaymentIdentifier == null)
            {
                return ServiceObjectResult<PaymentResult>.Failed(null, sendPaymentResponse.Errors);
            }
            
            if(!Enum.TryParse<PaymentStatus>(sendPaymentResponse.Result.Status, out var paymentStatus))
            {
                return ServiceObjectResult<PaymentResult>.Failed(null, ErrorMessages.UnableToProcessPayment);
            }

            var paymentResult = new PaymentResult
            {
                PaymentIdentifier = sendPaymentResponse.Result.PaymentIdentifier,
                CardNumber = paymentRequest.CardNumber,
                Amount = paymentRequest.Amount,
                Status = paymentStatus,
                Timestamp = DateTime.UtcNow
            };

            return ServiceObjectResult<PaymentResult>.Succeeded(paymentResult);
        }
    }
}
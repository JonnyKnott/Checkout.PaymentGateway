using System;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Models.ApiModels.Payment;
using Checkout.PaymentGateway.Models.External;
using Checkout.PaymentGateway.Models.ServiceModels;
using Checkout.PaymentGateway.Models.ServiceModels.Enums;

namespace Checkout.PaymentGateway.Services.Payment.Clients
{
    public class MockBankRequestClient : IBankRequestClient
    {
        public async Task<ServiceObjectResult<BankPaymentResponse>> SendPayment(PaymentRequest paymentRequest)
        {
            //TODO Mock the bank response based on user input (cardnumber)

            switch (paymentRequest.CardNumber)
            {
                case "1111111111111111":
                    return ServiceObjectResult<BankPaymentResponse>.Failed(null, "Unable to process payment");
                case "2222222222222222":
                    return ServiceObjectResult<BankPaymentResponse>.Succeeded(new BankPaymentResponse
                    {
                        Status = PaymentStatus.Failed.ToString()
                    });
                default:
                    return ServiceObjectResult<BankPaymentResponse>.Succeeded(new BankPaymentResponse
                    {
                        PaymentIdentifier = Guid.NewGuid().ToString(),
                        Status = PaymentStatus.Complete.ToString()
                    });
            }
        }
    }
}
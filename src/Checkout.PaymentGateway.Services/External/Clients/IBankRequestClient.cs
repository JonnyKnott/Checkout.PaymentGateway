using System.Threading.Tasks;
using Checkout.PaymentGateway.Models.ApiModels.Payment;
using Checkout.PaymentGateway.Models.External;
using Checkout.PaymentGateway.Models.ServiceModels;

namespace Checkout.PaymentGateway.Services.Payment.Clients
{
    public interface IBankRequestClient
    {
        Task<ServiceObjectResult<BankPaymentResponse>> SendPayment(PaymentRequest paymentRequest);
    }
}
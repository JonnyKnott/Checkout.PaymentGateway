using System.Threading.Tasks;
using Checkout.PaymentGateway.Models.ApiModels;
using Checkout.PaymentGateway.Models.ApiModels.Payment;
using Checkout.PaymentGateway.Models.ServiceModels;

namespace Checkout.PaymentGateway.WebApi.Client.Payment
{
    public interface IPaymentClient
    {
        Task<ResponseEnvelope<PaymentResponse>> MakePayment(PaymentRequest paymentRequest);
        Task<PaymentResult> GetPaymentResult(string paymentIdentifier);
    }
}
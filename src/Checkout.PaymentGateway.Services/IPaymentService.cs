using System.Threading.Tasks;
using Checkout.PaymentGateway.Models.ApiModels;
using Checkout.PaymentGateway.Models.ApiModels.Payment;
using Checkout.PaymentGateway.Models.ServiceModels;

namespace Checkout.PaymentGateway.Services
{
    public interface IPaymentService
    {
        Task<ServiceObjectResult<ResponseEnvelope<PaymentResponse>>> ProcessPaymentRequest(PaymentRequest request);
        Task<ServiceObjectResult<PaymentResult>> GetPaymentResult(string paymentIdentifier);
    }
}
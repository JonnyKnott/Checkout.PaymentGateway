using System.Threading.Tasks;
using Checkout.PaymentGateway.Models.ApiModels.Payment;
using Checkout.PaymentGateway.Models.ServiceModels;

namespace Checkout.PaymentGateway.Services.Payment
{
    public interface IPaymentExecutionService
    {
        Task<ServiceObjectResult<PaymentResponse>> ExecutePayment(PaymentRequest paymentRequest);
    }
}
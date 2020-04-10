using System;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Models.ServiceModels;

namespace Checkout.PaymentGateway.Data.Repository
{
    
    public interface IPaymentRepository
    {
        Task Add(PaymentResult entity);
        Task<PaymentResult> GetByPaymentIdentifier(string paymentIdentifier);
    }
}
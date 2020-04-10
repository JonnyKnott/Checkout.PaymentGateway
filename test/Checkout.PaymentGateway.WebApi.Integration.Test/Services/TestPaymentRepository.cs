using System;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Data.Repository;
using Checkout.PaymentGateway.Models.ServiceModels;

namespace Checkout.PaymentGateway.WebApi.Integration.Test.Services
{
    public class TestPaymentRepository : IPaymentRepository
    {
        public async Task Add(PaymentResult entity)
        {
            throw new NotImplementedException();
        }

        public async Task<PaymentResult> GetByPaymentIdentifier(string paymentIdentifier)
        {
            switch (paymentIdentifier)
            {
                case TestConstants.NotFoundPaymentIdentifier:
                    return null;
                default:
                    return new PaymentResult{ PaymentIdentifier = paymentIdentifier };
            }
        }
    }
}
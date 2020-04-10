using System;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Data.Cache.Extensions;
using Checkout.PaymentGateway.Models.ServiceModels;
using Microsoft.Extensions.Caching.Distributed;

namespace Checkout.PaymentGateway.Data.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IDistributedCache _cache;

        public PaymentRepository(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task Add(PaymentResult entity)
        {
            await _cache.SetValueObject(entity.PaymentIdentifier, entity);
        }

        public async Task<PaymentResult> GetByPaymentIdentifier(string paymentIdentifier)
        {
            var paymentResult = await _cache.Get<PaymentResult>(paymentIdentifier);

            return paymentResult;
        }
    }
}
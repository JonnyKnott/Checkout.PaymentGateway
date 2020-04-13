using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Data.Repository;
using Checkout.PaymentGateway.Models.ServiceModels;

namespace Checkout.PaymentGateway.WebApi.Integration.Test.Services
{
    public class TestPaymentRepository : IDynamoDbRepository<PaymentResult>
    {

        public Task<ServiceResult> PutItemAsync(PaymentResult dataModel)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceObjectResult<ICollection<PaymentResult>>> QueryByPartitionAsync(string partitionKey)
        {
            switch (partitionKey)
            {
                case TestConstants.NotFoundPaymentIdentifier:
                    return ServiceObjectResult<ICollection<PaymentResult>>.Failed(null, ErrorCodeStrings.NotFoundError);
                default:
                    return ServiceObjectResult<ICollection<PaymentResult>>.Succeeded(new List<PaymentResult>
                        {new PaymentResult {PaymentIdentifier = partitionKey}});
            }
        }
    }
}

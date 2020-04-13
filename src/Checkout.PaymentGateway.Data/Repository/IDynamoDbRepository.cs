using System.Collections.Generic;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Core.Interfaces;
using Checkout.PaymentGateway.Models.ServiceModels;

namespace Checkout.PaymentGateway.Data.Repository
{
    public interface IDynamoDbRepository<TDataModel>
        where TDataModel : class, IDynamoDbSerializable, new()
    {
        Task<ServiceResult> PutItemAsync(TDataModel dataModel);
        Task<ServiceObjectResult<ICollection<TDataModel>>> QueryByPartitionAsync(string partitionKey);
        
    }
}
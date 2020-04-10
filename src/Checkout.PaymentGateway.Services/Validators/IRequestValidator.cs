using System.Collections.Generic;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Models.ServiceModels;

namespace Checkout.PaymentGateway.Services.Validators
{
    public interface IRequestValidator<in TApiRequest>
    {
        ServiceObjectResult<ICollection<ValidationFieldError>> ValidateRequest(TApiRequest request);
    }
}
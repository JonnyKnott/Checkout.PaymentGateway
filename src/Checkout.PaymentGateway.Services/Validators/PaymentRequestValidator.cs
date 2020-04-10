using System.Collections.Generic;
using System.Linq;
using Checkout.PaymentGateway.Models.ApiModels.Payment;
using Checkout.PaymentGateway.Models.ServiceModels;

namespace Checkout.PaymentGateway.Services.Validators
{
    public class PaymentRequestValidator : IRequestValidator<PaymentRequest>
    {
        public ServiceObjectResult<ICollection<ValidationFieldError>> ValidateRequest(PaymentRequest request)
        {
            var validationErrors = new List<ValidationFieldError>();
            
            if(request.CardNumber.Length != 16)
                validationErrors.Add(new ValidationFieldError(nameof(request.CardNumber), ErrorMessages.FieldInvalidLength));
            
            if(request.Cvv.Length != 3)
                validationErrors.Add(new ValidationFieldError(nameof(request.Cvv), ErrorMessages.FieldInvalidLength));

            if (validationErrors.Any())
                return ServiceObjectResult<ICollection<ValidationFieldError>>.Failed(validationErrors, new List<string>{ ErrorCodeStrings.BadRequestError });
            
            return ServiceObjectResult<ICollection<ValidationFieldError>>.Succeeded(validationErrors);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Checkout.PaymentGateway.Models.ApiModels.Payment;
using Checkout.PaymentGateway.Models.ServiceModels;
using Checkout.PaymentGateway.Models.ServiceModels.Enums;

namespace Checkout.PaymentGateway.Services.Validators
{
    public class PaymentRequestValidator : IRequestValidator<PaymentRequest>
    {
        public ServiceObjectResult<ICollection<ValidationFieldError>> ValidateRequest(PaymentRequest request)
        {
            var validationErrors = new List<ValidationFieldError>();
            
            if(request.CardNumber == null)
                validationErrors.Add(new ValidationFieldError(nameof(request.CardNumber), ErrorMessages.Validation.FieldMissing));
            else
            {
                if(request.CardNumber?.Length != 16)
                    validationErrors.Add(new ValidationFieldError(nameof(request.CardNumber), ErrorMessages.Validation.FieldInvalidLength));
            
                if(!request.CardNumber.All(char.IsDigit))
                    validationErrors.Add(new ValidationFieldError(nameof(request.CardNumber), ErrorMessages.Validation.FieldInvalidContent));                

            }
            
            if(request.Cvv == null)
                validationErrors.Add(new ValidationFieldError(nameof(request.Cvv), ErrorMessages.Validation.FieldMissing));
            else
            {
                if(request.Cvv.Length != 3)
                    validationErrors.Add(new ValidationFieldError(nameof(request.Cvv), ErrorMessages.Validation.FieldInvalidLength));
            
                if(!request.Cvv.All(char.IsDigit))
                    validationErrors.Add(new ValidationFieldError(nameof(request.Cvv), ErrorMessages.Validation.FieldInvalidContent));
            }
            
            if(request.Currency == null)
                validationErrors.Add(new ValidationFieldError(nameof(request.Currency), ErrorMessages.Validation.FieldMissing));
            else
            {
                if(!Enum.TryParse<Currency>(request.Currency, out _))
                    validationErrors.Add(new ValidationFieldError(nameof(request.Currency), ErrorMessages.Validation.FieldInvalidContent));
            }
            
            if(request.Amount == default || request.Amount <= 0)
                validationErrors.Add(new ValidationFieldError(nameof(request.Amount), ErrorMessages.Validation.FieldInvalidContent));

            
            if(request.ExpiryDate == null)
                validationErrors.Add(new ValidationFieldError(nameof(request.ExpiryDate), ErrorMessages.Validation.FieldMissing));
            else
            {
                
                if(!DateTime.TryParseExact(request.ExpiryDate, "MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var expiryDate))
                    validationErrors.Add(new ValidationFieldError(nameof(request.ExpiryDate), ErrorMessages.Validation.FieldInvalidContent));
                
                if(expiryDate < DateTime.UtcNow)
                    validationErrors.Add(new ValidationFieldError(nameof(request.ExpiryDate), ErrorMessages.Validation.FieldInvalidContent));
            }
            
            if (validationErrors.Any())
                return ServiceObjectResult<ICollection<ValidationFieldError>>.Failed(validationErrors, new List<string>{ ErrorCodeStrings.BadRequestError });
            
            return ServiceObjectResult<ICollection<ValidationFieldError>>.Succeeded(validationErrors);
        }
    }
}
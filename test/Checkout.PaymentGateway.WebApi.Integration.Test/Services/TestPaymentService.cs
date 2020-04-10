using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Models.ApiModels;
using Checkout.PaymentGateway.Models.ApiModels.Payment;
using Checkout.PaymentGateway.Models.ServiceModels;
using Checkout.PaymentGateway.Services;

namespace Checkout.PaymentGateway.WebApi.Integration.Test.Services
{
    public class TestPaymentService : IPaymentService
    {
        public Task<ServiceObjectResult<ResponseEnvelope<PaymentResponse>>> ProcessPaymentRequest(PaymentRequest request)
        {
            switch (request.CardNumber)
            {
                case TestConstants.ErrorCardNumber:
                    return Task.FromResult(ServiceObjectResult<ResponseEnvelope<PaymentResponse>>.Failed(null,
                        new List<string> {ErrorCodeStrings.InternalError}));
                case TestConstants.BadRequestCardNumber:
                    var validationErrors = new List<ValidationFieldError>
                        {new ValidationFieldError(nameof(request.CardNumber), ErrorMessages.FieldInvalidContent)};
                    return Task.FromResult(ServiceObjectResult<ResponseEnvelope<PaymentResponse>>.Failed(
                        new ResponseEnvelope<PaymentResponse> { ValidationErrors = validationErrors},
                        new List<string> {ErrorCodeStrings.BadRequestError}));
                case TestConstants.SuccessCardNumber:
                    return Task.FromResult(ServiceObjectResult<ResponseEnvelope<PaymentResponse>>.Succeeded(
                        new ResponseEnvelope<PaymentResponse>
                        {
                            ResponseValue = new PaymentResponse()
                        }));
                default:
                    throw new NotImplementedException();
            }
            
        }
    }
}
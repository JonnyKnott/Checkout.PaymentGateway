using System.Collections.Generic;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Models.ApiModels;
using Checkout.PaymentGateway.Models.ApiModels.Payment;
using Checkout.PaymentGateway.Models.ServiceModels;
using Checkout.PaymentGateway.Services.Validators;
using Microsoft.Extensions.Logging;

namespace Checkout.PaymentGateway.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ILogger<PaymentService> _logger;
        private readonly IRequestValidator<PaymentRequest> _requestValidator;

        public PaymentService(ILogger<PaymentService> logger, IRequestValidator<PaymentRequest> requestValidator)
        {
            _logger = logger;
            _requestValidator = requestValidator;
        }

        public async Task<ServiceObjectResult<ResponseEnvelope<PaymentResponse>>> ProcessPaymentRequest(PaymentRequest request)
        {
            var responseEnvelope = new ResponseEnvelope<PaymentResponse>();
            
            var validationResult = _requestValidator.ValidateRequest(request);

            if (!validationResult.Success)
            {
                responseEnvelope.ValidationErrors = validationResult.Result;
                return ServiceObjectResult<ResponseEnvelope<PaymentResponse>>
                    .Failed(
                        responseEnvelope,
                        validationResult.Errors);
            }
            
            responseEnvelope.ResponseValue = new PaymentResponse();

            return ServiceObjectResult<ResponseEnvelope<PaymentResponse>>.Succeeded(responseEnvelope);
        }
    }
}
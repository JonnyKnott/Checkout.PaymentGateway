using System.Collections.Generic;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Data.Repository;
using Checkout.PaymentGateway.Models.ApiModels;
using Checkout.PaymentGateway.Models.ApiModels.Payment;
using Checkout.PaymentGateway.Models.ServiceModels;
using Checkout.PaymentGateway.Services.Payment;
using Checkout.PaymentGateway.Services.Validators;
using Microsoft.Extensions.Logging;

namespace Checkout.PaymentGateway.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ILogger<PaymentService> _logger;
        private readonly IRequestValidator<PaymentRequest> _requestValidator;
        private readonly IPaymentExecutionService _paymentExecutionService;
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(ILogger<PaymentService> logger, IRequestValidator<PaymentRequest> requestValidator, IPaymentExecutionService paymentExecutionService, IPaymentRepository paymentRepository)
        {
            _logger = logger;
            _requestValidator = requestValidator;
            _paymentExecutionService = paymentExecutionService;
            _paymentRepository = paymentRepository;
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

            var paymentResult = await _paymentExecutionService.ExecutePayment(request);

            if (!paymentResult.Success)
                return ServiceObjectResult<ResponseEnvelope<PaymentResponse>>.Failed(null,
                    new List<string> {ErrorCodeStrings.InternalError});
            
            await _paymentRepository.Add(paymentResult.Result);
            
            responseEnvelope.ResponseValue = new PaymentResponse
            {
                PaymentIdentifier = paymentResult.Result.PaymentIdentifier,
                Status = paymentResult.Result.Status.ToString()
            };

            return ServiceObjectResult<ResponseEnvelope<PaymentResponse>>.Succeeded(responseEnvelope);
        }

        public async Task<ServiceObjectResult<PaymentResult>> GetPaymentResult(string paymentIdentifier)
        {
            var paymentResult = await _paymentRepository.GetByPaymentIdentifier(paymentIdentifier);
            
            if(paymentResult == null)
                return ServiceObjectResult<PaymentResult>.Failed(null, ErrorCodeStrings.NotFoundError);
            
            return ServiceObjectResult<PaymentResult>.Succeeded(paymentResult);
        }
    }
}
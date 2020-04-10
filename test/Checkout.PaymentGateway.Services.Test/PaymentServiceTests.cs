using System;
using System.Collections.Generic;
using System.Linq;
using Checkout.PaymentGateway.Data.Repository;
using Checkout.PaymentGateway.Models.ApiModels.Payment;
using Checkout.PaymentGateway.Models.ServiceModels;
using Checkout.PaymentGateway.Services.Payment;
using Checkout.PaymentGateway.Services.Validators;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Checkout.PaymentGateway.Services.Test
{
    public class PaymentServiceTests
    {
        private readonly PaymentService _paymentService;

        private readonly Mock<IRequestValidator<PaymentRequest>> _requestValidator;
        private readonly Mock<IPaymentExecutionService> _paymentExecutionService;
        private readonly Mock<IPaymentRepository> _paymentRepository;
        
        public PaymentServiceTests()
        {
            var mockLogger = new Mock<ILogger<PaymentService>>();
            _requestValidator = new Mock<IRequestValidator<PaymentRequest>>();
            _paymentExecutionService = new Mock<IPaymentExecutionService>();
            _paymentRepository = new Mock<IPaymentRepository>();

            _paymentService = new PaymentService(
                mockLogger.Object, 
                _requestValidator.Object, 
                _paymentExecutionService.Object, 
                _paymentRepository.Object
                );
        }

        [Fact]
        public async void ProcessPaymentRequest_Should_Return_Validation_Errors_If_Validation_Fails()
        {
            string cardNumber = "111111111111111a";
            
            SetupValidationService(cardNumber, new List<ValidationFieldError>{ new ValidationFieldError("CardNumber", ErrorMessages.Validation.FieldInvalidContent) });

            var paymentServiceResult = await _paymentService.ProcessPaymentRequest(new PaymentRequest
            {
                CardNumber = cardNumber
            });
            
            Assert.False(paymentServiceResult.Success);
            Assert.NotEmpty(paymentServiceResult.Result.ValidationErrors);
            Assert.Contains(ErrorMessages.Validation.FieldInvalidContent, paymentServiceResult.Result.ValidationErrors.Select(x => x.Error));
        }

        [Fact]
        public async void ProcessPaymentRequest_Should_Attempt_Payment_Execution_If_Valid_Request()
        {
            string cardNumber = "111111111111111a";
            
            SetupValidationService(cardNumber, new List<ValidationFieldError>());
            SetupPaymentExecutionService(cardNumber, true);

            var result = await _paymentService.ProcessPaymentRequest(new PaymentRequest
            {
                CardNumber = cardNumber
            });
            
            _paymentExecutionService.Verify(x => x.ExecutePayment(It.Is<PaymentRequest>(request => request.CardNumber == cardNumber)));
        }

        [Fact]
        public async void ProcessPaymentRequest_Should_Return_Failed_Result_If_Payment_Processing_Fails()
        {
            string cardNumber = "111111111111111a";
            
            SetupValidationService(cardNumber, new List<ValidationFieldError>());
            SetupPaymentExecutionService(cardNumber, false);

            var result = await _paymentService.ProcessPaymentRequest(new PaymentRequest
            {
                CardNumber = cardNumber
            });
            
            Assert.False(result.Success);
        }

        [Fact]
        public async void ProcessPaymentRequest_Should_Return_PaymentResponse_With_PaymentIdentifier()
        {
            string cardNumber = "111111111111111a";
            string paymentIdentifier = Guid.NewGuid().ToString();
            
            SetupValidationService(cardNumber, new List<ValidationFieldError>());
            SetupPaymentExecutionService(cardNumber, true, paymentIdentifier);

            var result = await _paymentService.ProcessPaymentRequest(new PaymentRequest
            {
                CardNumber = cardNumber
            });
            
            Assert.True(result.Success);
            Assert.Equal(paymentIdentifier, result.Result.ResponseValue.PaymentIdentifier);
        }

        private void SetupValidationService(string cardNumber, ICollection<ValidationFieldError> validationErrors)
        {
            var result = validationErrors.Any()
                ? ServiceObjectResult<ICollection<ValidationFieldError>>.Failed(validationErrors,
                    ErrorCodeStrings.BadRequestError)
                : ServiceObjectResult<ICollection<ValidationFieldError>>.Succeeded(validationErrors);

                _requestValidator.Setup(x =>
                    x.ValidateRequest(It.Is<PaymentRequest>(request => request.CardNumber == cardNumber)))
                .Returns(result);
        }

        private void SetupPaymentExecutionService(string cardNumber, bool success, string resultReference = null)
        {
            var result = success
                ? ServiceObjectResult<PaymentResult>.Succeeded(new PaymentResult {PaymentIdentifier = resultReference})
                : ServiceObjectResult<PaymentResult>.Failed(null, "");

            _paymentExecutionService.Setup(x =>
                    x.ExecutePayment(It.Is<PaymentRequest>(request => request.CardNumber == cardNumber)))
                .ReturnsAsync(result);
        }
    }
}
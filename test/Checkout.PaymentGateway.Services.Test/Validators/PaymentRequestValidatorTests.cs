using System.Linq;
using Checkout.PaymentGateway.Models.ApiModels.Payment;
using Checkout.PaymentGateway.Models.ServiceModels;
using Checkout.PaymentGateway.Models.ServiceModels.Enums;
using Checkout.PaymentGateway.Services.Validators;
using Xunit;

namespace Checkout.PaymentGateway.Services.Test.Validators
{
    public class PaymentRequestValidatorTests
    {
        private readonly PaymentRequestValidator _paymentRequestValidator;

        public PaymentRequestValidatorTests()
        {
            _paymentRequestValidator = new PaymentRequestValidator();
        }

        [Fact]
        public void ValidateRequest_Should_Append_Expected_Validation_Errors()
        {
            var result = _paymentRequestValidator.ValidateRequest(new PaymentRequest
            {
                Currency = "invalid",
                CardNumber = "abcdefg",
                Cvv = "defdfg",
                Amount = -10.00M,
                ExpiryDate = "xx/9z"
            });
            
            Assert.False(result.Success);
            Assert.NotEmpty(result.Result);
            Assert.Contains(ErrorMessages.Validation.FieldInvalidContent,
                result.Result.Where(x => x.FieldName == nameof(PaymentRequest.CardNumber)).Select(x => x.Error));
            Assert.Contains(ErrorMessages.Validation.FieldInvalidLength,
                result.Result.Where(x => x.FieldName == nameof(PaymentRequest.CardNumber)).Select(x => x.Error));
            Assert.Contains(ErrorMessages.Validation.FieldInvalidContent,
                result.Result.Where(x => x.FieldName == nameof(PaymentRequest.Cvv)).Select(x => x.Error));
            Assert.Contains(ErrorMessages.Validation.FieldInvalidLength,
                result.Result.Where(x => x.FieldName == nameof(PaymentRequest.Cvv)).Select(x => x.Error));
            Assert.Contains(ErrorMessages.Validation.FieldInvalidContent,
                result.Result.Where(x => x.FieldName == nameof(PaymentRequest.Currency)).Select(x => x.Error));
            Assert.Contains(ErrorMessages.Validation.FieldInvalidContent,
                result.Result.Where(x => x.FieldName == nameof(PaymentRequest.ExpiryDate)).Select(x => x.Error));
            Assert.Contains(ErrorMessages.Validation.FieldInvalidContent,
                result.Result.Where(x => x.FieldName == nameof(PaymentRequest.Amount)).Select(x => x.Error));
        }
        
        [Fact]
        public void ValidateRequest_Should_Return_Success_If_Valid()
        {
            var result = _paymentRequestValidator.ValidateRequest(new PaymentRequest
            {
                Currency = Currency.GBP.ToString(),
                CardNumber = "1234567887654321",
                Cvv = "123",
                Amount = 10.00M,
                ExpiryDate = "09/20"
            });
            
            Assert.True(result.Success);
        }
    }
}
using System;
using System.Collections.Generic;
using Checkout.PaymentGateway.Models.ApiModels.Payment;
using Checkout.PaymentGateway.Models.External;
using Checkout.PaymentGateway.Models.ServiceModels;
using Checkout.PaymentGateway.Models.ServiceModels.Enums;
using Checkout.PaymentGateway.Services.External.Clients;
using Checkout.PaymentGateway.Services.Payment;
using Moq;
using Xunit;

namespace Checkout.PaymentGateway.Services.Test.Payment
{
    public class PaymentExecutionServiceTests
    {
        private readonly PaymentExecutionService _paymentExecutionService;

        private readonly Mock<IBankRequestClient> _mockBankRequestClient;
        
        public PaymentExecutionServiceTests()
        {
            _mockBankRequestClient = new Mock<IBankRequestClient>();
            
            _paymentExecutionService = new PaymentExecutionService(_mockBankRequestClient.Object);
        }

        [Fact]
        public async void ExecutePayment_Should_Return_Failed_If_Client_Request_Fails()
        {
            _mockBankRequestClient.Setup(x => x.SendPayment(It.IsAny<PaymentRequest>()))
                .ReturnsAsync(ServiceObjectResult<BankPaymentResponse>.Failed(null, new List<string>()));

            var result = await _paymentExecutionService.ExecutePayment(new PaymentRequest());
            
            Assert.False(result.Success);
        }
        
        [Fact]
        public async void ExecutePayment_Should_Return_Failed_If_Client_Result_Has_No_Identifier()
        {
            _mockBankRequestClient.Setup(x => x.SendPayment(It.IsAny<PaymentRequest>()))
                .ReturnsAsync(ServiceObjectResult<BankPaymentResponse>.Succeeded(new BankPaymentResponse()));

            var result = await _paymentExecutionService.ExecutePayment(new PaymentRequest());
            
            Assert.False(result.Success);
        }
        
        [Fact]
        public async void ExecutePayment_Should_Return_Failed_If_Client_Result_Status_Unrecognised()
        {
            _mockBankRequestClient.Setup(x => x.SendPayment(It.IsAny<PaymentRequest>()))
                .ReturnsAsync(ServiceObjectResult<BankPaymentResponse>.Succeeded(new BankPaymentResponse{ PaymentIdentifier = Guid.NewGuid().ToString(), Status = "Unknown"}));

            var result = await _paymentExecutionService.ExecutePayment(new PaymentRequest());
            
            Assert.False(result.Success);
        }

        [Fact]
        public async void ExecutePayment_Should_Succeed_If_Client_Call_Obtains_Valid_Result()
        {
            _mockBankRequestClient.Setup(x => x.SendPayment(It.IsAny<PaymentRequest>()))
                .ReturnsAsync(ServiceObjectResult<BankPaymentResponse>.Succeeded(new BankPaymentResponse{ PaymentIdentifier = Guid.NewGuid().ToString(), Status = PaymentStatus.Complete.ToString()}));
            
            var result = await _paymentExecutionService.ExecutePayment(new PaymentRequest());
            
            Assert.True(result.Success);
        }
    }
}
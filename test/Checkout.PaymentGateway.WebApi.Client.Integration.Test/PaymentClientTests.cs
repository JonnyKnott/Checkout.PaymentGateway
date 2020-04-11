using System;
using Checkout.PaymentGateway.WebApi.Client.Payment;
using Xunit;

namespace Checkout.PaymentGateway.WebApi.Client.Integration.Test
{
    public class PaymentClientTests
    {
        private readonly IPaymentClient _client;

        public PaymentClientTests(IPaymentClient client)
        {
            _client = client;
        }

        [Fact]
        public void PaymentClient_DI_Should_Provide_Injectable_Client()
        {
            Assert.True(true);
        }
    }
}
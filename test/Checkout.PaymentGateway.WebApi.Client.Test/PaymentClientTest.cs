using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Models.ApiModels;
using Checkout.PaymentGateway.Models.ApiModels.Payment;
using Checkout.PaymentGateway.Models.ServiceModels;
using Checkout.PaymentGateway.WebApi.Client.Exceptions;
using Checkout.PaymentGateway.WebApi.Client.Payment;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

namespace Checkout.PaymentGateway.WebApi.Client.Test
{
    public class PaymentClientTest
    {
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<HttpMessageHandler> _mockMessageHandler;
        private readonly PaymentClient _paymentClient;
        
        private const string BaseUri = "http://test.com/";
        public const string PaymentEndpoint = "api/v1/payment";
        
        public PaymentClientTest()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();

            _mockMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            _mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient(_mockMessageHandler.Object) {BaseAddress = new Uri(BaseUri)});
            
            _paymentClient = new PaymentClient(_mockHttpClientFactory.Object);
        }

        [Fact]
        public async void MakePayment_Should_Make_POST_Request_To_Payment_Endpoint()
        {
            var returnObject = new ResponseEnvelope<PaymentResponse>();

            var requestUri = BuildEndpoint(PaymentEndpoint);
            
            SetupMessageHandler(returnObject, requestUri, HttpStatusCode.OK);

            var result = await _paymentClient.MakePayment(new PaymentRequest());
            
            Assert.NotNull(result);
            VerifyClientInvocation(requestUri, Times.Once(), HttpMethod.Post);
        }

        [Fact]
        public async void MakePayment_Should_Return_Validation_Errors_For_BadRequest()
        {
            var returnObject = new ResponseEnvelope<PaymentResponse>
            {
                ValidationErrors = new List<ValidationFieldError>
                {
                    new ValidationFieldError("Field", "Error")
                }
            };
            
            var requestUri = BuildEndpoint(PaymentEndpoint);
            
            SetupMessageHandler(returnObject, requestUri, HttpStatusCode.BadRequest);
            
            var result = await _paymentClient.MakePayment(new PaymentRequest());
            
            VerifyClientInvocation(requestUri, Times.Once(), HttpMethod.Post);
            Assert.NotNull(result?.ValidationErrors);
            Assert.Single(result.ValidationErrors);
        }

        [Fact]
        public async void MakePayment_Should_Throw_If_Unhandled_Exception()
        {
            var returnObject = new ResponseEnvelope<PaymentResponse>();
            
            var requestUri = BuildEndpoint(PaymentEndpoint);
            
            SetupMessageHandler(returnObject, requestUri, HttpStatusCode.InternalServerError);
            
            await Assert.ThrowsAsync<UnexpectedRequestFailureException>(async () =>
            {
                var result = await _paymentClient.MakePayment(new PaymentRequest());
            });

        }
        
        [Fact]
        public async void GetPaymentDetails_Should_Make_GET_Request_To_Payment_Endpoint()
        {
            var paymentIdentifier = Guid.NewGuid().ToString();
            
            var returnObject = new PaymentResult();

            var requestUri = BuildEndpoint($"{PaymentEndpoint}/{paymentIdentifier}");
            
            SetupMessageHandler(returnObject, requestUri, HttpStatusCode.OK);

            var result = _paymentClient.GetPaymentResult(paymentIdentifier);

            Assert.NotNull(result);
            VerifyClientInvocation(requestUri, Times.Once(), HttpMethod.Get);
        }
        
        private void VerifyClientInvocation(string endpoint, Times times, HttpMethod httpMethod)
        {
            _mockMessageHandler.Protected()
                .Verify("SendAsync",
                    times, false, ItExpr.Is<HttpRequestMessage>(req => 
                        req.RequestUri.ToString() == endpoint && req.Method == httpMethod
                    ), ItExpr.IsAny<CancellationToken>());
        }

        private string BuildEndpoint(string endpoint)
        {
            return $"{BaseUri}{endpoint}";
        }

        private void SetupMessageHandler(object returnObject, string endpoint, HttpStatusCode statusCode)
        {
            _mockMessageHandler
                .Protected()
                // Setup the PROTECTED method to mock
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri.ToString() == endpoint),
                    ItExpr.IsAny<CancellationToken>()
                )
                // prepare the expected response of the mocked http call
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = statusCode,
                    Content = new StringContent(JsonConvert.SerializeObject(returnObject)),
                })
                .Verifiable();
        }
    }
}
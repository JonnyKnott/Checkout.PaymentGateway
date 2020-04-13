using System.Net;
using System.Net.Http;
using System.Text;
using Checkout.PaymentGateway.Models.ApiModels;
using Checkout.PaymentGateway.Models.ApiModels.Payment;
using Checkout.PaymentGateway.WebApi.Integration.Test.Infrastructure;
using Checkout.PaymentGateway.WebApi.Integration.Test.Services;
using Newtonsoft.Json;
using Xunit;

namespace Checkout.PaymentGateway.WebApi.Integration.Test.Controllers
{
    public class PaymentControllerTests : IClassFixture<ServerFactory>
    {
        private readonly HttpClient _client;

        public PaymentControllerTests(ServerFactory serverFactory)
        {
            _client = serverFactory.CreateClient();
        }

        [Fact]
        public async void POST_Should_Return_Bad_Request_If_Request_Invalid()
        {
            var request = new PaymentRequest
            {
                CardNumber = TestConstants.BadRequestCardNumber
            };

            var response = await _client.PostAsync("/api/v1/Payment",
                new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseEnvelope = JsonConvert.DeserializeObject<ResponseEnvelope<PaymentResponse>>(responseContent);
            
            Assert.NotNull(responseEnvelope.ValidationErrors);
            Assert.NotEmpty(responseEnvelope.ValidationErrors);
        }
        
        [Fact]
        public async void POST_Should_Return_Success_If_Request_Valid_And_Succeeds()
        {
            var request = new PaymentRequest
            {
                CardNumber = TestConstants.SuccessCardNumber
            };

            var response = await _client.PostAsync("/api/v1/Payment",
                new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
            
            Assert.True(response.IsSuccessStatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseEnvelope = JsonConvert.DeserializeObject<ResponseEnvelope<PaymentResponse>>(responseContent);
            
            Assert.Null(responseEnvelope.ValidationErrors);
            Assert.NotNull(responseEnvelope.ResponseValue);
        }
        
        [Fact]
        public async void POST_Should_Return_Error()
        {
            var request = new PaymentRequest
            {
                CardNumber = TestConstants.ErrorCardNumber
            };

            var response = await _client.PostAsync("/api/v1/Payment",
                new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
            
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }
    }
}
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Models.ApiModels;
using Checkout.PaymentGateway.Models.ApiModels.Payment;
using Checkout.PaymentGateway.Models.ServiceModels;
using Checkout.PaymentGateway.WebApi.Client.Exceptions;
using Newtonsoft.Json;

namespace Checkout.PaymentGateway.WebApi.Client.Payment
{
    public class PaymentClient : IPaymentClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PaymentClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        public async Task<ResponseEnvelope<PaymentResponse>> MakePayment(PaymentRequest paymentRequest)
        {
            var client = _httpClientFactory.CreateClient(ClientConstants.HttpClientName);
            
            var content = new StringContent(JsonConvert.SerializeObject(paymentRequest), Encoding.Default, "application/json");

            using (var response = await client.PostAsync(ClientConstants.PaymentEndpoint, content))
            {
                return await GenerateReturnTypeFromResponse<ResponseEnvelope<PaymentResponse>>(response);
            }
        }

        public async Task<PaymentResult> GetPaymentResult(string paymentIdentifier)
        {
            var client = _httpClientFactory.CreateClient(ClientConstants.HttpClientName);

            using (var response = await client.GetAsync($"{ClientConstants.PaymentEndpoint}/{paymentIdentifier}"))
            {
                return await GenerateReturnTypeFromResponse<PaymentResult>(response);
            }
        }

        private async Task<TResponseType> GenerateReturnTypeFromResponse<TResponseType>(HttpResponseMessage responseMessage)
        where TResponseType : class, new()
        {
            var content = await responseMessage.Content.ReadAsStringAsync();
            
            switch (responseMessage.StatusCode)
            {
                case HttpStatusCode.OK:
                    return JsonConvert.DeserializeObject<TResponseType>(content);;                        
                case HttpStatusCode.BadRequest:
                    return JsonConvert.DeserializeObject<TResponseType>(content);;
                default:
                    throw new UnexpectedRequestFailureException(
                        $"An error occurred processing the payment request",
                        await responseMessage.Content.ReadAsStringAsync(),
                        responseMessage.StatusCode
                    );
            }
            
            

            return JsonConvert.DeserializeObject<TResponseType>(content);
        }
        
        
    }
}
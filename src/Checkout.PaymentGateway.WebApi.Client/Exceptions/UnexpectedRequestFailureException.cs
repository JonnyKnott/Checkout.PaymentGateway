using System;
using System.Net;

namespace Checkout.PaymentGateway.WebApi.Client.Exceptions
{
    public class UnexpectedRequestFailureException : Exception
    {
        public string ResponseContent { get; }
        public HttpStatusCode ResponseCode { get; }
        
        public UnexpectedRequestFailureException(string message, string responseContent, HttpStatusCode responseCode) : base(message)
        {
            ResponseContent = responseContent;
            ResponseCode = responseCode;
        }
    }
}
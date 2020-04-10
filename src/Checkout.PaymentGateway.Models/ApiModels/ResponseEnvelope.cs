using System.Collections.Generic;
using Checkout.PaymentGateway.Models.ServiceModels;

namespace Checkout.PaymentGateway.Models.ApiModels
{
    public class ResponseEnvelope<TResponseType>
        where TResponseType : class, new()
    {
        public TResponseType ResponseValue { get; set; }
        public ICollection<ValidationFieldError> ValidationErrors { get; set; }
    }
}
using System.Collections.Generic;

namespace Checkout.PaymentGateway.Models.ServiceModels
{
    public class ServiceResult
    {
        protected ServiceResult()
        {
            
        }
        protected ServiceResult(ICollection<string> errors)
        {
            Errors = errors;
        }

        public bool Success => Errors == null;
        public ICollection<string> Errors { get; } 
        
        public static ServiceResult Succeeded()
        {
            return new ServiceResult();
        }

        public static ServiceResult Failed(ICollection<string> errors)
        {
            return new ServiceResult(errors);
        }

        public static ServiceResult Failed(string error)
        {
            return new ServiceResult(new List<string>{ error });
        }
    }
}
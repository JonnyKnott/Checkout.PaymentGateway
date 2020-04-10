namespace Checkout.PaymentGateway.Models.ServiceModels
{
    public static class ErrorMessages
    {
        public class Validation
        {
            public const string FieldInvalidLength = "The field was not of a valid length";
            public const string FieldInvalidContent = "The field contained invalid characters";
            public const string FieldMissing = "the field is required but was missing";
        }

        public const string UnableToProcessPayment = "The payment request failed and the reason is unknown. Please try again";
    }
}
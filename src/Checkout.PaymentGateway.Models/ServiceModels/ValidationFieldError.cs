namespace Checkout.PaymentGateway.Models.ServiceModels
{
    public class ValidationFieldError
    {
        public string FieldName { get; }
        public string Error { get; }

        public ValidationFieldError(string fieldName, string error)
        {
            FieldName = fieldName;
            Error = error;
        }
    }
}
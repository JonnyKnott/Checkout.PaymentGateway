using System.Net;

namespace Checkout.PaymentGateway.Models.ServiceModels
{
    public static class ErrorCodeStrings
    {
        public static string InternalError = HttpStatusCode.InternalServerError.ToString();
        public static string BadRequestError = HttpStatusCode.BadRequest.ToString();
        public static string NotFoundError = HttpStatusCode.NotFound.ToString();
    }
}
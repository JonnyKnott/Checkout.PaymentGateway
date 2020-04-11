using System.Linq;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Models.ApiModels;
using Checkout.PaymentGateway.Models.ApiModels.Payment;
using Checkout.PaymentGateway.Models.ServiceModels;
using Checkout.PaymentGateway.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Checkout.PaymentGateway.WebApi.Controllers
{
    [ApiController]
    [Route( "api/v{version:apiVersion}/{controller}" )]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IPaymentService _service;

        public PaymentController(ILogger<PaymentController> logger, IPaymentService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Post(PaymentRequest request)
        {
            var result = await _service.ProcessPaymentRequest(request);
            
            return GenerateResultFromServiceResult(result);
        }

        [HttpGet("{paymentIdentifier}")]
        public async Task<IActionResult> Get(string paymentIdentifier)
        {
            var result = await _service.GetPaymentResult(paymentIdentifier);

            return GenerateResultFromServiceResult(result);
        }

        private IActionResult GenerateResultFromServiceResult<TResultType>(
            ServiceObjectResult<TResultType> serviceObjectResult)
        {

            if (serviceObjectResult.Success)
                return Ok(serviceObjectResult.Result);

            if (serviceObjectResult.Errors.Contains(ErrorCodeStrings.InternalError))
                return StatusCode(500, serviceObjectResult.Errors);
            
            if (serviceObjectResult.Errors.Contains(ErrorCodeStrings.BadRequestError))
                return BadRequest(serviceObjectResult.Result);
            
            if (serviceObjectResult.Errors.Contains(ErrorCodeStrings.NotFoundError))
                return NotFound(serviceObjectResult.Result);

            if (serviceObjectResult.Errors.Any())
                return StatusCode(500, serviceObjectResult.Errors);

            return StatusCode(500, "The request could not be processed due to an unknown reason.");

        }
    }
}
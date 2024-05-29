using Ecommerce.Service.src.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Ecommerce.Controller.src.Controllers
{
    [ApiController]
    [Route("api/v1/webhooks")]
    public class StripeWebhookController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IConfiguration _config;

        public StripeWebhookController(IOrderService orderService, IConfiguration config)
        {
            _orderService = orderService;
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> HandleStripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _config["Stripe:WhSecret"]
            );

            if (stripeEvent.Type == Events.PaymentIntentSucceeded)
            {
                if (stripeEvent.Data.Object is PaymentIntent paymentIntent)
                {
                    await _orderService.MarkOrderAsPaid(paymentIntent.Id);
                }
            }
            else if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
            {
                if (stripeEvent.Data.Object is PaymentIntent paymentIntent)
                {
                    await _orderService.MarkOrderAsFailed(paymentIntent.Id);
                }
            }

            return Ok(stripeEvent);
        }
    }
}
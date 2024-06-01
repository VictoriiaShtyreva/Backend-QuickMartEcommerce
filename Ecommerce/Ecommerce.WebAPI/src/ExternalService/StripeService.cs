using Ecommerce.Service.src.Interfaces;
using Stripe;
using Stripe.Checkout;

namespace Ecommerce.WebAPI.src.ExternalService
{
    public class StripeService : IStripeService
    {
        private readonly string? _secretKey;
        public StripeService(IConfiguration configuration)
        {
            _secretKey = configuration["Stripe:SecretKey"];
            StripeConfiguration.ApiKey = _secretKey;
        }

        public async Task<string> CreateCheckoutSession(decimal amount, string currency = "usd")
        {
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(amount * 100),
                            Currency = currency,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Order",
                            },
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = "https://quick-mart-ecommerce.vercel.app/success",
                CancelUrl = "https://quick-mart-ecommerce.vercel.app/cancel",
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);
            return session.Url;
        }
    }
}
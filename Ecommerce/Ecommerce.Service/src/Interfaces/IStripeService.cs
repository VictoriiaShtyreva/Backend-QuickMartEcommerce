namespace Ecommerce.Service.src.Interfaces
{
    public interface IStripeService
    {
        Task<string> CreateCheckoutSession(decimal amount, string currency = "usd");
    }
}
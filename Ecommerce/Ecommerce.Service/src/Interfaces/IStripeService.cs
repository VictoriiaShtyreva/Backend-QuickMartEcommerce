namespace Ecommerce.Service.src.Interfaces
{
    public interface IStripeService
    {
        Task<string> CreatePaymentIntent(decimal amount, string currency);
    }
}
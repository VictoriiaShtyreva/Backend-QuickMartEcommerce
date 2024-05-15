namespace Ecommerce.Service.src.Interfaces
{
    public interface IShoppingCartService
    {
        Task<bool> AddProductToCartAsync(Guid userId, Guid productId, int quantity);
    }
}
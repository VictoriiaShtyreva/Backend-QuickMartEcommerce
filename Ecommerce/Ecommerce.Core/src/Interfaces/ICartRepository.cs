using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities.CartAggregate;

namespace Ecommerce.Core.src.Interfaces
{
    public interface ICartRepository : IBaseRepository<Cart, QueryOptions>
    {
        // Method to retrieve a cart by its associated user ID
        Task<Cart> GetCartByUserIdAsync(Guid userId);
    }
}
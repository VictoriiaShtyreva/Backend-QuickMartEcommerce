using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities.CartAggregate;
using Ecommerce.Service.src.DTOs;

namespace Ecommerce.Service.src.Interfaces
{
    public interface ICartService : IBaseService<CartReadDto, CartCreateDto, CartUpdateDto, QueryOptions>
    {
        Task<CartReadDto> GetCartByUserIdAsync(Guid userId);

        Task<CartItem> AddProductToCartAsync(Guid userId, Guid productId, int quantity);

        Task<bool> RemoveItemFromCartAsync(Guid cartId, Guid itemId);

        Task<bool> ClearCartAsync(Guid cartId);
    }
}
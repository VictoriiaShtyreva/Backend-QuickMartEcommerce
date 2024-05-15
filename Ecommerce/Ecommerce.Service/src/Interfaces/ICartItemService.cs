using Ecommerce.Core.src.Common;
using Ecommerce.Service.src.DTOs;

namespace Ecommerce.Service.src.Interfaces
{
    public interface ICartItemService : IBaseService<CartItemReadDto, CartItemCreateDto, CartItemUpdateDto, QueryOptions>
    {
        Task<CartItemReadDto> AddQuantityAsync(Guid cartItemId, Guid productId, int quantity);
        Task<CartItemReadDto> ReduceQuantityAsync(Guid cartItemId, Guid productId, int quantity);

    }
}
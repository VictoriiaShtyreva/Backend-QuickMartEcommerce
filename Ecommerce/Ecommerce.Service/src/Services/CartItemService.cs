using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities.CartAggregate;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Service.src.DTOs;
using Ecommerce.Service.src.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Ecommerce.Service.src.Services
{
    public class CartItemService : BaseService<CartItem, CartItemReadDto, CartItemCreateDto, CartItemUpdateDto, QueryOptions>, ICartItemService
    {
        public CartItemService(IBaseRepository<CartItem, QueryOptions> repository, IMapper mapper, IMemoryCache cache)
        : base(repository, mapper, cache)
        {

        }

        public async Task<CartItemReadDto> AddQuantityAsync(Guid cartItemId, Guid productId, int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
            }
            var cartItem = await _repository.GetByIdAsync(cartItemId);
            if (cartItem == null || cartItem.ProductId != productId)
            {
                throw new InvalidOperationException("Cart item not found or product mismatch.");
            }
            cartItem.AddQuantity(quantity);
            await _repository.UpdateAsync(cartItem);
            return _mapper.Map<CartItemReadDto>(cartItem);
        }

        public async Task<CartItemReadDto> ReduceQuantityAsync(Guid cartItemId, Guid productId, int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
            }
            var cartItem = await _repository.GetByIdAsync(cartItemId);
            if (cartItem == null || cartItem.ProductId != productId)
            {
                throw new InvalidOperationException("Cart item not found or product mismatch.");
            }
            cartItem.ReduceQuantity(quantity);
            await _repository.UpdateAsync(cartItem);
            return _mapper.Map<CartItemReadDto>(cartItem);
        }
    }
}
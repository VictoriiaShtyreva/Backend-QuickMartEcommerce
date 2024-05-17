using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities.CartAggregate;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Service.src.DTOs;
using Ecommerce.Service.src.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Ecommerce.Service.src.Services
{
    public class CartService : BaseService<Cart, CartReadDto, CartCreateDto, CartUpdateDto, QueryOptions>, ICartService
    {
        private readonly ICartRepository _cartRepository;


        public CartService(ICartRepository cartRepository, IMapper mapper, IMemoryCache cache)
           : base(cartRepository, mapper, cache)
        {
            _cartRepository = cartRepository;

        }

        public async Task<CartItemCreateDto> AddProductToCartAsync(Guid userId, Guid productId, int quantity)
        {
            var cartItem = await _cartRepository.AddProductToCartAsync(userId, productId, quantity);
            InvalidateCacheForCart(userId);
            return _mapper.Map<CartItemCreateDto>(cartItem);
        }

        public async Task<bool> ClearCartAsync(Guid cartId)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId) ?? throw AppException.NotFound();
            cart.ClearCart();
            await _cartRepository.UpdateAsync(cart);
            InvalidateCacheForCart(cart.UserId);
            return true;
        }

        public async Task<CartReadDto> GetCartByUserIdAsync(Guid userId)
        {
            if (!_cache.TryGetValue($"Cart-{userId}", out Cart? cart))
            {
                cart = await _cartRepository.GetCartByUserIdAsync(userId) ?? throw AppException.NotFound();
                _cache.Set($"Cart-{userId}", cart, TimeSpan.FromMinutes(30));
            }
            return _mapper.Map<CartReadDto>(cart);
        }

        public async Task<bool> RemoveItemFromCartAsync(Guid cartId, CartItem cartItem)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId) ?? throw AppException.NotFound();
            cart.RemoveItem(cartItem);
            await _cartRepository.UpdateAsync(cart);
            InvalidateCacheForCart(cart.UserId);
            return true;
        }

        private void InvalidateCacheForCart(Guid userId)
        {
            _cache.Remove($"Cart-{userId}");
        }
    }
}
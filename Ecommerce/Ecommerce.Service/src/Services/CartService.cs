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
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CartService(ICartRepository cartRepository, IMapper mapper, IUnitOfWork unitOfWork, IProductRepository productRepository, IMemoryCache cache)
           : base(cartRepository, mapper, cache)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<CartItem> AddProductToCartAsync(Guid userId, Guid productId, int quantity)
        {
            using (var transaction = _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    // Fetch the product and check inventory
                    var product = await _productRepository.GetByIdAsync(productId);
                    if (product == null || product.Inventory < quantity)
                    {
                        throw new InvalidOperationException("Product is unavailable or inventory is insufficient.");
                    }
                    var cart = await _cartRepository.GetCartByUserIdAsync(userId);
                    var cartItem = new CartItem(cart.Id, productId, quantity);
                    cart.AddItem(cartItem);
                    // Decrease product inventory
                    product.Inventory -= quantity;
                    await _productRepository.UpdateAsync(product);
                    // Update the cart
                    await _cartRepository.UpdateAsync(cart);
                    await _unitOfWork.CommitAsync();
                    // Return the newly added cart item
                    return cartItem;
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackAsync();
                    throw new ApplicationException("Failed to add product to cart.", ex);
                }
            }
        }

        public async Task<bool> ClearCartAsync(Guid cartId)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId) ?? throw AppException.NotFound();
            cart.ClearCart();
            await _cartRepository.UpdateAsync(cart);
            return true;
        }

        public async Task<CartReadDto> GetCartByUserIdAsync(Guid userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId) ?? throw AppException.NotFound();
            return _mapper.Map<CartReadDto>(cart);
        }

        public async Task<bool> RemoveItemFromCartAsync(Guid cartId, CartItem cartItem)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId) ?? throw AppException.NotFound();
            cart.RemoveItem(cartItem);
            await _cartRepository.UpdateAsync(cart);
            return true;
        }
    }
}
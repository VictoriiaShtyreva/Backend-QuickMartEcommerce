using AutoMapper;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.Entities.CartAggregate;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Service.src.DTOs;
using Ecommerce.Service.src.Interfaces;

namespace Ecommerce.Service.src.Services
{
    public class CartService : BaseService<Cart, CartReadDto, CartCreateDto, CartUpdateDto, QueryOptions>, ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CartService(ICartRepository cartRepository, IMapper mapper, IUnitOfWork unitOfWork, IProductRepository productRepository)
           : base(cartRepository, mapper)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<CartItem> AddProductToCartAsync(Guid userId, Guid productId, int quantity)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var product = await _productRepository.GetByIdAsync(productId);
                    if (product == null || product.Inventory < quantity)
                    {
                        throw new InvalidOperationException("Product is unavailable or inventory is insufficient.");
                    }
                    product.Inventory -= quantity;
                    await _productRepository.UpdateAsync(product);
                    var cart = await _cartRepository.GetCartByUserIdAsync(userId) ?? new Cart(userId);
                    cart.AddItem(productId, quantity);
                    await _cartRepository.UpdateAsync(cart);
                    await _unitOfWork.CommitAsync();
                    return cart.CartItems?.FirstOrDefault(ci => ci.ProductId == productId && ci.Quantity == quantity)!;
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
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

        public async Task<bool> RemoveItemFromCartAsync(Guid cartId, Guid itemId)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId) ?? throw AppException.NotFound();
            if (cart.CartItems == null || cart.CartItems.Count == 0)
            {
                throw new InvalidOperationException("There are no items in the cart.");
            }
            var item = cart.CartItems.FirstOrDefault(i => i.Id == itemId) ?? throw AppException.NotFound();
            cart.RemoveItem(item.ProductId, item.Quantity);
            await _cartRepository.UpdateAsync(cart);
            return false;
        }
    }
}
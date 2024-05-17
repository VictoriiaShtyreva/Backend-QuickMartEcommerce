using Ecommerce.Core.src.Entities.CartAggregate;
using Ecommerce.Service.src.DTOs;
using Ecommerce.Service.src.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controller.src.Controllers
{
    [ApiController]
    [Route("api/carts")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IAuthorizationService _authorizationService;

        public CartController(ICartService cartService, IAuthorizationService authorizationService)
        {
            _cartService = cartService;
            _authorizationService = authorizationService;
        }

        [HttpGet("users/{userId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CartReadDto>> GetCartByUserIdAsync([FromRoute] Guid userId)
        {
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, new UserReadDto { Id = userId }, "AdminOrOwnerAccount");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
            return Ok(cart);
        }

        [HttpPost("users/{userId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CartItemReadDto>> AddProductToCartAsync([FromRoute] Guid userId, [FromBody] CartItemCreateDto addProductDto)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, new UserReadDto { Id = userId }, "AdminOrOwnerAccount");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
            var cartItem = await _cartService.AddProductToCartAsync(userId, addProductDto.ProductId, addProductDto.Quantity);
            return Ok(cartItem);
        }

        [HttpDelete("users/{userId}/items/{itemId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RemoveItemFromCartAsync([FromRoute] Guid userId, [FromRoute] Guid itemId)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, new UserReadDto { Id = userId }, "AdminOrOwnerAccount");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            var cartItem = cart.Items.FirstOrDefault(item => item.Id == itemId);
            if (cartItem == null)
            {
                return NotFound("Cart item not found.");
            }
            var cartItemEntity = new CartItem { Id = itemId, CartId = cart.Id, ProductId = cartItem.ProductId, Quantity = cartItem.Quantity };
            await _cartService.RemoveItemFromCartAsync(cart.Id, cartItemEntity);
            return Ok();
        }

        [HttpPost("users/{userId}/clear")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ClearCartAsync([FromRoute] Guid userId)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, new UserReadDto { Id = userId }, "AdminOrOwnerAccount");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            await _cartService.ClearCartAsync(cart.Id);
            return Ok();
        }





    }
}
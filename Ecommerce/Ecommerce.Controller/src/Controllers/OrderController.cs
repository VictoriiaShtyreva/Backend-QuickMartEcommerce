using System.Security.Claims;
using Ecommerce.Core.src.Common;
using Ecommerce.Core.src.ValueObjects;
using Ecommerce.Service.src.DTOs;
using Ecommerce.Service.src.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controller.src.Controllers
{
    [ApiController]
    [Route("api/v1/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IOrderItemService _orderItemService;
        private readonly IUserService _userService;
        private readonly IAuthorizationService _authorizationService;
        public OrderController(IOrderService orderService, IOrderItemService orderItemService, IUserService userService, IAuthorizationService authorizationService)
        {
            _orderService = orderService;
            _orderItemService = orderItemService;
            _userService = userService;
            _authorizationService = authorizationService;

        }

        [HttpGet("{orderId}")]
        [Authorize]
        [ActionName(nameof(GetOrderAsync))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<OrderReadDto>> GetOrderAsync([FromRoute] Guid orderId)
        {
            var order = await _orderService.GetOneByIdAsync(orderId);
            var userId = GetUserIdClaim();
            if (order == null || (order.UserId != userId && !IsUserAdmin()))
            {
                return Forbid();
            }
            return Ok(order);
        }

        [HttpGet()]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllOrdersAsync([FromQuery] QueryOptions options)
        {
            var result = await _orderService.GetAllAsync(options);
            return Ok(result);
        }

        [HttpGet("users/{userId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetOrdersByUserIdAsync([FromRoute] Guid userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }

        [HttpPost()]
        [Authorize]
        [ActionName(nameof(CreateOrderAsync))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<OrderReadDto>> CreateOrderAsync([FromBody] OrderCreateDto orderCreateDto)
        {
            var createdOrder = await _orderService.CreateOrderAsync(orderCreateDto);
            return CreatedAtAction(nameof(GetOrderAsync), new { orderId = createdOrder.Id }, createdOrder);
        }

        [HttpPatch("{orderId}/status")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateOrderStatusAsync([FromRoute] Guid orderId, [FromBody] OrderStatusUpdateDto orderStatusUpdateDto)
        {
            var order = await _orderService.GetOneByIdAsync(orderId);
            var userId = GetUserIdClaim();
            if (order == null || (order.UserId != userId && !IsUserAdmin()))
            {
                return Forbid();
            }
            var updated = await _orderService.UpdateOrderStatusAsync(orderId, orderStatusUpdateDto.NewStatus);
            if (!updated) return NotFound();
            return Ok(updated);
        }

        [HttpPatch("{orderId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateOrderAsync([FromRoute] Guid orderId, [FromBody] OrderUpdateDto orderUpdateDto)
        {
            var order = await _orderService.GetOneByIdAsync(orderId);
            var userId = GetUserIdClaim();
            if (order == null || (order.UserId != userId && !IsUserAdmin()))
            {
                return Forbid();
            }
            var updated = await _orderService.UpdateOrderAsync(orderId, orderUpdateDto);
            if (!updated) return NotFound();
            return Ok(updated);
        }

        [HttpPatch("{orderId}/cancel")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CancelOrderAsync([FromRoute] Guid orderId)
        {
            var order = await _orderService.GetOneByIdAsync(orderId);
            var userId = GetUserIdClaim();
            if (order == null || (order.UserId != userId && !IsUserAdmin()))
            {
                return Forbid();
            }
            var canceled = await _orderService.CancelOrderAsync(orderId);
            if (!canceled) return BadRequest();
            return Ok(canceled);
        }

        [HttpDelete("{orderId}/delete")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteOrderAsync([FromRoute] Guid orderId)
        {
            var order = await _orderService.GetOneByIdAsync(orderId);
            var userId = GetUserIdClaim();

            if (order == null || (order.UserId != userId && !IsUserAdmin()))
            {
                return Forbid();
            }

            var deleted = await _orderService.DeleteOneAsync(orderId);
            if (!deleted) return NotFound();
            return Ok(deleted);
        }

        private Guid GetUserIdClaim()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? throw new Exception("User ID claim not found");
            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new Exception("Invalid user ID format");
            }
            return userId;
        }

        private bool IsUserAdmin()
        {
            var userRoleClaim = User.FindFirst(ClaimTypes.Role);
            return userRoleClaim != null && userRoleClaim.Value == UserRole.Admin.ToString();
        }

    }
}
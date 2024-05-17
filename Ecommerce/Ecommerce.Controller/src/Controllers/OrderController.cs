using Ecommerce.Core.src.Common;
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
            return Ok(order);
        }

        [HttpGet()]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IEnumerable<OrderReadDto>> GetAllOrdersAsync([FromQuery] QueryOptions options)
        {
            try
            {
                return await _orderService.GetAllAsync(options);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
            var createdOrder = await _orderService.CreateOrderFromCartAsync(orderCreateDto);
            return CreatedAtAction(nameof(GetOrderAsync), new { orderId = createdOrder.Id }, createdOrder);
        }

    }
}
using Ecommerce.Core.src.ValueObjects;

namespace Ecommerce.Service.src.DTOs
{
    public class OrderReadDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderItemReadDto>? OrderItems { get; set; }
        public AddressReadDto? ShippingAddress { get; set; }
        public string? CheckoutUrl { get; set; }
        public string? StripeSessionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class OrderCreateDto
    {
        public Guid UserId { get; set; }
        public List<OrderItemCreateDto> OrderItems { get; set; } = new List<OrderItemCreateDto>();
        public AddressCreateDto? ShippingAddress { get; set; } = new AddressCreateDto();
    }

    public class OrderUpdateDto
    {
        public OrderStatus? Status { get; set; }
        public List<OrderItemUpdateDto>? OrderItems { get; set; }
        public AddressUpdateDto? ShippingAddress { get; set; }
    }

    public class OrderStatusUpdateDto
    {
        public OrderStatus NewStatus { get; set; }
    }
}
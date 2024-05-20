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
        public AddressDto? ShippingAddress { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class OrderCreateDto
    {
        public Guid UserId { get; set; }
        public List<OrderItemCreateDto> OrderItems { get; set; } = new List<OrderItemCreateDto>();
        public AddressDto? ShippingAddress { get; set; } = new AddressDto();
    }

    public class OrderUpdateDto
    {
        public OrderStatus? Status { get; set; }
        public List<OrderItemUpdateDto>? OrderItems { get; set; }
        public AddressDto? ShippingAddress { get; set; }
    }

    public class OrderStatusUpdateDto
    {
        public OrderStatus NewStatus { get; set; }
    }
}
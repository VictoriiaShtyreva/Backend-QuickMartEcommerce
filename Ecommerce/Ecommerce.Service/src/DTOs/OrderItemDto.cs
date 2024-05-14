namespace Ecommerce.Service.src.DTOs
{
    public class OrderItemReadDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public ProductSnapshotDto? ProductSnapshot { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public class OrderItemCreateDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderItemUpdateDto
    {
        public Guid ItemId { get; set; }
        public int? Quantity { get; set; }  // Nullable if only updating is needed, not replacement
    }
}
namespace Ecommerce.Service.src.DTOs
{
    public class CartItemReadDto
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CartItemCreateDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class CartItemUpdateDto
    {
        public int Quantity { get; set; }
    }

}
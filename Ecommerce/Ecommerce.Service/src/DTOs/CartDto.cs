namespace Ecommerce.Service.src.DTOs
{

    public class CartReadDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public List<CartItemReadDto> Items { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public CartReadDto()
        {
            Items = new List<CartItemReadDto>();
        }
    }

    public class CartCreateDto
    {
        public Guid UserId { get; set; }
    }

    public class CartUpdateDto
    {
        public List<CartItemUpdateDto> Items { get; set; }

        public CartUpdateDto()
        {
            Items = new List<CartItemUpdateDto>();
        }
    }

}
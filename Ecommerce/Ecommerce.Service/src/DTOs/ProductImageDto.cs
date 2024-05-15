namespace Ecommerce.Service.src.DTOs
{
    public class ProductImageReadDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string? Url { get; set; }

    }

    public class ProductImageCreateDto
    {
        public Guid ProductId { get; set; }
        public string? Url { get; set; }
    }


    public class ProductImageUpdateDto
    {
        public string? Url { get; set; }
    }
}
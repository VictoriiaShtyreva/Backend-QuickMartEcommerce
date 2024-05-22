using Microsoft.AspNetCore.Http;

namespace Ecommerce.Service.src.DTOs
{
    public class ProductReadDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public CategoryReadDto? Category { get; set; }
        public IEnumerable<ReviewReadDto>? Reviews { get; set; }
        public int Inventory { get; set; }
        public IEnumerable<ProductImageReadDto>? Images { get; set; }
    }


    public class ProductCreateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int Price { get; set; }
        public Guid CategoryId { get; set; }
        public int Inventory { get; set; }
        public IEnumerable<IFormFile>? Images { get; set; }

    }

    public class ProductUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public Guid? CategoryId { get; set; }
        public int? Inventory { get; set; }
        public IEnumerable<IFormFile>? Images { get; set; }
    }

    public class TopProducDto
    {
        public Guid ProductId { get; set; }
        public string? Title { get; set; }
        public int TotalQuantityPurchased { get; set; }
    }

}
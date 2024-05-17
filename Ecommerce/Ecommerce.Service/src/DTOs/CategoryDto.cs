using Microsoft.AspNetCore.Http;

namespace Ecommerce.Service.src.DTOs
{
    public class CategoryReadDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
    }

    public class CategoryCreateDto
    {
        public string? Name { get; set; }
        public IFormFile? Image { get; set; }
    }

    public class CategoryUpdateDto
    {
        public string? Name { get; set; }
        public IFormFile? Image { get; set; }
    }
}
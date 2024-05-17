using System.ComponentModel.DataAnnotations;
using Ardalis.GuardClauses;

namespace Ecommerce.Core.src.ValueObjects
{
    public class ProductSnapshot
    {
        [Key]
        public Guid ProductId { get; set; }
        public string? Title { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public List<string>? ImageUrls { get; set; }

        public ProductSnapshot() { }
        public ProductSnapshot(Guid productId, string title, decimal price, string description, List<string> imageUrls)
        {
            ProductId = Guard.Against.Default(productId, nameof(productId));
            Title = Guard.Against.NullOrEmpty(title, nameof(title));
            Price = Guard.Against.NegativeOrZero(price, nameof(price));
            Description = Guard.Against.NullOrEmpty(description, nameof(description));
            ImageUrls = imageUrls ?? new List<string>();
        }
    }
}
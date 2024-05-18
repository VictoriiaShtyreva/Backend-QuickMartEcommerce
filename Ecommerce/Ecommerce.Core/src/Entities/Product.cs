using Ardalis.GuardClauses;
using Ecommerce.Core.src.Interfaces;
using Ecommerce.Core.src.ValueObjects;

namespace Ecommerce.Core.src.Entities
{
    public class Product : BaseEntity
    {
        public string? Title { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public virtual Category? Category { get; set; }
        public Guid CategoryId { get; set; }
        public int Inventory { get; set; }
        public virtual IEnumerable<ProductImage>? Images { get; set; }
        public virtual IEnumerable<Review>? Reviews { get; set; }

        public Product() { }
        public Product(string title, decimal price, string description, Guid categoryId, int inventory)
        {
            Id = Guid.NewGuid();
            Title = Guard.Against.NullOrWhiteSpace(title, nameof(title));
            Price = Guard.Against.NegativeOrZero(price, nameof(price));
            Description = Guard.Against.NullOrWhiteSpace(description, nameof(description));
            CategoryId = Guard.Against.Default(categoryId, nameof(categoryId));
            Inventory = Guard.Against.Negative(inventory, nameof(inventory));
        }

        // Method to create a snapshot of the product
        public async Task<ProductSnapshot> CreateSnapshotAsync(IProductImageRepository productImageRepository)
        {
            var productImages = await productImageRepository.GetProductImagesByProductIdAsync(Id);
            return new ProductSnapshot
            {
                ProductId = Id,
                Title = Title,
                Price = Price,
                Description = Description,
                ImageUrls = productImages.Select(img => img.Url).ToList()!
            };
        }
    }
}
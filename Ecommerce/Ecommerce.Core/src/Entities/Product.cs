using Ardalis.GuardClauses;
using Ecommerce.Core.src.Entities.CartAggregate;
using Ecommerce.Core.src.ValueObjects;

namespace Ecommerce.Core.src.Entities
{
    public class Product : BaseEntity
    {
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public Category? Category { get; set; }
        public Guid CategoryId { get; set; }
        public int Inventory { get; set; }
        public IEnumerable<ProductImage>? Images { get; set; }
        public IEnumerable<CartItem>? CartItems { get; set; }
        public IEnumerable<Review>? Reviews { get; set; }

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
        public ProductSnapshot CreateSnapshot()
        {
            return new ProductSnapshot(Id, Title, Price, Description);
        }
    }
}
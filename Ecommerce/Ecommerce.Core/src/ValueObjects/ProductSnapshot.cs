using Ardalis.GuardClauses;
using Ecommerce.Core.src.Entities;

namespace Ecommerce.Core.src.ValueObjects
{
    public class ProductSnapshot
    {
        public Guid ProductId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }

        public ProductSnapshot(Guid productId, string title, decimal price, string description)
        {

            ProductId = Guard.Against.Default(productId, nameof(productId));
            Title = Guard.Against.NullOrEmpty(title, nameof(title));
            Price = Guard.Against.NegativeOrZero(price, nameof(price));
            Description = Guard.Against.NullOrEmpty(description, nameof(description));
        }
    }
}
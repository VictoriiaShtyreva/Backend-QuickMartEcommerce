using Ardalis.GuardClauses;

namespace Ecommerce.Core.src.Entities
{
    public class Category : BaseEntity
    {
        public string? Name { get; set; }
        public string? Image { get; set; }
        public virtual IEnumerable<Product>? Products { get; set; }

        public Category() { }
        public Category(string name, string image)
        {
            Id = Guid.NewGuid();
            Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
            Image = Guard.Against.NullOrWhiteSpace(image, nameof(image));
        }

    }
}
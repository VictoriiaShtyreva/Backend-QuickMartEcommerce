using Ardalis.GuardClauses;

namespace Ecommerce.Core.src.Entities
{
    public class Review : TimeStamp
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public int Rating { get; set; }
        public string Content { get; set; }
        public User? User { get; set; }
        public Product? Product { get; set; }

        public Review(Guid userId, Guid productId, int rating, string content)
        {
            Id = Guid.NewGuid();
            UserId = Guard.Against.Default(userId, nameof(userId));
            ProductId = Guard.Against.Default(productId, nameof(productId));
            Rating = Guard.Against.OutOfRange(rating, nameof(rating), 1, 5);
            Content = Guard.Against.NullOrWhiteSpace(content, nameof(content));
        }
    }
}
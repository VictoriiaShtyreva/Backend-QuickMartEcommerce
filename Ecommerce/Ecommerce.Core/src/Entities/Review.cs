using Ardalis.GuardClauses;

namespace Ecommerce.Core.src.Entities
{
    public class Review : TimeStamp
    {
        public Guid? UserId { get; set; }
        public Guid? ProductId { get; set; }
        public int Rating { get; set; }
        public string? Content { get; set; }
        public virtual User? User { get; set; }
        public virtual Product? Product { get; set; }

        public Review() { }

        public Review(Guid userId, Guid productId, int rating, string content)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            ProductId = productId;
            Rating = Guard.Against.OutOfRange(rating, nameof(rating), 1, 5);
            Content = Guard.Against.NullOrWhiteSpace(content, nameof(content));
        }
    }
}
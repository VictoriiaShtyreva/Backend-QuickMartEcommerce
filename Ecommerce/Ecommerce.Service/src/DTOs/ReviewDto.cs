namespace Ecommerce.Service.src.DTOs
{
    public class ReviewReadDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public int Rating { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ReviewCreateDto
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public int Rating { get; set; }
        public string? Content { get; set; }
    }

    public class ReviewUpdateDto
    {
        public int? Rating { get; set; }
        public string? Content { get; set; }
    }
}
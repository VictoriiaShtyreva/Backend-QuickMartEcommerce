using Ardalis.GuardClauses;
using Ecommerce.Core.src.Entities.CartAggregate;
using Ecommerce.Core.src.Entities.OrderAggregate;
using Ecommerce.Core.src.ValueObjects;

namespace Ecommerce.Core.src.Entities
{
    public class User : BaseEntity
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Avatar { get; set; }
        public UserRole Role { get; set; } = UserRole.Customer;
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public int PostCode { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }

        public virtual IEnumerable<Order>? Orders { get; set; }
        public virtual Cart? Cart { get; set; }
        public virtual IEnumerable<Review>? Reviews { get; set; }

        public User() { }

        public User(string name, string email, string password, string avatar, UserRole role)
        {
            Id = Guid.NewGuid();
            Name = Guard.Against.NullOrWhiteSpace(name, nameof(Name));
            Email = Guard.Against.NullOrWhiteSpace(email, nameof(Email));
            Password = password;
            Avatar = Guard.Against.InvalidInput(avatar, nameof(avatar), uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute), "Image URL must be a valid URL.");
            Role = Guard.Against.Null(role, nameof(Role));
        }
    }
}
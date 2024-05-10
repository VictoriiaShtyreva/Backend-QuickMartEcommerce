using Ardalis.GuardClauses;
using Ecommerce.Core.src.Entities.CartAggregate;
using Ecommerce.Core.src.Entities.OrderAggregate;
using Ecommerce.Core.src.ValueObjects;

namespace Ecommerce.Core.src.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Avatar { get; set; }
        public UserRole Role { get; set; } = UserRole.Customer;
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public int PostCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public IEnumerable<Order>? Orders { get; set; }
        public Cart? Cart { get; set; }
        public IEnumerable<Review>? Reviews { get; set; }

        public User(string name, string email, string password, string avatar, UserRole role, string addressLine1, string addressLine2, int postCode, string city, string country)
        {
            Id = Guid.NewGuid();
            Name = Guard.Against.NullOrWhiteSpace(name, nameof(Name));
            Email = Guard.Against.NullOrWhiteSpace(email, nameof(Email));
            Password = Guard.Against.NullOrWhiteSpace(password, nameof(Password));
            Avatar = Guard.Against.InvalidInput(avatar, nameof(avatar), uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute), "Image URL must be a valid URL.");
            Role = Guard.Against.Null(role, nameof(Role));
            AddressLine1 = Guard.Against.NullOrWhiteSpace(addressLine1, nameof(AddressLine1));
            AddressLine2 = Guard.Against.NullOrWhiteSpace(addressLine2, nameof(AddressLine2));
            City = Guard.Against.NullOrWhiteSpace(city, nameof(City));
            Country = Guard.Against.NullOrWhiteSpace(country, nameof(Country));
        }
    }
}
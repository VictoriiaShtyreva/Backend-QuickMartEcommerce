using Ardalis.GuardClauses;

namespace Ecommerce.Core.src.Entities.OrderAggregate
{
    public class Address : BaseEntity
    {
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public int PostalCode { get; set; }
        public string? Country { get; set; }

        public Address()
        {
        }

        public Address(string addressLine, string city, int postalCode, string country)
        {
            Id = Guid.NewGuid();
            AddressLine = Guard.Against.NullOrWhiteSpace(addressLine, nameof(addressLine), "Address line is required.");
            City = Guard.Against.NullOrWhiteSpace(city, nameof(city), "City is required.");
            PostalCode = Guard.Against.Negative(postalCode, nameof(postalCode), "Postal code must be a positive number.");
            Country = Guard.Against.NullOrWhiteSpace(country, nameof(country), "Country is required.");
        }
    }
}
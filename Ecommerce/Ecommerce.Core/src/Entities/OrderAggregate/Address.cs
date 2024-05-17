using System.ComponentModel.DataAnnotations;
using Ardalis.GuardClauses;

namespace Ecommerce.Core.src.Entities.OrderAggregate
{
    public class Address : BaseEntity
    {
        public string? AddressLine { get; set; }
        public string? City { get; set; }

        [RegularExpression("^[0-9]{5}$", ErrorMessage = "Invalid postal code")]
        public string? PostalCode { get; set; }
        public string? Country { get; set; }

        public Address()
        {
        }

        public Address(string addressLine, string city, string postalCode, string country)
        {
            Id = Guid.NewGuid();
            AddressLine = Guard.Against.NullOrWhiteSpace(addressLine, nameof(addressLine), "Address line is required.");
            City = Guard.Against.NullOrWhiteSpace(city, nameof(city), "City is required.");
            PostalCode = Guard.Against.NullOrWhiteSpace(postalCode, nameof(postalCode), "Postal code is required");
            Country = Guard.Against.NullOrWhiteSpace(country, nameof(country), "Country is required.");
        }
    }
}
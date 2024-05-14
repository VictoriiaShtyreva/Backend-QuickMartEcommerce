namespace Ecommerce.Core.src.Entities.OrderAggregate
{
    public class Address : BaseEntity
    {
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public int PostalCode { get; set; }
        public string? Country { get; set; }
    }
}
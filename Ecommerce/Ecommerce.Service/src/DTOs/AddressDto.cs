namespace Ecommerce.Service.src.DTOs
{
    public class AddressDto
    {
        public Guid Id { get; set; }
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
    }
}
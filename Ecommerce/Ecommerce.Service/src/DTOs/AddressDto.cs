namespace Ecommerce.Service.src.DTOs
{
    public class AddressDto
    {
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public int PostalCode { get; set; }
        public string? Country { get; set; }
    }
}
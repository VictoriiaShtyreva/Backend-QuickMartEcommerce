using System.Text.Json.Serialization;

namespace Ecommerce.Core.src.ValueObjects
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserRole
    {
        Admin,
        Customer
    }
}
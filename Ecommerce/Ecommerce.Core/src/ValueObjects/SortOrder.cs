using System.Text.Json.Serialization;

namespace Ecommerce.Core.src.ValueObjects
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SortOrder
    {
        Ascending,   // Sort from lowest to highest (A-Z, 0-9, oldest to newest based on context)
        Descending   // Sort from highest to lowest (Z-A, 9-0, newest to oldest based on context)
    }
}
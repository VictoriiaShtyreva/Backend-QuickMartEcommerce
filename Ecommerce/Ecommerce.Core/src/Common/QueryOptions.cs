using System.Text.Json.Serialization;
using Ecommerce.Core.src.ValueObjects;

namespace Ecommerce.Core.src.Common
{
    public class QueryOptions
    {
        public virtual int Page { get; set; }
        public virtual int PageSize { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SortType SortBy { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SortOrder SortOrder { get; set; }
    }
}
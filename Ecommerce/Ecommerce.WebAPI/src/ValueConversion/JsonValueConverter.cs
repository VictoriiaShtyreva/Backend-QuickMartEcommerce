using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace Ecommerce.WebAPI.src.ValueConversion
{
    public class JsonValueConverter<T> : ValueConverter<T, string>
    {
        public JsonValueConverter()
            : base(
                v => JsonConvert.SerializeObject(v),
                v => DeserializeJson(v)
            )
        { }

        private static T DeserializeJson(string json)
        {
            var result = JsonConvert.DeserializeObject<T>(json);
            if (result == null)
            {
                throw new InvalidOperationException("Cannot convert JSON to object.");
            }
            return result;
        }
    }
}
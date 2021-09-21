using System.Text.Json.Serialization;

namespace AndNetwork9.Steam.Listeners
{
    public record PlayerActivityResult<T>
    {
        [JsonPropertyName("response")]
        public T Result { get; set; }
    }
}
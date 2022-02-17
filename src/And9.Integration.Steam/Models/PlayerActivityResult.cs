using System.Text.Json.Serialization;

namespace And9.Integration.Steam.Models;

public record PlayerActivityResult<T>
{
    [JsonPropertyName("response")]
    public T? Result { get; set; }
}
using System.Text.Json;
using System.Text.Json.Serialization;

namespace And9.Lib.Formatters.Json;

public class TimeOnlyConverter : JsonConverter<TimeOnly>
{
    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        new(reader.GetInt64());

    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Ticks);
    }
}
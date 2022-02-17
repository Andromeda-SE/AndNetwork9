using System.Text.Json;
using System.Text.Json.Serialization;

namespace And9.Lib.Formatters.Json;

public class DateOnlyConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        DateOnly.FromDayNumber(reader.GetInt32());

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.DayNumber);
    }
}
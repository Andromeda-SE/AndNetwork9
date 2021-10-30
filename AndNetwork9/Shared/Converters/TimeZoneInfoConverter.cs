using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AndNetwork9.Shared.Converters;

public class TimeZoneInfoConverter : JsonConverter<TimeZoneInfo>
{
    public override TimeZoneInfo? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? id = reader.GetString();
        if (id is null) return null;
        return TimeZoneInfo.FindSystemTimeZoneById(id);
    }

    public override void Write(Utf8JsonWriter writer, TimeZoneInfo value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Id);
    }
}
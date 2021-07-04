using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AndNetwork9.Shared.Converters
{
    public class DateOnlyConverter : JsonConverter<DateOnly>
    {
        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateOnly.FromDayNumber(reader.GetInt32());
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.DayNumber);
        }
    }
}
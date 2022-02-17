using MessagePack;
using MessagePack.Formatters;

namespace And9.Lib.Formatters.MessagePack;

public class TimeZoneInfoFormatter : IMessagePackFormatter<TimeZoneInfo?>
{
    public void Serialize(ref MessagePackWriter writer, TimeZoneInfo? value, MessagePackSerializerOptions options)
    {
        writer.Write(value?.Id);
    }

    public TimeZoneInfo? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.IsNil)
        {
            reader.ReadNil();
            return null;
        }

        string id = reader.ReadString();
        return TimeZoneInfo.FindSystemTimeZoneById(id);
    }
}
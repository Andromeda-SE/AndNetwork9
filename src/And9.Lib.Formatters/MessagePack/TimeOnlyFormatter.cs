using MessagePack;
using MessagePack.Formatters;

namespace And9.Lib.Formatters.MessagePack;

public class TimeOnlyFormatter : IMessagePackFormatter<TimeOnly>
{
    public void Serialize(ref MessagePackWriter writer, TimeOnly value, MessagePackSerializerOptions options) => writer.WriteInt64(value.Ticks);

    public TimeOnly Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        long ticks = reader.ReadInt64();
        return new(ticks);
    }
}
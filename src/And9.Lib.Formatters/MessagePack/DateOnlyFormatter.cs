using MessagePack;
using MessagePack.Formatters;

namespace And9.Lib.Formatters.MessagePack;

public class DateOnlyFormatter : IMessagePackFormatter<DateOnly>
{
    public void Serialize(ref MessagePackWriter writer, DateOnly value, MessagePackSerializerOptions options) => writer.WriteInt32(value.DayNumber);

    public DateOnly Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        int days = reader.ReadInt32();
        return DateOnly.FromDayNumber(days);
    }
}
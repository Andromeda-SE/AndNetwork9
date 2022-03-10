using System.Net;
using MessagePack;
using MessagePack.Formatters;

namespace And9.Lib.Formatters.MessagePack;

public class IpEndPointFormatter : IMessagePackFormatter<IPEndPoint?>
{
    public void Serialize(ref MessagePackWriter writer, IPEndPoint? value, MessagePackSerializerOptions options)
    {
        if (value is null)
            writer.WriteNil();
        else
            writer.Write(value.ToString());
    }

    public IPEndPoint? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        string? raw = reader.ReadString();
        return raw is null ? null : IPEndPoint.Parse(raw);
    }
}
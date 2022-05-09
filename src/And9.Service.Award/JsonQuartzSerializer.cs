using System.Text.Json;
using Quartz.Spi;

namespace And9.Service.Award;

public class JsonQuartzSerializer : IObjectSerializer
{
    public void Initialize() { }

    public byte[] Serialize<T>(T obj) where T : class => JsonSerializer.SerializeToUtf8Bytes(obj);

    public T? DeSerialize<T>(byte[] data) where T : class => JsonSerializer.Deserialize<T>(data);
}
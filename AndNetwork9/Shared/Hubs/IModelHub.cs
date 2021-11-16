using AndNetwork9.Shared.Interfaces;

namespace AndNetwork9.Shared.Hubs;

public interface IModelHub
{
    System.Threading.Tasks.Task ReceiveModelUpdate<T>(string? typeName, T model) where T : IId;
}
using And9.Gateway.Clan.Client.Interfaces;
using And9.Lib.Models.Abstractions;
using And9.Lib.Models.Abstractions.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace And9.Gateway.Clan.Client.Connections;

public class RepositoryConnection<T> : ReadOnlyRepositoryConnection<T> where T : IId
{
    internal RepositoryConnection(HubConnection connection, IAuthTokenProvider tokenProvider, ILogger<RepositoryConnection<T>> logger)
        : base(connection, tokenProvider, logger) { }

    public override bool IsReadOnly => false;


    public override async void Add(T item)
    {
        await Connection.InvokeAsync(nameof(IModelCrudServerMethods<T>.Create), item).ConfigureAwait(false);
    }

    public override bool Remove(T item)
    {
        RemoveAt(item.Id);
        return true;
    }


    public override async void Insert(int index, T item)
    {
        if (index != item.Id) throw new ArgumentException(nameof(index));
        await Connection.InvokeAsync(nameof(IModelCrudServerMethods<T>.Update), item).ConfigureAwait(false);
    }

    public override async void RemoveAt(int index)
    {
        await Connection.InvokeAsync(nameof(IModelCrudServerMethods<T>.Delete), index).ConfigureAwait(false);
    }
}
using System.Collections;
using System.Collections.Immutable;
using System.Collections.Specialized;
using And9.Gateway.Clan.Client.Interfaces;
using And9.Lib.Models.Abstractions;
using And9.Lib.Models.Abstractions.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace And9.Gateway.Clan.Client.Connections;

public class ReadOnlyRepositoryConnection<T> : ConnectionBase, IList<T>, IModelCrudClientMethods, INotifyCollectionChanged where T : IId
{
    protected IImmutableDictionary<int, T> Elements;

    internal ReadOnlyRepositoryConnection(HubConnection connection, IAuthTokenProvider tokenProvider, ILogger<ReadOnlyRepositoryConnection<T>> logger) : base(connection, tokenProvider, logger)
    {
        Connection.On(nameof(IModelCrudClientMethods.ModelUpdated), (Func<int, ModelState, Task>)ModelUpdated);
        Elements = ImmutableDictionary<int, T>.Empty;
    }

    public bool Loaded { get; protected set; }
    public IEnumerator<T> GetEnumerator() => Elements.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public virtual void Add(T item) => throw new NotSupportedException();

    public virtual void Clear() => throw new NotSupportedException();

    public bool Contains(T item) => Elements.Values.Contains(item);

    public void CopyTo(T[] array, int arrayIndex)
    {
        T[] elements = Elements.Values.ToArray();
        for (int i = 0; i < elements.Length; i++) array[arrayIndex + i] = elements[i];
    }

    public virtual bool Remove(T item) => throw new NotSupportedException();

    public int Count => Elements.Count;
    public virtual bool IsReadOnly => true;

    public int IndexOf(T item) => item.Id;

    public virtual void Insert(int index, T item) => throw new NotSupportedException();

    public virtual void RemoveAt(int index) => throw new NotSupportedException();

    public T this[int index]
    {
        get => Elements[index];
        set => Insert(index, value);
    }

    public async Task ModelUpdated(int id, ModelState state)
    {
        switch (state)
        {
            case ModelState.None:
                return;
            case ModelState.Created:
            {
                T element = await Connection.InvokeAsync<T>(nameof(IModelCrudServerMethods<T>.Read), id).ConfigureAwait(false);
                Elements = Elements.Add(id, element);
                CollectionChanged?.Invoke(this,
                    new(
                        NotifyCollectionChangedAction.Add,
                        element,
                        id));
            }

                break;
            case ModelState.Updated:
            {
                T newElement = await Connection.InvokeAsync<T>(nameof(IModelCrudServerMethods<T>.Read), id).ConfigureAwait(false);
                T oldElement = Elements[id];
                Elements = Elements.SetItem(id, newElement);
                CollectionChanged?.Invoke(this,
                    new(
                        NotifyCollectionChangedAction.Replace,
                        newElement,
                        oldElement,
                        id));
            }
                break;
            case ModelState.Deleted:
            {
                T oldElement = Elements[id];
                Elements = Elements.Remove(id);
                CollectionChanged?.Invoke(this,
                    new(NotifyCollectionChangedAction.Remove, oldElement, state));
            }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public async Task LoadAsync()
    {
        Elements = ImmutableDictionary<int, T>.Empty;
        if (Connection.State == HubConnectionState.Disconnected) await Connection.StartAsync().ConfigureAwait(false);
        await foreach (T element in Connection.StreamAsync<T>(nameof(IModelCrudServerMethods<T>.ReadAll)).ConfigureAwait(false)) Elements = Elements.Add(element.Id, element);
        CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Reset));
        Loaded = true;
    }
}
using System.Collections;
using System.Collections.Immutable;
using System.Collections.Specialized;
using And9.Gateway.Clan.Client.Interfaces;
using And9.Lib.Models.Abstractions;
using And9.Lib.Models.Abstractions.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace And9.Gateway.Clan.Client.Connections;

public class RepositoryConnection<T> : ConnectionBase, IList<T>, IModelCrudClientMethods, INotifyCollectionChanged where T : IId
{
    private IImmutableDictionary<int, T> _elements;

    internal RepositoryConnection(HubConnection connection, IAuthTokenProvider tokenProvider, ILogger<RepositoryConnection<T>> logger) : base(connection, tokenProvider, logger)
    {
        Connection.On(nameof(IModelCrudClientMethods.ModelUpdated), (Func<int, ModelState, Task>)ModelUpdated);
        _elements = ImmutableDictionary<int, T>.Empty;
    }

    public bool Loaded { get; protected set; }
    public IEnumerator<T> GetEnumerator() => _elements.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public async void Add(T item)
    {
        await Connection.InvokeAsync(nameof(IModelCrudServerMethods<T>.Create), item).ConfigureAwait(false);
    }

    public void Clear() => throw new NotSupportedException();

    public bool Contains(T item) => _elements.Values.Contains(item);

    public void CopyTo(T[] array, int arrayIndex)
    {
        T[] elements = _elements.Values.ToArray();
        for (int i = 0; i < elements.Length; i++) array[arrayIndex + i] = elements[i];
    }

    public bool Remove(T item)
    {
        RemoveAt(item.Id);
        return true;
    }

    public int Count => _elements.Count;
    public bool IsReadOnly => false;

    public int IndexOf(T item) => item.Id;

    public async void Insert(int index, T item)
    {
        if (index != item.Id) throw new ArgumentException(nameof(index));
        await Connection.InvokeAsync(nameof(IModelCrudServerMethods<T>.Update), item).ConfigureAwait(false);
    }

    public async void RemoveAt(int index)
    {
        await Connection.InvokeAsync(nameof(IModelCrudServerMethods<T>.Delete), index).ConfigureAwait(false);
    }

    public T this[int index]
    {
        get => _elements[index];
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
                _elements = _elements.Add(id, element);
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
                T oldElement = _elements[id];
                _elements = _elements.SetItem(id, newElement);
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
                T oldElement = _elements[id];
                _elements = _elements.Remove(id);
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
        _elements = ImmutableDictionary<int, T>.Empty;
        if (Connection.State == HubConnectionState.Disconnected) await Connection.StartAsync().ConfigureAwait(false);
        await foreach (T element in Connection.StreamAsync<T>(nameof(IModelCrudServerMethods<T>.ReadAll)).ConfigureAwait(false)) _elements = _elements.Add(element.Id, element);
        CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Reset));
        Loaded = true;
    }
}
using System.Net.Http;
using AndNetwork9.Client.Shared;
using AndNetwork9.Shared.Interfaces;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Pages;

public abstract class ElementPageBase<T> : ComponentBase where T : IId
{
    public T[] Entities { get; set; }

    [Parameter]
    public string SelectedKeyRaw
    {
        get => SelectedEntity.Id.ToString("D");
        set
        {
            if (!string.IsNullOrWhiteSpace(value) && int.TryParse(value, out int key))
                SetNewEntity(key);
            else
                SetNewEntity(null);
        }
    }

    [Inject]
    public HttpClient Client { get; set; }
    [Inject]
    public NavigationManager NavigationManager { get; set; }
    public T SelectedEntity { get; protected set; }
    public abstract ColumnDefinition[] ColumnDefinitions { get; }

    protected abstract void SetNewEntity(int? value);
}
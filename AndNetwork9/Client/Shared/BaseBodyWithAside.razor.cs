using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Shared;

public partial class BaseBodyWithAside
{
    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public RenderFragment Main { get; set; }

    [Parameter]
    public RenderFragment Aside { get; set; }

    [Parameter]
    public RenderFragment Footer { get; set; }
}
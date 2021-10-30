using System.Threading.Tasks;
using AndNetwork9.Client.Services;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Shared;

public partial class NavMenu
{
    [Inject]
    public AuthStateProvider AuthenticationStateProvider { get; set; } = null!;
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    public async Task Logout()
    {
        await AuthenticationStateProvider.LogoutAsync();
        NavigationManager.NavigateTo("/", true);
    }
}
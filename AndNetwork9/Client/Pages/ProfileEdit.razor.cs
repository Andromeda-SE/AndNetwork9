using System.Net.Http;
using System.Net.Http.Json;
using AndNetwork9.Client.Services;
using AndNetwork9.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Pages;

[Authorize]
public partial class ProfileEdit
{
    private Direction _direction;
    private string _nickname;
    private string _realname;
    [Inject]
    public HttpClient Client { get; set; }
    [Inject]
    public NavigationManager NavigationManager { get; set; }
    [Inject]
    public AuthStateProvider StateProvider { get; set; }

    [Parameter]
    public string Nickname
    {
        get => _nickname;
        set
        {
            _nickname = value;
            StateHasChanged();
        }
    }

    [Parameter]
    public string Realname
    {
        get => _realname;
        set
        {
            _realname = value;
            StateHasChanged();
        }
    }

    [Parameter]
    public Direction Direction
    {
        get => _direction;
        set
        {
            _direction = value;
            StateHasChanged();
        }
    }

    [Parameter]
    public string SelectedTimeZoneId { get; set; }


    [Parameter]
    public string PasswordFirst
    {
        get => _nickname;
        set
        {
            _nickname = value;
            StateHasChanged();
        }
    }

    [Parameter]
    public string PasswordSecond
    {
        get => _nickname;
        set
        {
            _nickname = value;
            StateHasChanged();
        }
    }

    private async System.Threading.Tasks.Task SetNewNickname()
    {
        await Client.PutAsJsonAsync("api/member/nickname", Nickname);
        NavigationManager.NavigateTo(NavigationManager.Uri, true);
    }

    private async System.Threading.Tasks.Task SetNewRealname()
    {
        await Client.PutAsJsonAsync("api/member/realname", Realname);
        NavigationManager.NavigateTo(NavigationManager.Uri, true);
    }

    private async System.Threading.Tasks.Task SetNewDirection()
    {
        await Client.PutAsJsonAsync("api/member/direction", Direction);
        NavigationManager.NavigateTo(NavigationManager.Uri, true);
    }

    private async System.Threading.Tasks.Task SetCurrentTimeZone()
    {
        await Client.PutAsJsonAsync("api/member/timezone", SelectedTimeZoneId);
        NavigationManager.NavigateTo(NavigationManager.Uri, true);
    }

    private async System.Threading.Tasks.Task SetNewPassword()
    {
        await Client.PutAsJsonAsync("api/auth/", PasswordFirst);
        await StateProvider.LogoutAsync();
        NavigationManager.NavigateTo(NavigationManager.Uri, true);
    }
}
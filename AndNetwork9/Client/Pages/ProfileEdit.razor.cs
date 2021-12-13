using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AndNetwork9.Client.Services;
using AndNetwork9.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Pages;

[Authorize]
public partial class ProfileEdit
{
    private Direction _direction;

    private string _firstPassword;
    private string _nickname;
    private string _realname;
    private string _secondPassword;
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
        get => _firstPassword;
        set
        {
            _firstPassword = value;
            StateHasChanged();
        }
    }

    [Parameter]
    public string PasswordSecond
    {
        get => _secondPassword;
        set
        {
            _secondPassword = value;
            StateHasChanged();
        }
    }

    private async Task SetNewNickname()
    {
        await Client.PutAsJsonAsync("api/member/nickname", Nickname);
        NavigationManager.NavigateTo(NavigationManager.Uri, false);
        AuthStateProvider.CurrentMember.Nickname = Nickname;
    }

    private async Task SetNewRealname()
    {
        await Client.PutAsJsonAsync("api/member/realname", Realname);
        NavigationManager.NavigateTo(NavigationManager.Uri, false);
    }

    private async Task SetNewDirection()
    {
        await Client.PutAsJsonAsync("api/member/direction", Direction);
        NavigationManager.NavigateTo(NavigationManager.Uri, false);
    }

    private async Task SetCurrentTimeZone()
    {
        await Client.PutAsJsonAsync("api/member/timezone", SelectedTimeZoneId);
        NavigationManager.NavigateTo(NavigationManager.Uri, false);
    }

    private async Task SetNewPassword()
    {
        await Client.PutAsJsonAsync("api/auth/", PasswordFirst);
        await StateProvider.LogoutAsync();
        NavigationManager.NavigateTo(NavigationManager.Uri, false);
    }
}
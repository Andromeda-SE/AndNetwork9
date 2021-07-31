using System;
using System.Net.Http;
using AndNetwork9.Client.Services;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Shared
{
    public partial class Login
    {
        [Inject]
        public AuthStateProvider AuthenticationStateProvider { get; set; } = null!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;
        [Inject]
        public HttpClient Client { get; set; } = null!;

        [Parameter]
        public string Nickname { get; set; }
        [Parameter]
        public string Password { get; set; }

        public bool LoginEnabled { get; set; } = true;
        public bool ErrorLogin { get; set; }

        public async void LoginAsync()
        {
            LoginEnabled = false;
            ErrorLogin = !await AuthenticationStateProvider.LoginAsync(new(Nickname, Password));
            if (!ErrorLogin) NavigationManager.NavigateTo("/", true);
            LoginEnabled = true;
            StateHasChanged();
        }
    }
}
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Task = System.Threading.Tasks.Task;

namespace AndNetwork9.Client.Services
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        public AuthStateProvider(HttpClient client) => Client = client;

        [Inject]
        public HttpClient Client { get; set; }
        public static Member CurrentMember { get; set; }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                CurrentMember = await Client.GetFromJsonAsync<Member>("api/member");
            }
            catch
            {
                await LogoutAsync();
                return new(new(new ClaimsIdentity(Array.Empty<Claim>(), null)));
            }

            return new(new(new ClaimsIdentity(Array.Empty<Claim>(), "Cookies")));
        }

        public async Task<bool> LoginAsync(AuthCredentials authCredentials)
        {
            HttpResponseMessage response = await Client.PostAsJsonAsync("api/auth", authCredentials);
            if (!response.IsSuccessStatusCode) return false;
            try
            {
                CurrentMember = await Client.GetFromJsonAsync<Member>("api/member");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            await Client.DeleteAsync("api/Auth");
            CurrentMember = null;
        }
    }
}
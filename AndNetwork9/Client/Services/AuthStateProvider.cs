using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Utility;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Task = System.Threading.Tasks.Task;

namespace AndNetwork9.Client.Services
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private const string _SESSION_ID_KEY = "SessionId";
        private const string _MEMBER_ID_KEY = "MemberId";

        public AuthStateProvider(HttpClient client, ILocalStorageService localStorage)
        {
            Client = client;
            LocalStorage = localStorage;
        }

        [Inject]
        public HttpClient Client { get; set; }
        [Inject]
        public ILocalStorageService LocalStorage { get; set; }
        public static Member CurrentMember { get; set; }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (await LocalStorage.ContainKeyAsync(_SESSION_ID_KEY))
            {
                Console.WriteLine(LocalStorage.GetItemAsStringAsync(_SESSION_ID_KEY));
                Guid sessionId = Guid.ParseExact(await LocalStorage.GetItemAsStringAsync(_SESSION_ID_KEY), "N");
                Console.WriteLine("ID = " + sessionId);
                HttpResponseMessage response = await Client.GetAsync($"api/Auth/restore/{sessionId}");
                if (response.IsSuccessStatusCode)
                {
                    CurrentMember = await Client.GetFromJsonAsync<Member>("api/member");
                    return new(new(new ClaimsIdentity(new[]
                    {
                        new Claim(_SESSION_ID_KEY, await LocalStorage.GetItemAsStringAsync(_SESSION_ID_KEY)),
                        new Claim(_MEMBER_ID_KEY, await LocalStorage.GetItemAsStringAsync(_MEMBER_ID_KEY)),
                    }, "Cookies")));
                }

                await LogoutAsync();
            }

            return new(new());
        }

        public async Task<bool> LoginAsync(AuthCredentials authCredentials)
        {
            HttpResponseMessage response = await Client.PostAsJsonAsync("api/Auth", authCredentials);
            if (!response.IsSuccessStatusCode) return false;
            Dictionary<string, string>
                values = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            Console.WriteLine(values["SessionId"]);
            await LocalStorage.SetItemAsStringAsync(_SESSION_ID_KEY, values[_SESSION_ID_KEY]);
            await LocalStorage.SetItemAsStringAsync(_MEMBER_ID_KEY, values[_MEMBER_ID_KEY]);
            CurrentMember = await Client.GetFromJsonAsync<Member>("api/member");
            return true;
        }

        public async Task LogoutAsync()
        {
            await Client.DeleteAsync("api/Auth");
            await LocalStorage.RemoveItemAsync(_SESSION_ID_KEY);
            await LocalStorage.RemoveItemAsync(_MEMBER_ID_KEY);
        }
    }
}
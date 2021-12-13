using System.ComponentModel.DataAnnotations;
using System.Net.Http;

namespace AndNetwork9.Client.Utility;

public class SteamValidation : RequiredAttribute
{
    public SteamValidation(HttpClient client) => Client = client;
    public HttpClient Client { get; }

    public override bool IsValid(object? value) =>
        /*Task<HttpResponseMessage> responseTask = new Task<HttpResponseMessage>(async () => await Post());
        //
        responseTask.Start();
        responseTask.WaitAsync(new TimeSpan(0,0,30));
        HttpResponseMessage response = responseTask.Result;
        switch (response.StatusCode)
        {
            case HttpStatusCode.NotFound:
                ErrorMessage = "Неверная ссылка на профиль Steam";
                return false;
            case HttpStatusCode.Conflict:
                ErrorMessage = "В клане уже известен игрок с такими данными. Обратитесь к первому советника клана за подробностями";
                return false;
        }
        ErrorMessage = null;*/
        base.IsValid(value);

    /* private async Task<HttpResponseMessage> Post()
    {
        await Client.PostAsJsonAsync("public/api/candidate/steam", value?.ToString());
    }*/
}
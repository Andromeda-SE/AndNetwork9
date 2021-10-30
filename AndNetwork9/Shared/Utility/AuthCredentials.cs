namespace AndNetwork9.Shared.Utility;

public record AuthCredentials(string Nickname, string Password)
{
    public string Nickname { get; set; } = Nickname;
    public string Password { get; set; } = Password;
}
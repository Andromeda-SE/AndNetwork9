namespace And9.Integration.Discord.API.Interfaces;

public interface IDiscordServerMethods
{
    Task SendDirectMessageAsync(ulong discordId, string message);
}
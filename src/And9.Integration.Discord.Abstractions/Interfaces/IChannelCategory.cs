using And9.Lib.Models.Abstractions.Interfaces;

namespace And9.Integration.Discord.Abstractions.Interfaces;

public interface IChannelCategory : IConcurrencyToken, IDiscordId
{
    public int Position { get; }
    public string Name { get; }
}
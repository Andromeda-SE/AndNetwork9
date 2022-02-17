using And9.Integration.Discord.Abstractions.Interfaces;

namespace And9.Integration.Discord.Database.Models;

public record class ChannelCategory : IChannelCategory
{
    public IReadOnlyCollection<Channel> Channels { get; set; } = null!;
    public ulong DiscordId { get; set; }
    public int Position { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid ConcurrencyToken { get; set; }
    public DateTime LastChanged { get; set; }
}
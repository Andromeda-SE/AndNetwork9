using And9.Integration.Discord.Senders;
using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using Discord;
using Discord.WebSocket;

namespace And9.Integration.Discord.ConsumerStrategies;

[QueueName(ResolveDiscordUserNameSender.QUEUE_NAME)]
public class ResolveDiscordUserNameConsumerStrategy : IBrokerConsumerWithResponseStrategy<string, ulong?>
{
    private readonly SocketGuild _guild;

    public ResolveDiscordUserNameConsumerStrategy(DiscordBot bot) => _guild ??= bot.GetGuild(bot.GuildId);


    public async ValueTask<ulong?> ExecuteAsync(string? request)
    {
        if (request is null) return default;
        request = request.Trim();
        if (string.IsNullOrWhiteSpace(request)) return default;
        string[] raw = request.Split('#', 2);
        if (raw.Length != 2) return default;
        IUser? user = await _guild.GetUsersAsync().Flatten()
            .FirstOrDefaultAsync(x => x.Username == raw[0] && x.Discriminator == raw[1]).ConfigureAwait(false);
        return user?.Id ?? default(ulong?);
    }
}
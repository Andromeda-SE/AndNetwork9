using And9.Integration.Discord.Database;
using And9.Integration.Discord.Senders;
using And9.Lib.Broker;
using And9.Lib.Broker.ConsumerStrategy;
using Discord.WebSocket;

namespace And9.Integration.Discord.ConsumerStrategies;

[QueueName(RegisterChannelCategorySender.QUEUE_NAME)]
public class RegisterChannelCategoryConsumerStrategy : IBrokerConsumerWithResponseStrategy<ulong, bool>
{
    private readonly DiscordBot _bot;
    private readonly DiscordDataContext _dataContext;

    public RegisterChannelCategoryConsumerStrategy(DiscordBot bot, DiscordDataContext dataContext)
    {
        _dataContext = dataContext;
        _bot = bot;
    }

    public async ValueTask<bool> ExecuteAsync(ulong categoryId)
    {
        SocketGuild? guild = _bot.GetGuild(_bot.GuildId);
        if (guild is null) throw new();

        SocketCategoryChannel? discordCategory = guild.GetCategoryChannel(categoryId);
        if (discordCategory is null) return false;

        await _dataContext.ChannelCategories.AddAsync(new()
        {
            DiscordId = discordCategory.Id,
            Position = discordCategory.Position,
            Name = discordCategory.Name,
            LastChanged = DateTime.UtcNow,
            ConcurrencyToken = Guid.NewGuid(),
        }).ConfigureAwait(false);
        await _dataContext.SaveChangesAsync().ConfigureAwait(false);
        return true;
    }
}
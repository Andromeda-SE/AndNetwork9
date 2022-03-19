using And9.Integration.Discord.Database;
using And9.Integration.Discord.Senders;
using And9.Lib.Broker;
using Discord.WebSocket;
using RabbitMQ.Client;

namespace And9.Integration.Discord.Listeners;

public class RegisterChannelCategoryListener : BaseRabbitListenerWithResponse<ulong, bool>
{
    private readonly DiscordBot _bot;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public RegisterChannelCategoryListener(IConnection connection, ILogger<RegisterChannelCategoryListener> logger, DiscordBot bot, IServiceScopeFactory serviceScopeFactory)
        : base(connection, RegisterChannelCategorySender.QUEUE_NAME, logger)
    {
        _bot = bot;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task<bool> GetResponseAsync(ulong categoryId)
    {
        SocketGuild? guild = _bot.GetGuild(_bot.GuildId);
        if (guild is null) throw new();

        SocketCategoryChannel? discordCategory = guild.GetCategoryChannel(categoryId);
        if (discordCategory is null) return false;

        await using AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        DiscordDataContext dataContext = scope.ServiceProvider.GetRequiredService<DiscordDataContext>();

        await dataContext.ChannelCategories.AddAsync(new()
        {
            DiscordId = discordCategory.Id,
            Position = discordCategory.Position,
            Name = discordCategory.Name,
            LastChanged = DateTime.UtcNow,
            ConcurrencyToken = Guid.NewGuid(),
        }).ConfigureAwait(false);
        await dataContext.SaveChangesAsync().ConfigureAwait(false);
        return true;
    }
}
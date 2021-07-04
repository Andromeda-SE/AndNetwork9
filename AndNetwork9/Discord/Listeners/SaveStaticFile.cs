using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.Discord;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Storage;
using AndNetwork9.Shared.Utility;
using Discord;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Direction = AndNetwork9.Shared.Enums.Direction;
using IConnection = RabbitMQ.Client.IConnection;

namespace AndNetwork9.Discord.Listeners
{
    public class SaveStaticFile : BaseRabbitListenerWithResponse<SaveStaticFileArg, StaticFile>
    {
        private readonly DiscordBot _bot;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ulong _storageId;

        public SaveStaticFile(IConnection connection, DiscordBot bot, IConfiguration configuration,
            IServiceScopeFactory scopeFactory) : base(connection, SaveStaticFileSender.QUEUE_NAME)
        {
            _bot = bot;
            _scopeFactory = scopeFactory;
            _storageId = ulong.Parse(configuration["Discord:StorageId"]);
        }

        protected override async Task<StaticFile> GetResponseAsync(SaveStaticFileArg request)
        {
            if (request.FileData.Length > 8388608) throw new ArgumentOutOfRangeException(nameof(request));
            using IServiceScope scope = _scopeFactory.CreateScope();
            ClanDataContext data = (ClanDataContext)scope.ServiceProvider.GetService(typeof(ClanDataContext))!;
            if (data is null) throw new ApplicationException();

            await using MemoryStream stream = new(request.FileData);
            IUserMessage result = await _bot.GetGuild(_bot.GuildId).GetTextChannel(_storageId)
                .SendFileAsync(stream, request.Name, string.Empty);
            string name = request.Name ?? Guid.NewGuid().ToString("N");
            int dotPos = name.LastIndexOf('.');
            AccessRule? accessRule = request.AccessRuleId is not null
                ? await data.AccessRules.FindAsync(request.AccessRuleId.Value)
                : null;
            if (accessRule is null)
            {
                accessRule = new()
                {
                    Id = 0,
                    Name = null,
                    Directions = Enum.GetValues<Direction>().ToArray(),
                    Squad = null,
                    MinRank = Rank.Neophyte,
                    SquadId = null,
                };
                await data.AccessRules.AddAsync(accessRule);
            }

            EntityEntry<StaticFile> staticFile = await data.StaticFiles.AddAsync(new()
            {
                Id = 0,
                Owner = request.OwnerId is not null ? await data.Members.FindAsync(request.OwnerId.Value) : null,
                Path = result.Attachments.Single().Url,
                ReadRule = accessRule,
                Extension = dotPos >= 0 ? name[(dotPos + 1)..] : string.Empty,
                Name = dotPos >= 0 ? name[..dotPos] : name,
            });
            await data.SaveChangesAsync();
            return staticFile.Entity;
        }
    }
}
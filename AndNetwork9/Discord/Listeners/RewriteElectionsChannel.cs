using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AndNetwork9.Discord.Services;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Discord.Channels;
using AndNetwork9.Shared.Backend.Discord.Enums;
using AndNetwork9.Shared.Backend.Elections;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.Discord;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Extensions;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ChannelType = AndNetwork9.Shared.Backend.Discord.Enums.ChannelType;
using Direction = AndNetwork9.Shared.Enums.Direction;
using IConnection = RabbitMQ.Client.IConnection;

namespace AndNetwork9.Discord.Listeners;

public class RewriteElectionsChannel : BaseRabbitListenerWithoutResponse<Election>
{
    private readonly DiscordBot _bot;
    private readonly IServiceScopeFactory _scopeFactory;

    public RewriteElectionsChannel(IConnection connection, DiscordBot bot, IServiceScopeFactory scopeFactory,
        ILogger<RewriteElectionsChannel> logger) :
        base(connection, RewriteElectionsChannelSender.QUEUE_NAME, logger)
    {
        _bot = bot;
        _scopeFactory = scopeFactory;
    }

    public override Task Run(Election request)
    {
        Process(request);
        return Task.CompletedTask;
    }

    private void Process(Election request)
    {
        Task.Run(async () =>
        {
            AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
            await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
            ClanDataContext data = (ClanDataContext)scope.ServiceProvider.GetService(typeof(ClanDataContext))!;
            if (data is null) throw new ApplicationException();

            int nicknameLength = Math.Max(data.Members.Select(x => x.Nickname.Length).Max(), "Против всех".Length);
            await foreach (Channel channel in data.DiscordChannels
                               .Where(x => x.Type == ChannelType.Text && x.ChannelFlags.HasFlag(ChannelFlags.Elections))
                               .ToAsyncEnumerable().ConfigureAwait(false))
            {
                SocketTextChannel discordChannel = _bot.GetGuild(_bot.GuildId).GetTextChannel(channel.DiscordId);
                IMessage[] messages = (await discordChannel.GetMessagesAsync(5, RequestOptions.Default)
                        .ToArrayAsync().ConfigureAwait(false))
                    .SelectMany(x => x).ToArray();

                for (int i = 0; i < 5 - messages.Length; i++)
                    await discordChannel.SendMessageAsync("…").ConfigureAwait(false);

                foreach ((IMessage message, Direction direction) in messages.Zip(Enum.GetValues<Direction>()
                             .Where(x => x > Direction.None)))
                    await discordChannel.ModifyMessageAsync(message.Id,
                        properties =>
                        {
                            properties.Content =
                                new(
                                    GetVotingMessage(request.Votings.Single(x => x.Direction == direction),
                                        nicknameLength));
                        },
                        RequestOptions.Default).ConfigureAwait(false);
            }
        });
    }

    private static string GetVotingMessage(ElectionVoting voting, int nicknameLength)
    {
        StringBuilder text = new(512);

        text.AppendFormat("Направление: **{0}**", voting.Direction.GetName());
        text.AppendLine();

        int totalVoters = voting.Members.Count(x => x.Votes is null);
        int currentVoters = voting.Members.Count(x => x.Votes is null && x.Voted);
        text.AppendFormat("Явка: {0:D}/{1:D}\t({2:P0})",
            currentVoters,
            totalVoters,
            totalVoters == 0 ? "--- %" : (double)currentVoters / totalVoters);
        text.AppendLine();

        const int rankLength = 5;
        const int barLength = 10;
        const int numLength = 3;
        int totalVotes = totalVoters * voting.Members.Count(x => x.Votes is not null);

        SortedDictionary<ElectionsMember, string> strings = new();

        foreach (ElectionsMember electionsMember in voting.Members.Where(x => x.Votes is not null))
            strings.Add(electionsMember, GetElectionsMemberString(electionsMember));

        ElectionsMember againstAllMember = new()
        {
            Voting = voting,
            Votes = voting.AgainstAll,
            Direction = voting.Direction,
            VotedTime = DateTime.MaxValue,
            Voted = true,
            MemberId = 0,
            ElectionId = voting.ElectionId,
            Member = new()
            {
                Nickname = "Против всех",
                Rank = Rank.None,
            },
        };
        strings.Add(againstAllMember, GetElectionsMemberString(againstAllMember));

        text.AppendLine("```");
        text.AppendJoin(Environment.NewLine, strings.Values);
        text.AppendLine("```");

        return text.ToString();

        string GetElectionsMemberString(ElectionsMember electionsMember)
        {
            StringBuilder stringText = new(64);
            string? rankIcon = electionsMember.Member.Rank.GetRankIcon();
            string rank = rankIcon is null ? string.Empty : $"[{rankIcon}]";
            stringText.Append(rank);
            stringText.Append(' ', rankLength - rank.Length + 1);

            stringText.Append(electionsMember.Member.Nickname);
            stringText.Append(' ', nicknameLength - electionsMember.Member.Nickname.Length + 1);

            double percent = electionsMember.Votes.GetValueOrDefault(0) / (double)totalVotes;
            int filledCount = (int)Math.Round(barLength * (double.IsNaN(percent) ? 0 : percent),
                0,
                MidpointRounding.ToEven);
            int emptyCount = barLength - filledCount;
            stringText.Append('[');
            if (filledCount > 0) stringText.Append('|', filledCount);
            if (emptyCount > 0) stringText.Append('-', emptyCount);
            stringText.Append(']');
            stringText.Append(' ');

            string votesCount = electionsMember.Votes is null ? "—" : electionsMember.Votes.Value.ToString("D");
            int numSpaces = numLength - votesCount.Length;
            if (numSpaces > 0) stringText.Append(' ', numSpaces);
            stringText.Append(votesCount);
            stringText.Append(' ');

            stringText.Append('(');
            stringText.Append(Math.Round(percent, 2, MidpointRounding.ToEven) switch
            {
                < 0 => throw new ArgumentOutOfRangeException(nameof(electionsMember), "percent < 0"),
                < 10 => percent.ToString("P3"),
                < 100 => percent.ToString("P2"),
                100 => percent.ToString("P1"),
                double.NaN => "-.--- %",
                _ => throw new ArgumentOutOfRangeException(nameof(electionsMember), "percent > 100"),
            });
            stringText.Append(')');
            return stringText.ToString();
        }
    }
}
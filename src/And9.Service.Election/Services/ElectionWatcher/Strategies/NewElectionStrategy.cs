using System.Globalization;
using System.Text;
using And9.Integration.Discord.Senders;
using And9.Service.Core.Abstractions;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Senders.Member;
using And9.Service.Election.Abstractions.Enums;
using And9.Service.Election.Abstractions.Interfaces;
using And9.Service.Election.Database;

namespace And9.Service.Election.Services.ElectionWatcher.Strategies;

public class NewElectionStrategy : IElectionWatcherStrategy
{
    private readonly ElectionDataContext _electionDataContext;
    private readonly ReadAllMembersSender _readAllMembersSender;
    private readonly ReadMemberByIdSender _readMemberByIdSender;
    private readonly SendLogMessageSender _sendLogMessageSender;

    public NewElectionStrategy(
        ElectionDataContext electionDataContext,
        ReadAllMembersSender readAllMembersSender,
        ReadMemberByIdSender readMemberByIdSender,
        SendLogMessageSender sendLogMessageSender)
    {
        _electionDataContext = electionDataContext;
        _readAllMembersSender = readAllMembersSender;
        _readMemberByIdSender = readMemberByIdSender;
        _sendLogMessageSender = sendLogMessageSender;
    }

    public async Task UpdateElections()
    {
        StringBuilder result = new(4096);
        result.AppendLine("Результаты выборов: ");
        result.AppendLine();
        await foreach (Member oldAdvisor in _readAllMembersSender.CallAsync(0).Where(x => x.Rank == Rank.Advisor).ConfigureAwait(false)) oldAdvisor.Rank = Rank.Neophyte;
        await foreach (Abstractions.Models.Election election in _electionDataContext.GetCurrentElectionsAsync().ConfigureAwait(false))
        {
            if (election.Status != ElectionStatus.Announcement) throw new();
            election.Status = ElectionStatus.Ended;
            _electionDataContext.Elections.Add(new()
            {
                AdvisorsStartDate = election.AdvisorsStartDate.AddDays(90),
                AgainstAllVotes = 0,
                Direction = election.Direction,
                Status = ElectionStatus.Registration,
                ConcurrencyToken = Guid.NewGuid(),
                LastChanged = DateTime.UtcNow,
            });

            IElectionVote? winner = election.Votes.Where(x => !x.Voted.HasValue).MaxBy(x => x.Votes);
            if (winner is null) continue;
            if (election.AgainstAllVotes > winner.Votes) continue;
            Member member = await _readMemberByIdSender.CallAsync(winner.MemberId).ConfigureAwait(false) ?? throw new();
            member.Rank = Rank.Advisor;

            result.Append("**");
            result.Append("Итоги выборов");
            result.Append("**");
            result.AppendLine();
            result.AppendFormat(election.Direction.GetDisplayString());
            result.AppendLine();
            result.AppendLine("```");
            double allVotesCount = election.Votes.Where(x => !x.Voted.HasValue).Sum(x => x.Votes);
            const int bar = 20;
            IEnumerable<(int?, int)> allVotes = election.Votes
                .Where(x => !x.Voted.HasValue)
                .Select(x => ((int?)x.MemberId, x.Votes))
                .Append((null, election.AgainstAllVotes))
                .OrderBy(x => x.Votes);
            foreach ((int? id, int votes) in allVotes)
            {
                string nickname = id is not null
                    ? (await _readMemberByIdSender.CallAsync(winner.MemberId).ConfigureAwait(false) ?? throw new()).Nickname
                    : "Против всех";
                result.Append(nickname.PadLeft(24, ' '));
                result.Append(' ');
                result.Append('[');
                double fillPercent = votes / allVotesCount;
                int barFilled = (int)Math.Round(fillPercent * bar, MidpointRounding.ToZero);
                if (barFilled > 0) result.Append('|', barFilled);
                if (barFilled < bar) result.Append('-', bar - barFilled);
                result.Append(']');
                result.Append(' ');
                string count = votes.ToString("D");
                result.Append(count.PadRight(4, ' '));
                result.Append('(');
                string p = fillPercent switch
                {
                    < 0.1d => fillPercent.ToString("P3", CultureInfo.InvariantCulture),
                    < 1d => fillPercent.ToString("P2", CultureInfo.InvariantCulture),
                    _ => fillPercent.ToString("P1", CultureInfo.InvariantCulture),
                };
                result.Append(p);
                result.Append(')');
                result.AppendLine();
            }

            result.AppendLine("```");
            result.AppendLine();
        }

        await Task.WhenAll(
                _sendLogMessageSender.CallAsync(result.ToString()).AsTask(),
                _electionDataContext.SaveChangesAsync())
            .ConfigureAwait(false);
    }
}
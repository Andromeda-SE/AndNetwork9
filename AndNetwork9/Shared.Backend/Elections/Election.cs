using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AndNetwork9.Shared.Converters;
using AndNetwork9.Shared.Elections;
using AndNetwork9.Shared.Enums;

namespace AndNetwork9.Shared.Backend.Elections
{
    public record Election
    {
        public int Id { get; set; }

        [JsonConverter(typeof(DateOnlyConverter))]
        public DateOnly RegistrationDate { get; set; }
        [JsonConverter(typeof(DateOnlyConverter))]
        public DateOnly VoteDate { get; set; }
        [JsonConverter(typeof(DateOnlyConverter))]
        public DateOnly AnnouncementDate { get; set; }
        [JsonConverter(typeof(DateOnlyConverter))]
        public DateOnly StartDate { get; set; }
        [JsonConverter(typeof(DateOnlyConverter))]
        public DateOnly EndDate { get; set; }

        public ElectionStage Stage { get; set; }

        public virtual IList<ElectionVoting> Votings { get; set; } = new List<ElectionVoting>();

        public  CouncilElection GetCouncilElection(Member member)
        {
            List<CouncilElectionVote>? votes = this.Votings.Select(x => new CouncilElectionVote()
            {
                Direction = x.Direction,
                Votes = x.Members.Where(y => y.Votes is not null).ToDictionary(y => y.MemberId, y => (uint)y.Votes!),
            }).ToList();
            foreach ((CouncilElectionVote councilElectionVote, ElectionVoting electionVoting) in
                votes.Join(this.Votings, x => x.Direction, x => x.Direction, (vote, voting) => (vote, voting)))
            {
                councilElectionVote.Votes.Add(0, (uint)electionVoting.AgainstAll);
                councilElectionVote.VoteAllowed = electionVoting.Members.Any(x => x.MemberId == member.Id && !x.Voted);
            }
            return new()
            {
                Id = this.Id,
                RegistrationDate = this.RegistrationDate,
                VoteDate = this.VoteDate,
                AnnouncementDate = this.AnnouncementDate,
                StartDate = this.StartDate,
                EndDate = this.EndDate,

                Stage = this.Stage,

                Votes = votes,
            };
        }
    }
}
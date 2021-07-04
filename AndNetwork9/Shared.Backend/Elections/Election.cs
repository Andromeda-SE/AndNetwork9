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

        public static explicit operator CouncilElection(Election election)
        {
            return new()
            {
                Id = election.Id,
                RegistrationDate = election.RegistrationDate,
                VoteDate = election.VoteDate,
                AnnouncementDate = election.AnnouncementDate,
                StartDate = election.StartDate,
                EndDate = election.EndDate,

                Stage = election.Stage,

                Votes = election.Votings.ToDictionary(x => x.Direction, x => x.Members.Where(y => y.Votes is not null)
                    .Append(new()
                    {
                        MemberId = 0,
                        Votes = x.AgainstAll,
                    }).ToDictionary(y => y.MemberId, y => y.Votes ?? throw new ArgumentException())),
            };
        }
    }
}
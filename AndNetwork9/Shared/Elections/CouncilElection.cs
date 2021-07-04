using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using AndNetwork9.Shared.Converters;
using AndNetwork9.Shared.Enums;

namespace AndNetwork9.Shared.Elections
{
    public record CouncilElection
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

        public Dictionary<Direction, Dictionary<int, int>> Votes { get; set; } = new();
    }
}
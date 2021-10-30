using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using AndNetwork9.Shared.Converters;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Interfaces;

namespace AndNetwork9.Shared.Elections;

public record CouncilElection : IId
{
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

    public List<CouncilElectionVote> Votes { get; set; } = new();
    public int Id { get; set; }
    public Guid ConcurrencyToken { get; set; }
}
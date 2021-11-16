using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Interfaces;

namespace AndNetwork9.Shared.Backend.Elections;

public record ElectionVoting : IConcurrencyToken
{
    public int ElectionId { get; set; }

    [JsonIgnore]
    public virtual Election Election { get; set; } = null!;

    public Direction Direction { get; set; }

    public int AgainstAll { get; set; } = 0;
    [JsonIgnore]
    public virtual IList<ElectionsMember> Members { get; set; } = new List<ElectionsMember>();

    public Guid ConcurrencyToken { get; set; }

    public Member? GetWinner()
    {
        ElectionsMember? winner = Members.OrderByDescending(x => x.Member.Awards.Sum(y => y.Points)).Where(x => x.Votes is not null).MaxBy(x => x.Votes);
        return winner is not null && AgainstAll <= winner.Votes ? winner.Member : null;
    }

    public DateTime LastChanged { get; set; }
}
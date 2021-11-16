using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Interfaces;

namespace AndNetwork9.Shared.Utility;

public record AccessRule : IComparable<AccessRule>, IId
{
    public string? Name { get; set; }

    public virtual Direction[] Directions { get; set; } = Enum.GetValues<Direction>().ToArray();
    public Rank MinRank { get; set; } = Rank.Neophyte;

    public short? SquadId { get; set; }
    public short? SquadPartId { get; set; }
    [JsonIgnore]
    public virtual Squad? Squad { get; set; }
    [JsonIgnore]
    public virtual SquadPart? SquadPart { get; set; }

    [JsonIgnore]
    public virtual IList<Member> AllowedMembers { get; set; } = new List<Member>();

    public virtual int[] AllowedMembersIds { get; set; } = Array.Empty<int>();

    public int CompareTo(AccessRule? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        return Id.CompareTo(other.Id);
    }

    public int Id { get; set; }

    public Guid ConcurrencyToken { get; set; }

    public bool HasAccess(Member member) => member.Rank == Rank.FirstAdvisor
                                            || AllowedMembersIds.Any(x => x == member.Id)
                                            || Directions.Any(x => x == member.Direction)
                                            && member.Rank >= MinRank
                                            && (
                                                SquadId is null && SquadPartId is null
                                                || SquadId is not null && SquadPartId is null && SquadId == member.SquadPartNumber
                                                || SquadId is not null
                                                && SquadPartId is not null
                                                && SquadId == member.SquadPartNumber
                                                && SquadPartId == member.SquadPartNumber);

    public DateTime LastChanged { get; set; }
}
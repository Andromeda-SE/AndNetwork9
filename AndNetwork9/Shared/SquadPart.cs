using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using AndNetwork9.Shared.Extensions;
using AndNetwork9.Shared.Interfaces;

namespace AndNetwork9.Shared;

public record SquadPart : IId
{
    public short Number { get; set; }
    public short Part { get; set; }

    [JsonIgnore]
    public ulong? DiscordRoleId { get; set; }

    [JsonIgnore]
    public virtual Squad Squad { get; set; }
    [JsonIgnore]
    public virtual IList<Member> Members { get; set; } = new List<Member>();

    [JsonIgnore]
    public Member Captain
    {
        get
        {
            return Squad?.SquadParts?.Single(x => x.Part == 0).Members.Single(x => x.SquadCommander)
                   ?? throw new();
        }
    }

    [JsonIgnore]
    public Member Lieutenant
    {
        get { return Members.Single(x => x.SquadCommander) ?? throw new(); }
    }


    public string? Description { get; set; }
    [JsonIgnore]
    public string? Comment { get; set; }
    public Guid ConcurrencyToken { get; set; }
    [JsonIgnore]
    int IId.Id => Number << (16 + Part);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToString(bool includeSquad)
    {
        string result = string.Empty;
        if (includeSquad) result += $"{Number.ToRoman()} отряд, ";
        return result + (Part == 0 ? "головное" : Part.ToString("D")) + " отделение";
    }

    public override string ToString()
    {
        return ToString(true);
    }
}
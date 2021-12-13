using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AndNetwork9.Shared.Converters;
using AndNetwork9.Shared.Extensions;
using AndNetwork9.Shared.Interfaces;

namespace AndNetwork9.Shared;

public record Squad : IId
{
    public short Number { get; set; }
    public string? Name { get; set; }

    public ulong? DiscordRoleId { get; set; }

    [JsonConverter(typeof(DateOnlyConverter))]
    public DateOnly CreateDate { get; set; }

    [JsonConverter(typeof(DateOnlyConverter))]
    public DateOnly? DisbandDate { get; set; }

    public string? Description { get; set; }
    [JsonIgnore]
    public string? Comment { get; set; }

    [JsonIgnore]
    public virtual Member Captain => SquadParts?.Single(x => x.Part == 0).Commander ?? throw new();

    [JsonIgnore]
    public virtual IList<SquadPart>? SquadParts { get; set; } = new List<SquadPart>();
    [JsonIgnore]
    public virtual IList<Member>? Candidates { get; set; } = new List<Member>();
    int IId.Id => Number;

    public Guid ConcurrencyToken { get; set; }
    public DateTime LastChanged { get; set; }

    public override string ToString()
    {
        return string.IsNullOrWhiteSpace(Name) ? $"Отряд {Number.ToRoman()}" : $"Отряд {Number.ToRoman()} {Name}";
    }
}
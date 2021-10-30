using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using AndNetwork9.Shared.Converters;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Extensions;
using AndNetwork9.Shared.Interfaces;
using AndNetwork9.Shared.Storage;
using AndNetwork9.Shared.Utility;
using AndNetwork9.Shared.Votings;

namespace AndNetwork9.Shared;

public record Member : IComparable<Member>, IEquatable<Member?>, IId, IComparable
{
    public ulong? SteamId { get; set; }
    public ulong? DiscordId { get; set; }
    public long? VkId { get; set; }
    public long? TelegramId { get; set; }

    public string Nickname { get; set; } = string.Empty;
    public string? RealName { get; set; }

    [JsonConverter(typeof(TimeZoneInfoConverter))]
    public TimeZoneInfo? TimeZone { get; set; }

    [JsonConverter(typeof(DateOnlyConverter))]
    public DateOnly JoinDate { get; set; }

    public Rank Rank { get; set; }
    public Direction Direction { get; set; }

    [JsonConverter(typeof(DateOnlyConverter))]
    public DateOnly LastDirectionChange { get; set; }

    public short? SquadNumber { get; set; }
    public short SquadPartNumber { get; set; }

    public bool SquadCommander { get; set; }

    [JsonIgnore]
    public SquadCommander CommanderLevel
    {
        get
        {
            if (SquadNumber is null && !SquadCommander) return Enums.SquadCommander.None;
            return SquadPartNumber == 0 ? Enums.SquadCommander.Captain : Enums.SquadCommander.Lieutenant;
        }
    }

    [JsonIgnore]
    public virtual SquadPart? SquadPart { get; set; }

    public string? Description { get; set; }
    [JsonIgnore]
    public string? Comment { get; set; }

    public bool DiscordNotificationsEnabled { get; set; } = true;

    [JsonIgnore]
    public byte[]? PasswordHash { get; set; }
    [JsonIgnore]
    public virtual IList<Award> Awards { get; set; } = new List<Award>();
    [JsonIgnore]
    public virtual IList<Award> GivenAwards { get; set; } = new List<Award>();
    [JsonIgnore]
    public virtual IList<Task> Tasks { get; set; } = new List<Task>();
    [JsonIgnore]
    public virtual IList<Task> CreatedTasks { get; set; } = new List<Task>();
    [JsonIgnore]
    public virtual IList<Task> WatchingTasks { get; set; } = new List<Task>();
    [JsonIgnore]
    public virtual IList<Vote> Votes { get; set; } = new List<Vote>();
    [JsonIgnore]
    public virtual IList<Repo> Repos { get; set; } = new List<Repo>();
    [JsonIgnore]
    public virtual IList<RepoNode> RepoNodes { get; set; } = new List<RepoNode>();
    [JsonIgnore]
    public virtual IList<StaticFile> StaticFiles { get; set; } = new List<StaticFile>();
    [JsonIgnore]
    public virtual IList<AccessRule> AccessRulesOverrides { get; set; } = new List<AccessRule>();
    [JsonIgnore]
    public virtual IList<Squad> PendingSquadMembership { get; set; } = new List<Squad>();

    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(this, obj)) return 0;
        if (obj is null) return 1;
        if (obj is not Member member) throw new ArgumentException();
        return CompareTo(member);
    }

    public int CompareTo(Member? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        int rankComparison = Rank.CompareTo(other.Rank);
        if (rankComparison != 0) return rankComparison;
        int directionComparison = Direction.CompareTo(other.Direction);
        if (directionComparison != 0) return directionComparison;
        return string.Compare(Nickname, other.Nickname, StringComparison.CurrentCultureIgnoreCase);
    }

    public virtual bool Equals(Member? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public int Id { get; set; }
    public Guid ConcurrencyToken { get; set; }

    public override string ToString()
    {
        string? rankIcon = Rank.GetRankIcon();
        string commanderIcon = CommanderLevel.GetSquadCommanderIcon();
        string result = string.Empty;
        if (rankIcon is not null) result += Rank > Rank.None ? $"[{commanderIcon}{rankIcon}] " : $"{{{rankIcon}}} ";
        result += Nickname;
        if (RealName is not null) result += $" ({RealName})";
        return result;
    }

    public override int GetHashCode()
    {
        return Id;
    }

    public string GetDiscordMention()
    {
        return DiscordId.HasValue ? $"<@{DiscordId.Value:D}>" : Nickname;
    }

    public string? GetSteamLink()
    {
        return SteamId.HasValue ? $"http://steamcommunity.com/profiles/{SteamId:D}" : null;
    }

    public string? GetVkLink()
    {
        return VkId.HasValue ? $"http://vk.com/id{VkId:D}" : null;
    }
}
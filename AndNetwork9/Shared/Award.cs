using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using AndNetwork9.Shared.Converters;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Interfaces;

namespace AndNetwork9.Shared;

public record Award : IComparable<Award>, IComparable, IId
{
    public int MemberId { get; set; }
    [JsonIgnore]
    public virtual Member? Member { get; set; } = null;

    public AwardType Type { get; set; }
    [JsonIgnore]
    public double Points =>
        Math.Round(
            (int)Type * Math.Pow(2, (Date.DayNumber - DateOnly.FromDateTime(DateTime.Today).DayNumber) / 365.25),
            10,
            MidpointRounding.ToPositiveInfinity);

    [JsonConverter(typeof(DateOnlyConverter))]
    public DateOnly Date { get; set; }
    public int? AutomationTag { get; set; }
    public int? GaveById { get; set; }
    [JsonIgnore]
    public virtual Member? GaveBy { get; set; }

    public string? Description { get; set; }

    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        if (ReferenceEquals(this, obj)) return 0;
        return obj is Award other
            ? CompareTo(other)
            : throw new ArgumentException($"Object must be of type {nameof(Award)}");
    }

    public int CompareTo(Award? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        int typeComparison = Type.CompareTo(other.Type);
        if (typeComparison != 0) return typeComparison;
        int dateComparison = Date.CompareTo(other.Date);
        if (dateComparison != 0) return dateComparison;
        int memberComparison = Comparer<Member?>.Default.Compare(Member, other.Member);
        if (memberComparison != 0) return memberComparison;
        return Comparer<Member?>.Default.Compare(GaveBy, other.GaveBy);
    }

    public int Id { get; set; }

    public Guid ConcurrencyToken { get; set; }
}
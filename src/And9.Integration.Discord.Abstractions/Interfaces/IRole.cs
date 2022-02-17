using System.Drawing;
using And9.Integration.Discord.Abstractions.Enums;
using And9.Lib.Models.Abstractions.Interfaces;
using And9.Service.Core.Abstractions.Enums;

namespace And9.Integration.Discord.Abstractions.Interfaces;

public interface IRole : IDiscordId, IConcurrencyToken, IComparable<IRole>, IComparable
{
    string Name { get; }
    Color? Color { get; }
    DiscordPermissions GlobalPermissions { get; }
    bool IsHoisted { get; }
    bool IsMentionable { get; }

    DiscordRoleScope Scope { get; }
    Direction? Direction { get; }
    short? SquadNumber { get; }
    short? SquadPartNumber { get; }

    int IComparable<IRole>.CompareTo(IRole? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        int scopeComparison = Scope.CompareTo(other.Scope);
        if (scopeComparison != 0) return scopeComparison;
        int directionComparison = Nullable.Compare(Direction, other.Direction);
        if (directionComparison != 0) return directionComparison;
        int squadNumberComparison = Nullable.Compare(SquadNumber, other.SquadNumber);
        if (squadNumberComparison != 0) return squadNumberComparison;
        return Nullable.Compare(SquadPartNumber, other.SquadPartNumber);
    }

    int IComparable.CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        if (ReferenceEquals(this, obj)) return 0;
        return obj is IRole other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(IRole)}");
    }
}
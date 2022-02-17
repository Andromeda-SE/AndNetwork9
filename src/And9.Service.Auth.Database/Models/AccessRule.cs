using And9.Service.Auth.Abstractions.Interfaces;
using And9.Service.Core.Abstractions.Enums;

namespace And9.Service.Auth.Database.Models;

public record class AccessRule : IAccessRule
{
    public int Id { get; set; }
    public Rank MinRank { get; set; }
    public Direction[] Directions { get; set; } = Enum.GetValues<Direction>().Where(x => x > Direction.None).ToArray();
    public short? SquadNumber { get; set; }
    public short? SquadPartNumber { get; set; }
    public List<int> AllowedMembersIds { get; set; } = new();
    public DateTime LastChanged { get; set; }
    public Guid ConcurrencyToken { get; set; }
}
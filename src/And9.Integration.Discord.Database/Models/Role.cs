using System.Drawing;
using And9.Integration.Discord.Abstractions.Enums;
using And9.Integration.Discord.Abstractions.Interfaces;
using And9.Service.Core.Abstractions.Enums;

namespace And9.Integration.Discord.Database.Models;

public record class Role : IRole
{
    public Direction? Direction { get; set; }
    public ulong DiscordId { get; set; }

    public string Name { get; set; } = string.Empty;
    public Color? Color { get; set; }
    public DiscordPermissions GlobalPermissions { get; set; }
    public bool IsHoisted { get; set; }
    public bool IsMentionable { get; set; }
    public DiscordRoleScope Scope { get; set; }
    public short? SquadNumber { get; set; }
    public short? SquadPartNumber { get; set; }
    public DateTime LastChanged { get; set; }
    public Guid ConcurrencyToken { get; set; }
}
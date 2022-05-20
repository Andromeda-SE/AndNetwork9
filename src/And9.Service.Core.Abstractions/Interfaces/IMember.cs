using And9.Service.Core.Abstractions.Models;

namespace And9.Service.Core.Abstractions.Interfaces;

public interface IMember : IPublicMember
{
    ulong? SteamId { get; }

    long? MicrosoftId { get; }
    long? VkId { get; }
    long? TelegramId { get; }
    IList<MemberSpecialization> Specializations { get; }
    TimeZoneInfo? TimeZone { get; set; }
    DateOnly JoinDate { get; }
}
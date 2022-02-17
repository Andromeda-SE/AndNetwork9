namespace And9.Service.Core.Abstractions.Interfaces;

public interface IMember : IPublicMember
{
    ulong? SteamId { get; }

    long? MicrosoftId { get; }
    long? VkId { get; }
    long? TelegramId { get; }
    TimeZoneInfo? TimeZone { get; set; }
    DateOnly JoinDate { get; }

    DateOnly LastDirectionChange { get; }
}
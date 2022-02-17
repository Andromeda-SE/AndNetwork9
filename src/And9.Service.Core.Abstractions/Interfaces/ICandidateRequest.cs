namespace And9.Service.Core.Abstractions.Interfaces;

public interface ICandidateRequest : ICandidateRequestBase
{
    string Nickname { get; }
    string? RealName { get; }
    ulong SteamId { get; }

    ulong DiscordId { get; }
    long? VkId { get; }

    TimeZoneInfo? TimeZone { get; }

    string GetDiscordMention() => $"<@{DiscordId:D}>";

    string GetSteamLink() => $"http://steamcommunity.com/profiles/{SteamId:D}";

    string? GetVkLink() => VkId.HasValue ? $"http://vk.com/id{VkId:D}" : null;
}
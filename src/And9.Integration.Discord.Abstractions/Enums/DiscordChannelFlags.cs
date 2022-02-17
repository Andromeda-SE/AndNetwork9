namespace And9.Integration.Discord.Abstractions.Enums;

[Flags]
public enum DiscordChannelFlags
{
    None = 0,
    CandidatesChannel = 0x1,
    PublicAnnouncement = 0x2,
    ClanAnnouncement = 0x4,
    GameAnnouncement = 0x8,
    AutoAnnouncement = 0x10,
}
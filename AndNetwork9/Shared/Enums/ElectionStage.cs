namespace AndNetwork9.Shared.Enums;

public enum ElectionStage
{
    None,
    Registration,
    Voting,
    Announcement,
    Ended = int.MaxValue,
}
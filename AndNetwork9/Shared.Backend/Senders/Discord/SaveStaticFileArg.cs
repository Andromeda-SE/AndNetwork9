namespace AndNetwork9.Shared.Backend.Senders.Discord;

public record SaveStaticFileArg(string? Name, byte[] FileData, int? OwnerId, int? AccessRuleId);
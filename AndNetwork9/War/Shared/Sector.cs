namespace AndNetwork9.War.Shared;

public record Sector : IId
{
public short OwnerId { get; set; }
public virtual Squad? Owner { get; set; }
public ulong SaveZoneEntityId { get; set; }
public DateTime LastChanged { get; set; }
public Guid ConcurrencyToken { get; set; }
public int Id { get; }
}
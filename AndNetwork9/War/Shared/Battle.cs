namespace AndNetwork9.War.Shared;

public record Battle : IId
{
public int SectorId { get; set; }
[JsonIgnore]
public virtual Sector Sector { get; set; }
public BattleStatus Status { get; set; }
public short AttackersId { get; set; }
[JsonIgnore]
public virtual Squad Attackers { get; set; }
public short DefendersId { get; set; }
[JsonIgnore]
public virtual Squad Defenders { get; set; }
public float DefendersPoints { get; set; }
public double DefendersMultiplier { get; set; }
public float AttackersPoints { get; set; }
public double AttackersMultiplier { get; set; }
public DateTime LastChanged { get; set; }
public Guid ConcurrencyToken { get; set; }
public int Id { get; }
}
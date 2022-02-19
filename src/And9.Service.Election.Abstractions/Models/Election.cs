using And9.Service.Core.Abstractions.Enums;
using And9.Service.Election.Abstractions.Enums;
using And9.Service.Election.Abstractions.Interfaces;
using MessagePack;

namespace And9.Service.Election.Abstractions.Models;

[MessagePackObject]
public class Election : IElection
{
    [Key(0)]
    public short ElectionId { get; set; }
    [Key(1)]
    public Direction Direction { get; set; }
    [Key(2)]
    public DateOnly AdvisorsStartDate { get; set; }
    [Key(3)]
    public ElectionStatus Status { get; set; }
    [Key(4)]
    public int AgainstAllVotes { get; set; }
    [Key(5)]
    public DateTime LastChanged { get; set; }
    [Key(6)]
    public Guid ConcurrencyToken { get; set; }
}
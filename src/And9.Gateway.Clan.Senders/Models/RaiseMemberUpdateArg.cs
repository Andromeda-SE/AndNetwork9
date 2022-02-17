using MessagePack;

namespace And9.Gateway.Clan.Senders.Models;

[MessagePackObject]
public record class RaiseMemberUpdateArg
{
    [Key(0)]
    public int MemberId { get; set; }
    [Key(1)]
    public double Points { get; set; }
}
namespace And9.Service.Award.Senders.Interfaces;

public interface IAwardModelServiceMethods
{
    IAsyncEnumerable<Abstractions.Models.Award> ReadByMemberId(int memberId, CancellationToken cancellationToken);
}
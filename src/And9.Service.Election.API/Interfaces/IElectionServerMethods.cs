using And9.Service.Core.Abstractions.Enums;

namespace And9.Service.Election.API.Interfaces;

public interface IElectionServerMethods
{
    Task<bool> Register();
    Task<bool> CancelRegister();
    Task<bool> Vote(IReadOnlyDictionary<Direction, IReadOnlyDictionary<int?, int>> votes);
    IAsyncEnumerable<Abstractions.Models.Election> GetElection(CancellationToken token);
}
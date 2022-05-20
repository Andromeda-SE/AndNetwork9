using And9.Gateway.Clan.Auth.Attributes;
using And9.Lib.Models.Abstractions;
using And9.Service.Core.Abstractions.Interfaces;
using And9.Service.Core.Senders.Squad;
using Microsoft.AspNetCore.SignalR;

namespace And9.Gateway.Clan.Hubs.Model;

public class SquadHub : Hub<IModelCrudClientMethods>, IModelReadOnlyCrudServerMethods<ISquad>
{
    private readonly ReadAllSquadSender _readAllSquadSender;
    private readonly ReadSquadSender _readSquadSender;

    public SquadHub(ReadAllSquadSender readAllSquadSender, ReadSquadSender readSquadSender)
    {
        _readAllSquadSender = readAllSquadSender;
        _readSquadSender = readSquadSender;
    }

    [MinRankAuthorize]
    public async Task<ISquad?> Read(int id) => await _readSquadSender.CallAsync(id).ConfigureAwait(false);

    [MinRankAuthorize]
    public IAsyncEnumerable<ISquad> ReadAll(CancellationToken cancellationToken) => _readAllSquadSender.CallAsync(0, cancellationToken);
}
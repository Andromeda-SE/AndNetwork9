using System.Runtime.CompilerServices;
using And9.Gateway.Clan.Senders;
using And9.Gateway.Clan.Senders.Models;
using And9.Integration.Discord.Senders;
using And9.Lib.Models.Abstractions;
using And9.Service.Award.Abstractions.Models;
using And9.Service.Award.Senders;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Senders;
using Microsoft.AspNetCore.SignalR;

namespace And9.Gateway.Clan.Hubs.Model;

public class AwardHub : Hub<IModelCrudClientMethods>
{
    private readonly AwardCrudSender _awardCrudSender;
    private readonly SyncUserSender _syncUserSender;
    private readonly MemberCrudSender _memberCrudSender;

    public AwardHub(AwardCrudSender awardCrudSender, SyncUserSender syncUserSender, MemberCrudSender memberCrudSender)
    {
        _awardCrudSender = awardCrudSender;
        _syncUserSender = syncUserSender;
        _memberCrudSender = memberCrudSender;
    }

    public async Task Create(Award model)
    {
        int id = await _awardCrudSender.Create(new()
        {
            AutomationTag = null,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            GaveById = int.Parse(Context.UserIdentifier),
            MemberId = model.MemberId,
            Type = model.Type,
            Description = model.Description,
        }).ConfigureAwait(false);
        await Clients.All.ModelUpdated(id, ModelState.Created).ConfigureAwait(false);
        await _syncUserSender.CallAsync(await _memberCrudSender.Read(model.MemberId).ConfigureAwait(false)).ConfigureAwait(false);
    }

    public Task Delete(int id) => throw new NotSupportedException();

    public Task Update(Award model) => throw new NotSupportedException();

    public async Task<Award?> Read(int id) => await _awardCrudSender.Read(id).ConfigureAwait(false);

    public async IAsyncEnumerable<Award> ReadAll([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (Award award in _awardCrudSender.ReadAll(cancellationToken).WithCancellation(cancellationToken).ConfigureAwait(false)) yield return award;
    }
}
using System.Runtime.CompilerServices;
using And9.Gateway.Clan.Auth.Attributes;
using And9.Lib.Models.Abstractions;
using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Senders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace And9.Gateway.Clan.Hubs.Model;

public class MemberHub : Hub<IModelCrudClientMethods>, IModelCrudServerMethods<Member>
{
    private readonly MemberCrudSender _memberCrudSender;
    public MemberHub(MemberCrudSender memberCrudSender) => _memberCrudSender = memberCrudSender;

    [MinRankAuthorize(Rank.FirstAdvisor)]
    public async Task Create(Member model)
    {
        int id = await _memberCrudSender.Create(model).ConfigureAwait(false);
        await Clients.All.ModelUpdated(id, ModelState.Created).ConfigureAwait(false);
    }

    [MinRankAuthorize(Rank.FirstAdvisor)]
    public async Task Delete(int id)
    {
        await _memberCrudSender.Delete(id).ConfigureAwait(false);
        await Clients.All.ModelUpdated(id, ModelState.Deleted).ConfigureAwait(false);
    }

    [MinRankAuthorize(Rank.FirstAdvisor)]
    public async Task Update(Member model)
    {
        await _memberCrudSender.Update(model).ConfigureAwait(false);
        await Clients.All.ModelUpdated(model.Id, ModelState.Updated).ConfigureAwait(false);
    }

    [MinRankAuthorize]
    public async Task<Member?> Read(int id) => await _memberCrudSender.Read(id).ConfigureAwait(false);

    [MinRankAuthorize]
    public async IAsyncEnumerable<Member> ReadAll([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (Member member in _memberCrudSender.ReadAll(cancellationToken).WithCancellation(cancellationToken).ConfigureAwait(false)) yield return member;
    }

    [Authorize]
    public async Task<Member?> ReadMe() => await Read(int.Parse(Context.UserIdentifier)).ConfigureAwait(false);
}
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
    private readonly CreateMemberSender _createMemberSender;
    private readonly UpdateMemberSender _updateMemberSender;
    private readonly ReadMemberByIdSender _readMemberByIdSender;
    private readonly ReadAllMembersSender _readAllMembersSender;
    public MemberHub(
        CreateMemberSender createMemberSender,
        UpdateMemberSender updateMemberSender,
        ReadMemberByIdSender readMemberByIdSender,
        ReadAllMembersSender readAllMembersSender)
    {
        _createMemberSender = createMemberSender;
        _updateMemberSender = updateMemberSender;
        _readMemberByIdSender = readMemberByIdSender;
        _readAllMembersSender = readAllMembersSender;
    }

    [MinRankAuthorize(Rank.FirstAdvisor)]
    public async Task Create(Member model)
    {
        int id = await _createMemberSender.CallAsync(model).ConfigureAwait(false);
        await Clients.All.ModelUpdated(id, ModelState.Created).ConfigureAwait(false);
    }

    [MinRankAuthorize(Rank.FirstAdvisor)]
    public Task Delete(int id)
    {
        throw new NotSupportedException();
    }

    [MinRankAuthorize(Rank.FirstAdvisor)]
    public async Task Update(Member model)
    {
        await _updateMemberSender.CallAsync(model).ConfigureAwait(false);
        await Clients.All.ModelUpdated(model.Id, ModelState.Updated).ConfigureAwait(false);
    }

    [MinRankAuthorize]
    public async Task<Member?> Read(int id) => await _readMemberByIdSender.CallAsync(id).ConfigureAwait(false);

    [MinRankAuthorize]
    public async IAsyncEnumerable<Member> ReadAll([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (Member member in _readAllMembersSender.CallAsync(0).WithCancellation(cancellationToken).ConfigureAwait(false)) yield return member;
    }

    [Authorize]
    public async Task<Member?> ReadMe() => await Read(int.Parse(Context.UserIdentifier)).ConfigureAwait(false);
}
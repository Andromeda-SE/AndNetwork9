using System.Runtime.CompilerServices;
using And9.Integration.Discord.Senders;
using And9.Lib.Models.Abstractions;
using And9.Service.Award.Abstractions.Models;
using And9.Service.Award.Senders;
using And9.Service.Core.Senders;
using Microsoft.AspNetCore.SignalR;

namespace And9.Gateway.Clan.Hubs.Model;

public class AwardHub : Hub<IModelCrudClientMethods>
{
    private readonly SyncUserSender _syncUserSender;
    private readonly CreateAwardSender _createAwardSender;
    private readonly ReadAllAwardsSender _readAllAwardsSender;
    private readonly ReadAwardSender _readAwardSender;
    private readonly ReadMemberByIdSender _readMemberByIdSender;

    public AwardHub(SyncUserSender syncUserSender, CreateAwardSender createAwardSender, ReadAllAwardsSender readAllAwardsSender, ReadAwardSender readAwardSender, ReadMemberByIdSender readMemberByIdSender)
    {
        _syncUserSender = syncUserSender;
        _createAwardSender = createAwardSender;
        _readAllAwardsSender = readAllAwardsSender;
        _readAwardSender = readAwardSender;
        _readMemberByIdSender = readMemberByIdSender;
    }

    public async Task Create(Award model)
    {
        int id = await _createAwardSender.CallAsync(new()
        {
            AutomationTag = null,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            GaveById = int.Parse(Context.UserIdentifier),
            MemberId = model.MemberId,
            Type = model.Type,
            Description = model.Description,
        }).ConfigureAwait(false);
        await Clients.All.ModelUpdated(id, ModelState.Created).ConfigureAwait(false);
        await _syncUserSender.CallAsync(await _readMemberByIdSender.CallAsync(model.MemberId).ConfigureAwait(false)).ConfigureAwait(false);
    }

    public Task Delete(int id) => throw new NotSupportedException();

    public Task Update(Award model) => throw new NotSupportedException();

    public async Task<Award?> Read(int id) => await _readAwardSender.CallAsync(id).ConfigureAwait(false);

    public async IAsyncEnumerable<Award> ReadAll([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (Award award in _readAllAwardsSender.CallAsync(0, cancellationToken).ConfigureAwait(false)) yield return award;
    }
}
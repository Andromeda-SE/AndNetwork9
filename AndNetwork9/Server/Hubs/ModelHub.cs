using AndNetwork9.Server.Auth.Attributes;
using AndNetwork9.Server.Extensions;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Hubs;
using Microsoft.AspNetCore.SignalR;
using Task = System.Threading.Tasks.Task;

namespace AndNetwork9.Server.Hubs;

[MinRankAuthorize]
public class ModelHub : Hub<IModelHub>
{
    private readonly ClanDataContext _dataContext;

    public ModelHub(ClanDataContext dataContext) => _dataContext = dataContext;

    public override async Task OnConnectedAsync()
    {
        Member? member = await Context.User.GetCurrentMember(_dataContext);
        if (member is null) Context.Abort();
        else await base.OnConnectedAsync().ConfigureAwait(false);
    }
}
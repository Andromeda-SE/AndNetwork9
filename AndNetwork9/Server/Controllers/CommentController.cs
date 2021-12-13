using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AndNetwork9.Server.Extensions;
using AndNetwork9.Server.Hubs;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Hubs;
using AndNetwork9.Shared.Storage;
using AndNetwork9.Shared.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Task = AndNetwork9.Shared.Task;

namespace AndNetwork9.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CommentController : ControllerBase
{
    private readonly ClanDataContext _data;
    private readonly IHubContext<ModelHub, IModelHub> _modelHub;

    public CommentController(ClanDataContext data, IHubContext<ModelHub, IModelHub> modelHub)
    {
        _data = data;
        _modelHub = modelHub;
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<ActionResult<Comment>> Put(int id, Comment comment)
    {
        if (id != comment.Id) return BadRequest();
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        Comment? oldComment = await _data.Comments.FindAsync(id).ConfigureAwait(false);
        if (oldComment is null) return NotFound();
        if (oldComment.AuthorId != member.Id) return Forbid();

        oldComment.LastEditTime = DateTime.UtcNow;
        oldComment.Text = comment.Text;

        await _data.SaveChangesAsync().ConfigureAwait(false);
        if (oldComment.Task is not null)
            await _modelHub.Clients
                .Users(await _data.Members.AsAsyncEnumerable()
                    .Where(x => oldComment.Task.ReadRule!.HasAccess(x))
                    .Select(x => x.Id.ToString("D", CultureInfo.InvariantCulture))
                    .ToArrayAsync().ConfigureAwait(false))
                .ReceiveModelUpdate(typeof(Task).FullName, oldComment.Task).ConfigureAwait(false);
        if (oldComment.Repo is not null)
            await _modelHub.Clients
                .Users(await _data.Members.AsAsyncEnumerable()
                    .Where(x => oldComment.Repo.ReadRule!.HasAccess(x))
                    .Select(x => x.Id.ToString("D", CultureInfo.InvariantCulture))
                    .ToArrayAsync().ConfigureAwait(false))
                .ReceiveModelUpdate(typeof(Repo).FullName, oldComment.Repo).ConfigureAwait(false);
        return Ok(oldComment);
    }
}
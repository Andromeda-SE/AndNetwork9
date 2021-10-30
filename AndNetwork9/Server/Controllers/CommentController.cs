using System;
using System.Threading.Tasks;
using AndNetwork9.Server.Extensions;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AndNetwork9.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CommentController : ControllerBase
{
    private readonly ClanDataContext _data;

    public CommentController(ClanDataContext data)
    {
        _data = data;
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
        oldComment.Files = comment.Files;

        await _data.SaveChangesAsync().ConfigureAwait(false);
        return Ok(oldComment);
    }
}
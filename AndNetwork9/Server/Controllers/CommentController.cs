using System;
using System.Threading.Tasks;
using AndNetwork9.Server.Extensions;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AndNetwork9.Server.Controllers
{
    public class CommentController : ControllerBase
    {
        private readonly ClanDataContext _data;

        public CommentController(ClanDataContext data)
        {
            _data = data;
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<Comment>> Put(int id, Comment comment)
        {
            if (id != comment.Id) return BadRequest();
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return Unauthorized();

            Comment? oldComment = await _data.Comments.FindAsync(id);
            if (oldComment is null) return NotFound();
            if (oldComment.AuthorId != member.Id) return Forbid();

            Comment resultComment = oldComment with
            {
                LastEditTime = DateTime.UtcNow,
                Text = comment.Text,
                Files = comment.Files,
            };
            _data.Comments.Update(resultComment);
            await _data.SaveChangesAsync();
            return Ok(resultComment);
        }
    }
}
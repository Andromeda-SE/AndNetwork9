using System;
using System.Linq;
using System.Threading.Tasks;
using AndNetwork9.Server.Auth.Attributes;
using AndNetwork9.Server.Extensions;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Utility;
using AndNetwork9.Shared.Votings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AndNetwork9.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VotingController : ControllerBase
{
    private readonly ClanDataContext _data;

    public VotingController(ClanDataContext data) => _data = data;

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<Voting>> Get()
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        return Ok(_data.Votings.Where(x =>
            x.Votes.Any(y => y.MemberId == member.Id && y.Result == MemberVote.NoVote)));
    }

    [HttpGet("all")]
    [Authorize]
    public async Task<ActionResult<Voting>> GetAll()
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        return Ok(_data.Votings.Where(x => x.HasReadAccess(member)));
    }

    [HttpGet("active")]
    [Authorize]
    public async Task<ActionResult<Voting>> GetActive()
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        return Ok(_data.Votings.Where(x =>
            x.HasReadAccess(member) && x.Votes.Any(y => y.Result == MemberVote.NoVote)));
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<Voting>> Get(int id)
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        Voting? voting = await _data.Votings.FindAsync(id).ConfigureAwait(false);
        if (voting is null) return NotFound();
        if (!voting.HasReadAccess(member)) return Forbid();

        return Ok(voting);
    }

    [HttpPost]
    [MinRankAuthorize]
    public async Task<ActionResult<Voting>> Post(Voting voting)
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        EntityEntry<Voting> result = await _data.Votings.AddAsync(voting with
        {
            Id = 0,
            ReporterId = member.Id,
            Reporter = member,
            CreateTime = DateTime.UtcNow,
            Votes = voting.Votes.Select(x => x with
            {
                Result = MemberVote.NoVote,
                Voting = voting,
                VotingId = voting.Id,
                VoteTime = null,
            }).ToArray(),
            Result = MemberVote.NoVote,
            Comments = Array.Empty<Comment>(),
        }).ConfigureAwait(false);
        await _data.SaveChangesAsync().ConfigureAwait(false);
        return result.Entity;
    }

    [HttpPost("{votingId:int}/comment")]
    [MinRankAuthorize]
    public async Task<ActionResult<Comment>> PostComment(int votingId, Comment comment)
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        Voting? voting = await _data.Votings.FindAsync(votingId).ConfigureAwait(false);
        if (voting is null) return NotFound();
        if (!voting.HasReadAccess(member)) return Forbid();

        if (comment.ParentId is not null)
        {
            int id = comment.ParentId.Value;
            comment.Parent = null;
            foreach (Comment votingComment in voting.Comments)
            {
                comment.Parent = votingComment.FindComment(id);
                if (comment.Parent is not null) break;
            }

            if (comment.Parent is null) return BadRequest();
        }

        Comment result = comment with
        {
            Id = 0,
            Author = member,
            AuthorId = member.Id,
            CreateTime = DateTime.UtcNow,
            LastEditTime = null,
            Children = Array.Empty<Comment>(),
        };
        voting.Comments.Add(result);

        await _data.SaveChangesAsync().ConfigureAwait(false);
        return Ok(result);
    }

    [HttpPatch("{id:int}")]
    [Authorize]
    public async Task<ActionResult<Vote>> Patch(int id, MemberVote memberVote)
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        Voting? voting = await _data.Votings.FindAsync(id).ConfigureAwait(false);
        if (voting is null) return NotFound();
        if (voting.EndTime is not null && voting.EndTime < DateTime.UtcNow) return Forbid();

        Vote? vote = voting.Votes.FirstOrDefault(x => x.MemberId == member.Id);
        if (vote is null) return Forbid();

        if (memberVote <= MemberVote.NoVote) return UnprocessableEntity();
        if (!voting.EditVoteEnabled && vote.Result > MemberVote.NoVote) return Forbid();

        vote.Result = memberVote;
        vote.VoteTime = DateTime.UtcNow;
        await _data.SaveChangesAsync().ConfigureAwait(false);
        return Ok(vote);
    }

    [HttpPut("{id:int}")]
    [MinRankAuthorize]
    public async Task<ActionResult<Voting>> Put(int id, Voting newVoting)
    {
        if (id != newVoting.Id) return BadRequest();
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        Voting? oldVoting = await _data.Votings.FindAsync(id).ConfigureAwait(false);
        if (oldVoting is null) return NotFound();
        if (!oldVoting.HasWriteAccess(member)) return Forbid();

        Voting resultVoting = oldVoting with
        {
            Title = newVoting.Title,
            EndTime = newVoting.EndTime,
            LastEditTime = DateTime.UtcNow,
            Result = newVoting.Result,
            ReadRuleId = newVoting.ReadRuleId,
            EditRuleId = newVoting.EditRuleId,
            Description = newVoting.Description,
        };
        _data.Votings.Update(resultVoting);
        await _data.SaveChangesAsync().ConfigureAwait(false);
        return resultVoting;
    }
}
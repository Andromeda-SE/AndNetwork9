using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AndNetwork9.Server.Extensions;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Senders.Storage;
using AndNetwork9.Shared.Extensions;
using AndNetwork9.Shared.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace AndNetwork9.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RepoController : ControllerBase
{
    private readonly ClanDataContext _data;
    private readonly NewRepoSender _newRepoSender;
    private readonly RepoGetFileSender _repoGetFileSender;
    private readonly RepoSetFileSender _repoSetFileSender;

    public RepoController(ClanDataContext data, RepoGetFileSender repoGetFileSender, NewRepoSender newRepoSender,
        RepoSetFileSender repoSetFileSender)
    {
        _data = data;
        _repoGetFileSender = repoGetFileSender;
        _newRepoSender = newRepoSender;
        _repoSetFileSender = repoSetFileSender;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Repo>>> Get()
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        return Ok(_data.Repos.ToArray().Where(x => x.CreatorId == member.Id || x.ReadRule.HasAccess(member)));
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Repo>> Post(Repo repo)
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        if (await _data.Repos.AnyAsync(x => x.Name == repo.Name).ConfigureAwait(false)) return Conflict();
        repo.Description ??= new();
        Repo? result = await _newRepoSender.CallAsync(repo with
        {
            CreatorId = member.Id,
            Creator = member,
            Description = repo.Description with
            {
                Id = 0,
                Author = member,
                Parent = null,
                CreateTime = DateTime.UtcNow,
                LastEditTime = null,
            },
            RepoName = string.Empty,
            Nodes = Array.Empty<RepoNode>(),
            CommentId = 0,
        }).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<ActionResult<Repo>> Put(int id, Repo newRepo)
    {
        if (id != newRepo.Id) return BadRequest();
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        Repo? oldRepo = await _data.Repos.FindAsync(id).ConfigureAwait(false);
        if (oldRepo is null) return NotFound();
        if (oldRepo.CreatorId != member.Id) return Forbid();

        oldRepo.ReadRuleId = newRepo.ReadRuleId;
        oldRepo.WriteRuleId = newRepo.WriteRuleId;
        oldRepo.Name = newRepo.Name;

        await _data.SaveChangesAsync().ConfigureAwait(false);
        return Ok(oldRepo);
    }


    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<Repo>> Get(int id)
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        Repo? result = await _data.Repos.FindAsync(id).ConfigureAwait(false);
        if (result is null) return NotFound();
        if (result.CreatorId != member.Id && !result.ReadRule.HasAccess(member)) return Forbid();
        return Ok(result);
    }

    [HttpGet("{id:int}/nodes")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<RepoNode>>> GetNodes(int id)
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        Repo? result = await _data.Repos.FindAsync(id).ConfigureAwait(false);
        if (result is null) return NotFound();
        if (result.CreatorId != member.Id && !result.ReadRule.HasAccess(member)) return Forbid();
        return Ok(result.Nodes);
    }

    [HttpGet("{id:int}/node/{version:int}/{modification:int}/{prototype:int}")]
    [Authorize]
    public async Task<ActionResult<RepoNode>> GetVersion(int id, int version, int modification, int prototype)
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        Repo? result = await _data.Repos.FindAsync(id).ConfigureAwait(false);
        if (result is null) return NotFound();
        if (!result.ReadRule.HasAccess(member)) return Forbid();
        RepoNode? node = await _data.RepoNodes.FindAsync(id, version, modification, prototype)
            .ConfigureAwait(false);
        if (node is null) return NotFound();
        return Ok(node);
    }

    [HttpPut("{id:int}/node/")]
    [Authorize]
    public async Task<ActionResult<RepoNode>> PutNode(RepoNodeWithData data)
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();
        if (member.Id != data.AuthorId) return BadRequest();

        Repo? repo = await _data.Repos.FindAsync(data.RepoId).ConfigureAwait(false);
        if (repo is null) return NotFound();
        if (repo.CreatorId != member.Id && !repo.WriteRule.HasAccess(member)) return Forbid();
        if (data.Data.Length > 157286400) return StatusCode(413);
        RepoNode? node =
            await _data.RepoNodes.FindAsync(data.RepoId, data.Version, data.Modification, data.Prototype)
                .ConfigureAwait(false);
        if (node is not null) return Conflict();
        RepoNodeWithData resultNode = data with
        {
            CreateTime = DateTime.UtcNow,
            Author = member,
            Repo = repo,
        };

        await _repoSetFileSender.CallAsync(resultNode).ConfigureAwait(false);
        return Ok(resultNode);
    }

    [HttpGet("{id:int}/node/{version:int}/{modification:int}/{prototype:int}/file")]
    [Authorize]
    public async Task<ActionResult> GetFile(int id, int version, int modification, int prototype)
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        Repo? repo = await _data.Repos.FindAsync(id).ConfigureAwait(false);
        if (repo is null) return NotFound();
        if (repo.CreatorId != member.Id && !repo.ReadRule.HasAccess(member)) return Forbid();
        RepoNode? node = await _data.RepoNodes.FindAsync(id, version, modification, prototype)
            .ConfigureAwait(false);
        if (node is null) return NotFound();

        byte[]? result = await _repoGetFileSender.CallAsync(node).ConfigureAwait(false);
        if (result is null) return StatusCode((int)HttpStatusCode.FailedDependency);
        return File(result,
            repo.Type.GetContentType(),
            node.GetFileName(),
            node.CreateTime,
            EntityTagHeaderValue.Any);
    }
}
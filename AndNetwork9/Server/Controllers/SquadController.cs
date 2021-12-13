using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AndNetwork9.Server.Auth.Attributes;
using AndNetwork9.Server.Extensions;
using AndNetwork9.Server.Hubs;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Senders.Discord;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AndNetwork9.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SquadController : ControllerBase
{
    private readonly ClanDataContext _data;
    private readonly IHubContext<ModelHub, IModelHub> _modelHub;
    private readonly PublishSender _publishSender;

    public SquadController(ClanDataContext data, PublishSender publishSender, IHubContext<ModelHub, IModelHub> modelHub)
    {
        _data = data;
        _publishSender = publishSender;
        _modelHub = modelHub;
    }

    [HttpGet("all")]
    [MinRankAuthorize]
    [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 30, NoStore = false)]
    public ActionResult<IAsyncEnumerable<Squad>> GetAll()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        return Ok(_data.Squads.Where(x => x.DisbandDate == null || x.DisbandDate > today).AsAsyncEnumerable());
    }

    [HttpGet]
    [MinRankAuthorize]
    [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 30, NoStore = false)]
    public async Task<ActionResult<Squad>> Get()
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        if (member.SquadPart is null) return NoContent();
        return Ok(member.SquadPart.Squad);
    }

    [HttpGet("{id:int}")]
    [MinRankAuthorize]
    [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 30, NoStore = false)]
    public async Task<ActionResult<Squad>> Get(short id)
    {
        Squad? squad = await _data.Squads.FindAsync(id).ConfigureAwait(false);
        return squad is not null ? Ok(squad) : NotFound();
    }

    [HttpGet("{id:int}/parts")]
    [MinRankAuthorize]
    [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 30, NoStore = false)]
    public async Task<ActionResult<IAsyncEnumerable<SquadPart>>> GetParts(short id)
    {
        Squad? squad = await _data.Squads.FindAsync(id).ConfigureAwait(false);
        return squad is not null ? Ok(squad.SquadParts) : NotFound();
    }

    [HttpGet("{id:int}/part/{part:int}")]
    [MinRankAuthorize]
    [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 30, NoStore = false)]
    public async Task<ActionResult<SquadPart>> GetPart(short id, short part)
    {
        Squad? squad = await _data.Squads.FindAsync(id).ConfigureAwait(false);
        return squad is not null ? Ok(squad.SquadParts!.Single(x => x.Part == part)) : NotFound();
    }

    [HttpGet("{id:int}/comment")]
    [MinRankAuthorize(Rank.Advisor)]
    [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 30, NoStore = false)]
    public async Task<ActionResult> GetComment(short id)
    {
        Squad? squad = await _data.Squads.FindAsync(id).ConfigureAwait(false);
        return squad is not null ? Ok(squad.Comment) : NotFound();
    }

    [HttpGet("{id:int}/members")]
    [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 30, NoStore = false)]
    [MinRankAuthorize]
    public async Task<ActionResult> GetMembers(short id)
    {
        Squad? squad = await _data.Squads.FindAsync(id).ConfigureAwait(false);
        return squad?.SquadParts is not null
            ? Ok(squad.SquadParts.SelectMany(x => x.Members).GetShort())
            : NotFound();
    }

    [HttpGet("{id:int}/candidates")]
    [CommanderAuthorize]
    public async Task<ActionResult> GetCandidates(short id)
    {
        Squad? squad = await _data.Squads.FindAsync(id).ConfigureAwait(false);
        return squad?.Candidates is not null ? Ok(squad.Candidates.GetShort()) : NotFound();
    }

    [HttpPut("{id:int}")]
    [MinRankAuthorize(Rank.Advisor)]
    public async Task<ActionResult<Squad>> Put(short id, Squad squad)
    {
        if (id != squad.Number) return BadRequest();
        Squad? oldSquad = await _data.Squads.FindAsync(id).ConfigureAwait(false);
        if (oldSquad is null) return NotFound();

        Squad newSquad = oldSquad with
        {
            Name = squad.Name,
            Comment = squad.Comment,
            DisbandDate = squad.DisbandDate,
            DiscordRoleId = squad.DiscordRoleId,
            Description = squad.Description,
        };

        _data.Squads.Update(newSquad);
        await _data.SaveChangesAsync().ConfigureAwait(false);
        await _modelHub.Clients.All.ReceiveModelUpdate(typeof(Squad).FullName, newSquad).ConfigureAwait(false);
        return newSquad;
    }

    [HttpPut]
    [CaptainAuthorize]
    public async Task<ActionResult<Squad>> Put(Squad squad)
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();
        Squad? oldSquad = member.SquadPart?.Squad;
        if (oldSquad is null) return NotFound();

        Squad newSquad = oldSquad with
        {
            Description = squad.Description,
        };

        _data.Squads.Update(newSquad);
        await _data.SaveChangesAsync().ConfigureAwait(false);
        await _modelHub.Clients.All.ReceiveModelUpdate(typeof(Squad).FullName, newSquad).ConfigureAwait(false);
        return newSquad;
    }

    [HttpPost]
    [MinRankAuthorize(Rank.JuniorEmployee)]
    public async Task<ActionResult<Squad>> Post(Squad squad)
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        if (member.SquadPart is not null) return Forbid();


        EntityEntry<Squad> addResult = await _data.Squads.AddAsync(squad with
        {
            Number = 0,
            Name = null,
            CreateDate = DateOnly.FromDateTime(DateTime.Today),
        }).ConfigureAwait(false);


        SquadPart leadPart = new()
        {
            Part = 0,
            Number = addResult.Entity.Number,
            Members = new List<Member>(),
            DiscordRoleId = null,
            Squad = addResult.Entity,
        };
        addResult.Entity.SquadParts = new List<SquadPart>();
        addResult.Entity.SquadParts.Add(leadPart);
        leadPart.Members.Add(member);
        leadPart.CommanderId = member.Id;
        leadPart.Commander = member;

        await _data.SaveChangesAsync().ConfigureAwait(false);
        //todo: add discord role creation
        await _publishSender.CallAsync($"Игроком {member.GetDiscordMention()} созван новый, {addResult.Entity}!")
            .ConfigureAwait(false);
        await _modelHub.Clients.All.ReceiveModelUpdate(typeof(Squad).FullName, addResult.Entity).ConfigureAwait(false);
        return addResult.Entity;
    }

    [HttpPatch("{id:int}/join")]
    [Authorize]
    public async Task<ActionResult> PatchJoin(short squadNumber)
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();
        if (member.SquadPart is not null) return Forbid();

        Squad? squad = await _data.Squads.FindAsync(squadNumber).ConfigureAwait(false);
        if (squad is null) return NotFound();

        squad.Candidates!.Add(member);
        await _data.SaveChangesAsync().ConfigureAwait(false);
        await _modelHub.Clients.All.ReceiveModelUpdate(typeof(Squad).FullName, squad).ConfigureAwait(false);
        return Ok();
    }

    [HttpPatch("{squadNumber:int}/cancelJoin")]
    [Authorize]
    public async Task<ActionResult> PatchCancelJoin(short squadNumber)
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();
        if (member.SquadPart is not null) return Forbid();

        Squad? squad = await _data.Squads.FindAsync(squadNumber).ConfigureAwait(false);
        if (squad is null) return NotFound();

        bool result = squad.Candidates!.Remove(member);
        if (result) await _data.SaveChangesAsync().ConfigureAwait(false);
        await _modelHub.Clients.All.ReceiveModelUpdate(typeof(Squad).FullName, squad).ConfigureAwait(false);
        return result ? Ok() : NoContent();
    }


    [HttpPatch("accept/{memberId:int}/{partNumber:int}")]
    [CaptainAuthorize]
    public async Task<ActionResult> PatchAccept(short memberId, short partNumber)
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        Squad? squad = member.SquadPart?.Squad;
        if (squad is null) return NotFound();
        SquadPart? squadPart = squad.SquadParts?.Single(x => x.Part == partNumber);
        if (squadPart is null) return NotFound();

        Member? candidate = squad.Candidates!.FirstOrDefault(x => x.Id == memberId);
        if (candidate is null) return NotFound();

        squad.Candidates!.Remove(candidate);
        squadPart.Members.Add(candidate);
        candidate.PendingSquadMembership.Clear();
        await _data.SaveChangesAsync().ConfigureAwait(false);
        await _publishSender.CallAsync($"{squad} пополняется игроком {candidate.GetDiscordMention()}")
            .ConfigureAwait(false);
        await _modelHub.Clients.All.ReceiveModelUpdate(typeof(Squad).FullName, squad).ConfigureAwait(false);
        return Ok();
    }

    [HttpPatch("decline")]
    [CaptainAuthorize]
    public async Task<ActionResult> PatchDecline(short memberId)
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();
        if (member.SquadPart?.Squad is null) return Forbid();

        Member? candidate = member.SquadPart?.Squad.Candidates!.FirstOrDefault(x => x.Id == memberId);
        if (candidate is null) return NotFound();

        member.SquadPart?.Squad.Candidates!.Remove(candidate);
        await _data.SaveChangesAsync().ConfigureAwait(false);
        await _modelHub.Clients.All.ReceiveModelUpdate(typeof(Squad).FullName, member.SquadPart?.Squad)
            .ConfigureAwait(false);
        return Ok();
    }
}
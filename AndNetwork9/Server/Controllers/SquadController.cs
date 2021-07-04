﻿using System;
using System.Linq;
using System.Threading.Tasks;
using AndNetwork9.Server.Auth.Attributes;
using AndNetwork9.Server.Extensions;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Senders.Discord;
using AndNetwork9.Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AndNetwork9.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SquadController : ControllerBase
    {
        private readonly ClanDataContext _data;
        private readonly PublishSender _publishSender;

        public SquadController(ClanDataContext data, PublishSender publishSender)
        {
            _data = data;
            _publishSender = publishSender;
        }

        [HttpGet("all")]
        [MinRankAuthorize]
        public ActionResult<Squad> GetAll()
        {
            return Ok(_data.Squads);
        }

        [HttpGet("current")]
        [MinRankAuthorize]
        public ActionResult<Squad> GetCurrent()
        {
            return Ok(_data.Squads
                .Where(x => x.DisbandDate == null || x.DisbandDate.Value > DateOnly.FromDateTime(DateTime.Today)));
        }

        [HttpGet]
        [MinRankAuthorize]
        public async Task<ActionResult<Squad>> Get()
        {
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return NotFound();

            return Ok(member.Squad);
        }

        [HttpGet("{id:int}")]
        [MinRankAuthorize]
        public async Task<ActionResult<Squad>> Get(int id)
        {
            Squad? squad = await _data.Squads.FindAsync(id);
            return squad is not null ? Ok(squad) : NotFound();
        }

        [HttpGet("{id:int}/comment")]
        [MinRankAuthorize(Rank.Advisor)]
        public async Task<ActionResult> GetComment(int id)
        {
            Squad? squad = await _data.Squads.FindAsync(id);
            return squad is not null ? Ok(squad.Comment) : NotFound();
        }

        [HttpGet("{id:int}/members")]
        [MinRankAuthorize]
        public async Task<ActionResult> GetMembers(int id)
        {
            Squad? squad = await _data.Squads.FindAsync(id);
            return squad is not null ? Ok(squad.Members.GetShort()) : NotFound();
        }

        [HttpGet("{id:int}/candidates")]
        [SquadCommanderAuthorize]
        public async Task<ActionResult> GetCandidates(int id)
        {
            Squad? squad = await _data.Squads.FindAsync(id);
            return squad is not null ? Ok(squad.Candidates.GetShort()) : NotFound();
        }

        [HttpPut("{id:int}")]
        [MinRankAuthorize(Rank.Advisor)]
        public async Task<ActionResult<Squad>> Put(int id, Squad squad)
        {
            if (id != squad.Number) return BadRequest();
            Squad? oldSquad = await _data.Squads.FindAsync(id);
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
            await _data.SaveChangesAsync();
            return newSquad;
        }

        [HttpPut]
        [SquadCommanderAuthorize]
        public async Task<ActionResult<Squad>> Put(Squad squad)
        {
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return Unauthorized();
            Squad? oldSquad = member.Squad;
            if (oldSquad is null) return NotFound();

            Squad newSquad = oldSquad with
            {
                Description = squad.Description,
            };

            _data.Squads.Update(newSquad);
            await _data.SaveChangesAsync();
            return newSquad;
        }

        [HttpPost]
        [MinRankAuthorize]
        public async Task<ActionResult<Squad>> Post(Squad squad)
        {
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return Unauthorized();

            if (member.Squad is not null) return Forbid();


            EntityEntry<Squad>? addResult = await _data.Squads.AddAsync(squad with
            {
                Number = 0,
                Name = null,
                CreateDate = DateOnly.FromDateTime(DateTime.Today),
            });
            addResult.Entity.Members.Add(member);
            member.IsSquadCommander = true;
            await _data.SaveChangesAsync();
            //todo: add discord role creation
            await _publishSender.CallAsync($"Игроком <@{member.DiscordId:D}> созван новый, {addResult.Entity}!");
            return addResult.Entity;
        }

        [HttpPatch("{id:int}/join")]
        [MinRankAuthorize]
        public async Task<ActionResult> PatchJoin(int squadNumber)
        {
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return Unauthorized();
            if (member.Squad is not null) return Forbid();

            Squad? squad = await _data.Squads.FindAsync(squadNumber);
            if (squad is null) return NotFound();

            squad.Candidates.Add(member);
            await _data.SaveChangesAsync();
            return Ok();
        }

        [HttpPatch("{id:int}/cancelJoin")]
        [MinRankAuthorize]
        public async Task<ActionResult> PatchCancelJoin(int squadNumber)
        {
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return Unauthorized();
            if (member.Squad is not null) return Forbid();

            Squad? squad = await _data.Squads.FindAsync(squadNumber);
            if (squad is null) return NotFound();

            bool result = squad.Candidates.Remove(member);
            if (result) await _data.SaveChangesAsync();
            return result ? Ok() : NoContent();
        }


        [HttpPatch("accept")]
        [SquadCommanderAuthorize]
        public async Task<ActionResult> PatchAccept(int memberId)
        {
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return Unauthorized();
            if (member.Squad is null) return Forbid();

            Member? candidate = member.Squad.Candidates.FirstOrDefault(x => x.Id == memberId);
            if (candidate is null) return NotFound();

            member.Squad.Candidates.Remove(candidate);
            member.Squad.Members.Add(candidate);
            candidate.PendingSquadMembership.Clear();
            await _data.SaveChangesAsync();
            await _publishSender.CallAsync($"{member.Squad} пополняется игроком <@{candidate.DiscordId:D}>");
            return Ok();
        }

        [HttpPatch("decline")]
        [SquadCommanderAuthorize]
        public async Task<ActionResult> PatchDecline(int memberId)
        {
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return Unauthorized();
            if (member.Squad is null) return Forbid();

            Member? candidate = member.Squad.Candidates.FirstOrDefault(x => x.Id == memberId);
            if (candidate is null) return NotFound();

            member.Squad.Candidates.Remove(candidate);
            await _data.SaveChangesAsync();
            return Ok();
        }
    }
}
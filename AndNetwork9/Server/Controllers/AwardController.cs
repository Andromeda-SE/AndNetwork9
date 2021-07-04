﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndNetwork9.Server.Auth.Attributes;
using AndNetwork9.Server.Extensions;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Senders.Discord;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;

namespace AndNetwork9.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AwardController : ControllerBase
    {
        private readonly ClanDataContext _data;
        private readonly PublishSender _publishSender;

        public AwardController(ClanDataContext data, PublishSender publishSender)
        {
            _data = data;
            _publishSender = publishSender;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Award>>> Get()
        {
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return NotFound();

            return Ok(member.Awards.ToArray());
        }

        [HttpGet("{id:int}")]
        [MinRankAuthorize]
        public async Task<ActionResult<Award>> Get(int id)
        {
            Member? result = await _data.Members.FindAsync(id);
            return result is not null ? Ok(result.Awards.ToArray()) : NotFound();
        }

        [HttpPost]
        [MinRankAuthorize(Rank.Advisor)]
        public async Task<IActionResult> Post([FromBody] params Award[] awards)
        {
            if (awards.Any(x => x.Type == AwardType.None)) return BadRequest();
            Member? caller = await this.GetCurrentMember(_data);
            if (caller is null) return Unauthorized();

            awards = awards.OrderByDescending(x => x.Type).ToArray();
            StringBuilder text = new(256);
            List<Member> members = new(awards.Length);
            foreach (Award award in awards)
            {
                Member? member = await _data.Members.FindAsync(award.Id);
                if (member is null) return NotFound();
                if (member.Rank < Rank.Neophyte) return Forbid();
                if (caller.Rank < Rank.FirstAdvisor && award.Type > AwardType.Bronze) return Forbid();

                Award result = award with
                {
                    Id = 0,
                    GaveBy = caller,
                    GaveById = caller.Id,
                    Date = DateOnly.FromDateTime(DateTime.Today),
                    Member = member,
                    MemberId = member.Id,
                };
                text.AppendLine($"{result.Type.GetDisplayName()} достается игроку {member.GetDiscordMention()}");
                members.Add(member);
                member.Awards.Add(result);
            }

            text.AppendLine();
            foreach (Member member in members.OrderByDescending(x => x.Rank))
            {
                Rank newRank = member.Awards.GetRank();
                if (member.Rank >= newRank) continue;
                member.Rank = newRank;
                text.AppendLine($"Игрок {member.GetDiscordMention()} повышен до ранга «{member.Rank.GetRankName()}»");
            }

            await _data.SaveChangesAsync();
            await _publishSender.CallAsync(text.ToString());
            return Ok(awards);
        }
    }
}
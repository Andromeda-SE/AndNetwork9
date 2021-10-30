using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AndNetwork9.Server.Auth.Attributes;
using AndNetwork9.Server.Extensions;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Senders.AwardDispenser;
using AndNetwork9.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AndNetwork9.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AwardController : ControllerBase
{
    private readonly ClanDataContext _data;
    private readonly GiveAwardSender _giveAwardSender;

    public AwardController(ClanDataContext data, GiveAwardSender giveAwardSender)
    {
        _data = data;
        _giveAwardSender = giveAwardSender;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Award>>> Get()
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        return Ok(member.Awards.ToArray());
    }

    [HttpGet("{id:int}")]
    [MinRankAuthorize]
    public async Task<ActionResult<Award>> Get(int id)
    {
        Member? result = await _data.Members.FindAsync(id).ConfigureAwait(false);
        return result is not null ? Ok(result.Awards.ToArray()) : NotFound();
    }

    [HttpPost]
    [MinRankAuthorize(Rank.Advisor)]
    public async Task<IActionResult> Post([FromBody] params Award[] awards)
    {
        if (awards.Any(x => x.Type == AwardType.None)) return BadRequest();
        Member? caller = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (caller is null) return Unauthorized();

        foreach (Award award in awards)
        {
            Member? member = await _data.Members.FindAsync(award.MemberId).ConfigureAwait(false);
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
            await _giveAwardSender.CallAsync(result).ConfigureAwait(false);
        }

        await _data.SaveChangesAsync().ConfigureAwait(false);
        return Ok();
    }
}
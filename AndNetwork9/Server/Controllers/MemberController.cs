using System;
using System.Linq;
using System.Threading.Tasks;
using AndNetwork9.Server.Auth.Attributes;
using AndNetwork9.Server.Extensions;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Senders.Discord;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Extensions;
using AndNetwork9.Shared.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AndNetwork9.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly ClanDataContext _data;
        private readonly PublishSender _publishSender;

        public MemberController(ClanDataContext data, PublishSender publishSender)
        {
            _data = data;
            _publishSender = publishSender;
        }

        [HttpGet("all")]
        [MinRankAuthorize]
        public ActionResult<Member> GetAll()
        {
            return Ok(_data.Members.Where(x => x.Rank > Rank.None).GetShort());
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<Member>> Get()
        {
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return Unauthorized();

            return Ok(member);
        }

        [HttpGet("{id:int}")]
        [MinRankAuthorize]
        public async Task<ActionResult<Member>> Get(int id)
        {
            Member? result = await _data.Members.FindAsync(id);
            return result is not null ? Ok(result) : NotFound();
        }

        [HttpGet("{id:int}/comment")]
        [MinRankAuthorize(Rank.Advisor)]
        public async Task<ActionResult<Member>> GetComment(int id)
        {
            Member? result = await _data.Members.FindAsync(id);
            return result is not null ? Ok(result.Comment) : NotFound();
        }

        [HttpPost]
        [MinRankAuthorize(Rank.FirstAdvisor)]
        public async Task<IActionResult> Post([FromBody] Member member)
        {
            EntityEntry<Member> result = await _data.Members.AddAsync(member with
            {
                Id = 0,
                AccessRulesOverrides = Array.Empty<AccessRule>(),
                JoinDate = DateOnly.FromDateTime(DateTime.UtcNow),
            });
            await _data.SaveChangesAsync();
            return Ok(result.Entity);
        }

        [HttpPut("direction")]
        [MinRankAuthorize(Rank.Trainee)]
        public async Task<IActionResult> Patch([FromBody] Direction direction)
        {
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return Unauthorized();
            if (direction <= Direction.None) return UnprocessableEntity();
            if (DateOnly.FromDateTime(DateTime.UtcNow).DayNumber - member.LastDirectionChange.DayNumber
                < ClanRulesExtensions.MIN_DIRECTION_CHANGE_DAYS) return Forbid();

            member.Direction = direction;
            await _data.SaveChangesAsync();
            await _publishSender.CallAsync(
                $"Игрок {member.GetDiscordMention()} сменил направление на «{member.Direction.GetName()}»");
            return Ok();
        }

        [HttpPut("nickname")]
        [MinRankAuthorize(Rank.Guest)]
        public async Task<IActionResult> PatchNickname([FromBody] string newNickname)
        {
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return Unauthorized();
            ;
            if (newNickname.Length > 26) return BadRequest();
            if (_data.Members.Any(x => x.Nickname == newNickname)) return Conflict();

            string oldNickname = member.Nickname;
            member.Nickname = newNickname;
            await _data.SaveChangesAsync();
            await _publishSender.CallAsync(
                $"Игрок {member.GetDiscordMention()} сменил никнейм с «{oldNickname}» на «{newNickname}»");
            return Ok();
        }

        [HttpPut("realname")]
        [MinRankAuthorize(Rank.Guest)]
        public async Task<IActionResult> PatchRealname([FromBody] string newRealname)
        {
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return Unauthorized();

            member.RealName = string.IsNullOrWhiteSpace(newRealname) ? null : newRealname;
            await _data.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("timezone")]
        [MinRankAuthorize(Rank.Guest)]
        public async Task<IActionResult> PatchTimezone([FromBody] string newTimezone)
        {
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return Unauthorized();

            try
            {
                TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById(newTimezone);
                member.TimeZone = zone;
            }
            catch (TimeZoneNotFoundException)
            {
                return NotFound();
            }


            await _data.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        [MinRankAuthorize(Rank.Advisor)]
        public async Task<IActionResult> Put(int id, [FromBody] Member newMember)
        {
            if (id != newMember.Id) return BadRequest();
            Member? oldMember = await _data.Members.FindAsync(id);
            if (oldMember is null) return NotFound();

            string? rawValue = ControllerContext.HttpContext.User.FindFirst(AuthExtensions.MEMBER_ID_CLAIM_NAME)?.Value;
            if (rawValue is null) return Unauthorized();
            Member? caller = await _data.Members.FindAsync(int.Parse(rawValue));
            if (caller is null) return Unauthorized();

            Member? resultMember = caller.Rank switch
            {
                >= Rank.FirstAdvisor => newMember with
                {
                    AccessRulesOverrides = oldMember.AccessRulesOverrides,
                },
                >= Rank.Advisor => oldMember with
                {
                    Nickname = newMember.Nickname,
                    RealName = newMember.RealName,
                    TimeZone = newMember.TimeZone,
                    Description = newMember.Description,
                    Comment = newMember.Comment,
                },
                _ when caller.Id == id => oldMember with
                {
                    SteamId = newMember.SteamId,
                    DiscordId = newMember.DiscordId,
                    VkId = newMember.VkId,
                    TelegramId = newMember.TelegramId,
                    Nickname = newMember.Nickname,
                    RealName = newMember.RealName,
                    TimeZone = newMember.TimeZone,
                    Description = newMember.Description,
                },
                _ => null,
            };
            if (resultMember is null) return Forbid();
            EntityEntry<Member> result = _data.Members.Update(resultMember);
            await _data.SaveChangesAsync();
            return Ok(result.Entity);
        }
    }
}
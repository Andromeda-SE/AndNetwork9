using System;
using System.Linq;
using System.Threading.Tasks;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Senders.Discord;
using AndNetwork9.Shared.Backend.Senders.Steam;
using AndNetwork9.Shared.Backend.Senders.VK;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AndNetwork9.Server.Controllers.Public;

[Route("public/api/candidate")]
[ApiController]
[AllowAnonymous]
public class CandidateController : ControllerBase
{
    private readonly ClanDataContext _data;
    private readonly NewCandidateSender _newCandidateSender;
    private readonly ResolveDiscordUserNameSender _resolveDiscordUserNameSender;
    private readonly ResolveSteamUrlSender _resolveSteamUrlSender;
    private readonly ResolveVkUrlSender _resolveVkUrlSender;

    public CandidateController(ResolveSteamUrlSender resolveSteamUrlSender,
        ResolveDiscordUserNameSender resolveDiscordUserNameSender, ResolveVkUrlSender resolveVkUrlSender,
        NewCandidateSender newCandidateSender, ClanDataContext data)
    {
        _resolveSteamUrlSender = resolveSteamUrlSender;
        _resolveDiscordUserNameSender = resolveDiscordUserNameSender;
        _resolveVkUrlSender = resolveVkUrlSender;
        _newCandidateSender = newCandidateSender;
        _data = data;
    }

    [HttpPost]
    [ProducesResponseType(typeof(void), 200)]
    [ProducesResponseType(typeof(void), 409)]
    public async Task<ActionResult> Post([FromBody] CandidateRequest request)
    {
        if (await _data.Members.AnyAsync(x =>
                x.Nickname == request.Nickname
                || x.Rank >= Rank.Candidate
                && (x.SteamId == request.SteamId
                    || x.DiscordId == request.DiscordId)).ConfigureAwait(false))
            return Conflict();
        Member[] similarMembers = await _data.Members.Where(x =>
                x.DiscordId == request.DiscordId || x.SteamId == request.SteamId)
            .ToArrayAsync()
            .ConfigureAwait(false);
        switch (similarMembers.Length)
        {
            case 0:
                await _data.Members.AddAsync(new()
                {
                    Nickname = request.Nickname,
                    RealName = request.RealName,
                    SteamId = request.SteamId,
                    DiscordId = request.DiscordId,
                    VkId = request.VkId,
                    JoinDate = DateOnly.FromDateTime(DateTime.UtcNow),
                    TimeZone = request.TimeZone,
                    PasswordHash = null,
                    Direction = Direction.None,
                    Rank = Rank.Candidate,
                }).ConfigureAwait(false);
                break;
            case 1:
            {
                Member similarMember = similarMembers.Single();
                similarMember.Nickname = request.Nickname;
                similarMember.RealName = request.RealName;
                similarMember.SteamId = request.SteamId;
                similarMember.DiscordId = request.DiscordId;
                similarMember.VkId = request.VkId;
                similarMember.JoinDate = DateOnly.FromDateTime(DateTime.UtcNow);
                similarMember.TimeZone = request.TimeZone;
                similarMember.PasswordHash = null;
                similarMember.Direction = Direction.None;
                similarMember.Rank = Rank.Candidate;
                break;
            }
            default:
                return Conflict();
        }

        await _data.SaveChangesAsync().ConfigureAwait(false);
        await _newCandidateSender.CallAsync(request).ConfigureAwait(false);
        return Ok();
    }


    [HttpPost("steam")]
    [ProducesResponseType(typeof(ulong), 200)]
    [ProducesResponseType(typeof(void), 404)]
    [ProducesResponseType(typeof(void), 409)]
    public async Task<ActionResult<ulong>> GetSteamId([FromBody] string url)
    {
        ulong? result = await _resolveSteamUrlSender.CallAsync(url).ConfigureAwait(false);
        if (result is null) return NotFound();
        if (await _data.Members.AnyAsync(x => x.SteamId == result.Value).ConfigureAwait(false)) return Conflict();
        return Ok(result.Value);
    }


    [HttpPost("discord")]
    [ProducesResponseType(typeof(ulong), 200)]
    [ProducesResponseType(typeof(void), 404)]
    [ProducesResponseType(typeof(void), 409)]
    public async Task<ActionResult<ulong?>> GetDiscordId([FromBody] string username)
    {
        ulong? result = await _resolveDiscordUserNameSender.CallAsync(username).ConfigureAwait(false);
        if (result is null) return NotFound();
        if (await _data.Members.AnyAsync(x => x.DiscordId == result.Value).ConfigureAwait(false)) return Conflict();
        return Ok(result.Value);
    }


    [HttpPost("vk")]
    [ProducesResponseType(typeof(ulong), 200)]
    [ProducesResponseType(typeof(void), 404)]
    [ProducesResponseType(typeof(void), 409)]
    public async Task<ActionResult<long?>> GetVkId([FromBody] string url)
    {
        long? result = await _resolveVkUrlSender.CallAsync(url).ConfigureAwait(false);
        if (result is null) return NotFound();
        if (await _data.Members.AnyAsync(x => x.VkId == result.Value).ConfigureAwait(false)) return Conflict();
        return Ok(result.Value);
    }
}
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AndNetwork9.Server.Auth.Attributes;
using AndNetwork9.Server.Extensions;
using AndNetwork9.Server.Hubs;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Elections;
using AndNetwork9.Shared.Backend.Senders.Elections;
using AndNetwork9.Shared.Elections;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AndNetwork9.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ElectionController : ControllerBase
{
    private readonly ClanDataContext _data;
    private readonly IHubContext<ModelHub, IModelHub> _modelHub;
    private readonly VoteSender _voteSender;

    public ElectionController(ClanDataContext data, VoteSender voteSender, IHubContext<ModelHub, IModelHub> modelHub)
    {
        _data = data;
        _voteSender = voteSender;
        _modelHub = modelHub;
    }

    [HttpGet]
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 30, NoStore = false)]
    public async Task<ActionResult<IAsyncEnumerable<CouncilElection>>> Get()
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        return Ok(_data.Elections.Select(x => new {x.Id, x.Stage}));
    }

    [HttpGet("current")]
    [Authorize]
    public async Task<ActionResult<CouncilElection>> GetCurrent()
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        return Ok(_data.Elections.Single(x => x.Stage != ElectionStage.Ended).GetCouncilElection(member));
    }

    [HttpGet("{id:int}")]
    [MinRankAuthorize]
    [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 30, NoStore = false)]
    public async Task<ActionResult<CouncilElection>> Get(int id)
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        Election? election = await _data.Elections.FindAsync(id).ConfigureAwait(false);
        if (election is null) return NotFound();
        CouncilElection result = election.GetCouncilElection(member);
        return Ok(result);
    }


    [HttpPost("vote")]
    [MinRankAuthorize]
    public async Task<IActionResult> PostVote(Dictionary<int, Dictionary<int, uint>> vote)
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();
        try
        {
            await _voteSender.CallAsync(new(member.Id, vote.ToDictionary(x => (Direction)x.Key, x => x.Value)))
                .ConfigureAwait(false);
        }
        catch
        {
            return BadRequest();
        }

        await _modelHub.Clients
            .Users(await _data.Members.AsAsyncEnumerable().Where(x => x.Rank > Rank.None)
                .Select(x => x.Id.ToString("D", CultureInfo.InvariantCulture)).ToArrayAsync().ConfigureAwait(false))
            .ReceiveModelUpdate(typeof(CouncilElection).FullName,
                _data.Elections.Single(x => x.Stage != ElectionStage.Ended).GetCouncilElection(member))
            .ConfigureAwait(false);
        return Ok();
    }

    [HttpPost("reg")]
    [MinRankAuthorize()]
    public async Task<IActionResult> PostReg()
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);

        return Ok();
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using AndNetwork9.Server.Auth.Attributes;
using AndNetwork9.Server.Extensions;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Elections;
using AndNetwork9.Shared.Backend.Senders.Elections;
using AndNetwork9.Shared.Elections;
using AndNetwork9.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AndNetwork9.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElectionController : ControllerBase
    {
        private readonly ClanDataContext _data;
        private readonly VoteSender _voteSender;

        public ElectionController(ClanDataContext data, VoteSender voteSender)
        {
            _data = data;
            _voteSender = voteSender;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<CouncilElection>> Get()
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

            return Ok((CouncilElection)_data.Elections.Single(x => x.Stage != ElectionStage.Ended));
        }

        [HttpGet("{id:int}")]
        [MinRankAuthorize]
        public async Task<ActionResult<CouncilElection>> Get(int id)
        {
            Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
            if (member is null) return Unauthorized();

            Election? election = await _data.Elections.FindAsync(id).ConfigureAwait(false);
            if (election is null) return NotFound();

            return Ok((CouncilElection)election);
        }

        [HttpGet("tokens")]
        [MinRankAuthorize]
        public async Task<ActionResult<CouncilElectionTokenPack>> GetTokens()
        {
            Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
            if (member is null) return Unauthorized();

            Election election = _data.Elections.Single(x => x.Stage != ElectionStage.Ended);

            return Ok(new CouncilElectionTokenPack
            {
                MemberId = member.Id,
                ElectionId = election.Id,
                Tokens = election.Votings
                    .Where(x => x.Members.Any(y =>
                        y.Votes is null
                        && !y.Voted
                        && y.MemberId == member.Id
                        && y.VoterKey is not null
                        && y.VoterKey != Guid.Empty))
                    .ToDictionary(x => x.Direction,
                        x => x.Members.First(y => y.MemberId == member.Id).VoterKey.GetValueOrDefault(Guid.Empty)),
            });
        }

        [HttpPost("vote")]
        [MinRankAuthorize]
        public async Task<IActionResult> PostVote(CouncilElectionVote vote)
        {
            Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
            if (member is null) return Unauthorized();
            try
            {
                await _voteSender.CallAsync(new(member.Id,
                    vote.Key,
                    vote.Direction,
                    vote.Votes.Select(x => new VoteArgNode(x.Key == 0 ? null : x.Key, x.Value)).ToArray())).ConfigureAwait(false);
            }
            catch
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
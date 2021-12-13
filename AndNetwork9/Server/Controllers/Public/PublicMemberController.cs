﻿using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using AndNetwork9.Server.Extensions;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AndNetwork9.Server.Controllers.Public;

[Route("public/api/member")]
[ApiController]
[AllowAnonymous]
public class PublicMemberController : ControllerBase
{
    private readonly ClanDataContext _data;

    public PublicMemberController(ClanDataContext data) => _data = data;

    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable), 200)]
    [ProducesResponseType(304)]
    [RequestSizeLimit(16384)]
    [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 86400, NoStore = false)]
    public async Task<ActionResult> GetAll()
    {
        IQueryable<Member> result = _data.Members.Where(x => x.Rank > Rank.None);
        return Ok((await result.ToArrayAsync().ConfigureAwait(false)).GetPublicShort());
    }
}
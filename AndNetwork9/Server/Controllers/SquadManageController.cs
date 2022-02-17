using System;
using System.Collections.Generic;
using System.Linq;
using AndNetwork9.Server.Auth.Attributes;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AndNetwork9.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SquadManageController : ControllerBase
{
    private readonly ClanDataContext _data;
    public SquadManageController(ClanDataContext data) => _data = data;

    [HttpGet("all")]
    [CaptainAuthorize]
    [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 30, NoStore = false)]
    public ActionResult<IAsyncEnumerable<Squad>> GetAll()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        return Ok(_data.Squads.Where(x => x.DisbandDate == null || x.DisbandDate > today).AsAsyncEnumerable());
    }
}
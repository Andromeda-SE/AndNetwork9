﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AndNetwork9.Server.Auth.Attributes;
using AndNetwork9.Server.Extensions;
using AndNetwork9.Server.Hubs;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Hubs;
using AndNetwork9.Shared.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AndNetwork9.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccessRuleController : ControllerBase
{
    private readonly ClanDataContext _data;
    private readonly IHubContext<ModelHub, IModelHub> _modelHub;

    public AccessRuleController(ClanDataContext data, IHubContext<ModelHub, IModelHub> modelHub)
    {
        _data = data;
        _modelHub = modelHub;
    }

    [HttpGet("{id:int}")]
    [Authorize]
    [ProducesResponseType(typeof(AccessRule), 200)]
    [ProducesResponseType(typeof(void), 404)]
    [ProducesResponseType(typeof(void), 401)]
    public async Task<ActionResult<AccessRule>> Get(int id)
    {
        AccessRule? result = await _data.AccessRules.FindAsync(id).ConfigureAwait(false);
        return result is not null ? Ok(result) : NotFound();
    }

    [HttpGet("{id:int}/overrides")]
    [Authorize]
    [ProducesResponseType(typeof(IList<Member>), 200)]
    [ProducesResponseType(typeof(void), 404)]
    [ProducesResponseType(typeof(void), 401)]
    public async Task<ActionResult<IAsyncEnumerable<Member>>> GetOverrides(int id)
    {
        AccessRule? result = await _data.AccessRules.FindAsync(id).ConfigureAwait(false);
        return result is not null
            ? Ok(result.AllowedMembers.GetShort())
            : NotFound();
    }

    [HttpPost]
    [MinRankAuthorize]
    [ProducesResponseType(typeof(AccessRule), 200)]
    [ProducesResponseType(typeof(void), 401)]
    public async Task<ActionResult<AccessRule>> Post(AccessRule rule)
    {
        rule.AllowedMembers = _data.Members.AsEnumerable()
            .Join(rule.AllowedMembersIds, x => x.Id, x => x, (member, _) => member)
            .ToArray();
        EntityEntry<AccessRule> result = await _data.AccessRules.AddAsync(rule with {Id = 0}).ConfigureAwait(false);
        await _data.SaveChangesAsync().ConfigureAwait(false);
        await _modelHub.Clients.All.ReceiveModelUpdate(typeof(AccessRule).FullName, result.Entity)
            .ConfigureAwait(false);
        return Ok(result.Entity);
    }
}
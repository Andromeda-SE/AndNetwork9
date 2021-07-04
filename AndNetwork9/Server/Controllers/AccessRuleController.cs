using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AndNetwork9.Server.Auth.Attributes;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AndNetwork9.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessRuleController : ControllerBase
    {
        private readonly ClanDataContext _data;

        public AccessRuleController(ClanDataContext data)
        {
            _data = data;
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<AccessRule>> Get(int id)
        {
            AccessRule? result = await _data.AccessRules.FindAsync(id);
            return result is not null ? Ok(result) : NotFound();
        }

        [HttpGet("{id:int}/overrides")]
        [Authorize]
        public async Task<ActionResult<IList<Member>>> GetOverrides(int id)
        {
            AccessRule? result = await _data.AccessRules.FindAsync(id);
            return result is not null
                ? Ok(result.AllowedMembers.Select(x => new { x.Id, x.Nickname, x.RealName, x.Rank, x.Direction }))
                : NotFound();
        }

        [HttpPost]
        [MinRankAuthorize]
        public async Task<ActionResult<AccessRule>> Post(AccessRule rule)
        {
            EntityEntry<AccessRule> result = await _data.AccessRules.AddAsync(rule with { Id = 0 });
            await _data.SaveChangesAsync();
            return Ok(result.Entity);
        }
    }
}
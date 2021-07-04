using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AndNetwork9.Server.Auth.Attributes;
using AndNetwork9.Server.Extensions;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Auth;
using AndNetwork9.Shared.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AndNetwork9.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ClanDataContext _data;

        public AuthController(ClanDataContext data)
        {
            _data = data;
        }

        // POST api/<AuthController>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyDictionary<string, string>>> Post([FromBody] AuthCredentials value)
        {
            Member? member = await _data.Members.FirstOrDefaultAsync(x => x.Nickname == value.Nickname);
            if (member is null
                || member.PasswordHash is null
                || !member.PasswordHash.SequenceEqual(value.Password.GetPasswordHash())) return NotFound();

            AuthSession session = new()
            {
                Member = member,
                Code = null,
                CreateTime = DateTime.UtcNow,
                Address = HttpContext.Connection.RemoteIpAddress ?? throw new ArgumentException("", nameof(value)),
                ExpireTime = DateTime.UtcNow.AddYears(1),
                SessionId = Guid.NewGuid(),
                UserAgent = HttpContext.Request.Headers["User-Agent"],
                CodeExpireTime = DateTime.UtcNow,
            };
            _data.Sessions.Add(session);
            List<Claim> claims = new()
            {
                new(AuthExtensions.MEMBER_ID_CLAIM_NAME, member.Id.ToString("D", CultureInfo.InvariantCulture)),
                new(AuthExtensions.SESSION_ID_CLAIM_NAME, session.SessionId.ToString("N")),
            };
            ClaimsIdentity claimsIdentity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties authProperties = new()
            {
                AllowRefresh = true,
                ExpiresUtc = session.ExpireTime,
                IsPersistent = true,
                IssuedUtc = session.CreateTime,
            };
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new(claimsIdentity),
                authProperties);
            await _data.SaveChangesAsync();
            return Ok(claims.ToDictionary(x => x.Type, x => x.Value));
        }

        [HttpGet("restore/{sessionId:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyDictionary<string, string>>> Post(Guid sessionId)
        {
            AuthSession? session = await _data.Sessions.FindAsync(sessionId);
            if (session is null) return NotFound();
            if (session.ExpireTime is not null && session.ExpireTime <= DateTime.Now) return NotFound();
            if (!Equals(session.Address, HttpContext.Connection.RemoteIpAddress)) return NotFound();

            Member member = session.Member;


            List<Claim> claims = new()
            {
                new(AuthExtensions.MEMBER_ID_CLAIM_NAME, member.Id.ToString("D", CultureInfo.InvariantCulture)),
                new(AuthExtensions.SESSION_ID_CLAIM_NAME, session.SessionId.ToString("N")),
            };
            ClaimsIdentity claimsIdentity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties authProperties = new()
            {
                AllowRefresh = true,
                ExpiresUtc = session.ExpireTime,
                IsPersistent = true,
                IssuedUtc = session.CreateTime,
            };
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new(claimsIdentity),
                authProperties);
            await _data.SaveChangesAsync();
            return Ok(claims.ToDictionary(x => x.Type, x => x.Value));
        }

        // PUT api/<AuthController>/5
        [HttpPut]
        [MinRankAuthorize]
        public async Task<IActionResult> Put([FromBody] string newPassword)
        {
            string? rawValue = ControllerContext.HttpContext.User.FindFirst(AuthExtensions.MEMBER_ID_CLAIM_NAME)?.Value;
            if (rawValue is null) return NotFound();
            Member? member = await _data.Members.FindAsync(int.Parse(rawValue));
            if (member is null) return NotFound();

            member.SetPassword(newPassword);
            await _data.SaveChangesAsync();
            return Ok();
        }
#if DEBUG
        // PUT api/<AuthController>/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] string newPassword)
        {
            Member? member = await _data.Members.FindAsync(id);
            if (member is not null)
            {
                member.SetPassword(newPassword);
                await _data.SaveChangesAsync();
                return Ok();
            }

            return NotFound();
        }

#endif

        // DELETE api/<AuthController>/5
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete()
        {
            Claim? sessionClaim = ControllerContext.HttpContext.User.FindFirst(AuthExtensions.SESSION_ID_CLAIM_NAME);
            if (sessionClaim is null) return NotFound();
            if (!Guid.TryParse(sessionClaim.Value, out Guid sessionId)) return BadRequest();
            AuthSession? session = await _data.Sessions.FindAsync(sessionId);
            if (session is null) return NotFound();
            _data.Sessions.Remove(session);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await _data.SaveChangesAsync();
            return Ok();
        }

        // DELETE api/<AuthController>/5
        [HttpDelete("other")]
        [Authorize]
        public async Task<IActionResult> DeleteOther()
        {
            Claim? sessionClaim = ControllerContext.HttpContext.User.FindFirst(AuthExtensions.SESSION_ID_CLAIM_NAME);
            if (sessionClaim is null) return NotFound();
            if (!Guid.TryParse(sessionClaim.Value, out Guid sessionId)) return BadRequest();
            AuthSession? session = await _data.Sessions.FindAsync(sessionId);
            if (session is null) return NotFound();
            _data.Sessions.RemoveRange(_data.Sessions
                .Where(x => x.SessionId != sessionId && x.Member.Id == session.Member.Id).ToArray());
            await _data.SaveChangesAsync();
            return Ok();
        }
    }
}
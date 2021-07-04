﻿using System;
using System.IO;
using System.Threading.Tasks;
using AndNetwork9.Server.Auth.Attributes;
using AndNetwork9.Server.Extensions;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Senders.Discord;
using AndNetwork9.Shared.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AndNetwork9.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaticFileController : ControllerBase
    {
        private readonly ClanDataContext _data;
        private readonly SaveStaticFileSender _staticFileSender;

        public StaticFileController(ClanDataContext data, SaveStaticFileSender staticFileSender)
        {
            _data = data;
            _staticFileSender = staticFileSender;
        }

        [HttpGet("{id:int}")]
        [MinRankAuthorize]
        public async Task<ActionResult> Get(int id)
        {
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return Unauthorized();

            StaticFile? file = await _data.StaticFiles.FindAsync(id);
            if (file is null) return NotFound();
            if (!file.ReadRule.HasAccess(member)) return Forbid();

            return Redirect(file.Path);
        }

        [HttpPost]
        [MinRankAuthorize]
        public async Task<ActionResult<StaticFile>> Post(IFormFile file)
        {
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return Unauthorized();

            byte[] content = new byte[file.Length];
            await using Stream stream = new MemoryStream(content);
            await file.CopyToAsync(stream);

            return await _staticFileSender.CallAsync(new(file.FileName, content, member.Id, null))
                   ?? throw new InvalidOperationException();
        }
    }
}
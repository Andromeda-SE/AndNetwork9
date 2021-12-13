using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AndNetwork9.Server.Auth.Attributes;
using AndNetwork9.Server.Extensions;
using AndNetwork9.Server.Hubs;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Senders.Discord;
using AndNetwork9.Shared.Hubs;
using AndNetwork9.Shared.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AndNetwork9.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StaticFileController : ControllerBase
{
    private readonly ClanDataContext _data;
    private readonly IHubContext<ModelHub, IModelHub> _modelHub;
    private readonly SaveStaticFileSender _staticFileSender;

    public StaticFileController(ClanDataContext data, SaveStaticFileSender staticFileSender,
        IHubContext<ModelHub, IModelHub> modelHub)
    {
        _data = data;
        _staticFileSender = staticFileSender;
        _modelHub = modelHub;
    }

    [HttpGet("{id:int}")]
    [MinRankAuthorize]
    public async Task<ActionResult> Get(int id)
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        StaticFile? file = await _data.StaticFiles.FindAsync(id).ConfigureAwait(false);
        if (file is null) return NotFound();
        if (!file.ReadRule.HasAccess(member)) return Forbid();

        return Redirect(file.Path);
    }

    [HttpPost]
    [MinRankAuthorize]
    public async Task<ActionResult<StaticFile>> Post(IFormFile file)
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        byte[] content = new byte[file.Length];
        Stream stream = new MemoryStream(content);
        await using ConfiguredAsyncDisposable _ = stream.ConfigureAwait(false);
        await file.CopyToAsync(stream).ConfigureAwait(false);

        StaticFile result =
            await _staticFileSender.CallAsync(new(file.FileName, content, member.Id, null)).ConfigureAwait(false)
            ?? throw new InvalidOperationException();
        //await _modelHub.Clients.Users(_data.Members.Where(x => x.)).ReceiveModelUpdate(typeof(StaticFile).FullName, result).ConfigureAwait(false);
        return result;
    }
}
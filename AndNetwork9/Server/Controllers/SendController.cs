using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AndNetwork9.Server.Auth.Attributes;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Senders.Discord;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AndNetwork9.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SendController : ControllerBase
{
    private readonly ClanDataContext _data;
    private readonly SendSender _sendSender;

    public SendController(ClanDataContext data, SendSender sendSender)
    {
        _data = data;
        _sendSender = sendSender;
    }

    [HttpPost]
    [MinRankAuthorize(Rank.Advisor)]
    public async Task<ActionResult> Post(MessageSendArgs args)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (args.Ids is null || args.Ids.Length == 0 || string.IsNullOrWhiteSpace(args.Message)) return NoContent();
        if (args.Message.Length > 2000) return StatusCode((int)HttpStatusCode.RequestEntityTooLarge);
        args = args with
        {
            Ids = args.Ids.Distinct().ToArray(),
        };
        await foreach (ulong id in _data.Members.Where(x => x.DiscordId != null).AsAsyncEnumerable().Join(
                               args.Ids.ToAsyncEnumerable(),
                               x => x.Id,
                               x => x,
                               (member, _) => member.DiscordId!.Value)
                           .ConfigureAwait(false))
            await _sendSender.CallAsync(new(id, args.Message)).ConfigureAwait(false);

        return Accepted();
    }
}
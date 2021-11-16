using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AndNetwork9.Server.Extensions;
using AndNetwork9.Server.Hubs;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Senders.Discord;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Extensions;
using AndNetwork9.Shared.Hubs;
using AndNetwork9.Shared.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Task = AndNetwork9.Shared.Task;
using TaskStatus = AndNetwork9.Shared.Enums.TaskStatus;

namespace AndNetwork9.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TaskController : ControllerBase
{
    private readonly ClanDataContext _data;
    private readonly SendSender _sendSender;
    private readonly IEqualityComparer<Tag> _tagComparer = new GenericEqualityComparer<Tag, string>(x => x.Name);
    private readonly IHubContext<ModelHub, IModelHub> _modelHub;

    public TaskController(ClanDataContext data, SendSender sendSender, IHubContext<ModelHub, IModelHub> modelHub)
    {
        _data = data;
        _sendSender = sendSender;
        _modelHub = modelHub;
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async System.Threading.Tasks.Task<ActionResult<Task>> Get(int id)
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        Task? task = await _data.Tasks.FindAsync(id).ConfigureAwait(false);
        if (task is null) return NotFound();
        if (!task.ReadRule.HasAccess(member) || task.AssigneeId != member.Id && task.ReporterId != member.Id)
            return Forbid();

        return Ok(task);
    }

    [HttpGet("{id:int}/children")]
    [Authorize]
    public async System.Threading.Tasks.Task<ActionResult<IList<Task>>> GetChildren(int id)
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        Task? task = await _data.Tasks.FindAsync(id).ConfigureAwait(false);
        if (task is null) return NotFound();
        if (!task.ReadRule.HasAccess(member) || task.AssigneeId != member.Id && task.ReporterId != member.Id)
            return Forbid();

        return Ok(task.Children);
    }

    [HttpGet("me")]
    [Authorize]
    public async System.Threading.Tasks.Task<ActionResult<IAsyncEnumerable<Task>>> GetMe()
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        // ReSharper disable once MergeIntoPattern
        // ReSharper disable once MergeIntoLogicalPattern
        return Ok(await _data.Tasks.Where(x =>
                x.AssigneeId == member.Id && x.Status > TaskStatus.New && x.Status < TaskStatus.Resolved
                || x.ReporterId == member.Id
                && (x.Status == TaskStatus.Resolved || x.Status == TaskStatus.Rejected))
            .OrderBy(x => x).ToArrayAsync().ConfigureAwait(false));
    }

    [HttpGet("squad")]
    [Authorize]
    public async System.Threading.Tasks.Task<ActionResult<Task>> GetSquad()
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();
        if (member.SquadNumber is null) return Forbid();

        // ReSharper disable once MergeIntoPattern
        // ReSharper disable once MergeIntoLogicalPattern
        return Ok(await _data.Tasks.Where(x =>
            x.SquadAssigneeId == member.SquadNumber
            && x.Status > TaskStatus.New
            && x.Status < TaskStatus.Resolved
            && x.ReadRule.HasAccess(member)).OrderBy(x => x).ToArrayAsync().ConfigureAwait(false));
    }

    [HttpGet("direction")]
    [Authorize]
    public async System.Threading.Tasks.Task<ActionResult<Task>> GetDirection()
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        // ReSharper disable once MergeIntoPattern
        // ReSharper disable once MergeIntoLogicalPattern
        return Ok(await _data.Tasks.Where(x =>
            x.DirectionAssignee == member.Direction
            && x.Status > TaskStatus.New
            && x.Status < TaskStatus.Resolved
            && x.ReadRule.HasAccess(member)).OrderBy(x => x).ToArrayAsync().ConfigureAwait(false));
    }

    [HttpGet("me/assignee")]
    [Authorize]
    public async System.Threading.Tasks.Task<ActionResult<IAsyncEnumerable<Task>>> GetMeAssignee()
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        return Ok(await _data.Tasks.Where(x => x.AssigneeId == member.Id).OrderBy(x => x).ToArrayAsync().ConfigureAwait(false));
    }

    [HttpGet("me/assignee/analysis")]
    [Authorize]
    public async System.Threading.Tasks.Task<ActionResult<IAsyncEnumerable<Task>>> GetMeAssigneeAnalysis()
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        return Ok(await _data.Tasks.Where(x =>
            x.AssigneeId == member.Id 
            && (x.Status >= TaskStatus.New 
                && x.Status < TaskStatus.ToDo)).OrderBy(x => x).ToArrayAsync().ConfigureAwait(false));
    }

    [HttpGet("me/assignee/active")]
    [Authorize]
    public async System.Threading.Tasks.Task<ActionResult<IAsyncEnumerable<Task>>> GetMeAssigneeActive()
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        return Ok(await _data.Tasks.Where(x => x.AssigneeId == member.Id 
                                               && (x.Status == TaskStatus.ToDo 
                                                   || x.Status == TaskStatus.InProgress)).OrderBy(x => x).ToArrayAsync().ConfigureAwait(false));
    }


    [HttpGet("me/assignee/closed")]
    [Authorize]
    public async System.Threading.Tasks.Task<ActionResult<IAsyncEnumerable<Task>>> GetMeAssigneeClosed()
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        return Ok(await _data.Tasks.Where(x => x.AssigneeId == member.Id 
                                               && (x.Status <= TaskStatus.Canceled 
                                                   || x.Status >= TaskStatus.Resolved)).OrderBy(x => x).ToArrayAsync().ConfigureAwait(false));
    }

    [HttpGet("squadpart/assignee/analysis")]
    [Authorize]
    public async System.Threading.Tasks.Task<ActionResult<IAsyncEnumerable<Task>>> GetSquadPartAssigneeAnalysis()
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        return Ok(await _data.Tasks.Where(x => x.SquadAssigneeId == member.SquadNumber 
                                               && x.SquadPartAssigneeId == member.SquadPartNumber 
                                               && (x.Status >= TaskStatus.New
                                                   && x.Status < TaskStatus.ToDo)).OrderBy(x => x).ToArrayAsync().ConfigureAwait(false));
    }

    [HttpGet("squadpart/assignee/active")]
    [Authorize]
    public async System.Threading.Tasks.Task<ActionResult<IAsyncEnumerable<Task>>> GetSquadPartAssigneeActive()
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        return Ok(await _data.Tasks.Where(x => x.SquadAssigneeId == member.SquadNumber
                                               && x.SquadPartAssigneeId == member.SquadPartNumber
                                               && (x.Status == TaskStatus.ToDo
                                                   || x.Status == TaskStatus.InProgress)).OrderBy(x => x).ToArrayAsync().ConfigureAwait(false));
    }


    [HttpGet("squadpart/assignee/closed")]
    [Authorize]
    public async System.Threading.Tasks.Task<ActionResult<IAsyncEnumerable<Task>>> GetSquadPartAssigneeClosed()
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        return Ok(await _data.Tasks.Where(x => x.SquadAssigneeId == member.SquadNumber
                                               && x.SquadPartAssigneeId == member.SquadPartNumber
                                               && (x.Status <= TaskStatus.Canceled
                                                   || x.Status >= TaskStatus.Resolved)).OrderBy(x => x).ToArrayAsync().ConfigureAwait(false));
    }

    [HttpGet("squad/assignee/analysis")]
    [Authorize]
    public async System.Threading.Tasks.Task<ActionResult<IAsyncEnumerable<Task>>> GetSquadAssigneeAnalysis()
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        return Ok(await _data.Tasks.Where(x => x.SquadAssigneeId == member.SquadNumber
                                               && (x.Status >= TaskStatus.New
                                                   && x.Status < TaskStatus.ToDo)).OrderBy(x => x).ToArrayAsync().ConfigureAwait(false));
    }

    [HttpGet("squad/assignee/active")]
    [Authorize]
    public async System.Threading.Tasks.Task<ActionResult<IAsyncEnumerable<Task>>> GetSquadAssigneeActive()
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        return Ok(await _data.Tasks.Where(x => x.SquadAssigneeId == member.SquadNumber
                                               && (x.Status == TaskStatus.ToDo
                                                   || x.Status == TaskStatus.InProgress)).OrderBy(x => x).ToArrayAsync().ConfigureAwait(false));
    }


    [HttpGet("squad/assignee/closed")]
    [Authorize]
    public async System.Threading.Tasks.Task<ActionResult<IAsyncEnumerable<Task>>> GetSquadAssigneeClosed()
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        return Ok(await _data.Tasks.Where(x => x.SquadAssigneeId == member.SquadNumber
                                               && (x.Status <= TaskStatus.Canceled
                                                   || x.Status >= TaskStatus.Resolved)).OrderBy(x => x).ToArrayAsync().ConfigureAwait(false));
    }

    [HttpGet("direction/assignee/analysis")]
    [Authorize]
    public async System.Threading.Tasks.Task<ActionResult<IAsyncEnumerable<Task>>> GetDirectionAssigneeAnalysis()
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        return Ok(await _data.Tasks.Where(x => x.DirectionAssignee == member.Direction
                                               && (x.Status >= TaskStatus.New
                                                   && x.Status < TaskStatus.ToDo)).OrderBy(x => x).ToArrayAsync().ConfigureAwait(false));
    }

    [HttpGet("direction/assignee/active")]
    [Authorize]
    public async System.Threading.Tasks.Task<ActionResult<IAsyncEnumerable<Task>>> GetDirectionAssigneeActive()
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        return Ok(await _data.Tasks.Where(x => x.DirectionAssignee == member.Direction
                                               && (x.Status == TaskStatus.ToDo
                                                   || x.Status == TaskStatus.InProgress)).OrderBy(x => x).ToArrayAsync().ConfigureAwait(false));
    }


    [HttpGet("direction/assignee/closed")]
    [Authorize]
    public async System.Threading.Tasks.Task<ActionResult<IAsyncEnumerable<Task>>> GetDirectionAssigneeClosed()
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        return Ok(await _data.Tasks.Where(x => x.DirectionAssignee == member.Direction
                                               && (x.Status <= TaskStatus.Canceled
                                                   || x.Status >= TaskStatus.Resolved)).OrderBy(x => x).ToArrayAsync().ConfigureAwait(false));
    }

    [HttpGet("me/reporter")]
    [Authorize]
    public async System.Threading.Tasks.Task<ActionResult<IAsyncEnumerable<Task>>> GetMeReporter()
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        return Ok(await _data.Tasks.Where(x => x.ReporterId == member.Id).OrderBy(x => x).ToArrayAsync().ConfigureAwait(false));
    }

    [HttpGet("me/watcher")]
    [Authorize]
    public async System.Threading.Tasks.Task<ActionResult<IAsyncEnumerable<Task>>> GetMeWatcher()
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        return Ok(await _data.Tasks.AsAsyncEnumerable().Where(x => x.WatchersId.Any(y => y == member.Id) && x.ReadRule.HasAccess(member))
            .OrderBy(x => x).ToArrayAsync().ConfigureAwait(false));
    }

    [HttpGet("unassigned")]
    [Authorize]
    public async System.Threading.Tasks.Task<ActionResult<IAsyncEnumerable<Task>>> GetUnassigned()
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        return Ok(await _data.Tasks.AsAsyncEnumerable().Where(x =>
            x.AssigneeId == null
            && x.ReadRule.HasAccess(member)
            && (x.AllowAssignByMember || x.WriteRule.HasAccess(member))
            && x.Status is >= TaskStatus.New and < TaskStatus.Resolved).OrderBy(x => x).ToArrayAsync().ConfigureAwait(false));
    }

    [HttpPost]
    [Authorize]
    public async System.Threading.Tasks.Task<ActionResult<Task>> Post(Task task)
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();
        if (string.IsNullOrWhiteSpace(task.Description?.Text)) return BadRequest();
        task = task with
        {
            Reporter = member,
            ReporterId = member.Id,
            Id = 0,
            CreateTime = DateTime.UtcNow,
            LastEditTime = null,
            Comments = Array.Empty<Comment>(),
            Status = TaskStatus.ToDo,
        };

        if (task.Tags?.Any() ?? false)
        {
            Tag[] newTags = task.Tags.Except(_data.Tags, _tagComparer).ToArray();
            if (newTags.Any()) await _data.Tags.AddRangeAsync(newTags).ConfigureAwait(false);
            task.Tags = task.Tags.Join(_data.Tags, x => x.Name, x => x.Name, (_, tag) => tag).ToArray();
        }

        if (task.Files?.Except(_data.StaticFiles).Any() ?? false) return BadRequest();

        AccessRule? readRule = await _data.AccessRules.FindAsync(task.ReadRuleId).ConfigureAwait(false);
        if (readRule is null) return BadRequest();
        task.ReadRule = readRule;

        AccessRule? writeRule = await _data.AccessRules.FindAsync(task.WriteRuleId).ConfigureAwait(false);
        if (writeRule is null) return BadRequest();
        task.WriteRule = writeRule;

        if (task.AssigneeId is not null)
        {
            task.Assignee = await _data.Members.FindAsync(task.AssigneeId.Value).ConfigureAwait(false);
            if (task.Assignee is null) return BadRequest();
        }
        else
        {
            task.Assignee = null;
        }

        if (task.SquadAssigneeId is not null)
        {
            task.SquadAssignee = await _data.Squads.FindAsync(task.SquadAssigneeId.Value).ConfigureAwait(false);
            if (task.SquadAssignee is null) return BadRequest();
        }
        else
        {
            task.SquadAssignee = null;
        }

        if (task.DirectionAssignee <= Direction.None) task.DirectionAssignee = null;

        List<Member?> watchers = new()
        {
            member,
            _data.Members.FirstOrDefault(x => x.Rank == Rank.FirstAdvisor),
        };
        if (task.DirectionAssignee is not null)
            watchers.Add(_data.Members.FirstOrDefault(x =>
                x.Rank == Rank.Advisor && x.Direction == task.DirectionAssignee));
        if (task.SquadAssigneeId is not null)
            watchers.AddRange(_data.Members.Where(x =>
                x.CommanderLevel > SquadCommander.None && x.SquadNumber == task.SquadAssigneeId));
        if (task.AssigneeId is not null)
            watchers.Add(await _data.Members.FindAsync(task.AssigneeId.Value).ConfigureAwait(false));

        task.Watchers = watchers.Where(x => x is not null).ToArray()!;

        if (task.ParentId is not null)
        {
            Task? parent = await _data.Tasks.FindAsync(task.ParentId).ConfigureAwait(false);
            if (parent is null) return BadRequest();
            task.Parent = parent;
        }
        else
        {
            task.Parent = null;
        }

        EntityEntry<Task> entity = await _data.Tasks.AddAsync(task).ConfigureAwait(false);
        Comment? description = entity.Entity.Description;
        entity.Entity.Description = null;
        entity.Entity.DescriptionId = null;
        await _data.SaveChangesAsync().ConfigureAwait(false);
        description = (await _data.Comments.AddAsync(description! with
        {
            TaskDescriptionId = entity.Entity.Id,
            AuthorId = member.Id,
            CreateTime = DateTime.UtcNow,
            LastEditTime = null,
            ParentId = null,
            RepoId = null,
            Children = new List<Comment>(0),
            TaskId = null,
            VotingId = null

        }).ConfigureAwait(false)).Entity;
        await _data.SaveChangesAsync().ConfigureAwait(false);
        entity.Entity.DescriptionId = description.Id;
        await _data.SaveChangesAsync().ConfigureAwait(false);
        _sendSender.NewTask(task, member);
        await _modelHub.Clients
            .Users(await _data.Members.AsAsyncEnumerable().Where(x => task.ReadRule.HasAccess(x)).Select(x => x.Id.ToString("D", CultureInfo.InvariantCulture)).ToArrayAsync().ConfigureAwait(false))
            .ReceiveModelUpdate(typeof(Task).FullName, task).ConfigureAwait(false);
        return Ok(task);
    }

    [HttpPost("{taskId:int}/comment")]
    [Authorize]
    public async Task<ActionResult<Comment>> PostComment(int taskId, Comment comment)
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        Task? task = await _data.Tasks.FindAsync(taskId).ConfigureAwait(false);
        if (task is null) return NotFound();
        if (!task.ReadRule?.HasAccess(member) ?? false) return Forbid();

        if (comment.ParentId is not null)
        {
            int id = comment.ParentId.Value;
            comment.Parent = null;
            if (task.Comments is not null) foreach (Comment votingComment in task.Comments)
            {
                comment.Parent = votingComment.FindComment(id);
                if (comment.Parent is not null) break;
            }

            if (comment.Parent is null) return BadRequest();
        }

        Comment result = comment with
        {
            Id = 0,
            Author = member,
            AuthorId = member.Id,
            CreateTime = DateTime.UtcNow,
            LastEditTime = null,
            Children = Array.Empty<Comment>(),
        };
        task.Comments ??= new List<Comment>(1);
        task.Comments.Add(result);

        await _data.SaveChangesAsync().ConfigureAwait(false);
        _sendSender.NewComment(task, member);
        await _modelHub.Clients
            .Users(_data.Members.Where(x => task.ReadRule.HasAccess(x)).Select(x => x.Id.ToString("D", CultureInfo.InvariantCulture)))
            .ReceiveModelUpdate(typeof(Comment).FullName, result).ConfigureAwait(false);
        await _modelHub.Clients
            .Users(_data.Members.Where(x => task.ReadRule.HasAccess(x)).Select(x => x.Id.ToString("D", CultureInfo.InvariantCulture)))
            .ReceiveModelUpdate(typeof(Task).FullName, task).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<ActionResult<Task>> Put(int id, Task newTask)
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        Task? oldTask = await _data.Tasks.FindAsync(id).ConfigureAwait(false);
        if (oldTask is null) return NotFound();
        if (!oldTask.WriteRule?.HasAccess(member) ?? true) return Forbid();
        if (oldTask.Id != newTask.Id) return BadRequest();

        oldTask.Title = newTask.Title;
        oldTask.StartTime = newTask.StartTime;
        oldTask.LastEditTime = DateTime.UtcNow;
        oldTask.EndTime = newTask.EndTime;
        oldTask.Award = newTask.Award;
        oldTask.AllowAssignByMember = newTask.AllowAssignByMember;
        oldTask.Status = newTask.Status;
        oldTask.Priority = newTask.Priority;

        if (newTask.Tags?.Any() ?? false)
        {
            Tag[] newTags = newTask.Tags.Except(_data.Tags, _tagComparer).ToArray();
            if (newTags.Any()) await _data.Tags.AddRangeAsync(newTags).ConfigureAwait(false);
            oldTask.Tags = newTask.Tags.Join(_data.Tags, x => x.Name, x => x.Name, (_, tag) => tag).ToArray();
        }

        if (newTask.Files?.Except(_data.StaticFiles).Any() ?? false) return BadRequest();

        AccessRule? readRule = await _data.AccessRules.FindAsync(newTask.ReadRuleId).ConfigureAwait(false);
        if (readRule is null) return BadRequest();
        oldTask.ReadRule = readRule;

        if (newTask.ReporterId == member.Id)
        {
            AccessRule? writeRule = await _data.AccessRules.FindAsync(newTask.WriteRuleId).ConfigureAwait(false);
            if (writeRule is null) return BadRequest();
            oldTask.WriteRule = writeRule;
        }

        if (newTask.AssigneeId is not null)
        {
            oldTask.Assignee = await _data.Members.FindAsync(newTask.AssigneeId.Value).ConfigureAwait(false);
            if (oldTask.Assignee is null) return BadRequest();
        }
        else
        {
            oldTask.Assignee = null;
        }

        if (newTask.SquadAssigneeId is not null)
        {
            oldTask.SquadAssignee =
                await _data.Squads.FindAsync(newTask.SquadAssigneeId.Value).ConfigureAwait(false);
            if (oldTask.SquadAssignee is null) return BadRequest();
        }
        else
        {
            oldTask.SquadAssignee = null;
        }

        if (newTask.DirectionAssignee <= Direction.None) oldTask.DirectionAssignee = null;

        if (newTask.ParentId is not null)
        {
            Task? parent = await _data.Tasks.FindAsync(newTask.ParentId).ConfigureAwait(false);
            if (parent is null) return BadRequest();
            oldTask.Parent = parent;
        }
        else
        {
            oldTask.Parent = null;
        }

        await _data.SaveChangesAsync().ConfigureAwait(false);
        if (oldTask.AssigneeId != newTask.AssigneeId) _sendSender.NewAssignee(oldTask);
        if (oldTask.Priority != newTask.Priority) _sendSender.NewPriority(oldTask, member);
        if (oldTask.Status != newTask.Status) _sendSender.NewStatus(oldTask, member);

        
        await _modelHub.Clients
            .Users(await _data.Members.AsAsyncEnumerable().Where(x => oldTask.ReadRule.HasAccess(x)).Select(x => x.Id.ToString("D", CultureInfo.InvariantCulture)).ToArrayAsync().ConfigureAwait(false))
            .ReceiveModelUpdate(typeof(Task).FullName, oldTask).ConfigureAwait(false);
        return Ok(oldTask);
    }

    [HttpPatch("{id:int}/status")]
    [Authorize]
    public async Task<ActionResult<Task>> PatchStatus(int id, TaskStatus status)
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        Task? task = await _data.Tasks.FindAsync(id).ConfigureAwait(false);
        if (task is null) return NotFound();
        if ((!task.WriteRule?.HasAccess(member) ?? false) && task.AssigneeId != member.Id && task.ReporterId != member.Id)
            return Forbid();
        if (task.Status == status) return Ok(task);
        task.Status = status;
        task.LastEditTime = DateTime.UtcNow;
        await _data.SaveChangesAsync().ConfigureAwait(false);
        _sendSender.NewStatus(task, member);
        await _modelHub.Clients
            .Users(_data.Members.Where(x => task.ReadRule.HasAccess(x)).Select(x => x.Id.ToString("D", CultureInfo.InvariantCulture)))
            .ReceiveModelUpdate(typeof(Task).FullName, task).ConfigureAwait(false);
        return Ok(task);
    }

    [HttpPatch("{id:int}/assign")]
    [Authorize]
    public async Task<ActionResult<Task>> PatchAssign(int id)
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        Task? task = await _data.Tasks.FindAsync(id).ConfigureAwait(false);
        if (task is null) return NotFound();
        if ((!task.WriteRule?.HasAccess(member) ?? false) && task.ReporterId != member.Id || !task.AllowAssignByMember)
            return Forbid();
        if (task.AssigneeId == member.Id) return Ok(task);
        task.Assignee = member;
        task.LastEditTime = DateTime.UtcNow;
        await _data.SaveChangesAsync().ConfigureAwait(false);
        _sendSender.NewAssignee(task);
        await _modelHub.Clients
            .Users(_data.Members.Where(x => task.ReadRule.HasAccess(x)).Select(x => x.Id.ToString("D", CultureInfo.InvariantCulture)))
            .ReceiveModelUpdate(typeof(Task).FullName, task).ConfigureAwait(false);
        return Ok(task);
    }


    [HttpPatch("{id:int}/watch/")]
    [Authorize]
    public async Task<ActionResult<Task>> PatchWatch(int id, [FromQuery] bool watching)
    {
        Member? member = await this.GetCurrentMember(_data).ConfigureAwait(false);
        if (member is null) return Unauthorized();

        Task? task = await _data.Tasks.FindAsync(id).ConfigureAwait(false);
        if (task is null) return NotFound();
        if ((!task.WriteRule?.HasAccess(member) ?? false) && task.ReporterId != member.Id || !task.AllowAssignByMember)
            return Forbid();
        if (task.AssigneeId == member.Id) return Ok(task);
        task.Assignee = member;
        task.LastEditTime = DateTime.UtcNow;
        await _data.SaveChangesAsync().ConfigureAwait(false);
        _sendSender.NewAssignee(task);
        await _modelHub.Clients
            .Users(_data.Members.Where(x => task.ReadRule.HasAccess(x)).Select(x => x.Id.ToString("D", CultureInfo.InvariantCulture)))
            .ReceiveModelUpdate(typeof(Task).FullName, task).ConfigureAwait(false);
        return Ok(task);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AndNetwork9.Server.Extensions;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Senders.Discord;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Extensions;
using AndNetwork9.Shared.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task = AndNetwork9.Shared.Task;
using TaskStatus = AndNetwork9.Shared.Enums.TaskStatus;

namespace AndNetwork9.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ClanDataContext _data;
        private readonly SendSender _sendSender;
        private readonly IEqualityComparer<Tag> _tagComparer = new GenericEqualityComparer<Tag, string>(x => x.Name);

        public TaskController(ClanDataContext data, SendSender sendSender)
        {
            _data = data;
            _sendSender = sendSender;
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async System.Threading.Tasks.Task<ActionResult<Task>> Get(int id)
        {
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return Unauthorized();

            Task? task = await _data.Tasks.FindAsync(id);
            if (task is null) return NotFound();
            if (!task.ReadRule.HasAccess(member) || task.AssigneeId != member.Id && task.ReporterId != member.Id)
                return Forbid();

            return Ok(task);
        }

        [HttpGet("me")]
        [Authorize]
        public async System.Threading.Tasks.Task<ActionResult<Task>> GetMe()
        {
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return Unauthorized();

            // ReSharper disable once MergeIntoPattern
            // ReSharper disable once MergeIntoLogicalPattern
            return Ok(_data.Tasks.Where(x =>
                    x.AssigneeId == member.Id && x.Status > TaskStatus.Inactive && x.Status < TaskStatus.Resolved
                    || x.ReporterId == member.Id
                    && (x.Status == TaskStatus.Resolved || x.Status == TaskStatus.Rejected))
                .OrderBy(x => x));
        }

        [HttpGet("squad")]
        [Authorize]
        public async System.Threading.Tasks.Task<ActionResult<Task>> GetSquad()
        {
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return Unauthorized();
            if (member.SquadNumber is null) return Forbid();

            // ReSharper disable once MergeIntoPattern
            // ReSharper disable once MergeIntoLogicalPattern
            return Ok(_data.Tasks.Where(x =>
                x.SquadAssigneeId == member.SquadNumber
                && x.Status > TaskStatus.Inactive
                && x.Status < TaskStatus.Resolved
                && x.ReadRule.HasAccess(member)).OrderBy(x => x));
        }

        [HttpGet("direction")]
        [Authorize]
        public async System.Threading.Tasks.Task<ActionResult<Task>> GetDirection()
        {
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return Unauthorized();

            // ReSharper disable once MergeIntoPattern
            // ReSharper disable once MergeIntoLogicalPattern
            return Ok(_data.Tasks.Where(x =>
                x.DirectionAssignee == member.Direction
                && x.Status > TaskStatus.Inactive
                && x.Status < TaskStatus.Resolved
                && x.ReadRule.HasAccess(member)).OrderBy(x => x));
        }

        [HttpGet("me/assignee")]
        [Authorize]
        public async System.Threading.Tasks.Task<ActionResult<Task>> GetMeAssignee()
        {
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return Unauthorized();

            return Ok(_data.Tasks.Where(x => x.AssigneeId == member.Id).OrderBy(x => x));
        }

        [HttpGet("me/reporter")]
        [Authorize]
        public async System.Threading.Tasks.Task<ActionResult<Task>> GetMeReporter()
        {
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return Unauthorized();

            return Ok(_data.Tasks.Where(x => x.ReporterId == member.Id).OrderBy(x => x));
        }

        [HttpGet("me/watcher")]
        [Authorize]
        public async System.Threading.Tasks.Task<ActionResult<Task>> GetMeWatcher()
        {
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return Unauthorized();

            return Ok(_data.Tasks.Where(x => x.WatchersId.Any(y => y == member.Id) && x.ReadRule.HasAccess(member))
                .OrderBy(x => x));
        }

        [HttpGet("unassigned")]
        [Authorize]
        public async System.Threading.Tasks.Task<ActionResult<Task>> GetUnassigned()
        {
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return Unauthorized();

            return Ok(_data.Tasks.Where(x =>
                x.AssigneeId == null
                && x.ReadRule.HasAccess(member)
                && (x.AllowAssignByMember || x.WriteRule.HasAccess(member))).OrderBy(x => x));
        }

        [HttpPost]
        [Authorize]
        public async System.Threading.Tasks.Task<ActionResult<Task>> Post(Task task)
        {
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return NotFound();

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

            if (task.Tags.Any())
            {
                Tag[] newTags = task.Tags.Except(_data.Tags, _tagComparer).ToArray();
                if (newTags.Any()) await _data.Tags.AddRangeAsync(newTags);
                task.Tags = task.Tags.Join(_data.Tags, x => x.Name, x => x.Name, (_, tag) => tag).ToArray();
            }

            if (task.Files.Except(_data.StaticFiles).Any()) return BadRequest();

            AccessRule? readRule = await _data.AccessRules.FindAsync(task.ReadRuleId);
            if (readRule is null) return BadRequest();
            task.ReadRule = readRule;

            AccessRule? writeRule = await _data.AccessRules.FindAsync(task.WriteRuleId);
            if (writeRule is null) return BadRequest();
            task.WriteRule = writeRule;

            if (task.AssigneeId is not null)
            {
                task.Assignee = await _data.Members.FindAsync(task.AssigneeId.Value);
                if (task.Assignee is null) return BadRequest();
            }
            else
            {
                task.Assignee = null;
            }

            if (task.SquadAssigneeId is not null)
            {
                task.SquadAssignee = await _data.Squads.FindAsync(task.SquadAssigneeId.Value);
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
                watchers.Add(_data.Members.FirstOrDefault(x =>
                    x.IsSquadCommander && x.SquadNumber == task.SquadAssigneeId));
            if (task.AssigneeId is not null) watchers.Add(await _data.Members.FindAsync(task.AssigneeId.Value));

            task.Watchers = watchers.Where(x => x is not null).ToArray()!;

            if (task.ParentId is not null)
            {
                Task? parent = await _data.Tasks.FindAsync(task.ParentId);
                if (parent is null) return BadRequest();
                task.Parent = parent;
            }
            else
            {
                task.Parent = null;
            }

            await _data.Tasks.AddAsync(task);
            await _data.SaveChangesAsync();

            _sendSender.NewTask(task, member);

            return Ok();
        }

        [HttpPost("{taskId:int}/comment")]
        [Authorize]
        public async Task<ActionResult<Comment>> PostComment(int taskId, Comment comment)
        {
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return Unauthorized();

            Task? task = await _data.Tasks.FindAsync(taskId);
            if (task is null) return NotFound();
            if (!task.ReadRule.HasAccess(member)) return Forbid();

            if (comment.ParentId is not null)
            {
                int id = comment.ParentId.Value;
                comment.Parent = null;
                foreach (var votingComment in task.Comments)
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
            task.Comments.Add(result);

            await _data.SaveChangesAsync();
            _sendSender.NewComment(task, member);

            return Ok(result);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<ActionResult<Task>> Put(int id, Task newTask)
        {
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return Unauthorized();

            Task? oldTask = await _data.Tasks.FindAsync(id);
            if (oldTask is null) return NotFound();
            if (!oldTask.WriteRule.HasAccess(member)) return Forbid();
            if (oldTask.Id != newTask.Id) return BadRequest();

            Task resultTask = oldTask with
            {
                Title = newTask.Title,
                StartTime = newTask.StartTime,
                LastEditTime = DateTime.UtcNow,
                EndTime = newTask.EndTime,
                Award = newTask.Award,
                AllowAssignByMember = newTask.AllowAssignByMember,
                Status = newTask.Status,
                Priority = newTask.Priority,
            };

            if (resultTask.Tags.Any())
            {
                Tag[] newTags = resultTask.Tags.Except(_data.Tags, _tagComparer).ToArray();
                if (newTags.Any()) await _data.Tags.AddRangeAsync(newTags);
                resultTask.Tags = resultTask.Tags.Join(_data.Tags, x => x.Name, x => x.Name, (_, tag) => tag).ToArray();
            }

            if (resultTask.Files.Except(_data.StaticFiles).Any()) return BadRequest();

            AccessRule? readRule = await _data.AccessRules.FindAsync(resultTask.ReadRuleId);
            if (readRule is null) return BadRequest();
            resultTask.ReadRule = readRule;

            if (resultTask.ReporterId == member.Id)
            {
                AccessRule? writeRule = await _data.AccessRules.FindAsync(resultTask.WriteRuleId);
                if (writeRule is null) return BadRequest();
                resultTask.WriteRule = writeRule;
            }

            if (resultTask.AssigneeId is not null)
            {
                resultTask.Assignee = await _data.Members.FindAsync(resultTask.AssigneeId.Value);
                if (resultTask.Assignee is null) return BadRequest();
            }
            else
            {
                resultTask.Assignee = null;
            }

            if (resultTask.SquadAssigneeId is not null)
            {
                resultTask.SquadAssignee = await _data.Squads.FindAsync(resultTask.SquadAssigneeId.Value);
                if (resultTask.SquadAssignee is null) return BadRequest();
            }
            else
            {
                resultTask.SquadAssignee = null;
            }

            if (resultTask.DirectionAssignee <= Direction.None) resultTask.DirectionAssignee = null;

            if (resultTask.ParentId is not null)
            {
                Task? parent = await _data.Tasks.FindAsync(resultTask.ParentId);
                if (parent is null) return BadRequest();
                resultTask.Parent = parent;
            }
            else
            {
                resultTask.Parent = null;
            }

            if (oldTask.AssigneeId != resultTask.AssigneeId) _sendSender.NewAssignee(resultTask);
            if (oldTask.Priority != resultTask.Priority) _sendSender.NewPriority(resultTask, member);
            if (oldTask.Status != resultTask.Status) _sendSender.NewStatus(resultTask, member);

            _data.Tasks.Update(resultTask);
            return Ok(resultTask);
        }

        [HttpPatch("{id:int}/status")]
        [Authorize]
        public async Task<ActionResult<Task>> PatchStatus(int id, TaskStatus status)
        {
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return Unauthorized();

            Task? task = await _data.Tasks.FindAsync(id);
            if (task is null) return NotFound();
            if (!task.WriteRule.HasAccess(member) && task.AssigneeId != member.Id && task.ReporterId != member.Id)
                return Forbid();
            if (task.Status == status) return Ok(task);
            task.Status = status;
            task.LastEditTime = DateTime.UtcNow;
            await _data.SaveChangesAsync();
            _sendSender.NewStatus(task, member);
            return Ok(task);
        }

        [HttpPatch("{id:int}/assign")]
        [Authorize]
        public async Task<ActionResult<Task>> PatchAssign(int id)
        {
            Member? member = await this.GetCurrentMember(_data);
            if (member is null) return Unauthorized();

            Task? task = await _data.Tasks.FindAsync(id);
            if (task is null) return NotFound();
            if (!task.WriteRule.HasAccess(member) && task.ReporterId != member.Id || !task.AllowAssignByMember)
                return Forbid();
            if (task.AssigneeId == member.Id) return Ok(task);
            task.Assignee = member;
            task.LastEditTime = DateTime.UtcNow;
            await _data.SaveChangesAsync();
            _sendSender.NewAssignee(task);
            return Ok(task);
        }
    }
}
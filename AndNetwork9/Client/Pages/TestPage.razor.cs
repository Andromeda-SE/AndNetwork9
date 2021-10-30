using System;
using AndNetwork9.Client.Services;
using AndNetwork9.Shared.Enums;
using Microsoft.AspNetCore.Components;
using Task = AndNetwork9.Shared.Task;

namespace AndNetwork9.Client.Pages
{
    public partial class TestPage
    {
        public TestPage()
        {
            ChildTask = new AndNetwork9.Shared.Task()
            {
                Level = TaskLevel.Subtask,
                Priority = TaskPriority.Medium,
                DirectionAssignee = Direction.Infrastructure,
                AllowAssignByMember = true,
                Assignee = AuthStateProvider.CurrentMember,
                AssigneeId = AuthStateProvider.CurrentMember.Id,
                Award = AwardType.Hero,
                Id = 954,
                Status = TaskStatus.Resolved,
                CreateTime = DateTime.UtcNow,
                Title = "Тестовая задача 2",
                Description = "frwgmwiogf,oqe,goqwm,goiqm,fgoqm,fio ewofk,oeqmf,qioe",
                Parent = Task,
                ParentId = Task.Id,
            };
        }
        [Parameter]
        public Task Task { get; set; } = new AndNetwork9.Shared.Task()
        {
            Level = TaskLevel.Task,
            Priority = TaskPriority.High,
            DirectionAssignee = Direction.Military,
            AllowAssignByMember = true,
            Assignee = AuthStateProvider.CurrentMember,
            AssigneeId = AuthStateProvider.CurrentMember.Id,
            Award = AwardType.Hero,
            Id = 947,
            Status = TaskStatus.InProgress,
            CreateTime = DateTime.UtcNow,
            Title = "Тестовая задача",
            Description = "frwgmwiogf,oqe,goqwm,goiqm,fgoqm,fio ewofk,oeqmf,qioe",
        };

        public Task ChildTask { get; set; }
    }
}

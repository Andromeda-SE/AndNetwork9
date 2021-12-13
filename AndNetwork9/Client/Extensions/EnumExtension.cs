using System;
using System.IO;
using AndNetwork9.Client.Shared;
using AndNetwork9.Client.Utility;
using AndNetwork9.Shared.Enums;

namespace AndNetwork9.Client.Extensions;

public static class EnumExtension
{
    public static string GetIconStyle(this SortType sortType, bool reverse)
    {
        return "bi-sort"
               + sortType switch
               {
                   SortType.Default => string.Empty,
                   SortType.Alphabet => "-alpha",
                   SortType.Numeric => "-numeric",
                   _ => throw new ArgumentOutOfRangeException(nameof(sortType), sortType, null),
               }
               + "-down"
               + (reverse ? "-alt" : string.Empty);
    }

    public static string GetColorStyle(this Direction direction)
    {
        return direction switch
        {
            Direction.Auxiliary => "#00FFFF",
            Direction.Reserve => "#dee2e6",
            Direction.None => "#dc3545",
            Direction.Training => "#ffc107",
            Direction.Infrastructure => "#fd7e14",
            Direction.Research => "#198754",
            Direction.Military => "#0d6efd",
            Direction.Agitation => "#6f42c1",

            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null),
        };
    }

    public static string GetColorStyle(this TaskStatus taskStatus)
    {
        return taskStatus switch
        {
            TaskStatus.Failed => "#dc3545",
            TaskStatus.Rejected => "#dc3545",
            TaskStatus.Canceled => "#dc3545",
            TaskStatus.New => "#ffc107",
            TaskStatus.Analysis => "#6f42c1",
            TaskStatus.ToDo => "#ffc107",
            TaskStatus.InProgress => "#0d6efd",
            TaskStatus.Postponed => "#fd7e14",
            TaskStatus.Resolved => "#198754",
            TaskStatus.Done => "#198754",
            _ => throw new ArgumentOutOfRangeException(nameof(taskStatus), taskStatus, null),
        };
    }

    public static string GetName(this RepoNodeEditor.VersionLevel level)
    {
        return level switch
        {
            RepoNodeEditor.VersionLevel.Version => "Версия",
            RepoNodeEditor.VersionLevel.Modification => "Модификация",
            RepoNodeEditor.VersionLevel.Prototype => "Прототип",
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, null),
        };
    }

    public static string GetName(this FastMemberSelector.FastEditType editType)
    {
        return editType switch
        {
            FastMemberSelector.FastEditType.One => "Единичный выбор",
            FastMemberSelector.FastEditType.Squad => "Отряд",
            FastMemberSelector.FastEditType.Direction => "Направление (общность игроков)",
            FastMemberSelector.FastEditType.VoteMembers => "Участники голосования",
            FastMemberSelector.FastEditType.VoteVoted => "Проголосовавшие участники голосования",
            FastMemberSelector.FastEditType.SquadCommanders => "Командующие отрядами",
            FastMemberSelector.FastEditType.Advisors => "Советники",
            FastMemberSelector.FastEditType.All => "Весь клан",
            FastMemberSelector.FastEditType.SquadPart => "Отделение",
            _ => throw new ArgumentOutOfRangeException(nameof(editType), editType, null),
        };
    }

    public static string GetIconLink(this TaskLevel taskLevel)
    {
        return Path.Combine("images/icons/taskLevel",
            taskLevel switch
            {
                TaskLevel.Unknown => "unknown.svg",
                TaskLevel.Subtask => "subtask.svg",
                TaskLevel.Task => "task.svg",
                TaskLevel.Quest => "quest.svg",
                TaskLevel.Story => "history.svg",
                TaskLevel.Epic => "epic.svg",
                TaskLevel.Initiative => "initiative.svg",
                _ => throw new ArgumentOutOfRangeException(nameof(taskLevel), taskLevel, null),
            });
    }

    public static string GetIconStyle(this TaskPriority taskPriority)
    {
        return taskPriority switch
        {
            TaskPriority.Trivial => "bi-arrow-down-circle",
            TaskPriority.Lowest => "bi-chevron-double-down",
            TaskPriority.Low => "bi-chevron-down",
            TaskPriority.Medium => "bi-dash-lg",
            TaskPriority.High => "bi-chevron-up",
            TaskPriority.Highest => "bi-chevron-double-up",
            TaskPriority.Vital => "bi-exclamation-square-fill",
            _ => throw new ArgumentOutOfRangeException(nameof(taskPriority), taskPriority, null),
        };
    }

    public static string GetIconColor(this TaskPriority taskPriority)
    {
        return taskPriority switch
        {
            TaskPriority.Trivial => "#dee2e6",
            TaskPriority.Lowest => "#6f42c1",
            TaskPriority.Low => "#0d6efd",
            TaskPriority.Medium => "#198754",
            TaskPriority.High => "#ffc107",
            TaskPriority.Highest => "#fd7e14",
            TaskPriority.Vital => "#dc3545",
            _ => throw new ArgumentOutOfRangeException(nameof(taskPriority), taskPriority, null),
        };
    }

    public static string GetIconColor(this TaskLevel taskLevel)
    {
        return taskLevel switch
        {
            TaskLevel.Unknown => "#dee2e6",
            TaskLevel.Subtask => "#6f42c1",
            TaskLevel.Task => "#0d6efd",
            TaskLevel.Quest => "#198754",
            TaskLevel.Story => "#ffc107",
            TaskLevel.Epic => "#fd7e14",
            TaskLevel.Initiative => "#dc3545",
            _ => throw new ArgumentOutOfRangeException(nameof(taskLevel), taskLevel, null),
        };
    }
}
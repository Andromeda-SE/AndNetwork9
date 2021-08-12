using System;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Storage;
using Markdig;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Extensions
{
    public static class ModelExtensions
    {
        public static MarkupString FromMarkdown(this string value)
        {
            return (MarkupString)Markdown.ToHtml(value);
        }

        public static string GetLink(this Member member)
        {
            return $"/member/{member.Id:D}";
        }
        

        public static MarkupString GetHtml(this Member member)
        {
            return (MarkupString)$"<a href=\"{member.GetLink()}\">{member}</a>";
        }
        public static string GetLink(this AndNetwork9.Shared.Task member)
        {
            return $"/task/{member.Id:D}";
        }

        public static string GetLink(this Squad squad)
        {
            return $"/squad/{squad.Number:D}";
        }

        public static string GetLink(this RepoNode node)
        {
            return $"api/repo/{node.RepoId}/node/{node.Version}/{node.Modification}/{node.Prototype}/file";
        }

        public static string ToLocalString(this AndNetwork9.Shared.Enums.TaskStatus status)
        {
            return status switch
            {
                AndNetwork9.Shared.Enums.TaskStatus.Failed => "Провалена",
                AndNetwork9.Shared.Enums.TaskStatus.Rejected => "Отклонена",
                AndNetwork9.Shared.Enums.TaskStatus.Canceled => "Отменена",
                AndNetwork9.Shared.Enums.TaskStatus.Inactive => "Неактивна",
                AndNetwork9.Shared.Enums.TaskStatus.ToDo => "Ожидает начала выполнения",
                AndNetwork9.Shared.Enums.TaskStatus.Postponed => "Отложена",
                AndNetwork9.Shared.Enums.TaskStatus.Analysis => "Анализ",
                AndNetwork9.Shared.Enums.TaskStatus.InProgress => "В процессе",
                AndNetwork9.Shared.Enums.TaskStatus.Resolved => "Выполнена",
                AndNetwork9.Shared.Enums.TaskStatus.Done => "Закрыта",
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, null),
            };
        }

        public static string ToLocalString(this TaskPriority priority)
        {
            return priority switch
            {
                TaskPriority.Lowest => "Очень низкий",
                TaskPriority.Low => "Низкий",
                TaskPriority.Medium => "Средний",
                TaskPriority.High => "Высокий",
                TaskPriority.Highest => "Очень высокий",
                TaskPriority.Vital => "Критический",
                _ => throw new ArgumentOutOfRangeException(nameof(priority), priority, null),
            };
        }
    }
}
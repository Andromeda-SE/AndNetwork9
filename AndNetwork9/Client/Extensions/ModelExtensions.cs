using System;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Enums;
using AndNetwork9.Shared.Storage;
using Markdig;
using Microsoft.AspNetCore.Components;
using Task = AndNetwork9.Shared.Task;
using TaskStatus = AndNetwork9.Shared.Enums.TaskStatus;

namespace AndNetwork9.Client.Extensions
{
    public static class ModelExtensions
    {
        public static MarkupString FromMarkdown(this string value) => (MarkupString)Markdown.ToHtml(value);

        public static string GetLink(this Member member) => $"/member/{member.Id:D}";


        public static MarkupString GetHtml(this Member member) =>
            (MarkupString)$"<a href=\"{member.GetLink()}\">{member}</a>";

        public static string GetLink(this Task member) => $"/task/{member.Id:D}";

        public static string GetLink(this Squad squad) => $"/squad/{squad.Number:D}";

        public static string GetLink(this RepoNode node) =>
            $"api/repo/{node.RepoId}/node/{node.Version}/{node.Modification}/{node.Prototype}/file";

        public static string ToLocalString(this TaskStatus status)
        {
            return status switch
            {
                TaskStatus.Failed => "Провалена",
                TaskStatus.Rejected => "Отклонена",
                TaskStatus.Canceled => "Отменена",
                TaskStatus.Inactive => "Неактивна",
                TaskStatus.ToDo => "Ожидает начала выполнения",
                TaskStatus.Postponed => "Отложена",
                TaskStatus.Analysis => "Анализ",
                TaskStatus.InProgress => "В процессе",
                TaskStatus.Resolved => "Выполнена",
                TaskStatus.Done => "Закрыта",
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
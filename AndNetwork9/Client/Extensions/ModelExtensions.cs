﻿using System;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Enums;
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

        public static string GetLink(this Task member)
        {
            return $"/task/{member.Id:D}";
        }

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
using System;
using System.Collections.Generic;
using System.Linq;
using AndNetwork9.Client.Extensions;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend.Senders.Discord;
using AndNetwork9.Shared.Extensions;
using Task = System.Threading.Tasks.Task;

namespace AndNetwork9.Server.Extensions
{
    public static class DiscordNotifications
    {
        public static void Send(this SendSender sender, IEnumerable<Member> recipients, string message)
        {
            Task.WaitAll(recipients.Select(watcher => Task.Run(async () => await sender.Send(watcher, message)))
                .ToArray());
        }

        public static async Task Send(this SendSender sender, Member recipient, string message)
        {
            await sender.CallAsync(new(recipient.DiscordId, message));
        }

        public static void NewAssignee(this SendSender sender, Shared.Task task)
        {
            string message = task.Assignee is not null
                ? $"Игрок {task.Assignee.GetDiscordMention()} назначен исполнителем задачи №{task.Id} {task.Title}{Environment.NewLine}{task.GetGlobalLink()}"
                : $"Задача №{task.Id} {task.Title} не имеет исполнителя{Environment.NewLine}{task.GetGlobalLink()}";
            sender.Send(task.GetAllWatchers(), message);
        }

        public static void NewTask(this SendSender sender, Shared.Task task, Member author)
        {
            sender.Send(task.GetAllWatchers(),
                $"Игроком {author.GetDiscordMention()} создана новая задача №{task.Id} {task.Title}{Environment.NewLine}{task.GetGlobalLink()}");
        }

        public static void NewStatus(this SendSender sender, Shared.Task task, Member author)
        {
            sender.Send(task.GetAllWatchers(),
                $"Игроком {author.GetDiscordMention()} статус задачи №{task.Id} {task.Title} изменен на «{task.Status.ToLocalString()}»{Environment.NewLine}{task.GetGlobalLink()}");
        }

        public static void NewPriority(this SendSender sender, Shared.Task task, Member author)
        {
            sender.Send(task.GetAllWatchers(),
                $"Игроком {author.GetDiscordMention()} приоритет задачи №{task.Id} {task.Title} изменен на «{task.Priority.ToLocalString()}»{Environment.NewLine}{task.GetGlobalLink()}");
        }

        public static void NewComment(this SendSender sender, Shared.Task task, Member author)
        {
            sender.Send(task.GetAllWatchers(),
                $"Игрок {author.GetDiscordMention()} оставил новый комментарий под задачей №{task.Id} {task.Title}{Environment.NewLine}{task.GetGlobalLink()}");
        }
    }
}
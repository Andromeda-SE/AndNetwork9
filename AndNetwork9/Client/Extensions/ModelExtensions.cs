using AndNetwork9.Shared;
using AndNetwork9.Shared.Storage;
using Markdig;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Extensions;

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

    public static string GetLink(this Task member)
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
}
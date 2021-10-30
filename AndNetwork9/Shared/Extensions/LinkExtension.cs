namespace AndNetwork9.Shared.Extensions;

public static class LinkExtension
{
    public static string SiteUrl { get; set; } = string.Empty;

    public static string GetGlobalLink(this Task task)
    {
        return $"https://{SiteUrl}/task/{task.Id:D}";
    }
}
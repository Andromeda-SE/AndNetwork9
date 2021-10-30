using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace AndNetwork9.Tests;

public class Config : IConfiguration
{
    public Dictionary<string, string> Parameters { get; set; } = new();

    public IEnumerable<IConfigurationSection> GetChildren() => default!;

    public IChangeToken GetReloadToken() => default!;

    public IConfigurationSection GetSection(string key) => default!;

    public string this[string key]
    {
        get => Parameters[key];
        set => Parameters[key] = value;
    }
}
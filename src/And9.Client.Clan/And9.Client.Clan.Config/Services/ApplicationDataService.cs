using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace And9.Client.Clan.Config.Services;

public class ApplicationDataService : DbContext, IConfiguration
{
    private readonly IChangeToken _token = new ConfigurationReloadToken();
    private readonly string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpaceEngineers", "AndConfig.sqlite");

    public ApplicationDataService() : base()
    {
        Database.Migrate();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={_filePath}");

    public DbSet<Tuple<string, string>> ConfigValues { get; set; } = null!;
    public DbSet<Tuple<int>> GaveAwards { get; set; } = null!;
    public IConfigurationSection GetSection(string key) => throw new NotSupportedException();

    public IEnumerable<IConfigurationSection> GetChildren() => throw new NotSupportedException();
    public IChangeToken GetReloadToken() => _token;

    public string this[string key]
    {
        get => ConfigValues.Find(key)?.Item2 ?? throw new KeyNotFoundException();
        set
        {
            Tuple<string, string>? item = ConfigValues.Find(key);
            if (item is null) throw new KeyNotFoundException();
            ConfigValues.Remove(item);
            ConfigValues.Add(new(key, value));
            SaveChanges();
            _token.RegisterChangeCallback(o => { }, this);
        }
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tuple<string, string>>(entity =>
        {
            entity.HasKey(x => x.Item1);
            entity.Property(x => x.Item2);
        });
        modelBuilder.Entity<Tuple<string, string>>().HasData(new List<Tuple<string, string>>()
        {
            new("CLAN_DOMAIN", "5.19.254.243:5240"),
            new("LOGIN", string.Empty),
            new("PASSWORD", string.Empty),
        });
        modelBuilder.Entity<Tuple<int>>(entity => { entity.HasKey(x => x.Item1); });
    }
}
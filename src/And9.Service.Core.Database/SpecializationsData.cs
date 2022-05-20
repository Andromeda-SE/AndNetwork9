using And9.Service.Core.Abstractions.Enums;
using And9.Service.Core.Abstractions.Models;
using And9.Service.Core.Database.Properties;
using Csv;

namespace And9.Service.Core.Database;

internal static class SpecializationsData
{
    private static readonly CsvOptions CsvOptions = new()
    {
        AllowBackSlashToEscapeQuote = true,
        HeaderMode = HeaderMode.HeaderAbsent,
        Separator = ';',
        TrimData = true,
        Comparer = StringComparer.Ordinal,
    };

    public static IEnumerable<Specialization> GetSpecializations() => CsvReader.ReadFromText(Resources.specializations, CsvOptions)
        .Select(line => new Specialization
        {
            Id = int.Parse(line[0]),
            Direction = Enum.Parse<Direction>(line[1]),
            Name = line[2],
            Description = line[3],
            LastChanged = DateTime.UtcNow,
            ConcurrencyToken = Guid.NewGuid(),
        });
}
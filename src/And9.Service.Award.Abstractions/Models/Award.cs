using System.Text.Json.Serialization;
using And9.Lib.Formatters;
using And9.Lib.Formatters.Json;
using And9.Lib.Formatters.MessagePack;
using And9.Service.Award.Abstractions.Enums;
using And9.Service.Award.Abstractions.Interfaces;
using MessagePack;

namespace And9.Service.Award.Abstractions.Models;

[MessagePackObject]
public class Award : IAward
{
    /*[IgnoreMember]
    [JsonIgnore]
    public double Points { get; set; } = Math.Round(
        (int)Type * Math.Pow(2, (Date.DayNumber - DateOnly.FromDateTime(DateTime.Today).DayNumber) / 365.25),
        10,
        MidpointRounding.ToPositiveInfinity);*/
    [Key(0)]
    public int Id { get; set; }
    [Key(1)]
    public int MemberId { get; set; }
    [Key(2)]
    public AwardType Type { get; set; }
    [Key(3)]
    public string? Description { get; set; }
    [Key(4)]
    [MessagePackFormatter(typeof(DateOnlyFormatter))]
    [JsonConverter(typeof(DateOnlyConverter))]
    public DateOnly Date { get; set; }
    [Key(5)]
    public int? GaveById { get; set; }
    [Key(6)]
    public int? AutomationTag { get; set; }
    [Key(7)]
    public DateTime LastChanged { get; set; }
    [Key(8)]
    public Guid ConcurrencyToken { get; set; }
}
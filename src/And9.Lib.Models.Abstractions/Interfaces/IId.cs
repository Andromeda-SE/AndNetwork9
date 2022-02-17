namespace And9.Lib.Models.Abstractions.Interfaces;

public interface IId : IConcurrencyToken
{
    int Id { get; }
}
namespace AndNetwork9.Shared.Interfaces;

public interface IId : IConcurrencyToken
{
    int Id { get; }
}
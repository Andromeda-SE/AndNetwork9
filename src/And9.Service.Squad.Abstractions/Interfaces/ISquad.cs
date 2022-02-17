using And9.Lib.Models.Abstractions.Interfaces;

namespace And9.Service.Squad.Abstractions.Interfaces;

public interface ISquad : IId
{
    public short Number { get; }
    public string? Name { get; }
    public DateOnly CreateDate { get; }
    public bool IsActive { get; }
    int IId.Id => Number;
}
using And9.Lib.Models.Abstractions.Interfaces;

namespace And9.Service.Core.Abstractions.Interfaces;

public interface ISquad : IId
{
    public short Number { get; }
    public List<string> Names { get; }
    public DateOnly CreateDate { get; }
    public bool IsActive { get; }
    int IId.Id => Number;
}
using And9.Lib.Models.Abstractions.Interfaces;
using And9.Service.Core.Abstractions.Enums;

namespace And9.Service.Core.Abstractions.Interfaces;

public interface ISpecialization : IId
{
    public Direction Direction { get; }
    public string Name { get; }
}
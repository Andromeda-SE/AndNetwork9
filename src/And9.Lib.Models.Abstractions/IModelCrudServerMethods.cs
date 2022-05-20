using System.Threading.Tasks;
using And9.Lib.Models.Abstractions.Interfaces;

namespace And9.Lib.Models.Abstractions;

public interface IModelCrudServerMethods<T> : IModelReadOnlyCrudServerMethods<T> where T : IId
{
    Task Create(T model);
    Task Delete(int id);
    Task Update(T model);
}
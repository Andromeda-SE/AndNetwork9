using System.Threading.Tasks;

namespace And9.Lib.Models.Abstractions;

public interface IModelCrudClientMethods
{
    Task ModelUpdated(int id, ModelState state);
}
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using And9.Lib.Models.Abstractions.Interfaces;

namespace And9.Lib.Models.Abstractions;

public interface IModelReadOnlyCrudServerMethods<T> where T : IId
{
    Task<T?> Read(int id);
    IAsyncEnumerable<T> ReadAll(CancellationToken cancellationToken);
}
using And9.Lib.Models.Abstractions.Interfaces;

namespace And9.Service.Core.Abstractions.Interfaces;

public interface ICandidateRegisteredRequest : ICandidateRequestBase, IId
{
    public int MemberId { get; set; }
    public bool? Accepted { get; set; }
}
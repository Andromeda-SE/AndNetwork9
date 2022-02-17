namespace And9.Service.Core.Abstractions.Interfaces;

public interface ICandidateRequestBase
{
    int? HoursCount { get; }
    int? Age { get; }
    string? Recommendation { get; }
    string? Description { get; }
    short? AuxiliarySquad { get; }
}
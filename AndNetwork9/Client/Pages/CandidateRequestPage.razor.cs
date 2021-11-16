using AndNetwork9.Shared.Utility;
using Microsoft.AspNetCore.Authorization;

namespace AndNetwork9.Client.Pages;

[AllowAnonymous]
public partial class CandidateRequestPage
{
    public CandidateRequest Model { get; set; }
}
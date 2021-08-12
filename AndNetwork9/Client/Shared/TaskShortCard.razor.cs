using AndNetwork9.Shared;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Shared
{
    public partial class TaskShortCard
    {
        [Parameter]
        public AndNetwork9.Shared.Task Task { get; set; }
    }
}
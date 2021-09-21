using AndNetwork9.Shared;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Shared
{
    public partial class TaskShortCard
    {
        [Parameter]
        public Task Task { get; set; }
    }
}
using System.Collections.Generic;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Enums;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Pages.Management
{
    public partial class AddAward
    {
        [Parameter]
        public AwardType Type { get; set; }
        [Parameter]
        public string Description { get; set; }
        [Parameter]
        public List<Member> SelectedMembers { get; set; }
    }
}
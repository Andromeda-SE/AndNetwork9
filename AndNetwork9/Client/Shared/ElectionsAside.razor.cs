using System;
using System.Globalization;
using System.Linq;
using AndNetwork9.Shared.Elections;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Shared
{
    public partial class ElectionsAside
    {
        [Parameter]
        public CouncilElection Election { get; set; }

        protected override void OnInitialized()
        {
            Console.WriteLine(string.Join(Environment.NewLine,
                CultureInfo.GetCultures(CultureTypes.AllCultures).Select(x => x.ToString())));
        }
    }
}
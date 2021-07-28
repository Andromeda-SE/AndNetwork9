using System;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Shared
{
    public partial class MarkdownEditor
    {
        [Parameter]
        public bool PreviewEnabled { get; set; }
        [Parameter]
        public Action<string> SetterFunc { get; set; }
        [Parameter]
        public Func<string> GetterFunc { get; set; }
        [Parameter]
        public Action ResetFunc { get; set; }
        [Parameter]
        public Action<string> SaveFunc { get; set; }

        public string Text
        {
            get => GetterFunc();
            set => SetterFunc(value);
        }
    }
}
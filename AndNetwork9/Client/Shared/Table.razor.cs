using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AndNetwork9.Client.Utility;
using AndNetwork9.Shared.Interfaces;
using Microsoft.AspNetCore.Components;

namespace AndNetwork9.Client.Shared
{
    public class Table<TItem> : ComponentBase where TItem : IId
    {
        private int? _selectedColumn;

        private int? _selectedRow;

        public int SelectedColumn
        {
            get => _selectedColumn ?? -1;
            set
            {
                _selectedColumn = value;
                View.SortSelector = value >= 0 ? Columns[value].SortFunc : null;
            }
        }

        [Parameter]
        public IReadOnlyCollection<TItem> Source { get; set; }
        [Parameter]
        public ColumnDefinition[] Columns { get; set; }
        [Parameter]
        public Action<int?> SelectionChanged { get; set; }
        [Parameter]
        public (int ColumnIndex, bool Descending) InitialSort { get; set; }

        public int? SelectedRow
        {
            get => _selectedRow;
            set
            {
                _selectedRow = value;
                SelectionUpdated?.Invoke(value);
            }
        }

        public CollectionView<TItem> View { get; private set; }
        public event Action<int?> SelectionUpdated;

        protected override Task OnInitializedAsync()
        {
            View = new(Source);
            if (SelectionChanged is not null) SelectionUpdated += SelectionChanged;
            SelectedColumn = InitialSort.ColumnIndex; //Not a error: trig SelectedColumn setter
            if (InitialSort.Descending) SelectedColumn = InitialSort.ColumnIndex;
            return base.OnInitializedAsync();
        }
    }

    public record ColumnDefinition(string Name, Func<dynamic, IComparable> SortFunc, Func<dynamic, string> ContentFunc,
        Func<dynamic, bool> HeaderFunc, (SortType Type, bool Reverse) SortType);
}
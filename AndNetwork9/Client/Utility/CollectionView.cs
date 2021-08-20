using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AndNetwork9.Client.Utility
{
    public class CollectionView<T> : IEnumerable<T>
    {
        private Func<T, bool> _filterPredicate = _ => true;
        private Func<dynamic, IComparable> _sortSelector;

        public CollectionView(IReadOnlyCollection<T> source) => Source = source;

        public IReadOnlyCollection<T> Source { get; set; }

        public Func<dynamic, IComparable> SortSelector
        {
            get => _sortSelector;
            set
            {
                Descending = SortSelector == value && !Descending;
                _sortSelector = value;
                Updated?.Invoke();
            }
        }

        public Func<T, bool> FilterPredicate
        {
            get => _filterPredicate;
            set
            {
                _filterPredicate = value;
                Updated?.Invoke();
            }
        }

        public bool Descending { get; set; }

        public IEnumerator<T> GetEnumerator()
        {
            IEnumerable<T> result = Source;
            if (FilterPredicate is not null) result = result.Where(FilterPredicate);
            if (SortSelector is not null)
                result = Descending
                    ? result.OrderByDescending(x => SortSelector(x))
                    : result.OrderBy(x => SortSelector(x));
            return result.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public event Action Updated;
    }
}
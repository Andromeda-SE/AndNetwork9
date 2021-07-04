using System;
using System.Collections.Generic;

namespace AndNetwork9.Shared.Extensions
{
    public class GenericEqualityComparer<T, TArg> : IEqualityComparer<T>
    {
        private readonly Func<T, IEquatable<TArg>> _delegate;

        public GenericEqualityComparer(Func<T, IEquatable<TArg>> @delegate)
        {
            _delegate = @delegate;
        }


        public bool Equals(T? x, T? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            return _delegate(x).Equals(y);
        }

        public int GetHashCode(T obj)
        {
            return _delegate(obj).GetHashCode();
        }
    }
}
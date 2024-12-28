using System;
using System.Collections.Generic;

namespace UI.Li.Internal
{
    public class WeakReferenceTargetComparer<T> : IEqualityComparer<WeakReference<T>> where T : class
    {
        public bool Equals(WeakReference<T> x, WeakReference<T> y)
        {
            if (x == null || y == null)
                return false;

            bool aExists = x.TryGetTarget(out var a);
            bool bExists = y.TryGetTarget(out var b);

            if (!aExists || !bExists)
                return aExists == bExists;

            return EqualityComparer<T>.Default.Equals(a, b);
        }

        public int GetHashCode(WeakReference<T> obj) =>
            !obj.TryGetTarget(out var target) ? 0 : EqualityComparer<T>.Default.GetHashCode(target);
    }
}
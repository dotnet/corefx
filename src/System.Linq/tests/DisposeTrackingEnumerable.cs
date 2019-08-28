using System.Collections.Generic;
using System.Collections;

namespace System.Linq.Tests
{
    internal sealed class DisposeTrackingEnumerable<T> : IEnumerable<T>
    {
        public bool EnumeratorDisposed { get; private set; }


        public IEnumerator<T> GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

        private sealed class Enumerator : IEnumerator<T>
        {
            private readonly DisposeTrackingEnumerable<T> _enumerable;
            public Enumerator(DisposeTrackingEnumerable<T> enumerable) => _enumerable = enumerable;
            public void Dispose() => _enumerable.EnumeratorDisposed = true;
            public T Current => default;
            object IEnumerator.Current => default;
            public bool MoveNext() => false;
            public void Reset() { }
        }
    }
}

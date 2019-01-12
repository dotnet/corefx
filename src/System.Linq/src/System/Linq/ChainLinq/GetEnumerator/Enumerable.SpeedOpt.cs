using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq.ChainLinq.GetEnumerator
{
    static partial class Enumerable
    {
        static partial void Optimized<T, U>(IEnumerable<T> enumerable, Link<T, U> link, ref IEnumerator<U> enumerator)
        {
            switch (link.LinkType)
            {
                case Links.LinkType.Select:
                    enumerator = new SelectEnumerator<T, U>(enumerable, ((Links.Select<T, U>)link).Selector);
                    break;
            }
        }

        class SelectEnumerator<T, U> : EnumeratorBase<U>
        {
            private Func<T, U> _selector;

            private IEnumerable<T> _enumerable;
            private IEnumerator<T> _enumerator;

            public override void Dispose()
            {
                if (_enumerator != null)
                {
                    _enumerator.Dispose();
                }
            }

            public SelectEnumerator(IEnumerable<T> enumerable, Func<T, U> selector) =>
                (_enumerable, _selector) = (enumerable, selector);

            public override bool MoveNext()
            {
                if (_enumerator == null)
                {
                    _enumerator = _enumerable.GetEnumerator();
                }

                if (_enumerator.MoveNext())
                {
                    Current = _selector(_enumerator.Current);
                    return true;
                }

                Current = default;
                return false;
            }
        }
    }
}

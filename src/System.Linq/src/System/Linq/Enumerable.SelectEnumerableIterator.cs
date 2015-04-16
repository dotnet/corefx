
using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        internal class SelectEnumerableIterator<TSource, TResult> : SelectIterator<TResult>
        {
            private IEnumerable<TSource> _source;
            private Func<TSource, TResult> _selector;
            private IEnumerator<TSource> _enumerator;

            public SelectEnumerableIterator(IEnumerable<TSource> source, Func<TSource, TResult> selector)
            {
                _source = source;
                _selector = selector;
            }

            public override Iterator<TResult> Clone()
            {
                return new SelectEnumerableIterator<TSource, TResult>(_source, _selector);
            }

            public override void Dispose()
            {
                if (_enumerator is IDisposable) ((IDisposable)_enumerator).Dispose();
                _enumerator = null;
                base.Dispose();
            }

            public override bool MoveNext()
            {
                switch (state)
                {
                    case 1:
                        _enumerator = _source.GetEnumerator();
                        state = 2;
                        goto case 2;
                    case 2:
                        while (_enumerator.MoveNext())
                        {
                            TSource item = _enumerator.Current;
                            current = _selector(item);
                            return true;
                        }
                        Dispose();
                        break;
                }
                return false;
            }

            public override IEnumerable<TResult> Where(Func<TResult, bool> predicate)
            {
                return new WhereEnumerableIterator<TResult>(this, predicate);
            }

            public override IEnumerable<TResult2> SelectImpl<TResult2>(Func<TResult, TResult2> selector)
            {
                return new SelectEnumerableIterator<TSource, TResult2>(_source, CombineSelectors(_selector, selector));
            }
        }
    }
}
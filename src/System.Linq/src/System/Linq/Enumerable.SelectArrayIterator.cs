
using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        internal class SelectArrayIterator<TSource, TResult> : SelectWithCountIterator<TResult>
        {
            private TSource[] _source;
            private Func<TSource, TResult> _selector;
            private int _index;

            public SelectArrayIterator(TSource[] source, Func<TSource, TResult> selector)
            {
                _source = source;
                _selector = selector;
            }

            public override int Count { get { return _source.Length; } }

            public override Iterator<TResult> Clone()
            {
                return new SelectArrayIterator<TSource, TResult>(_source, _selector);
            }

            public override bool MoveNext()
            {
                if (state == 1)
                {
                    while (_index < _source.Length)
                    {
                        TSource item = _source[_index];
                        _index++;
                        current = _selector(item);
                        return true;
                    }
                    Dispose();
                }
                return false;
            }

            public override IEnumerable<TResult> Where(Func<TResult, bool> predicate)
            {
                return new WhereEnumerableIterator<TResult>(this, predicate);
            }

            public override IEnumerable<TResult2> SelectImpl<TResult2>(Func<TResult, TResult2> selector)
            {
                return new SelectArrayIterator<TSource, TResult2>(_source, CombineSelectors(_selector, selector));
            }
        }
    }
}
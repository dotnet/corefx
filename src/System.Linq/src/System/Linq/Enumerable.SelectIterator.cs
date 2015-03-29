
using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        internal abstract class SelectIterator<TSource> : Iterator<TSource>
        {
            public override abstract Iterator<TSource> Clone();

            public override abstract bool MoveNext();

            public override abstract IEnumerable<TSource> Where(Func<TSource, bool> predicate);

            public abstract IEnumerable<TResult> SelectImpl<TResult>(Func<TSource, TResult> selector);
        }
    }
}

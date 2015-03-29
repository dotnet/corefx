
using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        internal abstract class SelectWithCountIterator<TSource> : SelectIterator<TSource>, ICollection<TSource>
        {
            public override abstract Iterator<TSource> Clone();

            public override abstract bool MoveNext();

            public override abstract IEnumerable<TSource> Where(Func<TSource, bool> predicate);

            public override abstract IEnumerable<TResult> SelectImpl<TResult>(Func<TSource, TResult> selector);

            #region ICollection<TSource> implementation

            public abstract int Count { get; }

            public bool IsReadOnly { get { return true; } }

            public void Add(TSource item)
            {
                throw NotImplemented.ByDesign;
            }

            public void Clear()
            {
                throw NotImplemented.ByDesign;
            }

            public bool Contains(TSource item)
            {
                throw NotImplemented.ByDesign;
            }

            public void CopyTo(TSource[] array, int arrayIndex)
            {
                IEnumerator<TSource> enumerator = GetEnumerator();
                int index = 0;
				for(; index < array.Length && enumerator.MoveNext(); index++)
                {
                    array[index] = enumerator.Current;
                }

				// verify that we copied expected number of items.
				// if not, it means that underlying collection was modified while iterating.
                if (index != array.Length)
                    throw new InvalidOperationException();
            }

            public bool Remove(TSource item)
            {
                throw NotImplemented.ByDesign;
            }

            #endregion
        }
    }
}

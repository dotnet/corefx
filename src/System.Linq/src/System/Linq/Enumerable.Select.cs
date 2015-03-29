using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (source == null) throw Error.ArgumentNull("source");
            if (selector == null) throw Error.ArgumentNull("selector");
            if (source is Iterator<TSource>) return ((Iterator<TSource>)source).Select(selector);
            if (source is TSource[]) return new SelectArrayIterator<TSource, TResult>((TSource[])source, selector);
            if (source is List<TSource>) return new SelectListIterator<TSource, TResult>((List<TSource>)source, selector);
            if (source is ICollection<TSource>) return new SelectCollectionIterator<TSource, TResult>((ICollection<TSource>)source, selector);
            return new SelectEnumerableIterator<TSource, TResult>(source, selector);
        }

        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
        {
            if (source == null) throw Error.ArgumentNull("source");
            if (selector == null) throw Error.ArgumentNull("selector");
            return SelectIteratorImpl<TSource, TResult>(source, selector);
        }

        private static IEnumerable<TResult> SelectIteratorImpl<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
        {
            int index = -1;
            foreach (TSource element in source)
            {
                checked { index++; }
                yield return selector(element, index);
            }
        }
    }
}

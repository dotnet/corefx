using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consume
{
    static class Lookup
    {
        public static Result Invoke<TKey, TElement, V, Result>(Grouping<TKey, TElement> lastGrouping, ILink<IGrouping<TKey, TElement>, V> composition, Consumer<V, Result> consumer)
        {
            var chain = composition.Compose(consumer);
            try
            {
                Pipeline(lastGrouping, chain);
                chain.ChainComplete();
            }
            finally
            {
                chain.ChainDispose();
            }
            return consumer.Result;
        }

        public static Result Invoke<TKey, TElement, TResult, V, Result>(Grouping<TKey, TElement> lastGrouping, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, ILink<TResult, V> composition, Consumer<V, Result> consumer)
        {
            var chain = composition.Compose(consumer);
            try
            {
                Pipeline(lastGrouping, resultSelector, chain);
                chain.ChainComplete();
            }
            finally
            {
                chain.ChainDispose();
            }
            return consumer.Result;
        }

        private static void Pipeline<TKey, TElement>(Grouping<TKey, TElement> lastGrouping, Chain<IGrouping<TKey, TElement>> chain)
        {
            Grouping<TKey, TElement> g = lastGrouping;
            if (g != null)
            {
                do
                {
                    g = g._next;
                    var state = chain.ProcessNext(g);
                    if (state.IsStopped())
                        break;
                }
                while (g != lastGrouping);
            }
        }

        private static void Pipeline<TKey, TElement, TResult>(Grouping<TKey, TElement> lastGrouping, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, Chain<TResult> chain)
        {
            Grouping<TKey, TElement> g = lastGrouping;
            if (g != null)
            {
                do
                {
                    g = g._next;
                    g.Trim();
                    var state = chain.ProcessNext(resultSelector(g.Key, g._elements));
                    if (state.IsStopped())
                        break;
                }
                while (g != lastGrouping);
            }
        }
    }
}

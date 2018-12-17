using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consume
{
    static class Concat
    {
        public static Result Invoke<T, V, Result>(IEnumerable<T> first, IEnumerable<T> second, ILink<T, V> composition, Consumer<V, Result> consumer)
        {
            var chain = composition.Compose(consumer);
            try
            {
                Pipeline(first, second, chain);
                chain.ChainComplete();
            }
            finally
            {
                chain.ChainDispose();
            }
            return consumer.Result;
        }

        private static void Pipeline<T>(IEnumerable<T> first, IEnumerable<T> second, Chain<T> chain)
        {
            foreach (var item in first)
            {
                var state = chain.ProcessNext(item);
                if (state.IsStopped())
                    return;
            }
            foreach (var item in second)
            {
                var state = chain.ProcessNext(item);
                if (state.IsStopped())
                    break;
            }
        }

    }
}

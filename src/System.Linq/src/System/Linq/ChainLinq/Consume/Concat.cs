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
            UnknownEnumerable.ChainConsumer<T> inner = null;

            var status = UnknownEnumerable.Consume(first, chain, ref inner);
            if (status.IsStopped())
                return;

            UnknownEnumerable.Consume(second, chain, ref inner);
        }

    }
}

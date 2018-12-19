using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consume
{
    static class Concat
    {
        public static Result Invoke<T, V, Result>(IEnumerable<T> firstOrNull, IEnumerable<T> second, IEnumerable<T> thirdOrNull, ILink<T, V> composition, Consumer<V, Result> consumer)
        {
            var chain = composition.Compose(consumer);
            try
            {
                Pipeline(firstOrNull, second, thirdOrNull, chain);
                chain.ChainComplete();
            }
            finally
            {
                chain.ChainDispose();
            }
            return consumer.Result;
        }

        private static void Pipeline<T>(IEnumerable<T> firstOrNull, IEnumerable<T> second, IEnumerable<T> thirdOrNull, Chain<T> chain)
        {
            UnknownEnumerable.ChainConsumer<T> inner = null;
            ChainStatus status;

            if (firstOrNull != null)
            {
                status = UnknownEnumerable.Consume(firstOrNull, chain, ref inner);
                if (status.IsStopped())
                    return;
            }

            status = UnknownEnumerable.Consume(second, chain, ref inner);
            if (status.IsStopped())
                return;

            if (thirdOrNull != null)
            {
                UnknownEnumerable.Consume(thirdOrNull, chain, ref inner);
            }
        }

    }
}

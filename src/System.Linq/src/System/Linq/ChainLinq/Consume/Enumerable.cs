using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consume
{
    static class Enumerable
    {
        public static void Invoke<T, V>(IEnumerable<T> e, Link<T, V> composition, Chain<V> consumer)
        {
            var chain = composition.Compose(consumer);
            try
            {
                if (chain is Optimizations.IPipelineEnumerable<T> optimized)
                {
                    optimized.Pipeline(e);
                }
                else
                {
                    Pipeline(e, chain);
                }
                chain.ChainComplete();
            }
            finally
            {
                chain.ChainDispose();
            }
        }

        private static void Pipeline<T>(IEnumerable<T> e, Chain<T> chain)
        {
            foreach (var item in e)
            {
                var state = chain.ProcessNext(item);
                if (state.IsStopped())
                    break;
            }
        }

    }
}

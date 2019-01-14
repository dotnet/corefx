using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consume
{
    static class List
    {
        public static void Invoke<T, V>(List<T> list, Link<T, V> composition, Chain<V> consumer)
        {
            var chain = composition.Compose(consumer);
            try
            {
                if (chain is Optimizations.IPipelineList<T> optimized)
                {
                    optimized.Pipeline(list);
                }
                else
                {
                    Pipeline(list, chain);
                }
                chain.ChainComplete();
            }
            finally
            {
                chain.ChainDispose();
            }
        }

        private static void Pipeline<T>(List<T> list, Chain<T> chain)
        {
            foreach (var item in list)
            {
                var state = chain.ProcessNext(item);
                if (state.IsStopped())
                    break;
            }
        }

    }
}

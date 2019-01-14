using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consume
{
    static class IList
    {
        public static void Invoke<T, V>(IList<T> array, int start, int count, Link<T, V> composition, Chain<V> consumer)
        {
            var chain = composition.Compose(consumer);
            try
            {
                if (chain is Optimizations.IPipelineIList<T> optimized)
                {
                    optimized.Pipeline(array, start, count);
                }
                else
                {
                    Pipeline(array, start, count, chain);
                }
                chain.ChainComplete();
            }
            finally
            {
                chain.ChainDispose();
            }
        }

        private static void Pipeline<T>(IList<T> list, int start, int count, Chain<T> chain)
        {
            int completeIdx;
            checked { completeIdx = start + count; }
            for (var idx = start; idx < completeIdx; ++idx)
            {
                var state = chain.ProcessNext(list[idx]);
                if (state.IsStopped())
                    break;
            }
        }
    }
}

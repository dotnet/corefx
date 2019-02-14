namespace System.Linq.ChainLinq.Consume
{
    static class ReadOnlyMemory
    {
        public static void Invoke<T, V>(ReadOnlyMemory<T> array, Link<T, V> composition, Chain<V> consumer)
        {
            var chain = composition.Compose(consumer);
            try
            {
                if (chain is Optimizations.IPipeline<ReadOnlyMemory<T>> optimized)
                {
                    optimized.Pipeline(array);
                }
                else
                {
                    Pipeline(array, chain);
                }
                chain.ChainComplete();
            }
            finally
            {
                chain.ChainDispose();
            }
        }

        private static void Pipeline<T>(ReadOnlyMemory<T> memory, Chain<T> chain)
        {
            foreach (var item in memory.Span)
            {
                var state = chain.ProcessNext(item);
                if (state.IsStopped())
                    break;
            }
        }

    }
}

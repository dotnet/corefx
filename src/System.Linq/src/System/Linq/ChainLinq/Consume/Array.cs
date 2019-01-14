namespace System.Linq.ChainLinq.Consume
{
    static class Array
    {
        public static void Invoke<T, V>(T[] array, Link<T, V> composition, Chain<V> consumer)
        {
            var chain = composition.Compose(consumer);
            try
            {
                if (chain is Optimizations.IPipelineArray<T> optimized)
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

        private static void Pipeline<T>(T[] array, Chain<T> chain)
        {
            foreach (var item in array)
            {
                var state = chain.ProcessNext(item);
                if (state.IsStopped())
                    break;
            }
        }

    }
}

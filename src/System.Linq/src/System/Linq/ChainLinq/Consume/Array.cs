namespace System.Linq.ChainLinq.Consume
{
    static class Array
    {
        public static Result Invoke<T, V, Result>(T[] array, ILink<T, V> composition, Consumer<V, Result> consumer)
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
            return consumer.Result;
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

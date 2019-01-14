namespace System.Linq.ChainLinq.Consume
{
    static class Range
    {
        public static void Invoke<V>(int start, int count, Link<int, V> composition, Chain<V> consumer)
        {
            var chain = composition.Compose(consumer);
            try
            {
                Pipeline(start, count, chain);
                chain.ChainComplete();
            }
            finally
            {
                chain.ChainDispose();
            }
        }

        private static void Pipeline(int start, int count, Chain<int> chain)
        {
            var current = unchecked(start - 1);
            var end = unchecked(start + count);
            while (unchecked(++current) != end)
            {
                var state = chain.ProcessNext(current);
                if (state.IsStopped())
                    break;
            }
        }

    }
}

namespace System.Linq.ChainLinq.Consume
{
    static class Repeat
    {
        public static void Invoke<T, V>(T element, int count, Link<T, V> composition, Chain<V> consumer)
        {
            var chain = composition.Compose(consumer);
            try
            {
                Pipeline(element, count, chain);
                chain.ChainComplete();
            }
            finally
            {
                chain.ChainDispose();
            }
        }

        private static void Pipeline<T>(T element, int count, Chain<T> chain)
        {
            for(var i=0; i < count; ++i)
            {
                var state = chain.ProcessNext(element);
                if (state.IsStopped())
                    break;
            }
        }

    }
}

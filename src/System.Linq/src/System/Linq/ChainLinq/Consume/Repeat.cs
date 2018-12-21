namespace System.Linq.ChainLinq.Consume
{
    static class Repeat
    {
        public static Result Invoke<T, V, Result>(T element, int count, ILink<T, V> composition, Consumer<V, Result> consumer)
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
            return consumer.Result;
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

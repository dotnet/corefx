using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consume
{
    static class List
    {
        public static Result Invoke<T, V, Result>(List<T> list, ILink<T, V> composition, Consumer<V, Result> consumer)
        {
            var chain = composition.Compose(consumer);
            try
            {
                Pipeline(list, chain);
                chain.ChainComplete();
            }
            finally
            {
                chain.ChainDispose();
            }
            return consumer.Result;
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

using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consume
{
    class Enumerable
    {
        public static Result Invoke<T, V, Result>(IEnumerable<T> e, ILink<T, V> composition, Consumer<V, Result> consumer)
        {
            Pipeline(e, composition.Compose(consumer));
            return consumer.Result;
        }

        private static void Pipeline<T>(IEnumerable<T> e, Chain<T> chain)
        {
            try
            {
                foreach (var item in e)
                {
                    var state = chain.ProcessNext(item);
                    if (state.IsStopped())
                        break;
                }
                chain.ChainComplete();
            }
            finally
            {
                chain.ChainDispose();
            }
        }

    }
}

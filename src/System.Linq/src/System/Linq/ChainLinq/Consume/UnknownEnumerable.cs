using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consume
{
    static class UnknownEnumerable
    {
        public class ChainConsumer<T> : Consumer<T, ChainStatus>
        {
            private readonly Chain<T> _chainT;

            public ChainConsumer(Chain<T> chainT) : base(ChainStatus.Flow) =>
                _chainT = chainT;

            public override ChainStatus ProcessNext(T input)
            {
                var state = _chainT.ProcessNext(input);
                Result = state;
                return state;
            }
        }

        private static ChainConsumer<T> GetInnerConsumer<T>(Chain<T> chain, ref ChainConsumer<T> consumer)
        {
            if (consumer == null)
            {
                consumer = new ChainConsumer<T>(chain);
                return consumer;
            }
            return consumer;
        }

        public static ChainStatus Consume<T>(IEnumerable<T> input, Chain<T> chain, ref ChainConsumer<T> consumer)
        {
            switch (input)
            {
                case Consumable<T> consumable:
                    return consumable.Consume(GetInnerConsumer(chain, ref consumer));

                case T[] array:
                    return ConsumerArray(chain, array);

                default:
                    return ConsumerEnumerable(input, chain);
            }
        }

        private static ChainStatus ConsumerEnumerable<T>(IEnumerable<T> input, Chain<T> chain)
        {
            var status = ChainStatus.Flow;
            foreach (var item in input)
            {
                status = chain.ProcessNext(item);
                if (status.IsStopped())
                    break;
            }
            return status;
        }

        private static ChainStatus ConsumerArray<T>(Chain<T> chain, T[] array)
        {
            var status = ChainStatus.Flow;
            foreach (var item in array)
            {
                status = chain.ProcessNext(item);
                if (status.IsStopped())
                    break;
            }
            return status;
        }
    }
}

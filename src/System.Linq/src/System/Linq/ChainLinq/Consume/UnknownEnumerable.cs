using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consume
{
    static class UnknownEnumerable
    {
        public sealed class ChainConsumer<T> : Consumer<T, ChainStatus>
        {
            private readonly Chain<T> _chainT;

            public ChainConsumer(Chain<T> chainT) : base(ChainStatus.Flow) =>
                _chainT = chainT;

            public override ChainStatus ProcessNext(T input)
            {
                var status = _chainT.ProcessNext(input);
                Result = status;
                return status;
            }
        }

        private static ChainConsumer<T> GetInnerConsumer<T>(Chain<T> chain, ref ChainConsumer<T> consumer) =>
            consumer ?? (consumer = new ChainConsumer<T>(chain));

        public static ChainStatus Consume<T>(IEnumerable<T> input, Chain<T> chain, ref ChainConsumer<T> consumer)
        {
            switch (input)
            {
                case Consumable<T> consumable:
                    return consumable.Consume(GetInnerConsumer(chain, ref consumer));

                case T[] array:
                    return ConsumerArray(array, chain);

                case List<T> list:
                    return ConsumerList(list, chain);

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

        private static ChainStatus ConsumerArray<T>(T[] array, Chain<T> chain)
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

        private static ChainStatus ConsumerList<T>(List<T> list, Chain<T> chain)
        {
            var status = ChainStatus.Flow;
            foreach (var item in list)
            {
                status = chain.ProcessNext(item);
                if (status.IsStopped())
                    break;
            }
            return status;
        }
    }
}

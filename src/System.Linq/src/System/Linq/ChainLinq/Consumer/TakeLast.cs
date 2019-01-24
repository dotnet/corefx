using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumer
{
    sealed class TakeLast<T> : Consumer<T, Queue<T>>
    {
        private readonly int _count;

        public TakeLast(int count) : base(new Queue<T>()) =>
            _count = count;

        public override ChainStatus ProcessNext(T input)
        {
            if (Result.Count < _count)
            {
                Result.Enqueue(input);
            }
            else
            {
                Result.Dequeue();
                Result.Enqueue(input);
            }

            return ChainStatus.Flow;
        }
    }
}

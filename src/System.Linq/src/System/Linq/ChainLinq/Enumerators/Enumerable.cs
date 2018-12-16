using System.Collections.Generic;

namespace System.Linq.ChainLinq.Enumerators
{
    internal class ConsumerEnumeratorEnumerable<T, TResult> : ConsumerEnumerator<TResult>
    {
        private IEnumerable<T> enumerable;
        private IEnumerator<T> enumerator;
        private Chain<T> chain = null;

        internal override Chain StartOfChain => chain;

        public ConsumerEnumeratorEnumerable(IEnumerable<T> enumerable, ILink<T, TResult> factory)
        {
            this.enumerable = enumerable;
            chain = factory.Compose(this);
        }

        public override void ChainDispose()
        {
            if (enumerator != null)
            {
                enumerator.Dispose();
                enumerator = null;
            }
            enumerable = null;
            chain = null;
        }

        public override bool MoveNext()
        {
            if (enumerable != null)
            {
                enumerator = enumerable.GetEnumerator();
                enumerable = null;
            }

        tryAgain:
            if (!enumerator.MoveNext() || state.IsStopped())
            {
                Result = default(TResult);
                chain.ChainComplete();
                return false;
            }

            state = chain.ProcessNext(enumerator.Current);
            if (!state.IsFlowing())
                goto tryAgain;

            return true;
        }
    }
}

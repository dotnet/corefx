using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq.ChainLinq.Enumerators
{
    internal class ConsumerEnumeratorEnumerable<T, TResult> : ConsumerEnumerator<TResult>
    {
        private IEnumerable<T> enumerable;
        private IEnumerator<T> enumerator;
        private Chain<T, ChainEnd> activity = null;

        internal override Chain StartOfChain => activity;

        public ConsumerEnumeratorEnumerable(IEnumerable<T> enumerable, ILink<T, TResult> factory)
        {
            this.enumerable = enumerable;
            activity = factory.Compose(this);
        }

        public override void ChainDispose()
        {
            if (enumerator != null)
            {
                enumerator.Dispose();
                enumerator = null;
            }
            enumerable = null;
            activity = null;
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
                activity.ChainComplete();
                return false;
            }

            state = activity.ProcessNext(enumerator.Current);
            if (!state.IsFlowing())
                goto tryAgain;

            return true;
        }
    }
}

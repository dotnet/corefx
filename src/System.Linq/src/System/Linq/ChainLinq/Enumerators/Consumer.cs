using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq.ChainLinq.Enumerators
{
    internal abstract class ConsumerEnumerator<T> : Consumer<T, T>, IEnumerator<T>
    {
        protected ChainStatus state = ChainStatus.Flow;

        protected ConsumerEnumerator() : base(default(T)) { }

        internal virtual Chain StartOfChain { get; }

        public override ChainStatus ProcessNext(T input)
        {
            Result = input;
            return ChainStatus.Flow;
        }

        public virtual T Current => Result;
        object IEnumerator.Current => Result;
        public virtual void Dispose() => StartOfChain.ChainDispose();
        public virtual void Reset() => throw new NotSupportedException();

        public abstract bool MoveNext();
    }
}

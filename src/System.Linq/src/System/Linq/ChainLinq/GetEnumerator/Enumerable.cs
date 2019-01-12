using System.Collections;
using System.Collections.Generic;

namespace System.Linq.ChainLinq.GetEnumerator
{
    static partial class Enumerable
    {
        static partial void Optimized<T, U>(IEnumerable<T> enumerable, Link<T, U> link, ref IEnumerator<U> enumerator);

        public static IEnumerator<U> Get<T, U>(IEnumerable<T> enumerable, Link<T, U> link)
        {
            IEnumerator<U> optimized = null;
            Optimized(enumerable, link, ref optimized);
            if (optimized != null)
            {
                return optimized;
            }

            return new ConsumerEnumerators.Enumerable<T, U>(enumerable, link);
        }

        internal abstract class EnumeratorBase<T> : IEnumerator<T>
        {
            object IEnumerator.Current => Current;
            public virtual void Dispose() { }
            public virtual void Reset() => throw Error.NotSupported();

            public T Current { get; protected set; }

            public abstract bool MoveNext();
        }

        internal class WhereEnumerator<T> : EnumeratorBase<T>
        {
            private readonly Func<T, bool> _predicate;

            private IEnumerable<T> _enumerable;
            private IEnumerator<T> _enumerator;

            public WhereEnumerator(IEnumerable<T> enumerable, Func<T, bool> predicate) =>
                (_enumerable, _predicate) = (enumerable, predicate);

            public override void Dispose()
            {
                if (_enumerator != null)
                {
                    _enumerator.Dispose();
                }
            }

            public override bool MoveNext()
            {
                if (_enumerator == null)
                {
                    _enumerator = _enumerable.GetEnumerator();
                }

                while (_enumerator.MoveNext())
                {
                    if (_predicate(_enumerator.Current))
                    {
                        Current = _enumerator.Current;
                        return true;
                    }
                }

                Current = default;
                return false;
            }
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq.ChainLinq.GetEnumerator
{
    static partial class Enumerable
    {
        static partial void Optimized<T, U>(Consumables.Enumerable<T, U> consumable, ref IEnumerator<U> enumerator)
        {
            switch (consumable.Link.LinkType)
            {
                case Links.LinkType.Where:
                    enumerator = GetWhereEnumerator<U>(consumable);
                    break;

                case Links.LinkType.Select:
                    enumerator = new SelectEnumerator<T, U>(consumable.Underlying, ((Links.Select<T, U>)consumable.Link).Selector);
                    break;
            }
        }

        private static IEnumerator<T> GetWhereEnumerator<T>(object thisAsObject)
        {
            var eVV = (Consumables.Enumerable<T, T>)thisAsObject;
            return new WhereEnumerator<T>(eVV.Underlying, ((Links.Where<T>)eVV.Link).Predicate);
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



        class SelectEnumerator<T, U> : EnumeratorBase<U>
        {
            private Func<T, U> _selector;

            private IEnumerable<T> _enumerable;
            private IEnumerator<T> _enumerator;

            public override void Dispose()
            {
                if (_enumerator != null)
                {
                    _enumerator.Dispose();
                }
            }

            public SelectEnumerator(IEnumerable<T> enumerable, Func<T, U> selector) =>
                (_enumerable, _selector) = (enumerable, selector);

            public override bool MoveNext()
            {
                if (_enumerator == null)
                {
                    _enumerator = _enumerable.GetEnumerator();
                }

                if (_enumerator.MoveNext())
                {
                    Current = _selector(_enumerator.Current);
                    return true;
                }

                Current = default;
                return false;
            }
        }
    }
}

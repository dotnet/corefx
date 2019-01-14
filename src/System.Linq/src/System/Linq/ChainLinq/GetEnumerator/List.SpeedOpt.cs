using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq.ChainLinq.GetEnumerator
{
    static partial class List
    {
        static partial void Optimized<T, U>(Consumables.List<T, U> consumable, ref IEnumerator<U> enumerator)
        {
            switch (consumable.Link.LinkType)
            {
                case Links.LinkType.Select:
                    enumerator = new SelectEnumerator<T, U>(consumable.Underlying, ((Links.Select<T, U>)consumable.Link).Selector);
                    break;

                case Links.LinkType.Where:
                    Debug.Assert(typeof(T) == typeof(U));
                    enumerator = GetWhereEnumerator<U>(consumable);
                    break;
            }
        }

        private static IEnumerator<T> GetWhereEnumerator<T>(object consumable)
        {
            var c = (Consumables.List<T, T>)consumable;
            return new WhereEnumerator<T>(c.Underlying, ((Links.Where<T>)c.Link).Predicate);
        }

        abstract class EnumeratorBase<T> : IEnumerator<T>
        {
            object IEnumerator.Current => Current;
            public virtual void Dispose() { }
            public virtual void Reset() => throw Error.NotSupported();

            public T Current { get; protected set; }

            public abstract bool MoveNext();
        }

        sealed class WhereEnumerator<T> : EnumeratorBase<T>
        {
            private readonly Func<T, bool> _predicate;

            private List<T>.Enumerator _list;

            public WhereEnumerator(List<T> list, Func<T, bool> predicate) =>
                (_list, _predicate) = (list.GetEnumerator(), predicate);

            public override bool MoveNext()
            {
                while (_list.MoveNext())
                {
                    if (_predicate(_list.Current))
                    {
                        Current = _list.Current;
                        return true;
                    }
                }

                Current = default;
                return false;
            }
        }

        sealed class SelectEnumerator<T, U> : EnumeratorBase<U>
        {
            private Func<T, U> _selector;

            private List<T>.Enumerator _list;

            public SelectEnumerator(List<T> list, Func<T, U> selector) =>
                (_list, _selector) = (list.GetEnumerator(), selector);

            public override bool MoveNext()
            {
                if (_list.MoveNext())
                {
                    Current = _selector(_list.Current);
                    return true;
                }
                Current = default;
                return false;
            }
        }
    }
}

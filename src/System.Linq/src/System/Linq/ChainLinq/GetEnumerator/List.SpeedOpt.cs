using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq.ChainLinq.GetEnumerator
{
    static partial class List
    {
        static partial void Optimized<T, U>(List<T> list, Link<T, U> link, ref IEnumerator<U> enumerator)
        {
            switch (link.LinkType)
            {
                case Links.LinkType.Select:
                    enumerator = new SelectEnumerator<T, U>(list, ((Links.Select<T, U>)link).Selector);
                    break;

                case Links.LinkType.Where:
                    Debug.Assert(typeof(T) == typeof(U));
                    enumerator = new WhereEnumerator<U>((List<U>)(object)list, ((Links.Where<U>)(object)link).Predicate);
                    break;
            }
        }

        abstract class EnumeratorBase<T> : IEnumerator<T>
        {
            object IEnumerator.Current => Current;
            public virtual void Dispose() { }
            public virtual void Reset() => throw Error.NotSupported();

            public T Current { get; protected set; }

            public abstract bool MoveNext();
        }

        class WhereEnumerator<T> : EnumeratorBase<T>
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

        class SelectEnumerator<T, U> : EnumeratorBase<U>
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

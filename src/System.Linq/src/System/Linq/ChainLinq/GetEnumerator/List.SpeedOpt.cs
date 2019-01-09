using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Linq.ChainLinq.GetEnumerator
{
    // The compiler generated IEnumerator does not clear IEnumerator.Current which means
    // using these causes test failures.
    //
    // The could be manually created, which would be more efficient as a bonus, but I just
    // haven't got around to it.
#if COMPILER_GENERATED_ENUMERATORS_FOLLOWED_RULES_REQUIRED_BY_THE_TEST_SUITE
    static partial class List
    {
        static partial void Optimized<T, U>(List<T> list, ILink<T, U> link, ref IEnumerator<U> enumerator)
        {
            switch (link)
            {
                case Links.Select<T, U> select:
                    enumerator = Select(list, select.Selector);
                    break;

                case Links.Where<T> where:
                    Debug.Assert(typeof(T) == typeof(U));
                    enumerator = (IEnumerator<U>)Where(list, where.Predicate);
                    break;
            }
        }

        private static IEnumerator<T> Where<T>(List<T> list, Func<T, bool> predicate)
        {
            foreach (var item in list)
            {
                if (predicate(item))
                    yield return item;
            }
        }

        private static IEnumerator<U> Select<T, U>(List<T> list, Func<T, U> selector)
        {
            foreach (var item in list)
            {
                yield return selector(item);
            }
        }
    }
#endif

    static partial class List
    {
        static partial void Optimized<T, U>(List<T> list, ILink<T, U> link, ref IEnumerator<U> enumerator)
        {
            switch (link)
            {
                case Links.Select<T, U> select:
                    enumerator = new SelectEnumerator<T, U>(list, select.Selector);
                    break;

                case Links.Where<T> where:
                    Debug.Assert(typeof(T) == typeof(U));
                    enumerator = (IEnumerator<U>)new WhereEnumerator<T>(list, where.Predicate);
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

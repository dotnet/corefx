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
    static partial class Enumerable
    {
        static partial void Optimized<T, U>(IIEnumerable<T> enumerable, ILink<T, U> link, ref IEnumerator<U> enumerator)
        {
            switch (link)
            {
                case Links.Select<T, U> select:
                    enumerator = Select(enumerable, select.Selector);
                    break;

                case Links.Where<T> where:
                    Debug.Assert(typeof(T) == typeof(U));
                    enumerator = (IEnumerator<U>)Where(enumerable, where.Predicate);
                    break;
            }
        }

        private static IEnumerator<T> Where<T>(IIEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            foreach (var item in enumerable)
            {
                if (predicate(item))
                    yield return item;
            }
        }

        private static IEnumerator<U> Select<T, U>(IIEnumerable<T> enumerable, Func<T, U> selector)
        {
            foreach (var item in enumerable)
            {
                yield return selector(item);
            }
        }
    }
#endif
    static partial class Enumerable
    {
        static partial void Optimized<T, U>(IEnumerable<T> enumerable, ILink<T, U> link, ref IEnumerator<U> enumerator)
        {
            switch (link)
            {
                case Links.Select<T, U> select:
                    enumerator = new SelectEnumerator<T, U>(enumerable, select.Selector);
                    break;

                case Links.Where<T> where:
                    Debug.Assert(typeof(T) == typeof(U));
                    enumerator = (IEnumerator<U>)new WhereEnumerator<T>(enumerable, where.Predicate);
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

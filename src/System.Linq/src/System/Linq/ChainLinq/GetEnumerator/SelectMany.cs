using System.Collections;
using System.Collections.Generic;

namespace System.Linq.ChainLinq.GetEnumerator
{
    static class SelectMany
    {
        // TODO: Modify to be ConsumerEnumerator
        class Enumerator<T, V> : IEnumerator<V>
        {
            /*
             * This class implements the following state machine manually due to failing the test
             * System.Linq.Tests.SelectManyTests.DisposeAfterEnumeration
             * which checks that the enumerator sets Current to default if MoveNext returns false
             * (which apparently the standard generated state machine doesn't perform)

                        var consumer = new Consumer.SetResult<V>();
                        var chain = composed.Compose(consumer);
                        try
                        {
                            foreach (var e in selectMany)
                            {
                                foreach (var item in e)
                                {
                                    var state = chain.ProcessNext(item);
                                    if (state.IsFlowing())
                                        yield return consumer.Result;
                                    if (state.IsStopped())
                                        break;
                                }
                            }
                            chain.ChainComplete();
                        }
                        finally
                        {
                            chain.ChainDispose();
                        }
            */

            const int Start = 0; // default
            const int OuterEnumeratorMoveNext = 1;
            const int InnerEnumeratorMoveNext = 2;
            const int CheckStopped = 3;
            const int Finished = 4;
            const int PostFinished = 5;

            int _state;

            Consumable<IEnumerable<T>> _consumable;
            ILink<T, V> _link;
            Consumer.SetResult<V> _consumer;
            Chain<T> _chain;
            IEnumerator<IEnumerable<T>> _outer;
            IEnumerator<T> _inner;
            ChainStatus _status;

            public Enumerator(Consumable<IEnumerable<T>> selectMany, ILink<T, V> link)
            {
                _consumable = selectMany;
                _link = link;

                _state = Start;
            }

            public V Current { get; private set; }
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                if (_outer != null)
                {
                    _outer.Dispose();
                    _outer = null;
                }

                if (_inner != null)
                {
                    _inner.Dispose();
                    _inner = null;
                }

                if (_chain != null)
                {
                    _chain.ChainDispose();
                    _chain = null;
                }

                Current = default;
            }

            public bool MoveNext()
            {
                switch (_state)
                {
                    case OuterEnumeratorMoveNext:
                        if (_outer.MoveNext())
                        {
                            _inner = _outer.Current.GetEnumerator();

                            _state = InnerEnumeratorMoveNext;
                            goto case InnerEnumeratorMoveNext;
                        }

                        _state = Finished;
                        goto case Finished;

                    case InnerEnumeratorMoveNext:
                        if (_inner.MoveNext())
                        {
                            _status = _chain.ProcessNext(_inner.Current);
                            if (_status.IsFlowing())
                            {
                                Current = _consumer.Result;

                                _state = CheckStopped;
                                return true;
                            }

                            _state = CheckStopped;
                            goto case CheckStopped;
                        }

                        _inner.Dispose();
                        _inner = null;

                        _state = OuterEnumeratorMoveNext;
                        goto case OuterEnumeratorMoveNext;

                    case CheckStopped:
                        if (_status.IsStopped())
                        {
                            _inner.Dispose();
                            _inner = null;

                            _state = Finished;
                            goto case Finished;
                        }

                        _state = InnerEnumeratorMoveNext;
                        goto case InnerEnumeratorMoveNext;

                    case Finished:
                        Current = default;

                        _outer.Dispose();
                        _outer = null;

                        _chain.ChainComplete();
                        _chain.ChainDispose();
                        _chain = null;

                        _state = PostFinished;
                        goto case PostFinished;

                    case PostFinished:
                        return false;

                    default:
                        _consumer = new Consumer.SetResult<V>();
                        _chain = _link.Compose(_consumer);
                        _link = null;

                        _outer = _consumable.GetEnumerator();
                        _consumable = null;

                        _state = OuterEnumeratorMoveNext;
                        goto case OuterEnumeratorMoveNext;
                }
            }

            public void Reset() => throw new NotImplementedException();
        }

        // TODO: Modify to be ConsumerEnumerator
        class Enumerator<TSource, TCollection, T, V> : IEnumerator<V>
        {
            /*
             * This class implements the following state machine manually. It does this to
             * set Current = default when the enumeration is ended which the complier generated
             * state machine does not do.

                        var consumer = new Consumer.SetResult<V>();
                        var chain = link.Compose(consumer);
                        try
                        {
                            foreach (var (source, items) in selectMany)
                            {
                                foreach (var item in items)
                                {
                                    var state = chain.ProcessNext(resultSelector(source, item));
                                    if (state.IsFlowing())
                                        yield return consumer.Result;
                                    if (state.IsStopped())
                                        break;
                                }
                            }
                            chain.ChainComplete();
                        }
                        finally
                        {
                            chain.ChainDispose();
                        }
            */

            const int Start = 0; // default
            const int OuterEnumeratorMoveNext = 1;
            const int InnerEnumeratorMoveNext = 2;
            const int CheckStopped = 3;
            const int Finished = 4;
            const int PostFinished = 5;

            int _state;

            Consumable<(TSource, IEnumerable<TCollection>)> _consumable;
            ILink<T, V> _link;
            Func<TSource, TCollection, T> _resultSelector;
            Consumer.SetResult<V> _consumer;
            Chain<T> _chain;
            IEnumerator<(TSource, IEnumerable<TCollection>)> _outer;
            TSource _source;
            IEnumerator<TCollection> _inner;
            ChainStatus _status;

            public Enumerator(Consumable<(TSource, IEnumerable<TCollection>)> selectMany, Func<TSource, TCollection, T> resultSelector, ILink<T, V> link)
            {
                _consumable = selectMany;
                _link = link;
                _resultSelector = resultSelector;

                _state = 0;
            }

            public V Current { get; private set; }
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                if (_outer != null)
                {
                    _outer.Dispose();
                    _outer = null;
                }

                if (_inner != null)
                {
                    _inner.Dispose();
                    _inner = null;
                }

                if (_chain != null)
                {
                    _chain.ChainDispose();
                    _chain = null;
                }

                _resultSelector = null;
                _source = default;
                Current = default;
            }

            public bool MoveNext()
            {
                switch (_state)
                {
                    case OuterEnumeratorMoveNext:
                        if (_outer.MoveNext())
                        {
                            var (source, e) = _outer.Current;
                            _source = source;
                            _inner = e.GetEnumerator();

                            _state = InnerEnumeratorMoveNext;
                            goto case InnerEnumeratorMoveNext;
                        }

                        _state = Finished;
                        goto case Finished;

                    case InnerEnumeratorMoveNext:
                        if (_inner.MoveNext())
                        {
                            _status = _chain.ProcessNext(_resultSelector(_source, _inner.Current));
                            if (_status.IsFlowing())
                            {
                                Current = _consumer.Result;

                                _state = CheckStopped;
                                return true;
                            }

                            _state = CheckStopped;
                            goto case CheckStopped;
                        }

                        _inner.Dispose();
                        _inner = null;

                        _state = OuterEnumeratorMoveNext;
                        goto case OuterEnumeratorMoveNext;

                    case CheckStopped:
                        if (_status.IsStopped())
                        {
                            _inner.Dispose();
                            _inner = null;

                            _state = Finished;
                            goto case Finished;
                        }

                        _state = InnerEnumeratorMoveNext;
                        goto case InnerEnumeratorMoveNext;

                    case Finished:
                        _source = default;
                        Current = default;

                        _outer.Dispose();
                        _outer = null;

                        _chain.ChainComplete();
                        _chain.ChainDispose();
                        _chain = null;

                        _resultSelector = null;

                        _state = PostFinished;
                        goto case PostFinished;

                    case PostFinished:
                        return false;

                    default:
                        _consumer = new Consumer.SetResult<V>();
                        _chain = _link.Compose(_consumer);
                        _link = null;

                        _outer = _consumable.GetEnumerator();
                        _consumable = null;

                        _state = OuterEnumeratorMoveNext;
                        goto case OuterEnumeratorMoveNext;
                }
            }

            public void Reset() => throw new NotImplementedException();
        }

        public static IEnumerator<V> Get<T, V>(Consumable<IEnumerable<T>> selectMany, ILink<T, V> link)
        {
            return new Enumerator<T, V>(selectMany, link);
        }

        public static IEnumerator<V> Get<TSource, TCollection, T, V>(Consumable<(TSource, IEnumerable<TCollection>)> selectMany, Func<TSource, TCollection, T> resultSelector, ILink<T, V> link)
        {
            return new Enumerator<TSource, TCollection, T, V>(selectMany, resultSelector, link);
        }
    }
}

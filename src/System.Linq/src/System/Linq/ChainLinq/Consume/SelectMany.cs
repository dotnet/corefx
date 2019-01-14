using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consume
{
    static class SelectMany
    {
        sealed class SelectManyInnerConsumer<TSource, TCollection, T> : Consumer<TCollection, ChainStatus>
        {
            private readonly Chain<T> _chainT;
            private readonly Func<TSource, TCollection, T> _resultSelector;

            public TSource Source { get; set; }

            public SelectManyInnerConsumer(Func<TSource, TCollection, T> resultSelector, Chain<T> chainT) : base(ChainStatus.Flow) =>
                (_chainT, _resultSelector) = (chainT, resultSelector);

            public override ChainStatus ProcessNext(TCollection input)
            {
                var state = _chainT.ProcessNext(_resultSelector(Source, input));
                Result = state;
                return state;
            }
        }

        sealed class SelectManyOuterConsumer<T> : Consumer<IEnumerable<T>, ChainEnd>
        {
            private readonly Chain<T> _chainT;
            private UnknownEnumerable.ChainConsumer<T> _inner;

            public SelectManyOuterConsumer(Chain<T> chainT) : base(default) =>
                _chainT = chainT;

            public override ChainStatus ProcessNext(IEnumerable<T> input) =>
                UnknownEnumerable.Consume(input, _chainT, ref _inner);
        }

        sealed class SelectManyOuterConsumer<TSource, TCollection, T> : Consumer<(TSource, IEnumerable<TCollection>), ChainEnd>
        {
            readonly Func<TSource, TCollection, T> _resultSelector;
            readonly Chain<T> _chainT;

            SelectManyInnerConsumer<TSource, TCollection, T> _inner;

            private SelectManyInnerConsumer<TSource, TCollection, T> GetInnerConsumer()
            {
                if (_inner == null)
                    _inner = new SelectManyInnerConsumer<TSource, TCollection, T>(_resultSelector, _chainT);
                return _inner;
            }

            public SelectManyOuterConsumer(Func<TSource, TCollection, T> resultSelector, Chain<T> chainT) : base(default(ChainEnd)) =>
                (_chainT, _resultSelector) = (chainT, resultSelector);

            public override ChainStatus ProcessNext((TSource, IEnumerable<TCollection>) input)
            {
                var state = ChainStatus.Flow;
                if (input.Item2 is Consumable<TCollection> consumable)
                {
                    var consumer = GetInnerConsumer();
                    consumer.Source = input.Item1;
                    consumable.Consume(consumer);
                    state = consumer.Result;
                }
                else if (input.Item2 is TCollection[] array)
                {
                    foreach (var item in array)
                    {
                        state = _chainT.ProcessNext(_resultSelector(input.Item1, item));
                        if (state.IsStopped())
                            break;
                    }
                }
                else if (input.Item2 is List<TCollection> list)
                {
                    foreach (var item in list)
                    {
                        state = _chainT.ProcessNext(_resultSelector(input.Item1, item));
                        if (state.IsStopped())
                            break;
                    }
                }
                else
                {
                    foreach (var item in input.Item2)
                    {
                        state = _chainT.ProcessNext(_resultSelector(input.Item1, item));
                        if (state.IsStopped())
                            break;
                    }
                }
                return state;
            }
        }

        public static void Invoke<T, V>(Consumable<IEnumerable<T>> e, Link<T, V> composition, Chain<V> consumer)
        {
            var chain = composition.Compose(consumer);
            try
            {
                e.Consume(new SelectManyOuterConsumer<T>(chain));
                chain.ChainComplete();
            }
            finally
            {
                chain.ChainDispose();
            }
        }

        public static void Invoke<TSource, TCollection, T, V>(Consumable<(TSource, IEnumerable<TCollection>)> e, Func<TSource, TCollection, T> resultSelector, Link<T, V> composition, Chain<V> consumer)
        {
            var chain = composition.Compose(consumer);
            try
            {
                e.Consume(new SelectManyOuterConsumer<TSource, TCollection, T>(resultSelector, chain));
                chain.ChainComplete();
            }
            finally
            {
                chain.ChainDispose();
            }
        }
    }
}

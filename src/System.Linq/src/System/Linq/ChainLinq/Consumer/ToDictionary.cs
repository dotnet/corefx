using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumer
{
    sealed class ToDictionary<TSource, TKey> : Consumer<TSource, Dictionary<TKey, TSource>>
    {
        private readonly Func<TSource, TKey> _keySelector;

        public ToDictionary(Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
            : base(new Dictionary<TKey, TSource>(comparer)) =>
        _keySelector = keySelector;

        public ToDictionary(Func<TSource, TKey> keySelector, int capacity, IEqualityComparer<TKey> comparer)
            : base(new Dictionary<TKey, TSource>(capacity, comparer)) =>
        _keySelector = keySelector;

        public override ChainStatus ProcessNext(TSource input)
        {
            Result.Add(_keySelector(input), input);

            return ChainStatus.Flow;
        }
    }

    sealed class ToDictionary<TSource, TKey, TElement> : Consumer<TSource, Dictionary<TKey, TElement>>
    {
        private readonly Func<TSource, TKey> _keySelector;
        private readonly Func<TSource, TElement> _elementSelector;

        public ToDictionary(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
            : base(new Dictionary<TKey, TElement>(comparer)) =>
        (_keySelector, _elementSelector) = (keySelector, elementSelector);

        public ToDictionary(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, int capacity, IEqualityComparer<TKey> comparer)
            : base(new Dictionary<TKey, TElement>(capacity, comparer)) =>
        (_keySelector, _elementSelector) = (keySelector, elementSelector);

        public override ChainStatus ProcessNext(TSource input)
        {
            Result.Add(_keySelector(input), _elementSelector(input));

            return ChainStatus.Flow;
        }
    }
}

using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumer
{
    sealed class Lookup<TSource, TKey> : Consumer<TSource, Consumables.Lookup<TKey, TSource>>
    {
        private readonly Func<TSource, TKey> _keySelector;

        public Lookup(Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer) : base(new Consumables.Lookup<TKey, TSource>(comparer)) =>
            (_keySelector) = (keySelector);

        public override ChainStatus ProcessNext(TSource item)
        {
            Result.GetGrouping(_keySelector(item), create: true).Add(item);
            return ChainStatus.Flow;
        }
    }

    sealed class LookupSplit<TSource, TKey, TElement> : Consumer<TSource, Consumables.Lookup<TKey, TElement>>
    {
        private readonly Func<TSource, TKey> _keySelector;
        private readonly Func<TSource, TElement> _elementSelector;

        public LookupSplit(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer) : base(new Consumables.Lookup<TKey, TElement>(comparer)) =>
            (_keySelector, _elementSelector) = (keySelector, elementSelector);

        public override ChainStatus ProcessNext(TSource item)
        {
            Result.GetGrouping(_keySelector(item), create: true).Add(_elementSelector(item));
            return ChainStatus.Flow;
        }
    }

    sealed class LookupForJoin<TSource, TKey> : Consumer<TSource, Consumables.Lookup<TKey, TSource>>
    {
        private readonly Func<TSource, TKey> _keySelector;

        public LookupForJoin(Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer) : base(new Consumables.Lookup<TKey, TSource>(comparer)) =>
            (_keySelector) = (keySelector);

        public override ChainStatus ProcessNext(TSource item)
        {
            TKey key = _keySelector(item);
            if (key != null)
            {
                Result.GetGrouping(key, create: true).Add(item);
            }
            return ChainStatus.Flow;
        }
    }
}

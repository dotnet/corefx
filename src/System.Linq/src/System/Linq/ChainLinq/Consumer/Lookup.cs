using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumer
{
    static class Lookup
    {
        private static Consumables.Lookup<TKey, TSource> GetLookupBuilder<TKey, TSource>(IEqualityComparer<TKey> comparer)
        {
            if (comparer == null || ReferenceEquals(comparer, EqualityComparer<TKey>.Default))
            {
                return new Consumables.LookupDefaultComparer<TKey, TSource>();
            }
            else
            {
                return new Consumables.LookupWithComparer<TKey, TSource>(comparer);
            }
        }

        internal static Consumables.Lookup<TKey, TSource> Consume<TKey, TSource>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            Consumables.Lookup<TKey, TSource> builder = GetLookupBuilder<TKey, TSource>(comparer);
            return Utils.Consume(source, new Lookup<TSource, TKey>(builder, keySelector));
        }

        internal static Consumables.Lookup<TKey, TElement> Consume<TSource, TKey, TElement>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            Consumables.Lookup<TKey, TElement> builder = GetLookupBuilder<TKey, TElement>(comparer);
            return Utils.Consume(source, new LookupSplit<TSource, TKey, TElement>(builder, keySelector, elementSelector));
        }

        internal static Consumables.Lookup<TKey, TSource> ConsumeForJoin<TKey, TSource>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            Consumables.Lookup<TKey, TSource> builder = GetLookupBuilder<TKey, TSource>(comparer);
            return Utils.Consume(source, new LookupForJoin<TSource, TKey>(builder, keySelector, comparer));
        }
    }

    sealed class Lookup<TSource, TKey> : Consumer<TSource, Consumables.Lookup<TKey, TSource>>
    {
        private readonly Func<TSource, TKey> _keySelector;

        public Lookup(Consumables.Lookup<TKey, TSource> builder, Func<TSource, TKey> keySelector) : base(builder) =>
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

        public LookupSplit(Consumables.Lookup<TKey, TElement> builder, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector) : base(builder) =>
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

        public LookupForJoin(Consumables.Lookup<TKey, TSource> builder, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer) : base(builder) =>
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

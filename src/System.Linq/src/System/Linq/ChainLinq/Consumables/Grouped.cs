using System.Collections.Generic;

namespace System.Linq.ChainLinq.Consumables
{
    internal sealed partial class GroupedEnumerable<TSource, TKey>
        : ConsumableForAddition<IGrouping<TKey, TSource>>
        , IConsumableInternal
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly IEqualityComparer<TKey> _comparer;

        public GroupedEnumerable(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            _source = source ?? throw Error.ArgumentNull(nameof(source));
            _keySelector = keySelector ?? throw Error.ArgumentNull(nameof(keySelector));
            _comparer = comparer;
        }

        public override Consumable<U> AddTail<U>(ILink<IGrouping<TKey, TSource>, U> transform) =>
            new GroupedEnumerableWithLinks<TSource, TKey, U>(_source, _keySelector, _comparer, transform);

        private Lookup<TKey, TSource> ToLookup() =>
            Utils.AsConsumable(_source).Consume(new Consumer.Lookup<TSource, TKey>(_keySelector, _comparer));

        public override Result Consume<Result>(Consumer<IGrouping<TKey, TSource>, Result> consumer) =>
            ToLookup().Consume(consumer);

        public override IEnumerator<IGrouping<TKey, TSource>> GetEnumerator() =>
            ToLookup().GetEnumerator();
    }

    internal sealed partial class GroupedEnumerableWithLinks<TSource, TKey, V> : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, IGrouping<TKey, TSource>>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly IEqualityComparer<TKey> _comparer;

        public GroupedEnumerableWithLinks(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer, ILink<IGrouping<TKey, TSource>, V> link) : base(link) =>
            (_source, _keySelector, _comparer) = (source, keySelector, comparer);

        public override Consumable<W> Create<W>(ILink<IGrouping<TKey, TSource>, W> first) =>
            new GroupedEnumerableWithLinks<TSource, TKey, W>(_source, _keySelector, _comparer, first);

        private Consumable<V> ToConsumable()
        {
            Consumable<TSource> source = Utils.AsConsumable(_source);
            Consumer.Lookup<TSource, TKey> consumerLookup = new Consumer.Lookup<TSource, TKey>(_keySelector, _comparer);
            Lookup<TKey, TSource> lookup = source.Consume(consumerLookup);
            return lookup.AddTail(Link);
        }

        public override IEnumerator<V> GetEnumerator() =>
            ToConsumable().GetEnumerator();

        public override TResult Consume<TResult>(Consumer<V, TResult> consumer) =>
            ToConsumable().Consume(consumer);
    }

    internal sealed partial class GroupedEnumerable<TSource, TKey, TElement>
        : ConsumableForAddition<IGrouping<TKey, TElement>>
        , IConsumableInternal
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly Func<TSource, TElement> _elementSelector;
        private readonly IEqualityComparer<TKey> _comparer;

        public GroupedEnumerable(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            _source = source ?? throw Error.ArgumentNull(nameof(source));
            _keySelector = keySelector ?? throw Error.ArgumentNull(nameof(keySelector));
            _elementSelector = elementSelector ?? throw Error.ArgumentNull(nameof(elementSelector));
            _comparer = comparer;
        }

        public override Consumable<U> AddTail<U>(ILink<IGrouping<TKey, TElement>, U> transform) =>
            new GroupedEnumerableWithLinks<TSource, TKey, TElement, U>(_source, _keySelector, _elementSelector, _comparer, transform);

        private Lookup<TKey, TElement> ToLookup() =>
            Utils.AsConsumable(_source).Consume(new Consumer.LookupSplit<TSource, TKey, TElement>(_keySelector, _elementSelector, _comparer));

        public override Result Consume<Result>(Consumer<IGrouping<TKey, TElement>, Result> consumer) =>
            ToLookup().Consume(consumer);

        public override IEnumerator<IGrouping<TKey, TElement>> GetEnumerator() =>
            ToLookup().GetEnumerator();
    }

    internal sealed partial class GroupedEnumerableWithLinks<TSource, TKey, TElement, V> : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, IGrouping<TKey, TElement>>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly Func<TSource, TElement> _elementSelector;
        private readonly IEqualityComparer<TKey> _comparer;

        public GroupedEnumerableWithLinks(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer, ILink<IGrouping<TKey, TElement>, V> link) : base(link) =>
            (_source, _keySelector, _elementSelector, _comparer) = (source, keySelector, elementSelector, comparer);

        public override Consumable<W> Create<W>(ILink<IGrouping<TKey, TElement>, W> first) =>
            new GroupedEnumerableWithLinks<TSource, TKey, TElement, W>(_source, _keySelector, _elementSelector, _comparer, first);

        private Consumable<V> ToConsumable()
        {
            Consumable<TSource> source = Utils.AsConsumable(_source);
            Consumer.LookupSplit<TSource, TKey, TElement> consumerLookup = new Consumer.LookupSplit<TSource, TKey, TElement>(_keySelector, _elementSelector, _comparer);
            Lookup<TKey, TElement> lookup = source.Consume(consumerLookup);
            return lookup.AddTail(Link);
        }

        public override IEnumerator<V> GetEnumerator() =>
            ToConsumable().GetEnumerator();

        public override TResult Consume<TResult>(Consumer<V, TResult> consumer) =>
            ToConsumable().Consume(consumer);
    }

    internal sealed partial class GroupedResultEnumerable<TSource, TKey, TResult>
        : ConsumableForAddition<TResult>
        , IConsumableInternal
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly IEqualityComparer<TKey> _comparer;
        private readonly Func<TKey, IEnumerable<TSource>, TResult> _resultSelector;

        public GroupedResultEnumerable(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            _source = source ?? throw Error.ArgumentNull(nameof(source));
            _keySelector = keySelector ?? throw Error.ArgumentNull(nameof(keySelector));
            _resultSelector = resultSelector ?? throw Error.ArgumentNull(nameof(resultSelector));
            _comparer = comparer;
        }

        public override Consumable<U> AddTail<U>(ILink<TResult, U> transform) =>
            new GroupedResultEnumerableWithLinks<TSource, TKey, TResult, U>(_source, _keySelector, _resultSelector, _comparer, transform);

        private Lookup<TKey, TSource> ToLookup() =>
            Utils.AsConsumable(_source).Consume(new Consumer.Lookup<TSource, TKey>(_keySelector, _comparer));

        public override Result Consume<Result>(Consumer<TResult, Result> consumer) =>
            ToLookup().ApplyResultSelector(_resultSelector).Consume(consumer);

        public override IEnumerator<TResult> GetEnumerator() =>
            ToLookup().ApplyResultSelector(_resultSelector).GetEnumerator();
    }

    internal sealed partial class GroupedResultEnumerableWithLinks<TSource, TKey, TResult, V> : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, TResult>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly IEqualityComparer<TKey> _comparer;
        private readonly Func<TKey, IEnumerable<TSource>, TResult> _resultSelector;

        public GroupedResultEnumerableWithLinks(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer, ILink<TResult, V> link) : base(link) =>
            (_source, _keySelector, _resultSelector, _comparer) = (source, keySelector, resultSelector, comparer);

        public override Consumable<W> Create<W>(ILink<TResult, W> first) =>
            new GroupedResultEnumerableWithLinks<TSource, TKey, TResult, W>(_source, _keySelector, _resultSelector, _comparer, first);

        private Consumable<V> ToConsumable()
        {
            Consumable<TSource> source = Utils.AsConsumable(_source);
            Consumer.Lookup<TSource, TKey> consumerLookup = new Consumer.Lookup<TSource, TKey>(_keySelector, _comparer);
            Lookup<TKey, TSource> lookup = source.Consume(consumerLookup);
            ConsumableForAddition<TResult> appliedSelector = lookup.ApplyResultSelector(_resultSelector);
            return appliedSelector.AddTail(Link);
        }

        public override IEnumerator<V> GetEnumerator() =>
            ToConsumable().GetEnumerator();

        public override Result Consume<Result>(Consumer<V, Result> consumer) =>
            ToConsumable().Consume(consumer);
    }

    internal sealed partial class GroupedResultEnumerable<TSource, TKey, TElement, TResult>
        : ConsumableForAddition<TResult>
        , IConsumableInternal
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly Func<TSource, TElement> _elementSelector;
        private readonly IEqualityComparer<TKey> _comparer;
        private readonly Func<TKey, IEnumerable<TElement>, TResult> _resultSelector;

        public GroupedResultEnumerable(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            _source = source ?? throw Error.ArgumentNull(nameof(source));
            _keySelector = keySelector ?? throw Error.ArgumentNull(nameof(keySelector));
            _elementSelector = elementSelector ?? throw Error.ArgumentNull(nameof(elementSelector));
            _comparer = comparer;
            _resultSelector = resultSelector ?? throw Error.ArgumentNull(nameof(resultSelector));
        }

        public override Consumable<U> AddTail<U>(ILink<TResult, U> transform) =>
            new GroupedResultEnumerableWithLinks<TSource, TKey, TElement, TResult, U>(_source, _keySelector, _elementSelector, _resultSelector, _comparer, transform);

        private Lookup<TKey, TElement> ToLookup() =>
            Utils.AsConsumable(_source).Consume(new Consumer.LookupSplit<TSource, TKey, TElement>(_keySelector, _elementSelector, _comparer));

        public override Result Consume<Result>(Consumer<TResult, Result> consumer) =>
            ToLookup().ApplyResultSelector(_resultSelector).Consume(consumer);

        public override IEnumerator<TResult> GetEnumerator() =>
            ToLookup().ApplyResultSelector(_resultSelector).GetEnumerator();
    }

    internal sealed partial class GroupedResultEnumerableWithLinks<TSource, TKey, TElement, TResult, V> : Base_Generic_Arguments_Reversed_To_Work_Around_XUnit_Bug<V, TResult>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Func<TSource, TKey> _keySelector;
        private readonly Func<TSource, TElement> _elementSelector;
        private readonly IEqualityComparer<TKey> _comparer;
        private readonly Func<TKey, IEnumerable<TElement>, TResult> _resultSelector;

        public GroupedResultEnumerableWithLinks(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer, ILink<TResult, V> link) : base(link) =>
            (_source, _keySelector, _elementSelector, _resultSelector, _comparer) = (source, keySelector, elementSelector, resultSelector, comparer);

        public override Consumable<W> Create<W>(ILink<TResult, W> first) =>
            new GroupedResultEnumerableWithLinks<TSource, TKey, TElement, TResult, W>(_source, _keySelector, _elementSelector, _resultSelector, _comparer, first);

        private Consumable<V> ToConsumable()
        {
            Consumable<TSource> source = Utils.AsConsumable(_source);
            Consumer.LookupSplit<TSource, TKey, TElement> consumerLookup = new Consumer.LookupSplit<TSource, TKey, TElement>(_keySelector, _elementSelector, _comparer);
            Lookup<TKey, TElement> lookup = source.Consume(consumerLookup);
            ConsumableForAddition<TResult> appliedSelector = lookup.ApplyResultSelector(_resultSelector);
            return appliedSelector.AddTail(Link);
        }

        public override IEnumerator<V> GetEnumerator() =>
            ToConsumable().GetEnumerator();

        public override Result Consume<Result>(Consumer<V, Result> consumer) =>
            ToConsumable().Consume(consumer);
    }

}

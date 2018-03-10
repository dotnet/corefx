// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// =+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
//
// HashJoinQueryOperatorEnumerator.cs
//
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace System.Linq.Parallel
{
    /// <summary>
    /// This enumerator implements the hash-join algorithm as noted earlier.
    ///
    /// Assumptions:
    ///     This enumerator type won't work properly at all if the analysis engine didn't
    ///     ensure a proper hash-partition. We expect inner and outer elements with equal
    ///     keys are ALWAYS in the same partition. If they aren't (e.g. if the analysis is
    ///     busted) we'll silently drop items on the floor. :( 
    ///     
    ///     
    ///  This is the enumerator class for two operators:
    ///   - Join
    ///   - GroupJoin
    /// </summary>
    /// <typeparam name="TLeftInput"></typeparam>
    /// <typeparam name="TLeftKey"></typeparam>
    /// <typeparam name="TRightInput"></typeparam>
    /// <typeparam name="THashKey"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    internal class HashJoinQueryOperatorEnumerator<TLeftInput, TLeftKey, TRightInput, TRightKey, THashKey, TOutput, TOutputKey>
        : QueryOperatorEnumerator<TOutput, TOutputKey>
    {
        private readonly QueryOperatorEnumerator<Pair<TLeftInput, THashKey>, TLeftKey> _leftSource; // Left (outer) data source. For probing.
        private readonly HashLookupBuilder<TRightInput, TRightKey, THashKey> _rightLookupBuilder; // Right (inner) data source. For building.
        private readonly Func<TLeftInput, TRightInput, TOutput> _resultSelector; // Result selector.
        private readonly HashJoinOutputKeyBuilder<TLeftKey, TRightKey, TOutputKey> _outputKeyBuilder;
        private readonly CancellationToken _cancellationToken;
        private Mutables _mutables;

        private class Mutables
        {
            internal TLeftInput _currentLeft; // The current matching left element.
            internal TLeftKey _currentLeftKey; // The current index of the matching left element.
            internal HashJoinHashLookup<THashKey, TRightInput, TRightKey> _rightHashLookup; // The hash lookup.
            internal ListChunk<Pair<TRightInput, TRightKey>> _currentRightMatches; // Current right matches (if any).
            internal int _currentRightMatchesIndex; // Current index in the set of right matches.
            internal int _outputLoopCount;
        }

        //---------------------------------------------------------------------------------------
        // Instantiates a new hash-join enumerator.
        //

        internal HashJoinQueryOperatorEnumerator(
            QueryOperatorEnumerator<Pair<TLeftInput, THashKey>, TLeftKey> leftSource,
            HashLookupBuilder<TRightInput, TRightKey, THashKey> rightLookupBuilder,
            Func<TLeftInput, TRightInput, TOutput> resultSelector,
            HashJoinOutputKeyBuilder<TLeftKey, TRightKey, TOutputKey> outputKeyBuilder,
            CancellationToken cancellationToken)
        {
            Debug.Assert(leftSource != null);
            Debug.Assert(rightLookupBuilder != null);
            Debug.Assert(resultSelector != null);
            Debug.Assert(outputKeyBuilder != null);

            _leftSource = leftSource;
            _rightLookupBuilder = rightLookupBuilder;
            _resultSelector = resultSelector;
            _outputKeyBuilder = outputKeyBuilder;
            _cancellationToken = cancellationToken;
        }

        //---------------------------------------------------------------------------------------
        // MoveNext implements all the hash-join logic noted earlier. When it is called first, it
        // will execute the entire inner query tree, and build a hash-table lookup. This is the
        // Building phase. Then for the first call and all subsequent calls to MoveNext, we will
        // incrementally perform the Probing phase. We'll keep getting elements from the outer
        // data source, looking into the hash-table we built, and enumerating the full results.
        //
        // This routine supports both inner and outer (group) joins. An outer join will yield a
        // (possibly empty) list of matching elements from the inner instead of one-at-a-time,
        // as we do for inner joins.
        //

        internal override bool MoveNext(ref TOutput currentElement, ref TOutputKey currentKey)
        {
            Debug.Assert(_resultSelector != null, "expected a compiled result selector");
            Debug.Assert(_leftSource != null);
            Debug.Assert(_rightLookupBuilder != null);

            // BUILD phase: If we haven't built the hash-table yet, create that first.
            Mutables mutables = _mutables;
            if (mutables == null)
            {
                mutables = _mutables = new Mutables();
                mutables._rightHashLookup = _rightLookupBuilder.BuildHashLookup(_cancellationToken);
            }

            // PROBE phase: So long as the source has a next element, return the match.
            ListChunk<Pair<TRightInput, TRightKey>> currentRightChunk = mutables._currentRightMatches;
            if (currentRightChunk != null && mutables._currentRightMatchesIndex == currentRightChunk.Count)
            {
                mutables._currentRightMatches = currentRightChunk.Next;
                mutables._currentRightMatchesIndex = 0;
            }

            if (mutables._currentRightMatches == null)
            {
                // We have to look up the next list of matches in the hash-table.
                Pair<TLeftInput, THashKey> leftPair = default(Pair<TLeftInput, THashKey>);
                TLeftKey leftKey = default(TLeftKey);
                while (_leftSource.MoveNext(ref leftPair, ref leftKey))
                {
                    if ((mutables._outputLoopCount++ & CancellationState.POLL_INTERVAL) == 0)
                        CancellationState.ThrowIfCanceled(_cancellationToken);

                    // Find the match in the hash table.
                    HashLookupValueList<TRightInput, TRightKey> matchValue = default(HashLookupValueList<TRightInput, TRightKey>);
                    TLeftInput leftElement = leftPair.First;
                    THashKey leftHashKey = leftPair.Second;

                    // Ignore null keys.
                    if (leftHashKey != null)
                    {
                        if (mutables._rightHashLookup.TryGetValue(leftHashKey, ref matchValue))
                        {
                            // We found a new match. We remember the list in case there are multiple
                            // values under this same key -- the next iteration will pick them up.
                            mutables._currentRightMatches = matchValue.Tail;
                            Debug.Assert(mutables._currentRightMatches == null || mutables._currentRightMatches.Count > 0,
                                            "we were expecting that the list would be either null or empty");
                            mutables._currentRightMatchesIndex = 0;

                            // Yield the value.
                            currentElement = _resultSelector(leftElement, matchValue.Head.First);
                            currentKey = _outputKeyBuilder.Combine(leftKey, matchValue.Head.Second);

                            // If there is a list of matches, remember the left values for next time.
                            if (matchValue.Tail != null)
                            {
                                mutables._currentLeft = leftElement;
                                mutables._currentLeftKey = leftKey;
                            }

                            return true;
                        }
                    }
                }

                // If we've reached the end of the data source, we're done.
                return false;
            }

            // Produce the next element.
            Debug.Assert(mutables._currentRightMatches != null);
            Debug.Assert(0 <= mutables._currentRightMatchesIndex && mutables._currentRightMatchesIndex < mutables._currentRightMatches.Count);

            Pair<TRightInput, TRightKey> rightMatch = mutables._currentRightMatches._chunk[mutables._currentRightMatchesIndex];

            currentElement = _resultSelector(mutables._currentLeft, rightMatch.First);
            currentKey = _outputKeyBuilder.Combine(mutables._currentLeftKey, rightMatch.Second);

            mutables._currentRightMatchesIndex++;

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            Debug.Assert(_leftSource != null && _rightLookupBuilder != null);
            _leftSource.Dispose();
            _rightLookupBuilder.Dispose();
        }
    }

    /// <summary>
    /// A class to create output order keys from the left and right keys.
    /// </summary>
    /// <typeparam name="TLeftKey"></typeparam>
    /// <typeparam name="TRightKey"></typeparam>
    /// <typeparam name="TOutputKey"></typeparam>
    internal abstract class HashJoinOutputKeyBuilder<TLeftKey, TRightKey, TOutputKey>
    {
        public abstract TOutputKey Combine(TLeftKey leftKey, TRightKey rightKey);
    }

    /// <summary>
    /// A key builder that simply returns the left key, ignoring the right key.
    /// 
    /// Used when the right source is unordered.
    /// </summary>
    /// <typeparam name="TLeftKey"></typeparam>
    /// <typeparam name="TRightKey"></typeparam>
    internal class LeftKeyOutputKeyBuilder<TLeftKey, TRightKey> : HashJoinOutputKeyBuilder<TLeftKey, TRightKey, TLeftKey>
    {
        public override TLeftKey Combine(TLeftKey leftKey, TRightKey rightKey)
        {
            return leftKey;
        }
    }

    /// <summary>
    /// A key builder that simply returns a left key, right key pair.
    /// 
    /// Used when the right source is ordered.
    /// </summary>
    /// <typeparam name="TLeftKey"></typeparam>
    /// <typeparam name="TRightKey"></typeparam>
    internal class PairOutputKeyBuilder<TLeftKey, TRightKey> : HashJoinOutputKeyBuilder<TLeftKey, TRightKey, Pair<TLeftKey, TRightKey>>
    {
        public override Pair<TLeftKey, TRightKey> Combine(TLeftKey leftKey, TRightKey rightKey)
        {
            return new Pair<TLeftKey, TRightKey>(leftKey, rightKey);
        }
    }

    /// <summary>
    /// Class to build a HashJoinHashLookup of right elements for use in HashJoin operations.
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    /// <typeparam name="TOrderKey"></typeparam>
    /// <typeparam name="THashKey"></typeparam>
    internal abstract class HashLookupBuilder<TElement, TOrderKey, THashKey>
    {
        public abstract HashJoinHashLookup<THashKey, TElement, TOrderKey> BuildHashLookup(CancellationToken cancellationToken);

        protected void BuildBaseHashLookup<TBaseBuilder, TBaseElement, TBaseOrderKey>(
            QueryOperatorEnumerator<Pair<TBaseElement, THashKey>, TBaseOrderKey> dataSource,
            TBaseBuilder baseHashBuilder,
            CancellationToken cancellationToken) where TBaseBuilder : IBaseHashBuilder<TBaseElement, TBaseOrderKey>
        {
            Debug.Assert(dataSource != null);

#if DEBUG
            int hashLookupCount = 0;
            int hashKeyCollisions = 0;
#endif

            Pair<TBaseElement, THashKey> currentPair = default(Pair<TBaseElement, THashKey>);
            TBaseOrderKey orderKey = default(TBaseOrderKey);
            int i = 0;
            while (dataSource.MoveNext(ref currentPair, ref orderKey))
            {
                if ((i++ & CancellationState.POLL_INTERVAL) == 0)
                    CancellationState.ThrowIfCanceled(cancellationToken);

                TBaseElement element = currentPair.First;
                THashKey hashKey = currentPair.Second;

                // We ignore null keys.
                if (hashKey != null)
                {
#if DEBUG
                    hashLookupCount++;
#endif

                    if (baseHashBuilder.Add(hashKey, element, orderKey))
                    {
#if DEBUG
                        hashKeyCollisions++;
#endif
                    }
                }
            }

#if DEBUG
            TraceHelpers.TraceInfo("HashLookupBuilder::BuildBaseHashLookup - built hash table [count = {0}, collisions = {1}]",
                hashLookupCount, hashKeyCollisions);
#endif
        }

        // Standard implementation of the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Used in BuildBaseHashLookup to translate from data in dataSource to data to be used
        /// by the HashJoin operator.
        /// </summary>
        /// <typeparam name="TBaseElement"></typeparam>
        /// <typeparam name="TBaseOrderKey"></typeparam>
        protected interface IBaseHashBuilder<TBaseElement, TBaseOrderKey>
        {
            // adds the value to the base HashLookup.
            // returns true if the addition is a hash collision
            bool Add(THashKey hashKey, TBaseElement element, TBaseOrderKey orderKey);
        }
    }

    /// <summary>
    /// A wrapper for the HashLookup returned by HashLookupBuilder.
    /// 
    /// This will allow for providing a default if there is no value in the base lookup.
    /// </summary>
    /// <typeparam name="THashKey"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    /// <typeparam name="TOrderKey"></typeparam>
    internal abstract class HashJoinHashLookup<THashKey, TElement, TOrderKey>
    {
        // get the current value if it exists.
        public abstract bool TryGetValue(THashKey key, ref HashLookupValueList<TElement, TOrderKey> value);
    }

    /// <summary>
    /// Class to build a HashJoinHashLookup of right elements for use in Join operations.
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    /// <typeparam name="TOrderKey"></typeparam>
    /// <typeparam name="THashKey"></typeparam>
    internal class JoinHashLookupBuilder<TElement, TOrderKey, THashKey> : HashLookupBuilder<TElement, TOrderKey, THashKey>
    {
        private readonly QueryOperatorEnumerator<Pair<TElement, THashKey>, TOrderKey> _dataSource; // data source. For building.
        private readonly IEqualityComparer<THashKey> _keyComparer; // An optional key comparison object.

        internal JoinHashLookupBuilder(QueryOperatorEnumerator<Pair<TElement, THashKey>, TOrderKey> dataSource, IEqualityComparer<THashKey> keyComparer)
        {
            Debug.Assert(dataSource != null);

            _dataSource = dataSource;
            _keyComparer = keyComparer;
        }

        public override HashJoinHashLookup<THashKey, TElement, TOrderKey> BuildHashLookup(CancellationToken cancellationToken)
        {
            HashLookup<THashKey, HashLookupValueList<TElement, TOrderKey>> lookup =
                new HashLookup<THashKey, HashLookupValueList<TElement, TOrderKey>>(_keyComparer);
            JoinBaseHashBuilder baseHashBuilder = new JoinBaseHashBuilder(lookup);

            BuildBaseHashLookup(_dataSource, baseHashBuilder, cancellationToken);

            return new JoinHashLookup(lookup);
        }

        protected override void Dispose(bool disposing)
        {
            Debug.Assert(_dataSource != null);

            _dataSource.Dispose();
        }

        /// <summary>
        /// Adds TElement,TOrderKey values to a HashLookup of HashLookupValueLists.
        /// </summary>
        private struct JoinBaseHashBuilder : IBaseHashBuilder<TElement, TOrderKey>
        {
            private readonly HashLookup<THashKey, HashLookupValueList<TElement, TOrderKey>> _base;

            public JoinBaseHashBuilder(HashLookup<THashKey, HashLookupValueList<TElement, TOrderKey>> baseLookup)
            {
                Debug.Assert(baseLookup != null);

                _base = baseLookup;
            }

            public bool Add(THashKey hashKey, TElement element, TOrderKey orderKey)
            {
                HashLookupValueList<TElement, TOrderKey> currentValue = default(HashLookupValueList<TElement, TOrderKey>);
                if (!_base.TryGetValue(hashKey, ref currentValue))
                {
                    currentValue = new HashLookupValueList<TElement, TOrderKey>(element, orderKey);
                    _base.Add(hashKey, currentValue);
                    return false;
                }
                else
                {
                    if (currentValue.Add(element, orderKey))
                    {
                        // We need to re-store this element because the pair is a value type.
                        _base[hashKey] = currentValue;
                    }
                    return true;
                }
            }
        }

        /// <summary>
        /// A wrapper for the HashLookup returned by JoinHashLookupBuilder.
        /// 
        /// Since Join operations do not require a default, this just passes the call on to the base lookup.
        /// </summary>
        private class JoinHashLookup : HashJoinHashLookup<THashKey, TElement, TOrderKey>
        {
            private readonly HashLookup<THashKey, HashLookupValueList<TElement, TOrderKey>> _base;

            internal JoinHashLookup(HashLookup<THashKey, HashLookupValueList<TElement, TOrderKey>> baseLookup)
            {
                Debug.Assert(baseLookup != null);

                _base = baseLookup;
            }

            public override bool TryGetValue(THashKey key, ref HashLookupValueList<TElement, TOrderKey> value)
            {
                return _base.TryGetValue(key, ref value);
            }
        }
    }

    /// <summary>
    /// Class to build a HashJoinHashLookup of right elements for use in GroupJoin operations.
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    /// <typeparam name="TOrderKey"></typeparam>
    /// <typeparam name="THashKey"></typeparam>
    internal class GroupJoinHashLookupBuilder<TElement, TOrderKey, THashKey> : HashLookupBuilder<IEnumerable<TElement>, int, THashKey>
    {
        private readonly QueryOperatorEnumerator<Pair<TElement, THashKey>, TOrderKey> _dataSource; // data source. For building.
        private readonly IEqualityComparer<THashKey> _keyComparer; // An optional key comparison object.

        internal GroupJoinHashLookupBuilder(QueryOperatorEnumerator<Pair<TElement, THashKey>, TOrderKey> dataSource, IEqualityComparer<THashKey> keyComparer)
        {
            Debug.Assert(dataSource != null);

            _dataSource = dataSource;
            _keyComparer = keyComparer;
        }

        public override HashJoinHashLookup<THashKey, IEnumerable<TElement>, int> BuildHashLookup(CancellationToken cancellationToken)
        {
            HashLookup<THashKey, ListChunk<TElement>> lookup = new HashLookup<THashKey, ListChunk<TElement>>(_keyComparer);
            GroupJoinBaseHashBuilder baseHashBuilder = new GroupJoinBaseHashBuilder(lookup);

            BuildBaseHashLookup(_dataSource, baseHashBuilder, cancellationToken);

            return new GroupJoinHashLookup(lookup);

        }

        /// <summary>
        /// Adds TElement values to a HashLookup of ListChunks. TOrderKey is ignored.
        /// </summary>
        private struct GroupJoinBaseHashBuilder : IBaseHashBuilder<TElement, TOrderKey>
        {
            private readonly HashLookup<THashKey, ListChunk<TElement>> _base;

            public GroupJoinBaseHashBuilder(HashLookup<THashKey, ListChunk<TElement>> baseLookup)
            {
                Debug.Assert(baseLookup != null);

                _base = baseLookup;
            }

            public bool Add(THashKey hashKey, TElement element, TOrderKey orderKey)
            {
                bool hasCollision = true;

                ListChunk<TElement> currentValue = default(ListChunk<TElement>);
                if (!_base.TryGetValue(hashKey, ref currentValue))
                {
                    const int INITIAL_CHUNK_SIZE = 2;
                    currentValue = new ListChunk<TElement>(INITIAL_CHUNK_SIZE);
                    _base.Add(hashKey, currentValue);
                    hasCollision = false;
                }

                currentValue.Add(element);

                return hasCollision;
            }
        }

        /// <summary>
        /// A wrapper for the HashLookup returned by GroupJoinHashLookupBuilder.
        /// 
        /// The order key is a dummy value since we are unordered.
        /// </summary>
        private class GroupJoinHashLookup : GroupJoinHashLookup<THashKey, TElement, ListChunk<TElement>, int>
        {
            const int OrderKey = unchecked((int)0xdeadbeef);

            internal GroupJoinHashLookup(HashLookup<THashKey, ListChunk<TElement>> lookup)
                : base(lookup)
            {
            }

            protected override int EmptyValueKey => OrderKey;

            protected override Pair<IEnumerable<TElement>, int> CreateValuePair(ListChunk<TElement> baseValue)
            {
                return new Pair<IEnumerable<TElement>, int>(baseValue, OrderKey);
            }
        }
    }

    /// <summary>
    /// Class to build a HashJoinHashLookup of ordered right elements for use in GroupJoin operations.
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    /// <typeparam name="TOrderKey"></typeparam>
    /// <typeparam name="THashKey"></typeparam>
    internal sealed class OrderedGroupJoinHashLookupBuilder<TElement, TOrderKey, THashKey> : HashLookupBuilder<IEnumerable<TElement>, Pair<bool, TOrderKey>, THashKey>
    {
        private readonly QueryOperatorEnumerator<Pair<TElement, THashKey>, TOrderKey> _dataSource; // data source. For building.
        private readonly IEqualityComparer<THashKey> _keyComparer; // An optional key comparison object.
        private readonly IComparer<TOrderKey> _orderKeyComparer;

        internal OrderedGroupJoinHashLookupBuilder(
            QueryOperatorEnumerator<Pair<TElement, THashKey>, TOrderKey> dataSource,
            IEqualityComparer<THashKey> keyComparer,
            IComparer<TOrderKey> orderKeyComparer)
        {
            Debug.Assert(dataSource != null);

            _dataSource = dataSource;
            _keyComparer = keyComparer;
            _orderKeyComparer = orderKeyComparer;
        }

        public override HashJoinHashLookup<THashKey, IEnumerable<TElement>, Pair<bool, TOrderKey>> BuildHashLookup(CancellationToken cancellationToken)
        {
            HashLookup<THashKey, GroupKeyData> lookup = new HashLookup<THashKey, GroupKeyData>(_keyComparer);
            OrderedGroupJoinBaseHashBuilder baseHashBuilder = new OrderedGroupJoinBaseHashBuilder(lookup, _orderKeyComparer);

            BuildBaseHashLookup(_dataSource, baseHashBuilder, cancellationToken);

            for (int i = 0; i < lookup.Count; i++)
            {
                lookup[i].Value._grouping.DoneAdding();
            }

            return new OrderedGroupJoinHashLookup(lookup);

        }

        /// <summary>
        /// Adds TElement values to a HashLookup of GroupKeyData. 
        /// TOrderKey is used for both ordering the elements that have the same hashKey
        /// and also for providing an order key for the resulting list.
        /// </summary>
        /// <remarks>
        /// The least order key in the list is chosen to represent the list
        /// </remarks>
        private struct OrderedGroupJoinBaseHashBuilder : IBaseHashBuilder<TElement, TOrderKey>
        {
            private readonly HashLookup<THashKey, GroupKeyData> _base;
            private readonly IComparer<TOrderKey> _orderKeyComparer;

            public OrderedGroupJoinBaseHashBuilder(
                HashLookup<THashKey, GroupKeyData> baseLookup,
                IComparer<TOrderKey> orderKeyComparer)
            {
                Debug.Assert(baseLookup != null);

                _base = baseLookup;
                _orderKeyComparer = orderKeyComparer;
            }

            public bool Add(THashKey hashKey, TElement element, TOrderKey orderKey)
            {
                bool hasCollision = true;

                GroupKeyData currentValue = default(GroupKeyData);
                if (!_base.TryGetValue(hashKey, ref currentValue))
                {
                    currentValue = new GroupKeyData(orderKey, hashKey, _orderKeyComparer);
                    _base.Add(hashKey, currentValue);
                    hasCollision = false;
                }

                currentValue._grouping.Add(element, orderKey);
                if (_orderKeyComparer.Compare(orderKey, currentValue._orderKey) < 0)
                {
                    currentValue._orderKey = orderKey;
                }

                return hasCollision;
            }
        }

        /// <summary>
        /// A wrapper for the HashLookup returned by OrderedGroupJoinHashLookupBuilder.
        /// 
        /// The order key is wrapped so that empty lists can be treated as less than all non-empty lists.
        /// </summary>
        private class OrderedGroupJoinHashLookup : GroupJoinHashLookup<THashKey, TElement, GroupKeyData, Pair<bool, TOrderKey>>
        {
            internal OrderedGroupJoinHashLookup(HashLookup<THashKey, GroupKeyData> lookup)
                : base(lookup)
            {
            }

            protected override Pair<bool, TOrderKey> EmptyValueKey => default(Pair<bool, TOrderKey>);

            protected override Pair<IEnumerable<TElement>, Pair<bool, TOrderKey>> CreateValuePair(GroupKeyData baseValue)
            {
                return new Pair<IEnumerable<TElement>, Pair<bool, TOrderKey>>(baseValue._grouping, Wrap(baseValue._orderKey));
            }

            private Pair<bool, TOrderKey> Wrap(TOrderKey orderKey)
            {
                return new Pair<bool, TOrderKey>(true, orderKey);
            }
        }

        /// <summary>
        /// A structure to hold both the elements that match a hash key and an order key for the grouping.
        /// </summary>
        private class GroupKeyData
        {
            internal TOrderKey _orderKey;
            internal OrderedGroupByGrouping<THashKey, TOrderKey, TElement> _grouping;

            internal GroupKeyData(TOrderKey orderKey, THashKey hashKey, IComparer<TOrderKey> orderComparer)
            {
                _orderKey = orderKey;
                _grouping = new OrderedGroupByGrouping<THashKey, TOrderKey, TElement>(hashKey, orderComparer);
            }
        }
    }

    /// <summary>
    /// A base wrapper for the HashLookup returned by GroupJoinHashLookupBuilder and OrderedGroupJoinHashLookupBuilder.
    /// 
    /// Since GroupJoin operations always match, if no matching elements exist, an empty enumerable is returned.
    /// </summary>
    internal abstract class GroupJoinHashLookup<THashKey, TElement, TBaseElement, TOrderKey> : HashJoinHashLookup<THashKey, IEnumerable<TElement>, TOrderKey>
    {
        private readonly HashLookup<THashKey, TBaseElement> _base;

        internal GroupJoinHashLookup(HashLookup<THashKey, TBaseElement> baseLookup)
        {
            Debug.Assert(baseLookup != null);

            _base = baseLookup;
        }

        public override bool TryGetValue(THashKey key, ref HashLookupValueList<IEnumerable<TElement>, TOrderKey> value)
        {
            Pair<IEnumerable<TElement>, TOrderKey> valueList = GetValueList(key);
            value = new HashLookupValueList<IEnumerable<TElement>, TOrderKey>(valueList.First, valueList.Second);
            return true;
        }

        private Pair<IEnumerable<TElement>, TOrderKey> GetValueList(THashKey key)
        {
            TBaseElement baseValue = default(TBaseElement);
            if (_base.TryGetValue(key, ref baseValue))
            {
                return CreateValuePair(baseValue);
            }
            else
            {
                return new Pair<IEnumerable<TElement>, TOrderKey>(ParallelEnumerable.Empty<TElement>(), EmptyValueKey);
            }
        }

        protected abstract Pair<IEnumerable<TElement>, TOrderKey> CreateValuePair(TBaseElement baseValue);
        protected abstract TOrderKey EmptyValueKey { get; }
    }

    /// <summary>
    /// A list to handle one or more right elements of a join operation.
    /// 
    /// It optimizes for 1 to 1 joins by only allocating heap space lazily
    /// once a second value is added.
    /// 
    /// This is built in the HashLookupBuilder classes and consumed by the
    /// HashJoinQueryOperatorEnumerator.
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    /// <typeparam name="TOrderKey"></typeparam>
    internal struct HashLookupValueList<TElement, TOrderKey>
    {
        internal Pair<TElement, TOrderKey> Head
        {
            get
            {
                return _head;
            }
        }
        private readonly Pair<TElement, TOrderKey> _head;

        internal ListChunk<Pair<TElement, TOrderKey>> Tail
        {
            get
            {
                return _tail;
            }
        }
        private ListChunk<Pair<TElement, TOrderKey>> _tail;

        private const int INITIAL_CHUNK_SIZE = 2;

        // constructor used to build a new list.
        internal HashLookupValueList(TElement firstValue, TOrderKey firstOrderKey)
        {
            _head = CreatePair(firstValue, firstOrderKey);
            _tail = null;
        }

        /// <summary>
        /// Adds a value/ordering key pair to the list.
        /// </summary>
        /// <param name="value">value to add</param>
        /// <param name="orderKey">ordering key</param>
        /// <returns>if true, the internal memory has changed</returns>
        /// <remarks>
        /// As this is a value type, if the internal memory changes,
        /// then the changes need to be reflected (to a HashLookup, for example)
        /// as necessary
        /// </remarks>
        internal bool Add(TElement value, TOrderKey orderKey)
        {
            bool requiresMemoryChange = (_tail == null);

            if (requiresMemoryChange)
            {
                _tail = new ListChunk<Pair<TElement, TOrderKey>>(INITIAL_CHUNK_SIZE);
            }
            _tail.Add(CreatePair(value, orderKey));

            return requiresMemoryChange;
        }

        private static Pair<TElement, TOrderKey> CreatePair(TElement value, TOrderKey orderKey)
        {
            return new Pair<TElement, TOrderKey>(value, orderKey);
        }
    }
}

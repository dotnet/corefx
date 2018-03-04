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
    internal class HashJoinQueryOperatorEnumerator<TLeftInput, TLeftKey, TRightInput, TRightKey, THashKey, TOutput>
        : QueryOperatorEnumerator<TOutput, TLeftKey>
    {
        private readonly QueryOperatorEnumerator<Pair<TLeftInput, THashKey>, TLeftKey> _leftSource; // Left (outer) data source. For probing.
        private readonly HashLookupBuilder<TRightInput, TRightKey, THashKey> _rightLookupBuilder; // Right (inner) data source. For building.
        private readonly Func<TLeftInput, TRightInput, TOutput> _singleResultSelector; // Single result selector.
        private readonly Func<TLeftInput, IEnumerable<TRightInput>, TOutput> _groupResultSelector; // Group result selector.
        private readonly CancellationToken _cancellationToken;
        private Mutables _mutables;

        private class Mutables
        {
            internal TLeftInput _currentLeft; // The current matching left element.
            internal TLeftKey _currentLeftKey; // The current index of the matching left element.
            internal HashJoinHashLookup<THashKey, TRightInput, TRightKey> _rightHashLookup; // The hash lookup.
            internal HashLookupValueList<TRightInput, TRightKey> _currentRightMatches; // Remaining right matches (if any).
            internal int _outputLoopCount;
        }

        //---------------------------------------------------------------------------------------
        // Instantiates a new hash-join enumerator.
        //

        internal HashJoinQueryOperatorEnumerator(
            QueryOperatorEnumerator<Pair<TLeftInput, THashKey>, TLeftKey> leftSource,
            QueryOperatorEnumerator<Pair<TRightInput, THashKey>, TRightKey> rightSource,
            Func<TLeftInput, TRightInput, TOutput> singleResultSelector,
            Func<TLeftInput, IEnumerable<TRightInput>, TOutput> groupResultSelector,
            IEqualityComparer<THashKey> keyComparer,
            CancellationToken cancellationToken)
            : this(leftSource, new JoinHashLookupBuilder<TRightInput, TRightKey, THashKey>(rightSource, keyComparer),
                  singleResultSelector, groupResultSelector, cancellationToken)
        {
        }

        internal HashJoinQueryOperatorEnumerator(
            QueryOperatorEnumerator<Pair<TLeftInput, THashKey>, TLeftKey> leftSource,
            HashLookupBuilder<TRightInput, TRightKey, THashKey> rightLookupBuilder,
            Func<TLeftInput, TRightInput, TOutput> singleResultSelector,
            Func<TLeftInput, IEnumerable<TRightInput>, TOutput> groupResultSelector,
            CancellationToken cancellationToken)
        {
            Debug.Assert(leftSource != null);
            Debug.Assert(rightLookupBuilder != null);
            Debug.Assert(singleResultSelector != null || groupResultSelector != null);

            _leftSource = leftSource;
            _rightLookupBuilder = rightLookupBuilder;
            _singleResultSelector = singleResultSelector;
            _groupResultSelector = groupResultSelector;
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

        internal override bool MoveNext(ref TOutput currentElement, ref TLeftKey currentKey)
        {
            Debug.Assert(_singleResultSelector != null || _groupResultSelector != null, "expected a compiled result selector");
            Debug.Assert(_leftSource != null);
            Debug.Assert(_rightLookupBuilder != null);

            // BUILD phase: If we haven't built the hash-table yet, create that first.
            Mutables mutables = _mutables;
            if (mutables == null)
            {
                mutables = _mutables = new Mutables();
                Debug.Assert(mutables._currentRightMatches.HasNext() == false, "empty list expected");

                mutables._rightHashLookup = _rightLookupBuilder.BuildHashLookup(_cancellationToken);
            }

            // PROBE phase: So long as the source has a next element, return the match.
            TRightInput rightElement = default(TRightInput);
            TRightKey rightKeyUnused = default(TRightKey);
            if (!mutables._currentRightMatches.MoveNext(ref rightElement, ref rightKeyUnused, ref mutables._currentRightMatches))
            {
                Debug.Assert(mutables._currentRightMatches.HasNext() == false, "empty list expected");

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
                            Debug.Assert(matchValue.HasNext(), "non-empty list expected");

                            // We found a new match. For inner joins, we remember the list in case
                            // there are multiple value under this same key -- the next iteration will pick
                            // them up. For outer joins, we will use the list momentarily.
                            if (_singleResultSelector != null)
                            {
                                bool hadNext = matchValue.MoveNext(ref rightElement, ref rightKeyUnused, ref mutables._currentRightMatches);
                                Debug.Assert(hadNext, "we were expecting MoveNext to return true (since the list should be non-empty)");

                                // Yield the value.
                                currentElement = _singleResultSelector(leftElement, rightElement);
                                currentKey = leftKey;

                                // If there is a list of matches, remember the left values for next time.
                                if (mutables._currentRightMatches.HasNext())
                                {
                                    mutables._currentLeft = leftElement;
                                    mutables._currentLeftKey = leftKey;
                                }

                                return true;
                            }
                        }
                    }

                    // For outer joins, we always yield a result.
                    if (_groupResultSelector != null)
                    {
                        // Generate the current value. If no match was found,
                        // matchValue will produce an empty enumerable.
                        currentElement = _groupResultSelector(leftElement, matchValue.AsEnumerable());
                        currentKey = leftKey;
                        return true;
                    }
                }

                // If we've reached the end of the data source, we're done.
                return false;
            }

            // Produce the next element.
            Debug.Assert(_singleResultSelector != null);

            currentElement = _singleResultSelector(mutables._currentLeft, rightElement);
            currentKey = mutables._currentLeftKey;

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
    /// Class to build a HashJoinHashLookup of right elements for use in HashJoin operations.
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    /// <typeparam name="TOrderKey"></typeparam>
    /// <typeparam name="THashKey"></typeparam>
    internal abstract class HashLookupBuilder<TElement, TOrderKey, THashKey>
    {
        public abstract HashJoinHashLookup<THashKey, TElement, TOrderKey> BuildHashLookup(CancellationToken cancellationToken);

        // Standard implementation of the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
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
            Debug.Assert(_dataSource != null);

#if DEBUG
            int hashLookupCount = 0;
            int hashKeyCollisions = 0;
#endif

            var lookup = new HashLookup<THashKey, HashLookupValueList<TElement, TOrderKey>>(_keyComparer);

            Pair<TElement, THashKey> currentPair = default(Pair<TElement, THashKey>);
            TOrderKey orderKey = default(TOrderKey);
            int i = 0;
            while (_dataSource.MoveNext(ref currentPair, ref orderKey))
            {
                if ((i++ & CancellationState.POLL_INTERVAL) == 0)
                    CancellationState.ThrowIfCanceled(cancellationToken);

                TElement element = currentPair.First;
                THashKey hashKey = currentPair.Second;

                // We ignore null keys.
                if (hashKey != null)
                {
#if DEBUG
                    hashLookupCount++;
#endif

                    // See if we've already stored an element under the current key. If not, we
                    // add a HashLookupValueList to hold the elements mapping to the same key.
                    HashLookupValueList<TElement, TOrderKey> currentValue = default(HashLookupValueList<TElement, TOrderKey>);
                    if (!lookup.TryGetValue(hashKey, ref currentValue))
                    {
                        currentValue = new HashLookupValueList<TElement, TOrderKey>(element, orderKey);
                        lookup.Add(hashKey, currentValue);
                    }
                    else
                    {
                        if (currentValue.Add(element, orderKey))
                        {
                            // We need to re-store this element because the pair is a value type.
                            lookup[hashKey] = currentValue;
                        }
#if DEBUG
                        hashKeyCollisions++;
#endif
                    }
                }
            }

#if DEBUG
            TraceHelpers.TraceInfo("ParallelJoinQueryOperator::BuildHashLookup - built hash table [count = {0}, collisions = {1}]",
                hashLookupCount, hashKeyCollisions);
#endif

            return new JoinHashLookup<THashKey, TElement, TOrderKey>(lookup);
        }

        protected override void Dispose(bool disposing)
        {
            Debug.Assert(_dataSource != null);

            _dataSource.Dispose();
        }
    }

    /// <summary>
    /// A wrapper for the HashLookup returned by JoinHashLookupBuilder.
    /// 
    /// Since Join operations do not require a default, this just passes the call on to the base lookup.
    /// </summary>
    /// <typeparam name="THashKey"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    /// <typeparam name="TOrderKey"></typeparam>
    internal class JoinHashLookup<THashKey, TElement, TOrderKey> : HashJoinHashLookup<THashKey, TElement, TOrderKey>
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
        private readonly Pair<TElement, TOrderKey> _head;
        private ListChunk<Pair<TElement, TOrderKey>> _tail;
        private int _currentIndex;

        private const int Head = -1;
        private const int INITIAL_CHUNK_SIZE = 2;

        // constructor used to build a new list.
        internal HashLookupValueList(TElement firstValue, TOrderKey firstOrderKey)
        {
            _head = CreatePair(firstValue, firstOrderKey);
            _tail = null;
            _currentIndex = Head;
        }

        // constructor used for enumeration.
        private HashLookupValueList(ListChunk<Pair<TElement, TOrderKey>> rest, int nextIndex)
        {
            Debug.Assert(nextIndex >= 0, "nextIndex must be non-negative");
            Debug.Assert(rest == null || nextIndex < rest.Count, "nextIndex not a valid index in chunk rest");

            _head = default;
            _tail = rest;
            _currentIndex = nextIndex;
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
            Debug.Assert(_currentIndex == Head, "expected a non-empty, non-enumerated list");

            bool requiresMemoryChange = (_tail == null);

            if (requiresMemoryChange)
            {
                _tail = new ListChunk<Pair<TElement, TOrderKey>>(INITIAL_CHUNK_SIZE);
            }
            _tail.Add(CreatePair(value, orderKey));

            return requiresMemoryChange;
        }

        /// <summary>
        /// Retrieves the next element and remaining values.
        /// </summary>
        /// <param name="currentElement"></param>
        /// <param name="currentKey"></param>
        /// <param name="remainingValues"></param>
        /// <returns>if true, a next element existed.</returns>
        public bool MoveNext(ref TElement currentElement, ref TOrderKey currentKey, ref HashLookupValueList<TElement, TOrderKey> remainingValues)
        {
            if (_currentIndex == Head)
            {
                currentElement = _head.First;
                currentKey = _head.Second;
                remainingValues = CreateRemainingList(_tail, 0);
                return true;
            }

            if (_tail != null)
            {
                Pair<TElement, TOrderKey> current = _tail._chunk[_currentIndex];
                currentElement = current.First;
                currentKey = current.Second;

                var nextIndex = _currentIndex + 1;
                if (nextIndex < _tail.Count)
                {
                    remainingValues = CreateRemainingList(_tail, nextIndex);
                }
                else
                {
                    remainingValues = CreateRemainingList(_tail.Next, 0);
                }
                return true;
            }

            return false;
        }

        //TODO reevaluate if this is necessary after refactoring complete
        internal bool HasNext()
        {
            return _currentIndex == Head || _tail != null;
        }

        //TODO reevaluate if this is necessary after refactoring complete
        internal IEnumerable<TElement> AsEnumerable()
        {
            HashLookupValueList<TElement, TOrderKey> remainder = this;
            TElement element = default;
            TOrderKey keyUnused = default;
            while (remainder.MoveNext(ref element, ref keyUnused, ref remainder))
            {
                yield return element;
            }
        }

        private static HashLookupValueList<TElement, TOrderKey> CreateRemainingList(ListChunk<Pair<TElement, TOrderKey>> nextChunk, int nextIndex)
        {
            return new HashLookupValueList<TElement, TOrderKey>(nextChunk, nextIndex);
        }

        private static Pair<TElement, TOrderKey> CreatePair(TElement value, TOrderKey orderKey)
        {
            return new Pair<TElement, TOrderKey>(value, orderKey);
        }
    }
}

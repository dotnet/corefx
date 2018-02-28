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
        private readonly QueryOperatorEnumerator<Pair<TRightInput, THashKey>, TRightKey> _rightSource; // Right (inner) data source. For building.
        private readonly Func<TLeftInput, TRightInput, TOutput> _singleResultSelector; // Single result selector.
        private readonly Func<TLeftInput, IEnumerable<TRightInput>, TOutput> _groupResultSelector; // Group result selector.
        private readonly IEqualityComparer<THashKey> _keyComparer; // An optional key comparison object.
        private readonly CancellationToken _cancellationToken;
        private Mutables _mutables;

        private class Mutables
        {
            internal TLeftInput _currentLeft; // The current matching left element.
            internal TLeftKey _currentLeftKey; // The current index of the matching left element.
            internal HashLookup<THashKey, HashLookupValueList<TRightInput, TRightKey>> _rightHashLookup; // The hash lookup.
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
        {
            Debug.Assert(leftSource != null);
            Debug.Assert(rightSource != null);
            Debug.Assert(singleResultSelector != null || groupResultSelector != null);

            _leftSource = leftSource;
            _rightSource = rightSource;
            _singleResultSelector = singleResultSelector;
            _groupResultSelector = groupResultSelector;
            _keyComparer = keyComparer;
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
            Debug.Assert(_rightSource != null);

            // BUILD phase: If we haven't built the hash-table yet, create that first.
            Mutables mutables = _mutables;
            if (mutables == null)
            {
                mutables = _mutables = new Mutables();
                Debug.Assert(mutables._currentRightMatches.HasNext() == false, "empty list expected");

                mutables._rightHashLookup = BuildHashLookup();
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

        //TODO this will be moved to a separate class to customize for GroupJoin ordered and unordered
        private HashLookup<THashKey, HashLookupValueList<TRightInput, TRightKey>> BuildHashLookup()
        {
#if DEBUG
            int hashLookupCount = 0;
            int hashKeyCollisions = 0;
#endif

            var lookup = new HashLookup<THashKey, HashLookupValueList<TRightInput, TRightKey>>(_keyComparer);

            Pair<TRightInput, THashKey> rightPair = default(Pair<TRightInput, THashKey>);
            TRightKey rightKey = default(TRightKey);
            int i = 0;
            while (_rightSource.MoveNext(ref rightPair, ref rightKey))
            {
                if ((i++ & CancellationState.POLL_INTERVAL) == 0)
                    CancellationState.ThrowIfCanceled(_cancellationToken);

                TRightInput rightElement = rightPair.First;
                THashKey rightHashKey = rightPair.Second;

                // We ignore null keys.
                if (rightHashKey != null)
                {
#if DEBUG
                    hashLookupCount++;
#endif

                    // See if we've already stored an element under the current key. If not, we
                    // add a RightValueList to hold the elements mapping to the same key.
                    HashLookupValueList<TRightInput, TRightKey> currentValue = default(HashLookupValueList<TRightInput, TRightKey>);
                    if (!lookup.TryGetValue(rightHashKey, ref currentValue))
                    {
                        currentValue = new HashLookupValueList<TRightInput, TRightKey>(rightElement, rightKey);
                        lookup.Add(rightHashKey, currentValue);
                    }
                    else
                    {
                        if (currentValue.Add(rightElement, rightKey))
                        {
                            // We need to re-store this element because the pair is a value type.
                            lookup[rightHashKey] = currentValue;
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

            return lookup;
        }

        protected override void Dispose(bool disposing)
        {
            Debug.Assert(_leftSource != null && _rightSource != null);
            _leftSource.Dispose();
            _rightSource.Dispose();
        }
    }

    internal struct HashLookupValueList<TRightInput, TRightKey>
    {
        private readonly Pair<TRightInput, TRightKey> _head;
        private ListChunk<Pair<TRightInput, TRightKey>> _tail;
        private int _currentIndex;

        private const int Head = -1;
        private const int INITIAL_CHUNK_SIZE = 2;

        // constructor used to build a new list.
        public HashLookupValueList(TRightInput firstValue, TRightKey firstOrderKey)
        {
            _head = CreatePair(firstValue, firstOrderKey);
            _tail = null;
            _currentIndex = Head;
        }

        // constructor used for enumeration.
        private HashLookupValueList(ListChunk<Pair<TRightInput, TRightKey>> rest, int nextIndex)
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
        internal bool Add(TRightInput value, TRightKey orderKey)
        {
            Debug.Assert(_currentIndex == Head, "expected a non-empty, non-enumerated list");

            bool requiresMemoryChange = (_tail == null);

            if (requiresMemoryChange)
            {
                _tail = new ListChunk<Pair<TRightInput, TRightKey>>(INITIAL_CHUNK_SIZE);
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
        public bool MoveNext(ref TRightInput currentElement, ref TRightKey currentKey, ref HashLookupValueList<TRightInput, TRightKey> remainingValues)
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
                Pair<TRightInput, TRightKey> current = _tail._chunk[_currentIndex];
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
        internal IEnumerable<TRightInput> AsEnumerable()
        {
            HashLookupValueList<TRightInput, TRightKey> remainder = this;
            TRightInput element = default;
            TRightKey keyUnused = default;
            while (remainder.MoveNext(ref element, ref keyUnused, ref remainder))
            {
                yield return element;
            }
        }

        private static HashLookupValueList<TRightInput, TRightKey> CreateRemainingList(ListChunk<Pair<TRightInput, TRightKey>> nextChunk, int nextIndex)
        {
            return new HashLookupValueList<TRightInput, TRightKey>(nextChunk, nextIndex);
        }

        private static Pair<TRightInput, TRightKey> CreatePair(TRightInput value, TRightKey orderKey)
        {
            return new Pair<TRightInput, TRightKey>(value, orderKey);
        }
    }
}

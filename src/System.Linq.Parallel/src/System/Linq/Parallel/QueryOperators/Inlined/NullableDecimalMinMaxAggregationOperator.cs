// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// =+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
//
// NullableDecimalMinMaxAggregationOperator.cs
//
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace System.Linq.Parallel
{
    /// <summary>
    /// An inlined min/max aggregation and its enumerator, for Nullable decimals. 
    /// </summary>
    internal sealed class NullableDecimalMinMaxAggregationOperator : InlinedAggregationOperator<decimal?, decimal?, decimal?>
    {
        private readonly int _sign; // The sign (-1 for min, 1 for max).

        //---------------------------------------------------------------------------------------
        // Constructs a new instance of a min/max associative operator.
        //

        internal NullableDecimalMinMaxAggregationOperator(IEnumerable<decimal?> child, int sign) : base(child)
        {
            Debug.Assert(sign == -1 || sign == 1, "invalid sign");
            _sign = sign;
        }

        //---------------------------------------------------------------------------------------
        // Executes the entire query tree, and aggregates the intermediate results into the
        // final result based on the binary operators and final reduction.
        //
        // Return Value:
        //     The single result of aggregation.
        //

        protected override decimal? InternalAggregate(ref Exception singularExceptionToThrow)
        {
            // Because the final reduction is typically much cheaper than the intermediate 
            // reductions over the individual partitions, and because each parallel partition
            // will do a lot of work to produce a single output element, we prefer to turn off
            // pipelining, and process the final reductions serially.
            using (IEnumerator<decimal?> enumerator = GetEnumerator(ParallelMergeOptions.FullyBuffered, true))
            {
                // Just return null right away for empty results.
                if (!enumerator.MoveNext())
                {
                    return null;
                }

                decimal? best = enumerator.Current;

                // Based on the sign, do either a min or max reduction.
                if (_sign == -1)
                {
                    while (enumerator.MoveNext())
                    {
                        decimal? current = enumerator.Current;
                        if (best == null || current < best)
                        {
                            best = current;
                        }
                    }
                }
                else
                {
                    while (enumerator.MoveNext())
                    {
                        decimal? current = enumerator.Current;
                        if (best == null || current > best)
                        {
                            best = current;
                        }
                    }
                }

                return best;
            }
        }

        //---------------------------------------------------------------------------------------
        // Creates an enumerator that is used internally for the final aggregation step.
        //

        protected override QueryOperatorEnumerator<decimal?, int> CreateEnumerator<TKey>(
            int index, int count, QueryOperatorEnumerator<decimal?, TKey> source, object sharedData, CancellationToken cancellationToken)
        {
            return new NullableDecimalMinMaxAggregationOperatorEnumerator<TKey>(source, index, _sign, cancellationToken);
        }

        //---------------------------------------------------------------------------------------
        // This enumerator type encapsulates the intermediary aggregation over the underlying
        // (possibly partitioned) data source.
        //

        private class NullableDecimalMinMaxAggregationOperatorEnumerator<TKey> : InlinedAggregationOperatorEnumerator<decimal?>
        {
            private QueryOperatorEnumerator<decimal?, TKey> _source; // The source data.
            private int _sign; // The sign for comparisons (-1 means min, 1 means max).

            //---------------------------------------------------------------------------------------
            // Instantiates a new aggregation operator.
            //

            internal NullableDecimalMinMaxAggregationOperatorEnumerator(QueryOperatorEnumerator<decimal?, TKey> source, int partitionIndex, int sign,
                CancellationToken cancellationToken) :
                base(partitionIndex, cancellationToken)
            {
                Debug.Assert(source != null);
                _source = source;
                _sign = sign;
            }

            //---------------------------------------------------------------------------------------
            // Tallies up the min/max of the underlying data source, walking the entire thing the first
            // time MoveNext is called on this object.
            //

            protected override bool MoveNextCore(ref decimal? currentElement)
            {
                // Based on the sign, do either a min or max reduction.
                QueryOperatorEnumerator<decimal?, TKey> source = _source;
                TKey keyUnused = default(TKey);

                if (source.MoveNext(ref currentElement, ref keyUnused))
                {
                    int i = 0;
                    // We just scroll through the enumerator and find the min or max.
                    if (_sign == -1)
                    {
                        decimal? elem = default(decimal?);
                        while (source.MoveNext(ref elem, ref keyUnused))
                        {
                            if ((i++ & CancellationState.POLL_INTERVAL) == 0)
                                CancellationState.ThrowIfCanceled(_cancellationToken);

                            if (currentElement == null || elem < currentElement)
                            {
                                currentElement = elem;
                            }
                        }
                    }
                    else
                    {
                        decimal? elem = default(decimal?);
                        while (source.MoveNext(ref elem, ref keyUnused))
                        {
                            if ((i++ & CancellationState.POLL_INTERVAL) == 0)
                                CancellationState.ThrowIfCanceled(_cancellationToken);

                            if (currentElement == null || elem > currentElement)
                            {
                                currentElement = elem;
                            }
                        }
                    }

                    // The sum has been calculated. Now just return.
                    return true;
                }

                return false;
            }

            //---------------------------------------------------------------------------------------
            // Dispose of resources associated with the underlying enumerator.
            //

            protected override void Dispose(bool disposing)
            {
                Debug.Assert(_source != null);
                _source.Dispose();
            }
        }
    }
}

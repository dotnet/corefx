// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// =+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
//
// GroupJoinQueryOperator.cs
//
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace System.Linq.Parallel
{
    /// <summary>
    /// A group join operator takes a left query tree and a right query tree, and then yields
    /// the matching elements between the two. This can be used for outer joins, i.e. those
    /// where an outer element has no matching inner elements -- the result is just an empty
    /// list. As with the join algorithm above, we currently use a hash join algorithm.
    /// </summary>
    /// <typeparam name="TLeftInput"></typeparam>
    /// <typeparam name="TRightInput"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    internal sealed class GroupJoinQueryOperator<TLeftInput, TRightInput, TKey, TOutput> : BinaryQueryOperator<TLeftInput, TRightInput, TOutput>
    {
        private readonly Func<TLeftInput, TKey> _leftKeySelector; // The key selection routine for the outer (left) data source.
        private readonly Func<TRightInput, TKey> _rightKeySelector; // The key selection routine for the inner (right) data source.
        private readonly Func<TLeftInput, IEnumerable<TRightInput>, TOutput> _resultSelector; // The result selection routine.
        private readonly IEqualityComparer<TKey> _keyComparer; // An optional key comparison object.

        //---------------------------------------------------------------------------------------
        // Constructs a new join operator.
        //

        internal GroupJoinQueryOperator(ParallelQuery<TLeftInput> left, ParallelQuery<TRightInput> right,
                                        Func<TLeftInput, TKey> leftKeySelector,
                                        Func<TRightInput, TKey> rightKeySelector,
                                        Func<TLeftInput, IEnumerable<TRightInput>, TOutput> resultSelector,
                                        IEqualityComparer<TKey> keyComparer)
            : base(left, right)
        {
            Debug.Assert(left != null && right != null, "child data sources cannot be null");
            Debug.Assert(leftKeySelector != null, "left key selector must not be null");
            Debug.Assert(rightKeySelector != null, "right key selector must not be null");
            Debug.Assert(resultSelector != null, "need a result selector function");

            _leftKeySelector = leftKeySelector;
            _rightKeySelector = rightKeySelector;
            _resultSelector = resultSelector;
            _keyComparer = keyComparer;
            _outputOrdered = LeftChild.OutputOrdered;

            SetOrdinalIndex(OrdinalIndexState.Shuffled);
        }

        //---------------------------------------------------------------------------------------
        // Just opens the current operator, including opening the child and wrapping it with
        // partitions as needed.
        //

        internal override QueryResults<TOutput> Open(QuerySettings settings, bool preferStriping)
        {
            QueryResults<TLeftInput> leftResults = LeftChild.Open(settings, false);
            QueryResults<TRightInput> rightResults = RightChild.Open(settings, false);

            return new BinaryQueryOperatorResults(leftResults, rightResults, this, settings, false);
        }

        public override void WrapPartitionedStream<TLeftKey, TRightKey>(
            PartitionedStream<TLeftInput, TLeftKey> leftStream, PartitionedStream<TRightInput, TRightKey> rightStream,
            IPartitionedStreamRecipient<TOutput> outputRecipient, bool preferStriping, QuerySettings settings)
        {
            Debug.Assert(rightStream.PartitionCount == leftStream.PartitionCount);
            int partitionCount = leftStream.PartitionCount;

            if (LeftChild.OutputOrdered)
            {
                WrapPartitionedStreamHelper<TLeftKey, TRightKey>(
                    ExchangeUtilities.HashRepartitionOrdered(leftStream, _leftKeySelector, _keyComparer, null, settings.CancellationState.MergedCancellationToken),
                    rightStream, outputRecipient, partitionCount, settings.CancellationState.MergedCancellationToken);
            }
            else
            {
                WrapPartitionedStreamHelper<int, TRightKey>(
                    ExchangeUtilities.HashRepartition(leftStream, _leftKeySelector, _keyComparer, null, settings.CancellationState.MergedCancellationToken),
                    rightStream, outputRecipient, partitionCount, settings.CancellationState.MergedCancellationToken);
            }
        }

        //---------------------------------------------------------------------------------------
        // This is a helper method. WrapPartitionedStream decides what type TLeftKey is going
        // to be, and then call this method with that key as a generic parameter.
        //

        private void WrapPartitionedStreamHelper<TLeftKey, TRightKey>(
            PartitionedStream<Pair<TLeftInput,TKey>, TLeftKey> leftHashStream, PartitionedStream<TRightInput, TRightKey> rightPartitionedStream,
            IPartitionedStreamRecipient<TOutput> outputRecipient, int partitionCount, CancellationToken cancellationToken)
        {
            if (RightChild.OutputOrdered)
            {
                PartitionedStream<Pair<TRightInput, TKey>, TRightKey> rePartitionedRightStream = ExchangeUtilities.HashRepartitionOrdered(
                    rightPartitionedStream, _rightKeySelector, _keyComparer, null, cancellationToken);

                HashLookupBuilder<IEnumerable<TRightInput>, Pair<bool, TRightKey>, TKey>[] rightLookupBuilders =
                    new HashLookupBuilder<IEnumerable<TRightInput>, Pair<bool, TRightKey>, TKey>[partitionCount];
                for (int i = 0; i < partitionCount; i++)
                {
                    rightLookupBuilders[i] = new OrderedGroupJoinHashLookupBuilder<TRightInput, TRightKey, TKey>(
                        rePartitionedRightStream[i], _keyComparer, rePartitionedRightStream.KeyComparer);
                }

                WrapPartitionedStreamHelper<TLeftKey, Pair<bool, TRightKey>>(leftHashStream, rightLookupBuilders,
                    CreateComparer(rightPartitionedStream.KeyComparer), outputRecipient, partitionCount, cancellationToken);
            }
            else
            {
                PartitionedStream<Pair<TRightInput, TKey>, int> rePartitionedRightStream = ExchangeUtilities.HashRepartition(
                    rightPartitionedStream, _rightKeySelector, _keyComparer, null, cancellationToken);

                HashLookupBuilder<IEnumerable<TRightInput>, int, TKey>[] rightLookupBuilders =
                    new HashLookupBuilder<IEnumerable<TRightInput>, int, TKey>[partitionCount];
                for (int i = 0; i < partitionCount; i++)
                {
                    rightLookupBuilders[i] = new GroupJoinHashLookupBuilder<TRightInput, int, TKey>(
                        rePartitionedRightStream[i], _keyComparer);
                }

                WrapPartitionedStreamHelper<TLeftKey, int>(leftHashStream, rightLookupBuilders,
                    null, outputRecipient, partitionCount, cancellationToken);
            }
        }

        private void WrapPartitionedStreamHelper<TLeftKey, TRightKey>(
            PartitionedStream<Pair<TLeftInput, TKey>, TLeftKey> leftHashStream,
            HashLookupBuilder<IEnumerable<TRightInput>, TRightKey, TKey>[] rightLookupBuilders,
            IComparer<TRightKey> rightKeyComparer, IPartitionedStreamRecipient<TOutput> outputRecipient,
            int partitionCount, CancellationToken cancellationToken)
        {
            if (RightChild.OutputOrdered && LeftChild.OutputOrdered)
            {
                LeftKeyOutputKeyBuilder<TLeftKey, TRightKey> outputKeyBuilder = new LeftKeyOutputKeyBuilder<TLeftKey, TRightKey>();

                WrapPartitionedStreamHelper<TLeftKey, TRightKey, TLeftKey>(leftHashStream, rightLookupBuilders,
                    outputKeyBuilder, leftHashStream.KeyComparer, outputRecipient, partitionCount, cancellationToken);
            }
            else
            {
                LeftKeyOutputKeyBuilder<TLeftKey, TRightKey> outputKeyBuilder = new LeftKeyOutputKeyBuilder<TLeftKey, TRightKey>();

                WrapPartitionedStreamHelper<TLeftKey, TRightKey, TLeftKey>(leftHashStream, rightLookupBuilders,
                    outputKeyBuilder, leftHashStream.KeyComparer, outputRecipient, partitionCount, cancellationToken);
            }
        }

        private IComparer<Pair<bool, TRightKey>> CreateComparer<TRightKey>(IComparer<TRightKey> comparer)
        {
            return CreateComparer(Comparer<bool>.Default, comparer);
        }

        private IComparer<Pair<TLeftKey, TRightKey>> CreateComparer<TLeftKey, TRightKey>(IComparer<TLeftKey> leftKeyComparer, IComparer<TRightKey> rightKeyComparer)
        {
            return new PairComparer<TLeftKey, TRightKey>(leftKeyComparer, rightKeyComparer);
        }

        private void WrapPartitionedStreamHelper<TLeftKey, TRightKey, TOutputKey>(
            PartitionedStream<Pair<TLeftInput, TKey>, TLeftKey> leftHashStream,
            HashLookupBuilder<IEnumerable<TRightInput>, TRightKey, TKey>[] rightLookupBuilders,
            HashJoinOutputKeyBuilder<TLeftKey, TRightKey, TOutputKey> outputKeyBuilder, IComparer<TOutputKey> outputKeyComparer,
            IPartitionedStreamRecipient<TOutput> outputRecipient, int partitionCount, CancellationToken cancellationToken)
        {
            PartitionedStream<TOutput, TOutputKey> outputStream = new PartitionedStream<TOutput, TOutputKey>(
                partitionCount, outputKeyComparer, OrdinalIndexState);

            for (int i = 0; i < partitionCount; i++)
            {
                outputStream[i] = new HashJoinQueryOperatorEnumerator<TLeftInput, TLeftKey, IEnumerable<TRightInput>, TRightKey, TKey, TOutput, TOutputKey>(
                    leftHashStream[i], rightLookupBuilders[i], _resultSelector, outputKeyBuilder, cancellationToken);
            }

            outputRecipient.Receive(outputStream);
        }

        //---------------------------------------------------------------------------------------
        // Returns an enumerable that represents the query executing sequentially.
        //

        internal override IEnumerable<TOutput> AsSequentialQuery(CancellationToken token)
        {
            IEnumerable<TLeftInput> wrappedLeftChild = CancellableEnumerable.Wrap(LeftChild.AsSequentialQuery(token), token);
            IEnumerable<TRightInput> wrappedRightChild = CancellableEnumerable.Wrap(RightChild.AsSequentialQuery(token), token);

            return wrappedLeftChild
                .GroupJoin(
                wrappedRightChild, _leftKeySelector, _rightKeySelector, _resultSelector, _keyComparer);
        }

        //---------------------------------------------------------------------------------------
        // Whether this operator performs a premature merge that would not be performed in
        // a similar sequential operation (i.e., in LINQ to Objects).
        //

        internal override bool LimitsParallelism
        {
            get { return false; }
        }
    }
}

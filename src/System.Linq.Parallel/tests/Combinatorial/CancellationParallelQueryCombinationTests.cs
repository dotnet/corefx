// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using Xunit;

namespace System.Linq.Parallel.Tests
{
    public partial class ParallelQueryCombinationTests
    {
        private const int EventualCancellationSize = 128;

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        public static void Aggregate_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Aggregate((i, j) => j));
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Aggregate(0, (i, j) => j));
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Aggregate(0, (i, j) => j, i => i));
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Aggregate(0, (i, j) => j, (i, j) => i, i => i));
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Aggregate(() => 0, (i, j) => j, (i, j) => i, i => i));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        public static void Aggregate_AggregateException_Wraps_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Aggregate((i, j) => j));
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Aggregate(0, (i, j) => j));
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Aggregate(0, (i, j) => j, i => i));
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Aggregate(0, (i, j) => j, (i, j) => i, i => i));
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Aggregate(() => 0, (i, j) => j, (i, j) => i, i => i));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void Aggregate_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).Aggregate((x, y) => x));
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).Aggregate(0, (x, y) => x + y));
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).Aggregate(0, (x, y) => x + y, r => r));
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).Aggregate(0, (a, x) => a + x, (l, r) => l + r, r => r));
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).Aggregate(() => 0, (a, x) => a + x, (l, r) => l + r, r => r));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        public static void All_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).All(x => true));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        public static void All_AggregateException_Wraps_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).All(x => true));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void All_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).All(x => true));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        public static void Any_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Any(x => false));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        public static void Any_AggregateException_Wraps_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Any(x => false));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void Any_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).Any());
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).Any(x => true));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        public static void Average_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Average());
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Average(x => (int?)x));

            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Average(x => (long)x));
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Average(x => (long?)x));

            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Average(x => (float)x));
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Average(x => (float?)x));

            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Average(x => (double)x));
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Average(x => (double?)x));

            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Average(x => (decimal)x));
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Average(x => (decimal?)x));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        public static void Average_AggregateException_Wraps_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Average());
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Average(x => (int?)x));

            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Average(x => (long)x));
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Average(x => (long?)x));

            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Average(x => (float)x));
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Average(x => (float?)x));

            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Average(x => (double)x));
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Average(x => (double?)x));

            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Average(x => (decimal)x));
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Average(x => (decimal?)x));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void Average_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).Average());
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        public static void Contains_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Contains(-1));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        public static void Contains_AggregateException_Wraps_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Contains(-1));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void Contains_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).Contains(DefaultStart));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        public static void Count_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Count());
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).LongCount());
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Count(x => true));
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).LongCount(x => true));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        public static void Count_AggregateException_Wraps_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Count());
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).LongCount());
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Count(x => true));
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).LongCount(x => true));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void Count_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).Count());
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).LongCount());
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).Count(x => true));
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).LongCount(x => true));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        [MemberData(nameof(OrderCancelingOperators))]
        public static void ElementAt_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).ElementAt(int.MaxValue));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        [MemberData(nameof(OrderCancelingOperators))]
        public static void ElementAt_AggregateException_Wraps_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).ElementAt(int.MaxValue));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void ElementAt_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).ElementAt(0));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        [MemberData(nameof(OrderCancelingOperators))]
        public static void ElementAtOrDefault_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).ElementAt(int.MaxValue));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        [MemberData(nameof(OrderCancelingOperators))]
        public static void ElementAtOrDefault_AggregateException_Wraps_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).ElementAtOrDefault(int.MaxValue));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void ElementAtOrDefault_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).ElementAtOrDefault(0));
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).ElementAtOrDefault(DefaultSize + 1));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        [MemberData(nameof(OrderCancelingOperators))]
        public static void First_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).First(x => false));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        [MemberData(nameof(OrderCancelingOperators))]
        public static void First_AggregateException_Wraps_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).First(x => false));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void First_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).First());
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).First(x => false));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        [MemberData(nameof(OrderCancelingOperators))]
        public static void FirstOrDefault_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).FirstOrDefault(x => false));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        [MemberData(nameof(OrderCancelingOperators))]
        public static void FirstOrDefault_AggregateException_Wraps_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).FirstOrDefault(x => false));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void FirstOrDefault_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).FirstOrDefault());
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).FirstOrDefault(x => false));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        public static void ForAll_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).ForAll(x => { }));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        public static void ForAll_AggregateException_Wraps_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).ForAll(x => { }));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void ForAll_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).ForAll(x => { }));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        [MemberData(nameof(OrderCancelingOperators))]
        public static void ForEach_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertEventuallyCanceled((token, canceler) => { foreach (int i in Cancel(token, canceler, source, operation)) ; });
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        [MemberData(nameof(OrderCancelingOperators))]
        public static void ForEach_AggregateException_Wraps_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertAggregateAlternateCanceled((token, canceler) => { foreach (int i in Cancel(token, canceler, source, operation)) ; });
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void ForEach_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => { foreach (int i in Cancel(token, source, operation)) ; });
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        [MemberData(nameof(OrderCancelingOperators))]
        public static void Last_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Last());
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Last(x => true));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        [MemberData(nameof(OrderCancelingOperators))]
        public static void Last_AggregateException_Wraps_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Last());
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Last(x => true));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void Last_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).Last());
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).Last(x => true));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        [MemberData(nameof(OrderCancelingOperators))]
        public static void LastOrDefault_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).LastOrDefault());
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).LastOrDefault(x => true));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        [MemberData(nameof(OrderCancelingOperators))]
        public static void LastOrDefault_AggregateException_Wraps_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).LastOrDefault());
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).LastOrDefault(x => true));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void LastOrDefault_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).LastOrDefault());
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).LastOrDefault(x => true));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        public static void Max_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Max());
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Max(x => (int?)x));

            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Max(x => (long)x));
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Max(x => (long?)x));

            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Max(x => (float)x));
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Max(x => (float?)x));

            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Max(x => (double)x));
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Max(x => (double?)x));

            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Max(x => (decimal)x));
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Max(x => (decimal?)x));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        public static void Max_AggregateException_Wraps_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Max());
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Max(x => (int?)x));

            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Max(x => (long)x));
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Max(x => (long?)x));

            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Max(x => (float)x));
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Max(x => (float?)x));

            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Max(x => (double)x));
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Max(x => (double?)x));

            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Max(x => (decimal)x));
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Max(x => (decimal?)x));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void Max_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).Max());
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        public static void Min_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Min());
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Min(x => (int?)x));

            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Min(x => (long)x));
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Min(x => (long?)x));

            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Min(x => (float)x));
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Min(x => (float?)x));

            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Min(x => (double)x));
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Min(x => (double?)x));

            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Min(x => (decimal)x));
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Min(x => (decimal?)x));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        public static void Min_AggregateException_Wraps_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Min());
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Min(x => (int?)x));

            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Min(x => (long)x));
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Min(x => (long?)x));

            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Min(x => (float)x));
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Min(x => (float?)x));

            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Min(x => (double)x));
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Min(x => (double?)x));

            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Min(x => (decimal)x));
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Min(x => (decimal?)x));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void Min_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).Min());
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        [MemberData(nameof(OrderCancelingOperators))]
        public static void SequenceEqual_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).SequenceEqual(ParallelEnumerable.Range(0, 128).AsOrdered()));
            Functions.AssertEventuallyCanceled((token, canceler) => ParallelEnumerable.Range(0, 128).AsOrdered().SequenceEqual(Cancel(token, canceler, source, operation)));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        [MemberData(nameof(OrderCancelingOperators))]
        public static void SequenceEqual_AggregateException_Wraps_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            SequenceEqual_AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).SequenceEqual(ParallelEnumerable.Range(0, 128).AsOrdered()));
            SequenceEqual_AssertAggregateAlternateCanceled((token, canceler) => ParallelEnumerable.Range(0, 128).AsOrdered().SequenceEqual(Cancel(token, canceler, source, operation)));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void SequenceEqual_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).SequenceEqual(ParallelEnumerable.Range(0, 2)));
            Functions.AssertAlreadyCanceled(token => ParallelEnumerable.Range(0, 2).SequenceEqual(Cancel(token, source, operation)));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        public static void Single_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Single(x => false));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        public static void Single_AggregateException_Wraps_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Single(x => false));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void Single_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).Single());
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).Single(x => false));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        public static void SingleOrDefault_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).SingleOrDefault(x => false));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        public static void SingleOrDefault_AggregateException_Wraps_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).SingleOrDefault(x => false));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void SingleOrDefault_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).SingleOrDefault());
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).SingleOrDefault(x => false));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        public static void Sum_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Sum());
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Sum(x => (int?)x));

            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Sum(x => (long)x));
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Sum(x => (long?)x));

            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Sum(x => (float)x));
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Sum(x => (float?)x));

            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Sum(x => (double)x));
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Sum(x => (double?)x));

            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Sum(x => (decimal)x));
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).Sum(x => (decimal?)x));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        public static void Sum_AggregateException_Wraps_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Sum());
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Sum(x => (int?)x));

            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Sum(x => (long)x));
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Sum(x => (long?)x));

            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Sum(x => (float)x));
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Sum(x => (float?)x));

            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Sum(x => (double)x));
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Sum(x => (double?)x));

            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Sum(x => (decimal)x));
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).Sum(x => (decimal?)x));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void Sum_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).Sum());
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        [MemberData(nameof(OrderCancelingOperators))]
        public static void ToArray_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).ToArray());
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        [MemberData(nameof(OrderCancelingOperators))]
        public static void ToArray_AggregateException_Wraps_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).ToArray());
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void ToArray_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).ToArray());
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        public static void ToDictionary_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).ToDictionary(x => x));
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).ToDictionary(x => x, y => y));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        public static void ToDictionary_AggregateException_Wraps_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).ToDictionary(x => x));
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).ToDictionary(x => x, y => y));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void ToDictionary_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).ToDictionary(x => x));
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).ToDictionary(x => x, y => y));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        [MemberData(nameof(OrderCancelingOperators))]
        public static void ToList_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).ToList());
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        [MemberData(nameof(OrderCancelingOperators))]
        public static void ToList_AggregateException_Wraps_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).ToList());
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void ToList_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).ToList());
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        [MemberData(nameof(OrderCancelingOperators))]
        public static void ToLookup_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).ToLookup(x => x));
            Functions.AssertEventuallyCanceled((token, canceler) => Cancel(token, canceler, source, operation).ToLookup(x => x, y => y));
        }

        [Theory]
        [MemberData(nameof(UnaryCancelingOperators))]
        [MemberData(nameof(BinaryCancelingOperators))]
        [MemberData(nameof(OrderCancelingOperators))]
        public static void ToLookup_AggregateException_Wraps_OperationCanceledException(Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).ToLookup(x => x));
            Functions.AssertAggregateAlternateCanceled((token, canceler) => Cancel(token, canceler, source, operation).ToLookup(x => x, y => y));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void ToLookup_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).ToLookup(x => x));
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).ToLookup(x => x, y => y));
        }

        private static ParallelQuery<int> Cancel(CancellationToken token, Action canceler, Labeled<Func<CancellationToken, Operation>> source, Labeled<Func<Action, Operation>> operation)
        {
            return operation.Item(canceler)(DefaultStart, EventualCancellationSize, source.Item(token));
        }

        private static ParallelQuery<int> Cancel(CancellationToken token, LabeledOperation source, LabeledOperation operation)
        {
            return operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item);
        }

        private static void SequenceEqual_AssertAggregateAlternateCanceled(Action<CancellationToken, Action> query)
        {
            CancellationTokenSource cs = new CancellationTokenSource();
            cs.Cancel();
            Action canceler = () => { throw new OperationCanceledException(cs.Token); };

            AggregateException outer = Assert.Throws<AggregateException>(() => query(new CancellationTokenSource().Token, canceler));
            AggregateException ae = Assert.Single<AggregateException>(outer.InnerExceptions.Cast<AggregateException>());
            Assert.All(ae.InnerExceptions, e => Assert.IsType<OperationCanceledException>(e));
        }
    }
}

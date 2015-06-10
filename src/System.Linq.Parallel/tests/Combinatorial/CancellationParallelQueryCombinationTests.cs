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
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void Contains_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).Contains(DefaultStart));
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
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void ElementAt_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).ElementAt(0));
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
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void First_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).First());
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).First(x => false));
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
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void ForAll_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).ForAll(x => { }));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void ForEach_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => { foreach (int i in Cancel(token, source, operation)) ; });
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
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void LastOrDefault_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).LastOrDefault());
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).LastOrDefault(x => true));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void Max_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).Max());
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void Min_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).Min());
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
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void Single_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).Single());
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).Single(x => false));
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
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void Sum_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).Sum());
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void ToArray_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).ToArray());
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
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void ToList_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => Cancel(token, source, operation).ToList());
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
    }
}

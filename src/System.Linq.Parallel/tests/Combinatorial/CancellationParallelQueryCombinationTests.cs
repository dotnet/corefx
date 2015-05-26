// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using Xunit;

namespace System.Linq.Parallel.Tests
{
    public partial class ParallelQueryCombinationTests
    {
        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void Aggregate_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {

            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).Aggregate((x, y) => x));
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).Aggregate(0, (x, y) => x + y));
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).Aggregate(0, (x, y) => x + y, r => r));
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).Aggregate(0, (a, x) => a + x, (l, r) => l + r, r => r));
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).Aggregate(() => 0, (a, x) => a + x, (l, r) => l + r, r => r));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void All_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).All(x => true));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void Any_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).Any());
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).Any(x => true));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void Average_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).Average());
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void Contains_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).Contains(DefaultStart));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void Count_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).Count());
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).LongCount());
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).Count(x => true));
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).LongCount(x => true));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void ElementAt_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, (start, count, ignore) => source.Item(start, count).WithCancellation(token)).ElementAt(0));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void ElementAtOrDefault_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, (start, count, ignore) => source.Item(start, count).WithCancellation(token)).ElementAtOrDefault(0));
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, (start, count, ignore) => source.Item(start, count).WithCancellation(token)).ElementAtOrDefault(DefaultSize + 1));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void First_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).First());
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).First(x => false));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void FirstOrDefault_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).FirstOrDefault());
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).FirstOrDefault(x => false));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void ForAll_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).ForAll(x => { }));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void ForEach_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => { foreach (int i in operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item)) ; });
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void Last_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).Last());
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).Last(x => true));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void LastOrDefault_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, (start, count, ignore) => source.Item(start, count).WithCancellation(token)).LastOrDefault());
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).LastOrDefault(x => true));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void Max_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, (start, count, ignore) => source.Item(start, count).WithCancellation(token)).Max());
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void Min_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).Min());
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void SequenceEqual_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).SequenceEqual(ParallelEnumerable.Range(0, 2)));
            Functions.AssertAlreadyCanceled(token => ParallelEnumerable.Range(0, 2).SequenceEqual(operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item)));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void Single_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).Single());
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).Single(x => false));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void SingleOrDefault_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).SingleOrDefault());
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).SingleOrDefault(x => false));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void Sum_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).Sum());
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void ToArray_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).ToArray());
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void ToDictionary_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).ToDictionary(x => x));
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).ToDictionary(x => x, y => y));
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void ToList_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).ToList());
        }

        [Theory]
        [MemberData(nameof(UnaryOperators))]
        [MemberData(nameof(BinaryOperators))]
        public static void ToLookup_OperationCanceledException_PreCanceled(LabeledOperation source, LabeledOperation operation)
        {
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).ToLookup(x => x));
            Functions.AssertAlreadyCanceled(token => operation.Item(DefaultStart, DefaultSize, source.Append(WithCancellation(token)).Item).ToLookup(x => x, y => y));
        }
    }
}

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;

namespace System.Linq.Parallel.Tests
{
    public partial class ParallelQueryCombinationTests
    {
        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void Cast_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize;
            foreach (int? i in operation.Item(DefaultStart, DefaultSize, source.Item).Cast<int?>())
            {
                Assert.True(i.HasValue);
                Assert.Equal(--seen, i.Value);
            }
            Assert.Equal(DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void Cast_NotPipelined_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize;
            Assert.All(operation.Item(DefaultStart, DefaultSize, source.Item).Cast<int?>().ToList(), x => Assert.Equal(--seen, x));
            Assert.Equal(DefaultStart, seen);
        }

        [Theory]
        [ActiveIssue(1332)]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void Concat_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            {
                int direction = -1;
                int seen = DefaultStart + DefaultSize;
                foreach (int i in operation.Item(DefaultStart, DefaultSize, source.Item)
                    .Concat(ParallelEnumerable.Range(DefaultStart + 1, DefaultSize)))
                {
                    Assert.Equal(seen += direction, i);
                    if (seen == DefaultStart) direction = 1;
                }
                Assert.Equal(DefaultStart + DefaultSize, seen);
            }
            {
                int direction = 1;
                int seen = DefaultStart;
                foreach (int i in ParallelEnumerable.Range(DefaultStart + 1, DefaultSize)
                    .Concat(operation.Item(DefaultStart, DefaultSize, source.Item)))
                {
                    Assert.Equal(seen += direction, i);
                    if (seen == DefaultStart + DefaultSize) direction = -1;
                }
                Assert.Equal(DefaultStart, seen);
            }
        }

        [Theory]
        [ActiveIssue(1332)]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void Concat_NotPipelined_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            {
                int direction = -1;
                int seen = DefaultStart + DefaultSize;
                Assert.All(
                    operation.Item(DefaultStart, DefaultSize, source.Item)
                        .Concat(ParallelEnumerable.Range(DefaultStart + 1, DefaultSize)).ToList(),
                    x =>
                    {
                        Assert.Equal(seen += direction, x);
                        if (seen == DefaultStart) direction = 1;
                    });
                Assert.Equal(DefaultStart + DefaultSize, seen);
            }
            {
                int direction = 1;
                int seen = DefaultStart;
                Assert.All(
                    ParallelEnumerable.Range(DefaultStart + 1, DefaultSize)
                        .Concat(operation.Item(DefaultStart, DefaultSize, source.Item)).ToList(),
                    x =>
                    {
                        Assert.Equal(seen += direction, x);
                        if (seen == DefaultStart + DefaultSize) direction = -1;
                    });
                Assert.Equal(DefaultStart, seen);
            }
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void DefaultIfEmpty_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize;
            foreach (int i in operation.Item(DefaultStart, DefaultSize, source.Item).DefaultIfEmpty())
            {
                Assert.Equal(--seen, i);
            }
            Assert.Equal(DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void DefaultIfEmpty_NotPipelined_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize;
            Assert.All(operation.Item(DefaultStart, DefaultSize, source.Item).DefaultIfEmpty().ToList(), x => Assert.Equal(--seen, x));
            Assert.Equal(DefaultStart, seen);
        }

        [Theory]
        [ActiveIssue(1330)]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void Distinct_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize;
            ParallelQuery<int> query = operation.Item(DefaultStart, DefaultSize, source.Item).Distinct(new ModularCongruenceComparer(DefaultSize / 2));
            foreach (int i in query)
            {
                Assert.Equal(--seen, i);
            }
            Assert.Equal(DefaultStart + DefaultSize / 2, seen);
        }

        [Theory]
        [ActiveIssue(1330)]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void Distinct_NotPipelined_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize;
            ParallelQuery<int> query = operation.Item(DefaultStart, DefaultSize, source.Item).Distinct(new ModularCongruenceComparer(DefaultSize / 2));
            Assert.All(query.ToList(), x => Assert.Equal(--seen, x));
            Assert.Equal(DefaultStart + DefaultSize / 2, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void ElementAt_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            ParallelQuery<int> query = operation.Item(DefaultStart, DefaultSize, source.Item);

            int seen = DefaultStart + DefaultSize;
            for (int i = 0; i < DefaultSize; i++)
            {
                Assert.Equal(--seen, query.ElementAt(i));
            }
            Assert.Equal(DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void ElementAtOrDefault_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            ParallelQuery<int> query = operation.Item(DefaultStart, DefaultSize, source.Item);

            int seen = DefaultStart + DefaultSize;
            for (int i = 0; i < DefaultSize; i++)
            {
                Assert.Equal(--seen, query.ElementAtOrDefault(i));
            }
            Assert.Equal(DefaultStart, seen);
            Assert.Equal(default(int), query.ElementAtOrDefault(-1));
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void Except_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize;
            ParallelQuery<int> query = operation.Item(DefaultStart, DefaultSize + DefaultSize / 2, source.Item)
                .Except(operation.Item(DefaultStart + DefaultSize, DefaultSize, source.Item));
            foreach (int i in query)
            {
                Assert.Equal(--seen, i);
            }
            Assert.Equal(DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void Except_NotPipelined_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize;
            ParallelQuery<int> query = operation.Item(DefaultStart, DefaultSize + DefaultSize / 2, source.Item)
                .Except(operation.Item(DefaultStart + DefaultSize, DefaultSize, source.Item));
            Assert.All(query.ToList(), x => Assert.Equal(--seen, x));
            Assert.Equal(DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void First_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            Assert.Equal(DefaultStart + DefaultSize - 1, operation.Item(DefaultStart, DefaultSize, source.Item).First());
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void First_Predicate_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            Assert.Equal(DefaultStart + DefaultSize / 2 - 1, operation.Item(DefaultStart, DefaultSize, source.Item).First(x => x < DefaultStart + DefaultSize / 2));
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void FirstOrDefault_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            Assert.Equal(DefaultStart + DefaultSize - 1, operation.Item(DefaultStart, DefaultSize, source.Item).FirstOrDefault());
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void FirstOrDefault_Predicate_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            Assert.Equal(DefaultStart + DefaultSize / 2 - 1, operation.Item(DefaultStart, DefaultSize, source.Item).FirstOrDefault(x => x < DefaultStart + DefaultSize / 2));
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void GroupBy_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seenKey = (DefaultStart + DefaultSize) / GroupFactor;
            foreach (IGrouping<int, int> group in operation.Item(DefaultStart, DefaultSize, source.Item).GroupBy(x => x / GroupFactor))
            {
                Assert.Equal(--seenKey, group.Key);
                int seenElement = (group.Key + 1) * GroupFactor;
                Assert.All(group, x => Assert.Equal(--seenElement, x));
                Assert.Equal(group.Key * GroupFactor, seenElement);
            }
            Assert.Equal(DefaultStart / GroupFactor, seenKey);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void GroupBy_NotPipelined_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seenKey = (DefaultStart + DefaultSize) / GroupFactor;
            foreach (IGrouping<int, int> group in operation.Item(DefaultStart, DefaultSize, source.Item).GroupBy(x => x / GroupFactor).ToList())
            {
                Assert.Equal(--seenKey, group.Key);
                int seenElement = (group.Key + 1) * GroupFactor;
                Assert.All(group, x => Assert.Equal(--seenElement, x));
                Assert.Equal(group.Key * GroupFactor, seenElement);
            }
            Assert.Equal(DefaultStart / GroupFactor, seenKey);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void GroupBy_ElementSelector_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seenKey = (DefaultStart + DefaultSize) / GroupFactor;
            foreach (IGrouping<int, int> group in operation.Item(DefaultStart, DefaultSize, source.Item).GroupBy(x => x / GroupFactor, y => -y))
            {
                Assert.Equal(--seenKey, group.Key);
                int seenElement = -(group.Key + 1) * GroupFactor;
                Assert.All(group, x => Assert.Equal(++seenElement, x));
                Assert.Equal(-group.Key * GroupFactor, seenElement);
            }
            Assert.Equal(DefaultStart / GroupFactor, seenKey);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void GroupBy_ElementSelector_NotPipelined_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seenKey = (DefaultStart + DefaultSize) / GroupFactor;
            foreach (IGrouping<int, int> group in operation.Item(DefaultStart, DefaultSize, source.Item).GroupBy(x => x / GroupFactor, y => -y).ToList())
            {
                Assert.Equal(--seenKey, group.Key);
                int seenElement = -(group.Key + 1) * GroupFactor;
                Assert.All(group, x => Assert.Equal(++seenElement, x));
                Assert.Equal(-group.Key * GroupFactor, seenElement);
            }
            Assert.Equal(DefaultStart / GroupFactor, seenKey);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void Intersect_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize;
            ParallelQuery<int> query = operation.Item(DefaultStart - DefaultSize / 2, DefaultSize + DefaultSize / 2, source.Item)
                .Intersect(operation.Item(DefaultStart, DefaultSize + DefaultSize / 2, source.Item));
            foreach (int i in query)
            {
                Assert.Equal(--seen, i);
            }
            Assert.Equal(DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void Intersect_NotPipelined_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize;
            ParallelQuery<int> query = operation.Item(DefaultStart - DefaultSize / 2, DefaultSize + DefaultSize / 2, source.Item)
                .Intersect(operation.Item(DefaultStart, DefaultSize + DefaultSize / 2, source.Item));
            Assert.All(query.ToList(), x => Assert.Equal(--seen, x));
            Assert.Equal(DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void Last_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            Assert.Equal(DefaultStart, operation.Item(DefaultStart, DefaultSize, source.Item).Last());
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void Last_Predicate_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            Assert.Equal(DefaultStart + DefaultSize / 2, operation.Item(DefaultStart, DefaultSize, source.Item).Last(x => x >= DefaultStart + DefaultSize / 2));
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void LastOrDefault_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            Assert.Equal(DefaultStart, operation.Item(DefaultStart, DefaultSize, source.Item).LastOrDefault());
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void LastOrDefault_Predicate_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            Assert.Equal(DefaultStart + DefaultSize / 2, operation.Item(DefaultStart, DefaultSize, source.Item).LastOrDefault(x => x >= DefaultStart + DefaultSize / 2));
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void OfType_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize;
            foreach (int i in operation.Item(DefaultStart, DefaultSize, source.Item).OfType<int>())
            {
                Assert.Equal(--seen, i);
            }
            Assert.Equal(DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void OfType_NotPipelined_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize;
            Assert.All(operation.Item(DefaultStart, DefaultSize, source.Item).OfType<int>().ToList(), x => Assert.Equal(--seen, x));
            Assert.Equal(DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void Reverse_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart;
            foreach (int i in operation.Item(DefaultStart, DefaultSize, source.Item).Reverse())
            {
                Assert.Equal(seen++, i);
            }
            Assert.Equal(DefaultStart + DefaultSize, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void Reverse_NotPipelined_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart;
            foreach (int i in operation.Item(DefaultStart, DefaultSize, source.Item).Reverse().ToList())
            {
                Assert.Equal(seen++, i);
            }
            Assert.Equal(DefaultStart + DefaultSize, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void Select_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = -(DefaultStart + DefaultSize);
            foreach (int i in operation.Item(DefaultStart, DefaultSize, source.Item).Select(x => -x))
            {
                Assert.Equal(++seen, i);
            }
            Assert.Equal(-DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void Select_NotPipelined_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = -(DefaultStart + DefaultSize);
            Assert.All(operation.Item(DefaultStart, DefaultSize, source.Item).Select(x => -x).ToList(), x => Assert.Equal(++seen, x));
            Assert.Equal(-DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void Select_Indexed_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = -(DefaultStart + DefaultSize);
            foreach (int i in operation.Item(DefaultStart, DefaultSize, source.Item).Select((x, index) => { Assert.Equal(DefaultStart + DefaultSize - index - 1, x); return -x; }))
            {
                Assert.Equal(++seen, i);
            }
            Assert.Equal(-DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void Select_Indexed_NotPipelined_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = -(DefaultStart + DefaultSize);
            Assert.All(operation.Item(DefaultStart, DefaultSize, source.Item).Select((x, index) => { Assert.Equal(DefaultStart + DefaultSize - index - 1, x); return -x; }).ToList(), x => Assert.Equal(++seen, x));
            Assert.Equal(-DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void SelectMany_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = -(DefaultStart + DefaultSize * 2);
            foreach (int i in operation.Item(0, DefaultSize, source.Item).SelectMany(x => new[] { -1, 0 }.Select(y => y + -DefaultStart - 2 * x)))
            {
                Assert.Equal(++seen, i);
            }
            Assert.Equal(-DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void SelectMany_NotPipelined_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = -(DefaultStart + DefaultSize * 2);
            Assert.All(operation.Item(0, DefaultSize, source.Item).SelectMany(x => new[] { -1, 0 }.Select(y => y + -DefaultStart - 2 * x)).ToList(), x => Assert.Equal(++seen, x));
            Assert.Equal(-DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void SelectMany_Indexed_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = -(DefaultStart + DefaultSize * 2);
            foreach (int i in operation.Item(0, DefaultSize, source.Item).SelectMany((x, index) => { Assert.Equal(DefaultSize - index - 1, x); return new[] { -1, 0 }.Select(y => y + -DefaultStart - 2 * x); }))
            {
                Assert.Equal(++seen, i);
            }
            Assert.Equal(-DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void SelectMany_Indexed_NotPipelined_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = -(DefaultStart + DefaultSize * 2);
            Assert.All(operation.Item(0, DefaultSize, source.Item).SelectMany((x, index) => { Assert.Equal(DefaultSize - index - 1, x); return new[] { -1, 0 }.Select(y => y + -DefaultStart - 2 * x); }).ToList(), x => Assert.Equal(++seen, x));
            Assert.Equal(-DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void SelectMany_ResultSelector_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = -(DefaultStart + DefaultSize * 2);
            foreach (int i in operation.Item(0, DefaultSize, source.Item).SelectMany(x => new[] { -1, 0 }, (x, y) => y + -DefaultStart - 2 * x))
            {
                Assert.Equal(++seen, i);
            }
            Assert.Equal(-DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void SelectMany_ResultSelector_NotPipelined_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = -(DefaultStart + DefaultSize * 2);
            Assert.All(operation.Item(0, DefaultSize, source.Item).SelectMany(x => new[] { -1, 0 }, (x, y) => y + -DefaultStart - 2 * x).ToList(), x => Assert.Equal(++seen, x));
            Assert.Equal(-DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void SelectMany_Indexed_ResultSelector_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = -(DefaultStart + DefaultSize * 2);
            foreach (int i in operation.Item(0, DefaultSize, source.Item).SelectMany((x, index) => { Assert.Equal(DefaultSize - index - 1, x); return new[] { -1, 0 }; }, (x, y) => y + -DefaultStart - 2 * x))
            {
                Assert.Equal(++seen, i);
            }
            Assert.Equal(-DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void SelectMany_Indexed_ResultSelector_NotPipelined_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = -(DefaultStart + DefaultSize * 2);
            Assert.All(operation.Item(0, DefaultSize, source.Item).SelectMany((x, index) => { Assert.Equal(DefaultSize - index - 1, x); return new[] { -1, 0 }; }, (x, y) => y + -DefaultStart - 2 * x).ToList(), x => Assert.Equal(++seen, x));
            Assert.Equal(-DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void SequenceEqual_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            Assert.True(operation.Item(DefaultStart, DefaultSize, source.Item).SequenceEqual(ParallelEnumerable.Range(DefaultStart, DefaultSize).AsOrdered().Reverse()));
            Assert.True(ParallelEnumerable.Range(DefaultStart, DefaultSize).AsOrdered().Reverse().SequenceEqual(operation.Item(DefaultStart, DefaultSize, source.Item)));
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void Skip_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize / 2 - (DefaultSize % 2);
            foreach (int i in operation.Item(DefaultStart, DefaultSize, source.Item).Skip(DefaultSize / 2))
            {
                Assert.Equal(--seen, i);
            }
            Assert.Equal(DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void Skip_NotPipelined_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize / 2 - (DefaultSize % 2);
            Assert.All(operation.Item(DefaultStart, DefaultSize, source.Item).Skip(DefaultSize / 2).ToList(), x => Assert.Equal(--seen, x));
            Assert.Equal(DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void SkipWhile_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize / 2;
            foreach (int i in operation.Item(DefaultStart, DefaultSize, source.Item).SkipWhile(x => x >= DefaultStart + DefaultSize / 2))
            {
                Assert.Equal(--seen, i);
            }
            Assert.Equal(DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void SkipWhile_NotPipelined_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize / 2;
            Assert.All(operation.Item(DefaultStart, DefaultSize, source.Item).SkipWhile(x => x >= DefaultStart + DefaultSize / 2).ToList(), x => Assert.Equal(--seen, x));
            Assert.Equal(DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void SkipWhile_Indexed_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize / 2;
            foreach (int i in operation.Item(DefaultStart, DefaultSize, source.Item).SkipWhile((x, index) => index < DefaultSize / 2))
            {
                Assert.Equal(--seen, i);
            }
            Assert.Equal(DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void SkipWhile_Indexed_NotPipelined_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize / 2;
            Assert.All(operation.Item(DefaultStart, DefaultSize, source.Item).SkipWhile((x, index) => index < DefaultSize / 2).ToList(), x => Assert.Equal(--seen, x));
            Assert.Equal(DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void Take_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize;
            foreach (int i in operation.Item(DefaultStart, DefaultSize, source.Item).Take(DefaultSize / 2))
            {
                Assert.Equal(--seen, i);
            }
            Assert.Equal(DefaultStart + DefaultSize / 2, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void Take_NotPipelined_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize;
            Assert.All(operation.Item(DefaultStart, DefaultSize, source.Item).Take(DefaultSize / 2).ToList(), x => Assert.Equal(--seen, x));
            Assert.Equal(DefaultStart + DefaultSize / 2, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void TakeWhile_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize;
            foreach (int i in operation.Item(DefaultStart, DefaultSize, source.Item).TakeWhile(x => x >= DefaultStart + DefaultSize / 2))
            {
                Assert.Equal(--seen, i);
            }
            Assert.Equal(DefaultStart + DefaultSize / 2, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void TakeWhile_NotPipelined_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize;
            Assert.All(operation.Item(DefaultStart, DefaultSize, source.Item).TakeWhile(x => x >= DefaultStart + DefaultSize / 2).ToList(), x => Assert.Equal(--seen, x));
            Assert.Equal(DefaultStart + DefaultSize / 2, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void TakeWhile_Indexed_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize;
            foreach (int i in operation.Item(DefaultStart, DefaultSize, source.Item).TakeWhile((x, index) => index < DefaultSize / 2))
            {
                Assert.Equal(--seen, i);
            }
            Assert.Equal(DefaultStart + DefaultSize / 2, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void TakeWhile_Indexed_NotPipelined_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize;
            Assert.All(operation.Item(DefaultStart, DefaultSize, source.Item).TakeWhile((x, index) => index < DefaultSize / 2).ToList(), x => Assert.Equal(--seen, x));
            Assert.Equal(DefaultStart + DefaultSize / 2, seen);
        }

        [Theory]
        //[Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void ToArray_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize;
            Assert.All(operation.Item(DefaultStart, DefaultSize, source.Item).ToArray(), x => Assert.Equal(--seen, x));
            Assert.Equal(DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void ToList_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize;
            Assert.All(operation.Item(DefaultStart, DefaultSize, source.Item).ToList(), x => Assert.Equal(--seen, x));
            Assert.Equal(DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void Where_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize;
            foreach (int i in operation.Item(DefaultStart, DefaultSize, source.Item).Where(x => x >= DefaultStart + DefaultSize / 2))
            {
                Assert.Equal(--seen, i);
            }
            Assert.Equal(DefaultStart + DefaultSize / 2, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void Where_NotPipelined_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize;
            Assert.All(operation.Item(DefaultStart, DefaultSize, source.Item).Where(x => x >= DefaultStart + DefaultSize / 2).ToList(), x => Assert.Equal(--seen, x));
            Assert.Equal(DefaultStart + DefaultSize / 2, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void Where_Indexed_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize;
            foreach (int i in operation.Item(DefaultStart, DefaultSize, source.Item).Where((x, index) => index < DefaultSize / 2))
            {
                Assert.Equal(--seen, i);
            }
            Assert.Equal(DefaultStart + DefaultSize / 2, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void Where_Indexed_NotPipelined_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize;
            Assert.All(operation.Item(DefaultStart, DefaultSize, source.Item).Where((x, index) => index < DefaultSize / 2).ToList(), x => Assert.Equal(--seen, x));
            Assert.Equal(DefaultStart + DefaultSize / 2, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void Zip_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize;
            ParallelQuery<int> query = operation.Item(DefaultStart * 2, DefaultSize, source.Item)
                .Zip(operation.Item(0, DefaultSize, source.Item), (x, y) => (x + y) / 2);
            foreach (int i in query)
            {
                Assert.Equal(--seen, i);
            }
            Assert.Equal(DefaultStart, seen);
        }

        [Theory]
        [Trait("category", "reverse")]
        [MemberData("UnaryReversedOperators")]
        [MemberData("BinaryReversedOperators")]
        public static void Zip_NotPipelined_Reversed(LabeledOperation source, LabeledOperation operation)
        {
            int seen = DefaultStart + DefaultSize;
            ParallelQuery<int> query = operation.Item(0, DefaultSize, source.Item)
                .Zip(operation.Item(DefaultStart * 2, DefaultSize, source.Item), (x, y) => (x + y) / 2);
            Assert.All(query.ToList(), x => Assert.Equal(--seen, x));
            Assert.Equal(DefaultStart, seen);
        }
    }
}

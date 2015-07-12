// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace System.Linq.Tests
{
    public partial class EnumerableTests
    {
        [Fact]
        public void Average()
        {
            var one = new[] {0, 1, 2};
            var two = new[] {0, 1, 2, 3, 4};
            var fourByThree = new[] {-2, 1, 5};
            var oneByThree = new[] {-2, 1, 2};

            var oneWithNulls = new int?[] {0, 1, 2, null, null, null, null};
            var twoWithNulls = new int?[] {0, 1, 2, 3, 4, null, null, null};
            var fourByThreeWithNulls = new int?[] {-2, 1, 5, null};
            var oneByThreeWithNulls = new int?[] {-2, 1, 2, null, null};

            Assert.Equal(1, one.Average());
            Assert.Equal(2, two.Average());
            Assert.Equal(4.0/3, fourByThree.Average());
            Assert.Equal(1.0/3, oneByThree.Average());

            // Repeat the tests with some null values and ensure they don't affect the result.
            Assert.Equal(1, oneWithNulls.Average());
            Assert.Equal(2, twoWithNulls.Average());
            Assert.Equal(4.0/3, fourByThreeWithNulls.Average());
            Assert.Equal(1.0/3, oneByThreeWithNulls.Average());

            // Cast the arrays' elements types to different data types and repeat the tests.
            // 1. long and long?
            Assert.Equal(1, one.Select<int, long>(i => i).Average());
            Assert.Equal(2, two.Select<int, long>(i => i).Average());
            Assert.Equal(4.0/3, fourByThree.Select<int, long>(i => i).Average());
            Assert.Equal(1.0/3, oneByThree.Select<int, long>(i => i).Average());
            Assert.Equal(1, oneWithNulls.Select<int?, long?>(i => i).Average());
            Assert.Equal(2, twoWithNulls.Select<int?, long?>(i => i).Average());
            Assert.Equal(4.0/3, fourByThreeWithNulls.Select<int?, long?>(i => i).Average());
            Assert.Equal(1.0/3, oneByThreeWithNulls.Select<int?, long?>(i => i).Average());
            // 2. float and float?
            Assert.Equal(1, one.Select<int, float>(i => i).Average());
            Assert.Equal(2, two.Select<int, float>(i => i).Average());
            Assert.Equal(4.0f/3, fourByThree.Select<int, float>(i => i).Average());
            Assert.Equal(1.0f/3, oneByThree.Select<int, float>(i => i).Average());
            Assert.Equal(1, oneWithNulls.Select<int?, float?>(i => i).Average());
            Assert.Equal(2, twoWithNulls.Select<int?, float?>(i => i).Average());
            Assert.Equal(4.0f/3, fourByThreeWithNulls.Select<int?, float?>(i => i).Average());
            Assert.Equal(1.0f/3, oneByThreeWithNulls.Select<int?, float?>(i => i).Average());
            // 3. double and double?
            Assert.Equal(1, one.Select<int, double>(i => i).Average());
            Assert.Equal(2, two.Select<int, double>(i => i).Average());
            Assert.Equal(4.0/3, fourByThree.Select<int, double>(i => i).Average());
            Assert.Equal(1.0/3, oneByThree.Select<int, double>(i => i).Average());
            Assert.Equal(1, oneWithNulls.Select<int?, double?>(i => i).Average());
            Assert.Equal(2, twoWithNulls.Select<int?, double?>(i => i).Average());
            Assert.Equal(4.0/3, fourByThreeWithNulls.Select<int?, double?>(i => i).Average());
            Assert.Equal(1.0/3, oneByThreeWithNulls.Select<int?, double?>(i => i).Average());
            // 4. decimal and decimal?
            Assert.Equal(1, one.Select<int, decimal>(i => i).Average());
            Assert.Equal(2, two.Select<int, decimal>(i => i).Average());
            Assert.Equal(4.0m/3, fourByThree.Select<int, decimal>(i => i).Average());
            Assert.Equal(1.0m/3, oneByThree.Select<int, decimal>(i => i).Average());
            Assert.Equal(1, oneWithNulls.Select<int?, decimal?>(i => i).Average());
            Assert.Equal(2, twoWithNulls.Select<int?, decimal?>(i => i).Average());
            Assert.Equal(4.0m/3, fourByThreeWithNulls.Select<int?, decimal?>(i => i).Average());
            Assert.Equal(1.0m/3, oneByThreeWithNulls.Select<int?, decimal?>(i => i).Average());

            // Convert the array to different data structures and repeat the tests to
            // ensure that the method works with other data structures as well.
            // 1. List
            Assert.Equal(1, one.ToList().Average());
            Assert.Equal(2, two.ToList().Average());
            Assert.Equal(4.0/3, fourByThree.ToList().Average());
            Assert.Equal(1.0/3, oneByThree.ToList().Average());
            // 2. Stack
            Assert.Equal(1, new Stack<int>(one).Average());
            Assert.Equal(2, new Stack<int>(two).Average());
            Assert.Equal(4.0/3, new Stack<int>(fourByThree).Average());
            Assert.Equal(1.0/3, new Stack<int>(oneByThree).Average());
        }

        [Fact]
        public void All()
        {
            var array = Enumerable.Range(1, 10).ToArray();
            Assert.True(array.All(i => i > 0));
            for (var j = 1; j <= 10; j++)
                Assert.False(array.All(i => i > j));
        }

        [Fact]
        public void Any()
        {
            var array = Enumerable.Range(1, 10).ToArray();
            for (var j = 0; j <= 9; j++)
                Assert.True(array.Any(i => i > j));
            Assert.False(array.Any(i => i > 10));
        }

        private void CountAndValidate<T>(IEnumerable<T> enumerable, int expectedCount)
        {
            Assert.Equal(expectedCount, enumerable.Count());
            Assert.Equal(expectedCount, enumerable.ToList().Count());
            Assert.Equal(expectedCount, enumerable.ToArray().Count());
            Assert.Equal(expectedCount, new Stack<T>(enumerable).Count());
        }

        [Fact]
        public void Count()
        {
            const int count = 100;
            var range = Enumerable.Range(1, count).ToArray();
            CountAndValidate(range, count);
            CountAndValidate(range.Select<int, long>(i => i), count);
            CountAndValidate(range.Select<int, float>(i => i), count);
            CountAndValidate(range.Select<int, double>(i => i), count);
            CountAndValidate(range.Select<int, decimal>(i => i), count);
        }

        private void FindDistinctAndValidate<T>(IEnumerable<T> original)
        {
            // Convert to list to avoid repeated enumerations of the enumerables.
            var originalList = original.ToList();
            var distinctList = originalList.Distinct().ToList();

            // Ensure the result doesn't contain duplicates.
            var hashSet = new HashSet<T>();
            foreach (var i in distinctList)
            {
                Assert.False(hashSet.Contains(i), "Found a duplicate in the result of Distinct(): " + i);
                hashSet.Add(i);
            }

            // Ensure that every element in 'originalList' exists in 'distinctList'.
            Assert.DoesNotContain(originalList, x => !distinctList.Contains(x));

            // Ensure that every element in 'distinctList' exists in 'originalList'.
            Assert.DoesNotContain(distinctList, x => !originalList.Contains(x));
        }

        [Fact]
        public void Distinct()
        {
            // Validate an array of different numeric data types.
            FindDistinctAndValidate(new int[] {1, 1, 1, 2, 3, 5, 5, 6, 6, 10});
            FindDistinctAndValidate(new long[] {1, 1, 1, 2, 3, 5, 5, 6, 6, 10});
            FindDistinctAndValidate(new float[] {1, 1, 1, 2, 3, 5, 5, 6, 6, 10});
            FindDistinctAndValidate(new double[] {1, 1, 1, 2, 3, 5, 5, 6, 6, 10});
            FindDistinctAndValidate(new decimal[] {1, 1, 1, 2, 3, 5, 5, 6, 6, 10});
            // Try strings
            FindDistinctAndValidate(new []
            {
                "add",
                "add",
                "subtract",
                "multiply",
                "divide",
                "divide2",
                "subtract",
                "add",
                "power",
                "exponent",
                "hello",
                "class",
                "namespace",
                "namespace",
                "namespace",
            });
        }

        [Fact]
        public void Min()
        {
            var one = Enumerable.Range(1, 10).ToArray();
            var minusTen = new [] {-1, -10, 10, 200, 1000};
            var hundred = new [] {3000, 100, 200, 1000};
            Assert.Equal(1, one.Min());
            Assert.Equal(-10, minusTen.Min());
            Assert.Equal(100, hundred.Min());
        }

        [Fact]
        public void Max()
        {
            var ten = Enumerable.Range(1, 10).ToArray();
            var minusTen = new[] {-100, -15, -50, -10};
            var thousand = new[] {-16, 0, 50, 100, 1000};
            Assert.Equal(10, ten.Max());
            Assert.Equal(-10, minusTen.Max());
            Assert.Equal(1000, thousand.Max());
        }

        private class ExtremeComparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                if (x == y)
                    return 0;
                if (x < y)
                    return int.MinValue;
                return int.MaxValue;
            }
        }

        [Fact]
        public void OrderByExtremeComparer()
        {
            var outOfOrder = new[] { 7, 1, 0, 9, 3, 5, 4, 2, 8, 6 };
            Assert.Equal(Enumerable.Range(0, 10), outOfOrder.OrderBy(i => i, new ExtremeComparer()));
            Assert.Equal(Enumerable.Range(0, 10).Reverse(), outOfOrder.OrderByDescending(i => i, new ExtremeComparer()));
        }

#pragma warning disable 1720 // Triggered on purpose to test exception.

        [Fact]
        public void MinStringByInt()
        {
            var one = Enumerable.Range(1, 10).Select(i => "A" + i).ToArray();
            var minusTen = new[] { "B-1", "B-10", "B10", "B200", "B1000" };
            var hundred = new[] { "C3000", "C100", "C200", "C1000" };
            Assert.Equal("A1", one.MinBy(s => int.Parse(s.Substring(1))));
            Assert.Equal("B-10", minusTen.MinBy(s => int.Parse(s.Substring(1))));
            Assert.Equal("C100", hundred.MinBy(s => int.Parse(s.Substring(1))));
            Assert.Equal(null, Enumerable.Empty<string>().MinBy(s => int.Parse(s.Substring(1))));
            Assert.Throws<ArgumentNullException>(() => default(IEnumerable<int>).MinBy(s => s));
            Assert.Throws<ArgumentNullException>(() => one.MinBy(default(Func<string, int>)));
            Assert.Throws<ArgumentNullException>(() => one.MinBy(default(Func<string, int>), null));
        }
        [Fact]
        public void MinIntByString()
        {
            var fifteenEitherWay = Enumerable.Range(-15, 31).ToArray();
            var minusTen = new[] { -1, -10, 10, 200, 1000 };
            var hundred = new[] { 3000, 100, 200, 1000 };
            Assert.Equal(0, fifteenEitherWay.MinBy(i => i.ToString()));
            Assert.Equal(-1, minusTen.MinBy(i => i.ToString()));
            Assert.Equal(100, hundred.MinBy(i => i.ToString()));
        }
        [Fact]
        public void MaxStringByInt()
        {
            var ten = Enumerable.Range(1, 10).Select(i => "A" + i).ToArray();
            var minusTen = new[] { "B-100", "B-15", "B-50", "B-10" };
            var thousand = new[] { "C-16", "C0", "C50", "C100", "C1000" };
            Assert.Equal("A10", ten.MaxBy(s => int.Parse(s.Substring(1))));
            Assert.Equal("B-10", minusTen.MaxBy(s => int.Parse(s.Substring(1))));
            Assert.Equal("C1000", thousand.MaxBy(s => int.Parse(s.Substring(1))));
            Assert.Equal(null, Enumerable.Empty<string>().MaxBy(s => int.Parse(s.Substring(1))));
            Assert.Throws<ArgumentNullException>(() => default(IEnumerable<int>).MaxBy(s => s));
            Assert.Throws<ArgumentNullException>(() => ten.MaxBy(default(Func<string, int>)));
            Assert.Throws<ArgumentNullException>(() => ten.MaxBy(default(Func<string, int>), null));
        }
        [Fact]
        public void MaxIntByString()
        {
            var ten = Enumerable.Range(3, 10);
            var minusTen = new[] { -100, -15, -50, -10 };
            var thousand = new[] { -16, 0, 50, 100, 1000 };
            Assert.Equal(9, ten.MaxBy(i => i.ToString()));
            Assert.Equal(-50, minusTen.MaxBy(i => i.ToString()));
            Assert.Equal(50, thousand.MaxBy(i => i.ToString()));
        }
        [Fact]
        public void KeyedNoElements()
        {
            Assert.Throws<InvalidOperationException>(() => Enumerable.Empty<int>().MaxBy(i => i));
            Assert.Throws<InvalidOperationException>(() => Enumerable.Empty<int>().MinBy(i => i));
        }
        [Fact]
        public void DistinctBy()
        {
            Assert.Equal(Enumerable.Range(0, 100).DistinctBy(i => i / 10), Enumerable.Range(0, 10).Select(i => i * 10));
            Assert.Equal(Enumerable.Range(0, 100).DistinctBy(i => i / 10, EqualityComparer<int>.Default), Enumerable.Range(0, 10).Select(i => i * 10));
            Assert.Throws<ArgumentNullException>(() => default(IEnumerable<int>).DistinctBy(i => i));
            Assert.Throws<ArgumentNullException>(() => Enumerable.Range(0, 1).DistinctBy(default(Func<int, int>)));
        }
        [Fact]
        public void Chunk()
        {
            Assert.Equal(3, Enumerable.Range(0, 12).Chunk(5).Count());
            Assert.Equal(3, Enumerable.Range(0, 15).Chunk(5).Count());
            Assert.Equal(4, Enumerable.Range(0, 16).Chunk(5).Count());
            Assert.Equal(new[] { 5, 5, 2 }, Enumerable.Range(0, 12).Chunk(5).Select(c => c.Count()));
            Assert.Equal(Enumerable.Range(0, 3), Enumerable.Range(0, 12).Chunk(5).Select(g => g.Key));
            Assert.Equal(Enumerable.Range(0, 12), Enumerable.Range(0, 12).Chunk(7).SelectMany(g => g));
            Assert.Throws<ArgumentOutOfRangeException>(() => Enumerable.Range(0, 3).Chunk(0));
            Assert.Throws<ArgumentNullException>(() => default(IEnumerable<int>).Chunk(10));
            Assert.Equal(3, Enumerable.Range(0, 12).ToList().Chunk(5).Count());
            Assert.Equal(3, Enumerable.Range(0, 15).ToList().Chunk(5).Count());
            Assert.Equal(4, Enumerable.Range(0, 16).ToList().Chunk(5).Count());
            Assert.Equal(new[] { 5, 5, 2 }, Enumerable.Range(0, 12).ToList().Chunk(5).Select(c => c.Count()));
            Assert.Equal(Enumerable.Range(0, 3), Enumerable.Range(0, 12).ToList().Chunk(5).Select(g => g.Key));
            Assert.Equal(Enumerable.Range(0, 12), Enumerable.Range(0, 12).ToList().Chunk(7).SelectMany(g => g));
            Assert.Throws<ArgumentOutOfRangeException>(() => Enumerable.Range(0, 3).ToList().Chunk(0));
        }
        [Fact]
        public void ChunkRacingEnumerators()
        {
            var chunks = Enumerable.Range(0, 13).Select(i => i % 5).Chunk(5)
                .Concat(Enumerable.Range(0, 9).Select(i => i % 5).ToList().Chunk(5));
            foreach (var chunk in chunks)
            {
                using (var en = (IEnumerator<int>)((IEnumerable)chunk).GetEnumerator())
                {
                    en.MoveNext();
                    Assert.Equal(0, en.Current);
                    en.MoveNext();
                    Assert.Equal(1, en.Current);
                    int count = 0;
                    foreach (var item in chunk)
                        Assert.Equal(count++, item);
                    en.MoveNext();
                    Assert.Equal(2, en.Current);
                    if (en.MoveNext())
                        Assert.Equal(3, en.Current);
                }
            }
        }
        [Fact]
        public void UnionBy()
        {
            Assert.Equal(
                new[] { "a1", "c2", "d3", "g4", "h5", "i9", "m8" },
                new[] { "a1", "b1", "c2", "d3", "e3", "f2", "g4", "h5", "i2", "j1" }.UnionBy(
                    new[] { "h2", "i9", "j2", "k2", "l3", "m8" }, s => s.Substring(1)
                )
            );
            Assert.Equal(
                new[] { "a1", "c2", "d3", "g4", "h5", "i9", "m8" },
                new[] { "a1", "b1", "c2", "d3", "e3", "f2", "g4", "h5", "i2", "j1" }.UnionBy(
                    new[] { "h2", "i9", "j2", "k2", "l3", "m8" }, s => s.Substring(1), EqualityComparer<string>.Default
                )
            );
            Assert.Throws<ArgumentNullException>(() => default(IEnumerable<int>).UnionBy(Enumerable.Range(0, 1), i => i));
            Assert.Throws<ArgumentNullException>(() => Enumerable.Range(0, 1).UnionBy(null, i => i));
            Assert.Throws<ArgumentNullException>(() => Enumerable.Range(0, 1).UnionBy(Enumerable.Range(0, 1), default(Func<int, int>)));
        }
        [Fact]
        public void IntersectBy()
        {
            Assert.Equal(
                new[] { "c2", "d3" },
                new[] { "a1", "b1", "c2", "d3", "e3", "f2", "g4", "h5", "i2", "j1" }.IntersectBy(
                    new[] { "h2", "i9", "j2", "k2", "l3", "m8" }, s => s.Substring(1)
                )
            );
            Assert.Equal(
                new[] { "c2", "d3" },
                new[] { "a1", "b1", "c2", "d3", "e3", "f2", "g4", "h5", "i2", "j1" }.IntersectBy(
                    new[] { "h2", "i9", "j2", "k2", "l3", "m8" }, s => s.Substring(1), EqualityComparer<string>.Default
                )
            );
            Assert.Throws<ArgumentNullException>(() => default(IEnumerable<int>).IntersectBy(Enumerable.Range(0, 1), i => i));
            Assert.Throws<ArgumentNullException>(() => Enumerable.Range(0, 1).IntersectBy(null, i => i));
            Assert.Throws<ArgumentNullException>(() => Enumerable.Range(0, 1).IntersectBy(Enumerable.Range(0, 1), default(Func<int, int>)));
        }
        [Fact]
        public void ExceptBy()
        {
            Assert.Equal(
                new[] { "a1", "g4", "h5" },
                new[] { "a1", "b1", "c2", "d3", "e3", "f2", "g4", "h5", "i2", "j1" }.ExceptBy(
                    new[] { "h2", "i9", "j2", "k2", "l3", "m8" }, s => s.Substring(1)
                )
            );
            Assert.Equal(
                new[] { "a1", "g4", "h5" },
                new[] { "a1", "b1", "c2", "d3", "e3", "f2", "g4", "h5", "i2", "j1" }.ExceptBy(
                    new[] { "h2", "i9", "j2", "k2", "l3", "m8" }, s => s.Substring(1), EqualityComparer<string>.Default
                )
            );
            Assert.Throws<ArgumentNullException>(() => default(IEnumerable<int>).ExceptBy(Enumerable.Range(0, 1), i => i));
            Assert.Throws<ArgumentNullException>(() => Enumerable.Range(0, 1).ExceptBy(null, i => i));
            Assert.Throws<ArgumentNullException>(() => Enumerable.Range(0, 1).ExceptBy(Enumerable.Range(0, 1), default(Func<int, int>)));
        }
#pragma warning restore 1720 // Triggered on purpose to test exception.
    }
}



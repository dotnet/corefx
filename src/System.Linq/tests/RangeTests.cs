﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.Linq.Tests
{
    public class RangeTests : EnumerableTests
    {
        [Fact]
        public void Range_ProduceCorrectSequence()
        {
            var rangeSequence = Enumerable.Range(1, 100);
            int expected = 0;
            foreach (var val in rangeSequence)
            {
                expected++;
                Assert.Equal(expected, val);
            }

            Assert.Equal(100, expected);
        }

        [Fact]
        public void Range_ToArray_ProduceCorrectResult()
        {
            var array = Enumerable.Range(1, 100).ToArray();
            Assert.Equal(array.Length, 100);
            for (var i = 0; i < array.Length; i++)
                Assert.Equal(i + 1, array[i]);
        }

        [Fact]
        public void Range_ToList_ProduceCorrectResult()
        {
            var list = Enumerable.Range(1, 100).ToList();
            Assert.Equal(list.Count, 100);
            for (var i = 0; i < list.Count; i++)
                Assert.Equal(i + 1, list[i]);
        }

        [Fact]
        public void Range_ZeroCountLeadToEmptySequence()
        {
            var array = Enumerable.Range(1, 0).ToArray();
            var array2 = Enumerable.Range(int.MinValue, 0).ToArray();
            var array3 = Enumerable.Range(int.MaxValue, 0).ToArray();
            Assert.Equal(array.Length, 0);
            Assert.Equal(array2.Length, 0);
            Assert.Equal(array3.Length, 0);
        }

        [Fact]
        public void Range_ThrowExceptionOnNegativeCount()
        {
            Assert.Throws<ArgumentOutOfRangeException>("count", () => Enumerable.Range(1, -1));
            Assert.Throws<ArgumentOutOfRangeException>("count", () => Enumerable.Range(1, int.MinValue));
        }

        [Fact]
        public void Range_ThrowExceptionOnOverflow()
        {
            Assert.Throws<ArgumentOutOfRangeException>("count", () => Enumerable.Range(1000, int.MaxValue));
            Assert.Throws<ArgumentOutOfRangeException>("count", () => Enumerable.Range(int.MaxValue, 1000));
            Assert.Throws<ArgumentOutOfRangeException>("count", () => Enumerable.Range(Int32.MaxValue - 10, 20));
        }

        [Fact]
        public void Range_NotEnumerateAfterEnd()
        {
            using (var rangeEnum = Enumerable.Range(1, 1).GetEnumerator())
            {
                Assert.True(rangeEnum.MoveNext());
                Assert.False(rangeEnum.MoveNext());
                Assert.False(rangeEnum.MoveNext());
            }
        }

        [Fact]
        public void Range_EnumerableAndEnumeratorAreSame()
        {
            var rangeEnumerable = Enumerable.Range(1, 1);
            using (var rangeEnumerator = rangeEnumerable.GetEnumerator())
            {
                Assert.Same(rangeEnumerable, rangeEnumerator);
            }
        }

        [Fact]
        public void Range_GetEnumeratorReturnUniqueInstances()
        {
            var rangeEnumerable = Enumerable.Range(1, 1);
            using (var enum1 = rangeEnumerable.GetEnumerator())
            using (var enum2 = rangeEnumerable.GetEnumerator())
            {
                Assert.NotSame(enum1, enum2);
            }
        }

        [Fact]
        public void Range_ToInt32MaxValue()
        {
            int from = Int32.MaxValue - 3;
            int count = 4;
            var rangeEnumerable = Enumerable.Range(from, count);

            Assert.Equal(count, rangeEnumerable.Count());

            int[] expected = { Int32.MaxValue - 3, Int32.MaxValue - 2, Int32.MaxValue - 1, Int32.MaxValue };
            Assert.Equal(expected, rangeEnumerable);
        }

        [Fact]
        public void RepeatedCallsSameResults()
        {
            Assert.Equal(Enumerable.Range(-1, 2), Enumerable.Range(-1, 2));
            Assert.Equal(Enumerable.Range(0, 0), Enumerable.Range(0, 0));
        }

        [Fact]
        public void NegativeStart()
        {
            int start = -5;
            int count = 1;
            int[] expected = { -5 };

            Assert.Equal(expected, Enumerable.Range(start, count));
        }

        [Fact]
        public void ArbitraryStart()
        {
            int start = 12;
            int count = 6;
            int[] expected = { 12, 13, 14, 15, 16, 17 };

            Assert.Equal(expected, Enumerable.Range(start, count));
        }

        [Fact]
        public void Take()
        {
            Assert.Equal(Enumerable.Range(0, 10), Enumerable.Range(0, 20).Take(10));
        }

        [Fact]
        public void TakeExcessive()
        {
            Assert.Equal(Enumerable.Range(0, 10), Enumerable.Range(0, 10).Take(int.MaxValue));
        }

        [Fact]
        public void Skip()
        {
            Assert.Equal(Enumerable.Range(10, 10), Enumerable.Range(0, 20).Skip(10));
        }

        [Fact]
        public void SkipExcessive()
        {
            Assert.Empty(Enumerable.Range(10, 10).Skip(20));
        }

        [Fact]
        public void ElementAt()
        {
            Assert.Equal(4, Enumerable.Range(0, 10).ElementAt(4));
        }

        [Fact]
        public void ElementAtExcessiveThrows()
        {
            Assert.Throws<ArgumentOutOfRangeException>("index", () => Enumerable.Range(0, 10).ElementAt(100));
        }

        [Fact]
        public void ElementAtOrDefault()
        {
            Assert.Equal(4, Enumerable.Range(0, 10).ElementAtOrDefault(4));
        }

        [Fact]
        public void ElementAtOrDefaultExcessiveIsDefault()
        {
            Assert.Equal(0, Enumerable.Range(52, 10).ElementAtOrDefault(100));
        }

        [Fact]
        public void First()
        {
            Assert.Equal(57, Enumerable.Range(57, 1000000000).First());
        }

        [Fact]
        public void FirstOrDefault()
        {
            Assert.Equal(-100, Enumerable.Range(-100, int.MaxValue).FirstOrDefault());
        }

        [Fact]
        public void Last()
        {
            Assert.Equal(1000000056, Enumerable.Range(57, 1000000000).Last());
        }

        [Fact]
        public void LastOrDefault()
        {
            Assert.Equal(int.MaxValue - 101, Enumerable.Range(-100, int.MaxValue).LastOrDefault());
        }
    }
}

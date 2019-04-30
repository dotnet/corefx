// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => Enumerable.Range(1, -1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => Enumerable.Range(1, int.MinValue));
        }

        [Fact]
        public void Range_ThrowExceptionOnOverflow()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => Enumerable.Range(1000, int.MaxValue));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => Enumerable.Range(int.MaxValue, 1000));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => Enumerable.Range(int.MaxValue - 10, 20));
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
            int from = int.MaxValue - 3;
            int count = 4;
            var rangeEnumerable = Enumerable.Range(from, count);

            Assert.Equal(count, rangeEnumerable.Count());

            int[] expected = { int.MaxValue - 3, int.MaxValue - 2, int.MaxValue - 1, int.MaxValue };
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
        public void SkipTakeCanOnlyBeOne()
        {
            Assert.Equal(new[] { 1 }, Enumerable.Range(1, 10).Take(1));
            Assert.Equal(new[] { 2 }, Enumerable.Range(1, 10).Skip(1).Take(1));
            Assert.Equal(new[] { 3 }, Enumerable.Range(1, 10).Take(3).Skip(2));
            Assert.Equal(new[] { 1 }, Enumerable.Range(1, 10).Take(3).Take(1));
        }

        [Fact]
        public void ElementAt()
        {
            Assert.Equal(4, Enumerable.Range(0, 10).ElementAt(4));
        }

        [Fact]
        public void ElementAtExcessiveThrows()
        {
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => Enumerable.Range(0, 10).ElementAt(100));
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
        [SkipOnTargetFramework(~TargetFrameworkMonikers.Netcoreapp, ".NET Core optimizes Enumerable.Range().Last(). Without this optimization, this test takes a long time. See https://github.com/dotnet/corefx/pull/2401.")]
        public void Last()
        {
            Assert.Equal(1000000056, Enumerable.Range(57, 1000000000).Last());
        }

        [Fact]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.Netcoreapp, ".NET Core optimizes Enumerable.Range().LastOrDefault(). Without this optimization, this test takes a long time. See https://github.com/dotnet/corefx/pull/2401.")]
        public void LastOrDefault()
        {
            Assert.Equal(int.MaxValue - 101, Enumerable.Range(-100, int.MaxValue).LastOrDefault());
        }

        [Fact]
        public void ICollection_IsReadOnly()
        {
            var rangeSequence = Enumerable.Range(1, 100) as ICollection<int>;

            Assert.Equal(true, rangeSequence.IsReadOnly);
            Assert.Throws<NotSupportedException>(() => rangeSequence.Add(0));
            Assert.Throws<NotSupportedException>(() => rangeSequence.Remove(0));
            Assert.Throws<NotSupportedException>(() => rangeSequence.Clear());
        }

        [Fact]
        public void ICollection_Count()
        {
            var rangeSequence = Enumerable.Range(1, 100) as ICollection<int>;

            Assert.Equal(100, rangeSequence.Count);
        }

        [Fact]
        public void ICollection_CopyTo_ProduceCorrectSequence()
        {
            var rangeSequence = Enumerable.Range(1, 100) as ICollection<int>;

            var arrayIndex = 10;
            var array = new int[rangeSequence.Count + arrayIndex];
            rangeSequence.CopyTo(array, arrayIndex);
            
            int expected = 0;
            for (var index = arrayIndex; index < rangeSequence.Count + arrayIndex; index++)
            {
                expected++;
                Assert.Equal(expected, array[index]);
            }

            Assert.Equal(100, expected);
        }

        [Fact]
        public void ICollection_CopyTo_ThrowExceptionOnNullArray()
        {
            var rangeSequence = Enumerable.Range(1, 100) as ICollection<int>;

            AssertExtensions.Throws<ArgumentNullException>("array", () => rangeSequence.CopyTo(null, 0));
        }

        [Fact]
        public void ICollection_CopyTo_ThrowExceptionOnOutOfRangeArrayIndex()
        {
            var rangeSequence = Enumerable.Range(1, 100) as ICollection<int>;
            var array = new int[100];

            AssertExtensions.Throws<ArgumentOutOfRangeException>("arrayIndex", () => rangeSequence.CopyTo(array, int.MinValue));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("arrayIndex", () => rangeSequence.CopyTo(array, -1));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("arrayIndex", () => rangeSequence.CopyTo(array, 100));
            AssertExtensions.Throws<ArgumentOutOfRangeException>("arrayIndex", () => rangeSequence.CopyTo(array, int.MaxValue));
        }

        [Fact]
        public void ICollection_CopyTo_ThrowExceptionOnArrayOverflow()
        {
            var rangeSequence = Enumerable.Range(1, 100) as ICollection<int>;
            var array = new int[100];

            Assert.Throws<ArgumentException>(() => rangeSequence.CopyTo(array, 1));
        }

        [Fact]
        public void ICollection_Contains()
        {
            var emptyRangeSequence = Enumerable.Range(0, 0) as ICollection<int>;

            Assert.Equal(false, emptyRangeSequence.Contains(int.MinValue));
            Assert.Equal(false, emptyRangeSequence.Contains(0));
            Assert.Equal(false, emptyRangeSequence.Contains(int.MaxValue));

            var rangeSequence = Enumerable.Range(1, 100) as ICollection<int>;

            Assert.Equal(false, rangeSequence.Contains(int.MinValue));
            Assert.Equal(false, rangeSequence.Contains(0));
            Assert.Equal(true, rangeSequence.Contains(1));
            Assert.Equal(true, rangeSequence.Contains(100));
            Assert.Equal(false, rangeSequence.Contains(101));
            Assert.Equal(false, rangeSequence.Contains(int.MaxValue));
        }

        [Fact]
        public void IList_IsReadOnly()
        {
            var rangeSequence = Enumerable.Range(0, 100) as IList<int>;

            Assert.Throws<NotSupportedException>(() => rangeSequence.Insert(0, 0));
            Assert.Throws<NotSupportedException>(() => rangeSequence.RemoveAt(0));
        }

        [Fact]
        public void IList_Indexer_ProduceCorrectSequence()
        {
            var rangeSequence = Enumerable.Range(1, 100) as IList<int>;
            int expected = 0;
            for (var index = 0; index < rangeSequence.Count; index++)
            {
                expected++;
                Assert.Equal(expected, rangeSequence[index]);
            }

            Assert.Equal(100, expected);
        }

        [Fact]
        public void IList_Indexer_ThrowExceptionOnOutOfRangeIndex()
        {
            var rangeSequence = Enumerable.Range(0, 100) as IList<int>;

            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => rangeSequence[int.MinValue]);
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => rangeSequence[-1]);
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => rangeSequence[100]);
            AssertExtensions.Throws<ArgumentOutOfRangeException>("index", () => rangeSequence[int.MaxValue]);
        }

        [Fact]
        public void IList_IndexOf()
        {
            var emptyRangeSequence = Enumerable.Range(0, 0) as IList<int>;

            Assert.Equal(-1, emptyRangeSequence.IndexOf(int.MinValue));
            Assert.Equal(-1, emptyRangeSequence.IndexOf(0));
            Assert.Equal(-1, emptyRangeSequence.IndexOf(int.MaxValue));

            var rangeSequence = Enumerable.Range(1, 100) as IList<int>;

            Assert.Equal(-1, rangeSequence.IndexOf(int.MinValue));
            Assert.Equal(-1, rangeSequence.IndexOf(0));
            Assert.Equal(0, rangeSequence.IndexOf(1));
            Assert.Equal(99, rangeSequence.IndexOf(100));
            Assert.Equal(-1, rangeSequence.IndexOf(101));
            Assert.Equal(-1, rangeSequence.IndexOf(int.MaxValue));
        }    
    }
}

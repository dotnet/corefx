// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Linq.Tests
{
    public class TryElementAtTests : EnumerableTests
    {
        [Fact]
        public void SameResultsRepeatCallsIntQuery()
        {
            var q = from x in new[] { 9999, 0, 888, -1, 66, -777, 1, 2, -12345 }
                    where x > int.MinValue
                    select x;

            int r1, r2;
            Assert.Equal(q.TryElementAt(3, out r1), q.TryElementAt(3, out r2));
            Assert.Equal(r1, r2);
        }

        [Fact]
        public void SameResultsRepeatCallsStringQuery()
        {
            var q = from x in new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS", string.Empty }
                    where !string.IsNullOrEmpty(x)
                    select x;

            string r1, r2;
            Assert.Equal(q.TryElementAt(4, out r1), q.TryElementAt(4, out r2));
            Assert.Equal(r1, r2);
        }

        public static IEnumerable<object[]> TestData()
        {
            yield return new object[] { NumberRangeGuaranteedNotCollectionType(9, 1), 0, true, 9 };
            yield return new object[] { NumberRangeGuaranteedNotCollectionType(9, 10), 9, true, 18 };
            yield return new object[] { NumberRangeGuaranteedNotCollectionType(-4, 10), 3, true, -1 };

            yield return new object[] { new int[] { 1, 2, 3, 4 }, 4, false, 0 };
            yield return new object[] { new int[0], 0, false, 0 };
            yield return new object[] { new int[] { -4 }, 0, true, -4 };
            yield return new object[] { new int[] { 9, 8, 0, -5, 10 }, 4, true, 10 };

            yield return new object[] { NumberRangeGuaranteedNotCollectionType(-4, 5), -1, false, 0 };
            yield return new object[] { NumberRangeGuaranteedNotCollectionType(5, 5), 5, false, 0 };
            yield return new object[] { NumberRangeGuaranteedNotCollectionType(0, 0), 0, false, 0 };
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void TryElementAt(IEnumerable<int> source, int index, bool found, int expected)
        {
            int r1;
            Assert.Equal(found, source.TryElementAt(index, out r1));
            Assert.Equal(expected, r1);
        }

        [Fact]
        public void NullableArray_NegativeIndex_ReturnsNull()
        {
            int?[] source = { 9, 8 };

            int? r1;
            Assert.Equal(false, source.TryElementAt(-1, out r1));
            Assert.Null(r1);
        }

        [Fact]
        public void NullableArray_ValidIndex_ReturnsCorrectObjecvt()
        {
            int?[] source = { 9, 8, null, -5, 10 };

            int? r1;
            Assert.Equal(true, source.TryElementAt(2, out r1));
            Assert.Null(r1);
            Assert.Equal(true, source.TryElementAt(3, out r1));
            Assert.Equal(-5, r1);
        }

        [Fact]
        public void NullSource_ThrowsArgumentNullException()
        {
            int r1;
            Assert.Throws<ArgumentNullException>("source", () => ((IEnumerable<int>)null).TryElementAt(2, out r1));
        }
    }
}

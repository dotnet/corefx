// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Linq.Tests
{
    public class ExceptTests : EnumerableTests
    {
        [Fact]
        public void SameResultsRepeatCallsIntQuery()
        {
            var q1 = from x1 in new int?[] { 2, 3, null, 2, null, 4, 5 }
                     select x1;
            var q2 = from x2 in new int?[] { 1, 9, null, 4 }
                     select x2;

            Assert.Equal(q1.Except(q2), q1.Except(q2));
        }

        [Fact]
        public void SameResultsRepeatCallsStringQuery()
        {
            var q1 = from x1 in new[] { "AAA", string.Empty, "q", "C", "#", "!@#$%^", "0987654321", "Calling Twice" }
                     select x1;
            var q2 = from x2 in new[] { "!@#$%^", "C", "AAA", "", "Calling Twice", "SoS" }
                     select x2;

            var rst1 = q1.Except(q2);
            var rst2 = q1.Except(q2);

            Assert.Equal(q1.Except(q2), q1.Except(q2));
        }

        public static IEnumerable<object[]> Int_TestData()
        {
            yield return new object[] { new int[0], new int[0], null, new int[0] };
            yield return new object[] { new int[0], new int[] { -6, -8, -6, 2, 0, 0, 5, 6 }, null, new int[0] };

            yield return new object[] { new int[] { 1, 1, 1, 1, 1 }, new int[] { 2, 3, 4 }, null, new int[] { 1 } };
        }

        [Theory]
        [MemberData(nameof(Int_TestData))]
        public void Int(IEnumerable<int> first, IEnumerable<int> second, IEqualityComparer<int> comparer, IEnumerable<int> expected)
        {
            if (comparer == null)
            {
                Assert.Equal(expected, first.Except(second));
            }
            Assert.Equal(expected, first.Except(second, comparer));
        }

        public static IEnumerable<object[]> String_TestData()
        {
            IEqualityComparer <string> defaultComparer = EqualityComparer<string>.Default;
            yield return new object[] { new string[1], new string[0], defaultComparer, new string[1] };
            yield return new object[] { new string[] { null, null, string.Empty }, new string[1], defaultComparer, new string[] { string.Empty } };
            yield return new object[] { new string[2], new string[0], defaultComparer, new string[1] };

            yield return new object[] { new string[] { "Bob", "Tim", "Robert", "Chris" }, new string[] { "bBo", "shriC" }, null, new string[] { "Bob", "Tim", "Robert", "Chris" } };
            yield return new object[] { new string[] { "Bob", "Tim", "Robert", "Chris" }, new string[] { "bBo", "shriC" }, new AnagramEqualityComparer(), new string[] { "Tim", "Robert" } };
        }

        [Theory]
        [MemberData(nameof(String_TestData))]
        public void String(IEnumerable<string> first, IEnumerable<string> second, IEqualityComparer<string> comparer, IEnumerable<string> expected)
        {
            if (comparer == null)
            {
                Assert.Equal(expected, first.Except(second));
            }
            Assert.Equal(expected, first.Except(second, comparer));
        }

        public static IEnumerable<object[]> NullableInt_TestData()
        {
            yield return new object[] { new int?[] { -6, -8, -6, 2, 0, 0, 5, 6, null, null }, new int?[0], new int?[] { -6, -8, 2, 0, 5, 6, null } };

            yield return new object[] { new int?[] { 1, 2, 2, 3, 4, 5 }, new int?[] { 5, 3, 2, 6, 6, 3, 1, null, null }, new int?[] { 4 } };
            yield return new object[] { new int?[] { 2, 3, null, 2, null, 4, 5 }, new int?[] { 1, 9, null, 4 }, new int?[] { 2, 3, 5 } };
        }

        [Theory]
        [MemberData(nameof(NullableInt_TestData))]
        public void NullableInt(IEnumerable<int?> first, IEnumerable<int?> second, IEnumerable<int?> expected)
        {
            Assert.Equal(expected, first.Except(second));
        }

        [Theory]
        [MemberData(nameof(NullableInt_TestData))]
        public void NullableIntRunOnce(IEnumerable<int?> first, IEnumerable<int?> second, IEnumerable<int?> expected)
        {
            Assert.Equal(expected, first.RunOnce().Except(second.RunOnce()));
        }

        [Fact]
        public void FirstNull_ThrowsArgumentNullException()
        {
            string[] first = null;
            string[] second = { "bBo", "shriC" };

            AssertExtensions.Throws<ArgumentNullException>("first", () => first.Except(second));
            AssertExtensions.Throws<ArgumentNullException>("first", () => first.Except(second, new AnagramEqualityComparer()));
        }
        
        [Fact]
        public void SecondNull_ThrowsArgumentNullException()
        {
            string[] first = { "Bob", "Tim", "Robert", "Chris" };
            string[] second = null;

            AssertExtensions.Throws<ArgumentNullException>("second", () => first.Except(second));
            AssertExtensions.Throws<ArgumentNullException>("second", () => first.Except(second, new AnagramEqualityComparer()));
        }

        [Fact]
        public void ForcedToEnumeratorDoesntEnumerate()
        {
            var iterator = NumberRangeGuaranteedNotCollectionType(0, 3).Except(Enumerable.Range(0, 3));
            // Don't insist on this behaviour, but check it's correct if it happens
            var en = iterator as IEnumerator<int>;
            Assert.False(en != null && en.MoveNext());
        }

        [Fact]
        public void HashSetWithBuiltInComparer_HashSetContainsNotUsed()
        {
            IEnumerable<string> input1 = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "a" };
            IEnumerable<string> input2 = new[] { "A" };

            Assert.Equal(new[] { "a" }, input1.Except(input2));
            Assert.Equal(new[] { "a" }, input1.Except(input2, null));
            Assert.Equal(new[] { "a" }, input1.Except(input2, EqualityComparer<string>.Default));
            Assert.Equal(Enumerable.Empty<string>(), input1.Except(input2, StringComparer.OrdinalIgnoreCase));

            Assert.Equal(new[] { "A" }, input2.Except(input1));
            Assert.Equal(new[] { "A" }, input2.Except(input1, null));
            Assert.Equal(new[] { "A" }, input2.Except(input1, EqualityComparer<string>.Default));
            Assert.Equal(Enumerable.Empty<string>(), input2.Except(input1, StringComparer.OrdinalIgnoreCase));
        }
    }
}

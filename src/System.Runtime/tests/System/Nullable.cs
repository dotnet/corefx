// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

using Xunit;

namespace System.Runtime.Tests
{
    public static class NullableTests
    {
        [Fact]
        public static void TestBasics()
        {
            // Nullable and Nullable<T> are mostly verbatim ports so we don't test much here.
            int? n = default(int?);
            Assert.False(n.HasValue);
            Assert.Throws<InvalidOperationException>(() => n.Value);
            Assert.Throws<InvalidOperationException>(() => (int)n);
            Assert.Equal(null, n);
            Assert.NotEqual(7, n);
            Assert.Equal(0, n.GetHashCode());
            Assert.Equal("", n.ToString());
            Assert.Equal(default(int), n.GetValueOrDefault());
            Assert.Equal(999, n.GetValueOrDefault(999));

            n = new int?(42);
            Assert.True(n.HasValue);
            Assert.Equal(42, n.Value);
            Assert.Equal(42, (int)n);
            Assert.NotEqual(null, n);
            Assert.NotEqual(7, n);
            Assert.Equal(42, n);
            Assert.Equal(42.GetHashCode(), n.GetHashCode());
            Assert.Equal(42.ToString(), n.ToString());
            Assert.Equal(42, n.GetValueOrDefault());
            Assert.Equal(42, n.GetValueOrDefault(999));

            n = 88;
            Assert.True(n.HasValue);
            Assert.Equal(88, n.Value);
        }

        [Fact]
        public static void TestBoxing()
        {
            var n = new int?(42);
            Unbox(n);
        }

        private static void Unbox(object o)
        {
            Type t = o.GetType();

            Assert.IsNotType<int?>(t);
            Assert.Equal(typeof(int), t);
        }

        public static IEnumerable<object[]> CompareEqualsTestData()
        {
            yield return new object[] { default(int?), default(int?), 0 };
            yield return new object[] { new int?(7), default(int?), 1 };
            yield return new object[] { default(int?), new int?(7), -1 };
            yield return new object[] { new int?(7), new int?(7), 0 };
            yield return new object[] { new int?(7), new int?(5), 1 };
            yield return new object[] { new int?(5), new int?(7), -1 };
        }

        [Theory, MemberData("CompareEqualsTestData")]
        public static void TestCompareEquals(int? n1, int? n2, int expected)
        {
            Assert.Equal(expected == 0, Nullable.Equals(n1, n2));
            Assert.Equal(expected, Nullable.Compare(n1, n2));
        }

        [Theory]
        [InlineData(typeof(int?), typeof(int))]
        [InlineData(typeof(int), null)]
        [InlineData(typeof(G<int>), null)]
        public static void TestGetUnderlyingType(Type nullableType, Type expected)
        {
            Assert.Equal(expected, Nullable.GetUnderlyingType(nullableType));
        }

        [Fact]
        public static void TestGetUnderlyingType_Invalid()
        {
            Assert.Throws<ArgumentNullException>("nullableType", () => Nullable.GetUnderlyingType(null)); // Type is null
        }

        private class G<T>
        {
        }
    }
}

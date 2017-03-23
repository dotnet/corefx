// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Tests
{
    public static partial class DBNullTests
    {
        [Fact]
        public static void TestToUInt64_Invalid()
        {
            Assert.Throws<InvalidCastException>(() => ((IConvertible)DBNull.Value).ToUInt64(null));
        }

        [Fact]
        public static void TestToChar_Invalid()
        {
            Assert.Throws<InvalidCastException>(() => ((IConvertible)DBNull.Value).ToChar(null));
        }

        [Fact]
        public static void TestToDateTime_Invalid()
        {
            Assert.Throws<InvalidCastException>(() => ((IConvertible)DBNull.Value).ToDateTime(null));
        }

        [Fact]
        public static void TestEquals_Invalid()
        {
            DBNull MyNull = null;
            Assert.False(DBNull.Value.Equals(MyNull));
        }

        [Fact]
        public static void Test_ToType()
        {
            Type typeParam = Type.GetType("System.String");
            Assert.Equal(string.Empty, ((IConvertible)DBNull.Value).ToType(typeParam, null));
        }

        [Fact]
        public static void Test_ToType_Invalid()
        {
            Type typeParam = Type.GetType("System.Int32");
            Assert.Throws<InvalidCastException>(() => ((IConvertible)DBNull.Value).ToType(typeParam, null));
        }
    }
}
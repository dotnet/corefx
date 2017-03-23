// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Tests
{
    public static partial class DBNullTests
    {
        [Fact]
        public static void TestToString()
        {
            Assert.Equal(string.Empty, DBNull.Value.ToString());
        }

        [Fact]
        public static void TestGetTypeCode()
        {
            Assert.Equal(TypeCode.DBNull, DBNull.Value.GetTypeCode());
        }

        [Fact]
        public static void TestEquals()
        {
            DBNull MyNull = DBNull.Value;
            Assert.True(DBNull.Value.Equals(MyNull));
        }

        [Fact]
        public static void Test_ConvertIsDBNull()
        {
            Assert.Equal(true, Convert.IsDBNull(DBNull.Value));
            Assert.Equal(false, Convert.IsDBNull((Int32)6678));
            Assert.Equal(false, Convert.IsDBNull(new object()));
        }

        [Fact]
        public static void Test_ConvertDBNUll()
        {
            Assert.Equal(Convert.DBNull, DBNull.Value);
        }
    }
}
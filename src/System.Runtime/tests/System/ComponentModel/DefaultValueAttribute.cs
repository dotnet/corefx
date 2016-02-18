// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.ComponentModel.Tests
{
    public static class DefaultValueAttributeTests
    {
        [Fact]
        public static void TestCtor_Bool()
        {
            Assert.Equal(true, new DefaultValueAttribute(true).Value);
            Assert.Equal(false, new DefaultValueAttribute(false).Value);
        }

        [Fact]
        public static void TestCtor_Byte()
        {
            Assert.Equal((byte)1, new DefaultValueAttribute((byte)1).Value);
        }

        [Fact]
        public static void TestCtor_Char()
        {
            Assert.Equal('c', new DefaultValueAttribute('c').Value);
        }

        [Fact]
        public static void TestCtor_Double()
        {
            Assert.Equal(3.14, new DefaultValueAttribute(3.14).Value);
        }

        [Fact]
        public static void TestCtor_Float()
        {
            Assert.Equal(3.14f, new DefaultValueAttribute(3.14f).Value);
        }

        [Fact]
        public static void TestCtor_Int()
        {
            Assert.Equal(42, new DefaultValueAttribute(42).Value);
        }

        [Fact]
        public static void TestCtor_Long()
        {
            Assert.Equal(42L, new DefaultValueAttribute(42L).Value);
        }

        [Fact]
        public static void TestCtor_Object()
        {
            Assert.Equal("test", new DefaultValueAttribute((object)"test").Value);
        }

        [Fact]
        public static void TestCtor_Short()
        {
            Assert.Equal((short)42, new DefaultValueAttribute((short)42).Value);
        }

        [Fact]
        public static void TestCtor_String()
        {
            Assert.Equal("test", new DefaultValueAttribute("test").Value);
        }

        [Fact]
        public static void TestCtor_Type_String()
        {
            Assert.Equal(DayOfWeek.Monday, new DefaultValueAttribute(typeof(DayOfWeek), "Monday").Value);
            Assert.Equal(TimeSpan.FromHours(1), new DefaultValueAttribute(typeof(TimeSpan), "1:00:00").Value);

            Assert.Equal(42, new DefaultValueAttribute(typeof(int), "42").Value);
            Assert.Null(new DefaultValueAttribute(typeof(int), "caughtException").Value);
        }

        [Fact]
        public static void TestEqual()
        {
            var attr = new DefaultValueAttribute(42);
            Assert.Equal(attr, attr);
            Assert.True(attr.Equals(attr));
            Assert.Equal(attr.GetHashCode(), attr.GetHashCode());

            Assert.Equal(attr, new DefaultValueAttribute(42));
            Assert.Equal(attr.GetHashCode(), new DefaultValueAttribute(42).GetHashCode());

            Assert.NotEqual(new DefaultValueAttribute(43), attr);
            Assert.NotEqual(new DefaultValueAttribute(null), attr);
            Assert.False(attr.Equals(null));

            attr = new DefaultValueAttribute(null);
            Assert.Equal(new DefaultValueAttribute(null), attr);
            Assert.Equal(attr.GetHashCode(), new DefaultValueAttribute(null).GetHashCode());
        }
    }
}

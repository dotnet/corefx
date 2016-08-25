// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit;

namespace System.Tests
{
    public static partial class CompareToTests
    {
        [Fact]
        public static void FromBoolean()
        {
            object testValue = false;
            object testValue2 = true;
            object wrongValue = "hello";
            bool val1 = false;
            bool val2 = true;
            Assert.Equal(0, val1.CompareTo(testValue));
            Assert.True(0 > val1.CompareTo(testValue2));
            Assert.True(0 < val2.CompareTo(testValue));
            Assert.Throws<ArgumentException>(() => val1.CompareTo(wrongValue));
        }

        [Fact]
        public static void FromChar()
        {
            object testValue = 'c';
            char val1 = 'c';
            char val2 = 'a';
            char val3 = 'd';
            object wrongValue = "hello";
            Assert.Equal(0, val1.CompareTo(testValue));
            Assert.True(0 > val2.CompareTo(testValue));
            Assert.True(0 < val3.CompareTo(testValue));
            Assert.Throws<ArgumentException>(() => val1.CompareTo(wrongValue));
        }

        [Fact]
        public static void FromDouble()
        {
            object testValue = 1.1;
            double val1 = 1.1;
            double val2 = double.MinValue;
            double val3 = double.MaxValue;
            object wrongValue = "hello";
            Assert.Equal(0, val1.CompareTo(testValue));
            Assert.True(0 > val2.CompareTo(testValue));
            Assert.True(0 < val3.CompareTo(testValue));
            Assert.Throws<ArgumentException>(() => val1.CompareTo(wrongValue));
        }

        [Fact]
        public static void FromInt16()
        {
            object testValue = (Int16)0;
            Int16 val1 = 0;
            Int16 val2 = Int16.MinValue;
            Int16 val3 = Int16.MaxValue;
            object wrongValue = "hello";
            Assert.Equal(0, val1.CompareTo(testValue));
            Assert.True(0 > val2.CompareTo(testValue));
            Assert.True(0 < val3.CompareTo(testValue));
            Assert.Throws<ArgumentException>(() => val1.CompareTo(wrongValue));
        }

        [Fact]
        public static void FromInt32()
        {
            object testValue = (Int32)0;
            Int32 val1 = 0;
            Int32 val2 = Int32.MinValue;
            Int32 val3 = Int32.MaxValue;
            object wrongValue = "hello";
            Assert.Equal(0, val1.CompareTo(testValue));
            Assert.True(0 > val2.CompareTo(testValue));
            Assert.True(0 < val3.CompareTo(testValue));
            Assert.Throws<ArgumentException>(() => val1.CompareTo(wrongValue));
        }

        [Fact]
        public static void FromInt64()
        {
            object testValue = (Int64)0;
            Int64 val1 = 0;
            Int64 val2 = Int64.MinValue;
            Int64 val3 = Int64.MaxValue;
            object wrongValue = "hello";
            Assert.Equal(0, val1.CompareTo(testValue));
            Assert.True(0 > val2.CompareTo(testValue));
            Assert.True(0 < val3.CompareTo(testValue));
            Assert.Throws<ArgumentException>(() => val1.CompareTo(wrongValue));
        }

        public static void FromSByte()
        {
            object testValue = sbyte.MinValue;
            object testNull = null;
            sbyte val1 = sbyte.MinValue;
            sbyte val2 = sbyte.MaxValue;
            object wrongValue = "hello";
            Assert.Equal(0, val1.CompareTo(testValue));
            Assert.True(0 > val1.CompareTo(testNull));
            Assert.True(0 < val2.CompareTo(testValue));
            Assert.Throws<ArgumentException>(() => val1.CompareTo(wrongValue));
        }

        [Fact]
        public static void FromSingle()
        {
            object testValue = (Single)1.1234f;
            Single val1 = 1.1234f;
            Single val2 = Single.MinValue;
            Single val3 = Single.MaxValue;
            object wrongValue = "hello";
            Assert.Equal(0, val1.CompareTo(testValue));
            Assert.True(0 > val2.CompareTo(testValue));
            Assert.True(0 < val3.CompareTo(testValue));
            Assert.Throws<ArgumentException>(() => val1.CompareTo(wrongValue));
        }

        [Fact]
        public static void FromUInt16()
        {
            object testValue = (UInt16)1;
            UInt16 val1 = 1;
            UInt16 val2 = UInt16.MinValue;
            UInt16 val3 = UInt16.MaxValue;
            object wrongValue = "hello";
            Assert.Equal(0, val1.CompareTo(testValue));
            Assert.True(0 > val2.CompareTo(testValue));
            Assert.True(0 < val3.CompareTo(testValue));
            Assert.Throws<ArgumentException>(() => val1.CompareTo(wrongValue));
        }

        [Fact]
        public static void FromUInt32()
        {
            object testValue = (UInt32)1;
            UInt32 val1 = 1;
            UInt32 val2 = UInt32.MinValue;
            UInt32 val3 = UInt32.MaxValue;
            object wrongValue = "hello";
            Assert.Equal(0, val1.CompareTo(testValue));
            Assert.True(0 > val2.CompareTo(testValue));
            Assert.True(0 < val3.CompareTo(testValue));
            Assert.Throws<ArgumentException>(() => val1.CompareTo(wrongValue));
        }

        [Fact]
        public static void FromUInt64()
        {
            object testValue = (UInt64)1;
            UInt64 val1 = 1;
            UInt64 val2 = UInt64.MinValue;
            UInt64 val3 = UInt64.MaxValue;
            object wrongValue = "hello";
            Assert.Equal(0, val1.CompareTo(testValue));
            Assert.True(0 > val2.CompareTo(testValue));
            Assert.True(0 < val3.CompareTo(testValue));
            Assert.Throws<ArgumentException>(() => val1.CompareTo(wrongValue));
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Xunit;

namespace System.Tests
{
    public static partial class GetTypeCodeTests
    {
        [Fact]
        public static void FromBoolean()
        {
            var val = false;
            TypeCode testValue = val.GetTypeCode();
            Assert.Equal(TypeCode.Boolean, testValue);
        }

        [Fact]
        public static void FromChar()
        {
            var val = char.MinValue;
            TypeCode testValue = val.GetTypeCode();
            Assert.Equal(TypeCode.Char, testValue);
        }

        [Fact]
        public static void FromDouble()
        {
            var val = double.MinValue;
            TypeCode testValue = val.GetTypeCode();
            Assert.Equal(TypeCode.Double, testValue);
        }

        [Fact]
        public static void FromInt16()
        {
            var val = Int16.MinValue;
            TypeCode testValue = val.GetTypeCode();
            Assert.Equal(TypeCode.Int16, testValue);
        }

        [Fact]
        public static void FromInt32()
        {
            var val = Int32.MinValue;
            TypeCode testValue = val.GetTypeCode();
            Assert.Equal(TypeCode.Int32, testValue);
        }

        [Fact]
        public static void FromInt64()
        {
            var val = Int64.MinValue;
            TypeCode testValue = val.GetTypeCode();
            Assert.Equal(TypeCode.Int64, testValue);
        }

        public static void FromSByte()
        {
            var val = sbyte.MinValue;
            TypeCode testValue = val.GetTypeCode();
            Assert.Equal(TypeCode.SByte, testValue);
        }

        [Fact]
        public static void FromSingle()
        {
            var val = Single.MinValue;
            TypeCode testValue = val.GetTypeCode();
            Assert.Equal(TypeCode.Single, testValue);
        }

        [Fact]
        public static void FromUInt16()
        {
            var val = UInt16.MinValue;
            TypeCode testValue = val.GetTypeCode();
            Assert.Equal(TypeCode.UInt16, testValue);
        }

        [Fact]
        public static void FromUInt32()
        {
            var val = UInt32.MinValue;
            TypeCode testValue = val.GetTypeCode();
            Assert.Equal(TypeCode.UInt32, testValue);

        }

        [Fact]
        public static void FromUInt64()
        {
            var val = Int64.MinValue;
            TypeCode testValue = val.GetTypeCode();
            Assert.Equal(TypeCode.Int64, testValue);
        }
    }
}

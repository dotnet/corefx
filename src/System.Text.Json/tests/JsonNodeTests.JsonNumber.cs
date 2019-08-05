﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Json.Tests
{
    public static partial class JsonNodeTests
    {
        private delegate bool TryGetValue<T>(JsonNumber number, out T result);

        private static void TestInitialization<T>(
                T value,
                Func<T, JsonNumber> ctor,
                Action<JsonNumber, T> setter,
                Func<JsonNumber, T> getter, 
                TryGetValue<T> tryGetter,
                Func<T, JsonNumber> implicitCaster)
        {
            // Default constructor:
            JsonNumber number = new JsonNumber();
            setter(number, value);
            AssertValue(value, number, getter, tryGetter);

            // Numeric type constructor:
            number = ctor(value);
            AssertValue(value, number, getter, tryGetter);

            // String constructor:
            number = new JsonNumber(value.ToString());
            AssertValue(value, number, getter, tryGetter);
            
            // Implicit cast:
            number = implicitCaster(value);
            AssertValue(value, number, getter, tryGetter);
        }

        private static void AssertValue<T>(
                T value,
                JsonNumber number,
                Func<JsonNumber, T> getter,
                TryGetValue<T> tryGetter)
        {
            Assert.Equal(value, getter(number));
            Assert.True(tryGetter(number, out T result));
            Assert.Equal(value, result);
        }

        [Fact]
        public static void TestDefaultCtor()
        {
            var jsonNumber = new JsonNumber();
            Assert.Equal(0, jsonNumber.GetByte());
            Assert.Equal(0, jsonNumber.GetInt16());
            Assert.Equal(0, jsonNumber.GetInt32());
            Assert.Equal(0, jsonNumber.GetInt64());
            Assert.Equal(0, jsonNumber.GetSingle());
            Assert.Equal(0, jsonNumber.GetDouble());
            Assert.Equal(0, jsonNumber.GetSByte());
            Assert.Equal((ushort)0, jsonNumber.GetUInt16());
            Assert.Equal((uint)0, jsonNumber.GetUInt32());
            Assert.Equal((ulong)0, jsonNumber.GetUInt64());
            Assert.Equal(0, jsonNumber.GetDecimal());
        }

        [Theory]
        [InlineData(17)]
        [InlineData(byte.MinValue)]
        [InlineData(byte.MaxValue)]
        public static void TestByte(byte value)
        {
            TestInitialization(
                value,
                v => new JsonNumber(v),
                (number, v) => number.SetByte(v),
                number => number.GetByte(),
                (JsonNumber number, out byte v) => number.TryGetByte(out v),
                v => v);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-17)]
        [InlineData(17)]
        [InlineData(short.MinValue)]
        [InlineData(short.MaxValue)]
        public static void TestShort(short value)
        {
            TestInitialization(
                value,
                v => new JsonNumber(v),
                (number, v) => number.SetInt16(v),
                number => number.GetInt16(),
                (JsonNumber number, out short v) => number.TryGetInt16(out v),
                v => v);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-17)]
        [InlineData(17)]
        [InlineData(0x2A)]
        [InlineData(0b_0110_1010)]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        public static void TestInt(int value)
        {
            TestInitialization(
                value,
                v => new JsonNumber(v),
                (number, v) => number.SetInt32(v),
                number => number.GetInt32(),
                (JsonNumber number, out int v) => number.TryGetInt32(out v),
                v => v);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-17)]
        [InlineData(17)]
        [InlineData(long.MinValue)]
        [InlineData(long.MaxValue)]
        public static void TestLong(long value)
        {
            TestInitialization(
                value,
                v => new JsonNumber(v),
                (number, v) => number.SetInt64(v),
                number => number.GetInt64(),
                (JsonNumber number, out long v) => number.TryGetInt64(out v),
                v => v);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(17)]
        [InlineData(-17)]
        [InlineData(3.14)]
        [InlineData(-15.5)]
        [InlineData(float.MinValue)]
        [InlineData(float.MaxValue)]
        public static void TestFloat(float value)
        {
            TestInitialization(
                value,
                v => new JsonNumber(v),
                (number, v) => number.SetSingle(v),
                number => number.GetSingle(),
                (JsonNumber number, out float v) => number.TryGetSingle(out v),
                v => v);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(17)]
        [InlineData(-17)]
        [InlineData(3.14)]
        [InlineData(-15.5)]
        [InlineData(double.MinValue)]
        [InlineData(double.MaxValue)]
        public static void TestDouble(double value)
        {
            TestInitialization(
                value,
                v => new JsonNumber(v),
                (number, v) => number.SetDouble(v),
                number => number.GetDouble(),
                (JsonNumber number, out double v) => number.TryGetDouble(out v),
                v => v);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(17)]
        [InlineData(-17)]
        [InlineData(sbyte.MinValue)]
        [InlineData(sbyte.MaxValue)]
        public static void TestSByte(sbyte value)
        {
            TestInitialization(
                value,
                v => new JsonNumber(v),
                (number, v) => number.SetSByte(v),
                number => number.GetSByte(),
                (JsonNumber number, out sbyte v) => number.TryGetSByte(out v),
                v => v);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(17)]
        [InlineData(ushort.MaxValue)]
        public static void TestUInt16(ushort value)
        {
            TestInitialization(
                value,
                v => new JsonNumber(v),
                (number, v) => number.SetUInt16(v),
                number => number.GetUInt16(),
                (JsonNumber number, out ushort v) => number.TryGetUInt16(out v),
                v => v);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(17)]
        [InlineData(uint.MaxValue)]
        public static void TestUInt32(uint value)
        {
            TestInitialization(
                value,
                v => new JsonNumber(v),
                (number, v) => number.SetUInt32(v),
                number => number.GetUInt32(),
                (JsonNumber number, out uint v) => number.TryGetUInt32(out v),
                v => v);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(17)]
        [InlineData(ulong.MaxValue)]
        public static void TestUInt64(ulong value)
        {
            TestInitialization(
                value,
                v => new JsonNumber(v),
                (number, v) => number.SetUInt64(v),
                number => number.GetUInt64(),
                (JsonNumber number, out ulong v) => number.TryGetUInt64(out v),
                v => v);
        }

        [Fact]
        public static void TestDecimal()
        {
            decimal value = decimal.MaxValue;
            TestInitialization(
                value,
                v => new JsonNumber(v),
                (number, v) => number.SetDecimal(v),
                number => number.GetDecimal(),
                (JsonNumber number, out decimal v) => number.TryGetDecimal(out v),
                v => v);
        }

        [Theory]
        [InlineData("0")]
        [InlineData("17")]
        [InlineData("-456")]
        [InlineData("2.3")]
        [InlineData("-17.009")]
        [InlineData("1e400")]
        [InlineData("1e+100000002")]
        [InlineData("184467440737095516150.184467440737095516150")]
        [InlineData("184467440737095516150184467440737095516150")]
        public static void TestString(string value)
        {
            var jsonNumber = new JsonNumber(value);
            Assert.Equal(value, jsonNumber.ToString());

            jsonNumber = new JsonNumber();
            jsonNumber.SetFormattedValue(value);
            Assert.Equal(value, jsonNumber.ToString());
        }

        [Theory]
        [InlineData("")]
        [InlineData("3,14")]
        [InlineData("this is not a number")]
        [InlineData("NAN")]
        [InlineData("0.")]
        [InlineData("008")]
        [InlineData("0e")]
        [InlineData("5e")]
        [InlineData("5a")]
        [InlineData("0.1e")]
        [InlineData("-01")]
        [InlineData("10.5e")]
        [InlineData("10.5e-")]
        [InlineData("10.5e+")]
        [InlineData("10.5e-0.2")]
        [InlineData(" 6")]
        [InlineData("6 ")]
        [InlineData(" 6 ")]
        [InlineData("+0")]
        [InlineData("+1")]
        [InlineData("long.MaxValue")]
        [InlineData("3.14f")]
        [InlineData("0x2A")]
        [InlineData("0b_0110_1010")]
        public static void TestInvalidString(string value)
        {
            Assert.Throws<ArgumentException>(() => new JsonNumber(value));
        }

        [Fact]
        public static void TestNullString()
        {
            Assert.Throws<ArgumentNullException>(() => new JsonNumber(null));
        }

        [Theory]
        [InlineData("0")]
        [InlineData("0.0")]
        [InlineData("-17")]
        [InlineData("17")]
        [InlineData("3.14")]
        [InlineData("1.1e1")]
        [InlineData("-3.1415")]
        [InlineData("1234567890")]
        [InlineData("1e400")]
        [InlineData("1e+100000002")]
        [InlineData("184467440737095516150.184467440737095516150")]
        public static void TestToString(string value)
        {
            var jsonNumber = new JsonNumber();
            jsonNumber.SetFormattedValue(value);
            Assert.Equal(value, jsonNumber.ToString());
        }

        [Fact]
        public static void TestUpcasts()
        {
            byte value = 17;
            var jsonNumber = new JsonNumber(value);

            // Getting other types should also succeed:
            Assert.Equal(value, jsonNumber.GetInt16());
            Assert.True(jsonNumber.TryGetInt16(out short shortResult));
            Assert.Equal(value, shortResult);

            Assert.Equal(value, jsonNumber.GetInt32());
            Assert.True(jsonNumber.TryGetInt32(out int intResult));
            Assert.Equal(value, intResult);

            Assert.Equal(value, jsonNumber.GetInt64());
            Assert.True(jsonNumber.TryGetInt64(out long longResult));
            Assert.Equal(value, longResult);

            Assert.Equal(value, jsonNumber.GetSingle());
            Assert.True(jsonNumber.TryGetSingle(out float floatResult));
            Assert.Equal(value, floatResult);

            Assert.Equal(value, jsonNumber.GetDouble());
            Assert.True(jsonNumber.TryGetDouble(out double doubleResult));
            Assert.Equal(value, doubleResult);

            Assert.Equal(value, (byte)jsonNumber.GetSByte());
            Assert.True(jsonNumber.TryGetSByte(out sbyte sbyteResult));
            Assert.Equal(value, (byte)sbyteResult);

            Assert.Equal(value, jsonNumber.GetUInt16());
            Assert.True(jsonNumber.TryGetUInt16(out ushort ushortResult));
            Assert.Equal(value, ushortResult);

            Assert.Equal(value, jsonNumber.GetUInt32());
            Assert.True(jsonNumber.TryGetUInt32(out uint uintResult));
            Assert.Equal(value, uintResult);

            Assert.Equal(value, jsonNumber.GetUInt64());
            Assert.True(jsonNumber.TryGetUInt64(out ulong ulongResult));
            Assert.Equal(value, ulongResult);

            Assert.Equal(value, jsonNumber.GetDecimal());
            Assert.True(jsonNumber.TryGetDecimal(out decimal decimalResult));
            Assert.Equal(value, decimalResult);
        }

        [Fact]
        public static void TestIntegerGetMismatches()
        {
            var jsonNumber = new JsonNumber(long.MaxValue);

            // Getting smaller types should fail:
            Assert.False(jsonNumber.TryGetByte(out byte byteResult));
            Assert.Throws<OverflowException>(() => jsonNumber.GetByte());

            Assert.False(jsonNumber.TryGetInt16(out short shortResult));
            Assert.Throws<OverflowException>(() => jsonNumber.GetInt16());

            Assert.False(jsonNumber.TryGetInt32(out int intResult));
            Assert.Throws<OverflowException>(() => jsonNumber.GetInt32());

            Assert.False(jsonNumber.TryGetSByte(out sbyte sbyteResult));
            Assert.Throws<OverflowException>(() => jsonNumber.GetSByte());

            Assert.False(jsonNumber.TryGetUInt16(out ushort ushortResult));
            Assert.Throws<OverflowException>(() => jsonNumber.GetUInt16());

            Assert.False(jsonNumber.TryGetUInt32(out uint uintResult));
            Assert.Throws<OverflowException>(() => jsonNumber.GetUInt32());
        }

        [Fact]
        public static void TestUnsignedGetMismatches()
        {
            var jsonNumber = new JsonNumber("-1");

            // Getting unsigned types should fail:
            Assert.False(jsonNumber.TryGetByte(out byte byteResult));
            Assert.Throws<OverflowException>(() => jsonNumber.GetByte());

            Assert.False(jsonNumber.TryGetUInt16(out ushort ushortResult));
            Assert.Throws<OverflowException>(() => jsonNumber.GetUInt16());

            Assert.False(jsonNumber.TryGetUInt32(out uint uintResult));
            Assert.Throws<OverflowException>(() => jsonNumber.GetUInt32());

            Assert.False(jsonNumber.TryGetUInt64(out ulong ulongResult));
            Assert.Throws<OverflowException>(() => jsonNumber.GetUInt64());
        }

        [Fact]
        public static void TestRationalGetMismatches()
        {
            var jsonNumber = new JsonNumber("3.14");

            // Getting integer types should fail:
            Assert.False(jsonNumber.TryGetByte(out byte byteResult));
            Assert.Throws<FormatException>(() => jsonNumber.GetByte());

            Assert.False(jsonNumber.TryGetInt16(out short shortResult));
            Assert.Throws<FormatException>(() => jsonNumber.GetInt16());

            Assert.False(jsonNumber.TryGetInt32(out int intResult));
            Assert.Throws<FormatException>(() => jsonNumber.GetInt32());

            Assert.False(jsonNumber.TryGetInt64(out long longResult));
            Assert.Throws<FormatException>(() => jsonNumber.GetInt64());

            Assert.False(jsonNumber.TryGetSByte(out sbyte sbyteResult));
            Assert.Throws<FormatException>(() => jsonNumber.GetSByte());

            Assert.False(jsonNumber.TryGetUInt16(out ushort ushortResult));
            Assert.Throws<FormatException>(() => jsonNumber.GetUInt16());

            Assert.False(jsonNumber.TryGetUInt32(out uint uintResult));
            Assert.Throws<FormatException>(() => jsonNumber.GetUInt32());

            Assert.False(jsonNumber.TryGetUInt64(out ulong ulongResult));
            Assert.Throws<FormatException>(() => jsonNumber.GetUInt64());
        }

        [Fact]
        public static void TestChangingTypes()
        {
            var jsonNumber = new JsonNumber(5);
            Assert.Equal(5, jsonNumber.GetInt32());

            jsonNumber.SetDouble(3.14);
            Assert.Equal(3.14, jsonNumber.GetDouble());

            jsonNumber.SetByte(17);
            Assert.Equal(17, jsonNumber.GetByte());

            jsonNumber.SetInt64(long.MaxValue);
            Assert.Equal(long.MaxValue, jsonNumber.GetInt64());

            jsonNumber.SetUInt16(ushort.MaxValue);
            Assert.Equal(ushort.MaxValue, jsonNumber.GetUInt16());

            jsonNumber.SetSingle(-1.1f);
            Assert.Equal(-1.1f, jsonNumber.GetSingle());

            jsonNumber.SetSByte(4);
            Assert.Equal(4, jsonNumber.GetSByte());

            jsonNumber.SetUInt32(127);
            Assert.Equal((uint)127, jsonNumber.GetUInt32());

            jsonNumber.SetFormattedValue("1e400");
            Assert.Equal("1e400", jsonNumber.ToString());

            jsonNumber.SetUInt64(ulong.MaxValue);
            Assert.Equal(ulong.MaxValue, jsonNumber.GetUInt64());

            jsonNumber.SetDecimal(decimal.MaxValue);
            Assert.Equal(decimal.MaxValue, jsonNumber.GetDecimal());
        }
    }
}

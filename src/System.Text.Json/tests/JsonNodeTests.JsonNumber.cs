// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Json.Tests
{
    public static partial class JsonNodeTests
    {
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
            // Default constructor:
            var jsonNumber = new JsonNumber();
            jsonNumber.SetByte(value);
            Assert.Equal(value, jsonNumber.GetByte());
            Assert.True(jsonNumber.TryGetByte(out byte result));
            Assert.Equal(value, result);

            // Numeric type constructor:
            jsonNumber = new JsonNumber(value);
            Assert.Equal(value, jsonNumber.GetByte());
            Assert.True(jsonNumber.TryGetByte(out result));
            Assert.Equal(value, result);

            // Implicit cast:
            jsonNumber = value;
            Assert.Equal(value, jsonNumber.GetByte());
            Assert.True(jsonNumber.TryGetByte(out result));
            Assert.Equal(value, result);

            // String constructor:
            jsonNumber = new JsonNumber(value.ToString());
            Assert.Equal(value, jsonNumber.GetByte());
            Assert.True(jsonNumber.TryGetByte(out result));
            Assert.Equal(value, result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-17)]
        [InlineData(17)]
        [InlineData(short.MinValue)]
        [InlineData(short.MaxValue)]
        public static void TestShort(short value)
        {
            // Default constructor:
            var jsonNumber = new JsonNumber();
            jsonNumber.SetInt16(value);
            Assert.Equal(value, jsonNumber.GetInt16());
            Assert.True(jsonNumber.TryGetInt16(out short result));
            Assert.Equal(value, result);

            // Numeric type constructor:
            jsonNumber = new JsonNumber(value);
            Assert.Equal(value, jsonNumber.GetInt16());
            Assert.True(jsonNumber.TryGetInt16(out result));
            Assert.Equal(value, result);

            // Implicit cast:
            jsonNumber = value;
            Assert.Equal(value, jsonNumber.GetInt16());
            Assert.True(jsonNumber.TryGetInt16(out result));
            Assert.Equal(value, result);

            // String constructor:
            jsonNumber = new JsonNumber(value.ToString());
            Assert.Equal(value, jsonNumber.GetInt16());
            Assert.True(jsonNumber.TryGetInt16(out result));
            Assert.Equal(value, result);
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
            // Default constructor:
            var jsonNumber = new JsonNumber();
            jsonNumber.SetInt32(value);
            Assert.Equal(value, jsonNumber.GetInt32());
            Assert.True(jsonNumber.TryGetInt32(out int result));
            Assert.Equal(value, result);

            // Numeric type constructor:
            jsonNumber = new JsonNumber(value);
            Assert.Equal(value, jsonNumber.GetInt32());
            Assert.True(jsonNumber.TryGetInt32(out result));
            Assert.Equal(value, result);

            // Implicit cast:
            jsonNumber = value;
            Assert.Equal(value, jsonNumber.GetInt32());
            Assert.True(jsonNumber.TryGetInt32(out result));
            Assert.Equal(value, result);

            // String constructor:
            jsonNumber = new JsonNumber(value.ToString());
            Assert.Equal(value, jsonNumber.GetInt32());
            Assert.True(jsonNumber.TryGetInt32(out result));
            Assert.Equal(value, result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-17)]
        [InlineData(17)]
        [InlineData(long.MinValue)]
        [InlineData(long.MaxValue)]
        public static void TestLong(long value)
        {
            // Default constructor:
            var jsonNumber = new JsonNumber();
            jsonNumber.SetInt64(value);
            Assert.Equal(value, jsonNumber.GetInt64());
            Assert.True(jsonNumber.TryGetInt64(out long result));
            Assert.Equal(value, result);

            // Numeric type constructor:
            jsonNumber = new JsonNumber(value);
            Assert.Equal(value, jsonNumber.GetInt64());
            Assert.True(jsonNumber.TryGetInt64(out result));
            Assert.Equal(value, result);

            // Implicit cast:
            jsonNumber = value;
            Assert.Equal(value, jsonNumber.GetInt64());
            Assert.True(jsonNumber.TryGetInt64(out result));
            Assert.Equal(value, result);

            // String constructor:
            jsonNumber = new JsonNumber(value.ToString());
            Assert.Equal(value, jsonNumber.GetInt64());
            Assert.True(jsonNumber.TryGetInt64(out result));
            Assert.Equal(value, result);
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
            // Default constructor:
            var jsonNumber = new JsonNumber();
            jsonNumber.SetSingle(value);
            Assert.Equal(value, jsonNumber.GetSingle());
            Assert.True(jsonNumber.TryGetSingle(out float result));
            Assert.Equal(value, result);

            // Numeric type constructor:
            jsonNumber = new JsonNumber(value);
            Assert.Equal(value, jsonNumber.GetSingle());
            Assert.True(jsonNumber.TryGetSingle(out result));
            Assert.Equal(value, result);

            // Implicit cast:
            jsonNumber = value;
            Assert.Equal(value, jsonNumber.GetSingle());
            Assert.True(jsonNumber.TryGetSingle(out result));
            Assert.Equal(value, result);

            // String constructor:
            jsonNumber = new JsonNumber(value.ToString());
            Assert.Equal(value, jsonNumber.GetSingle());
            Assert.True(jsonNumber.TryGetSingle(out result));
            Assert.Equal(value, result);
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
            // Default constructor:
            var jsonNumber = new JsonNumber();
            jsonNumber.SetDouble(value);
            Assert.Equal(value, jsonNumber.GetDouble());
            Assert.True(jsonNumber.TryGetDouble(out double result));
            Assert.Equal(value, result);

            // Numeric type constructor:
            jsonNumber = new JsonNumber(value);
            Assert.Equal(value, jsonNumber.GetDouble());
            Assert.True(jsonNumber.TryGetDouble(out result));
            Assert.Equal(value, result);

            // Implicit cast:
            jsonNumber = value;
            Assert.Equal(value, jsonNumber.GetDouble());
            Assert.True(jsonNumber.TryGetDouble(out result));
            Assert.Equal(value, result);

            // String constructor:
            jsonNumber = new JsonNumber(value.ToString());
            Assert.Equal(value, jsonNumber.GetDouble());
            Assert.True(jsonNumber.TryGetDouble(out result));
            Assert.Equal(value, result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(17)]
        [InlineData(-17)]
        [InlineData(sbyte.MinValue)]
        [InlineData(sbyte.MaxValue)]
        public static void TestSByte(sbyte value)
        {
            // Default constructor:
            var jsonNumber = new JsonNumber();
            jsonNumber.SetSByte(value);
            Assert.Equal(value, jsonNumber.GetSByte());
            Assert.True(jsonNumber.TryGetSByte(out sbyte result));
            Assert.Equal(value, result);

            // Numeric type constructor:
            jsonNumber = new JsonNumber(value);
            Assert.Equal(value, jsonNumber.GetSByte());
            Assert.True(jsonNumber.TryGetSByte(out result));
            Assert.Equal(value, result);

            // Implicit cast:
            jsonNumber = value;
            Assert.Equal(value, jsonNumber.GetSByte());
            Assert.True(jsonNumber.TryGetSByte(out result));
            Assert.Equal(value, result);

            // String constructor:
            jsonNumber = new JsonNumber(value.ToString());
            Assert.Equal(value, jsonNumber.GetSByte());
            Assert.True(jsonNumber.TryGetSByte(out result));
            Assert.Equal(value, result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(17)]
        [InlineData(ushort.MaxValue)]
        public static void TestUInt16(ushort value)
        {
            // Default constructor:
            var jsonNumber = new JsonNumber();
            jsonNumber.SetUInt16(value);
            Assert.Equal(value, jsonNumber.GetUInt16());
            Assert.True(jsonNumber.TryGetUInt16(out ushort result));
            Assert.Equal(value, result);

            // Numeric type constructor:
            jsonNumber = new JsonNumber(value);
            Assert.Equal(value, jsonNumber.GetUInt16());
            Assert.True(jsonNumber.TryGetUInt16(out result));
            Assert.Equal(value, result);

            // Implicit cast:
            jsonNumber = value;
            Assert.Equal(value, jsonNumber.GetUInt16());
            Assert.True(jsonNumber.TryGetUInt16(out result));
            Assert.Equal(value, result);

            // String constructor:
            jsonNumber = new JsonNumber(value.ToString());
            Assert.Equal(value, jsonNumber.GetUInt16());
            Assert.True(jsonNumber.TryGetUInt16(out result));
            Assert.Equal(value, result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(17)]
        [InlineData(uint.MaxValue)]
        public static void TestUInt32(uint value)
        {
            // Default constructor:
            var jsonNumber = new JsonNumber();
            jsonNumber.SetUInt32(value);
            Assert.Equal(value, jsonNumber.GetUInt32());
            Assert.True(jsonNumber.TryGetUInt32(out uint result));
            Assert.Equal(value, result);

            // Numeric type constructor:
            jsonNumber = new JsonNumber(value);
            Assert.Equal(value, jsonNumber.GetUInt32());
            Assert.True(jsonNumber.TryGetUInt32(out result));
            Assert.Equal(value, result);

            // Implicit cast:
            jsonNumber = value;
            Assert.Equal(value, jsonNumber.GetUInt32());
            Assert.True(jsonNumber.TryGetUInt32(out result));
            Assert.Equal(value, result);

            // String constructor:
            jsonNumber = new JsonNumber(value.ToString());
            Assert.Equal(value, jsonNumber.GetUInt32());
            Assert.True(jsonNumber.TryGetUInt32(out result));
            Assert.Equal(value, result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(17)]
        [InlineData(ulong.MaxValue)]
        public static void TestUInt64(ulong value)
        {
            // Default constructor:
            var jsonNumber = new JsonNumber();
            jsonNumber.SetUInt64(value);
            Assert.Equal(value, jsonNumber.GetUInt64());
            Assert.True(jsonNumber.TryGetUInt64(out ulong result));
            Assert.Equal(value, result);

            // Numeric type constructor:
            jsonNumber = new JsonNumber(value);
            Assert.Equal(value, jsonNumber.GetUInt64());
            Assert.True(jsonNumber.TryGetUInt64(out result));
            Assert.Equal(value, result);

            // Implicit cast:
            jsonNumber = value;
            Assert.Equal(value, jsonNumber.GetUInt64());
            Assert.True(jsonNumber.TryGetUInt64(out result));
            Assert.Equal(value, result);

            // String constructor:
            jsonNumber = new JsonNumber(value.ToString());
            Assert.Equal(value, jsonNumber.GetUInt64());
            Assert.True(jsonNumber.TryGetUInt64(out result));
            Assert.Equal(value, result);
        }

        [Theory]
        [InlineData(0)]
        public static void TestDecimal(decimal value)
        {
            //var value = decimal.MaxValue;

            // Default constructor:
            var jsonNumber = new JsonNumber();
            jsonNumber.SetDecimal(value);
            Assert.Equal(value, jsonNumber.GetDecimal());
            Assert.True(jsonNumber.TryGetDecimal(out decimal result));
            Assert.Equal(value, result);

            // Numeric type constructor:
            jsonNumber = new JsonNumber(value);
            Assert.Equal(value, jsonNumber.GetDecimal());
            Assert.True(jsonNumber.TryGetDecimal(out result));
            Assert.Equal(value, result);

            // Implicit cast:
            jsonNumber = value;
            Assert.Equal(value, jsonNumber.GetDecimal());
            Assert.True(jsonNumber.TryGetDecimal(out result));
            Assert.Equal(value, result);

            // String constructor:
            jsonNumber = new JsonNumber(value.ToString());
            Assert.Equal(value, jsonNumber.GetDecimal());
            Assert.True(jsonNumber.TryGetDecimal(out result));
            Assert.Equal(value, result);
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

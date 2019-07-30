// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Json
{
    public static partial class WritableJsonApiTests
    {
        [Theory]
        [InlineData(-456)]
        [InlineData(0)]
        [InlineData(17)]
        [InlineData(long.MaxValue)]
        [InlineData(2.3)]
        [InlineData(-17.009)]
        [InlineData(3.14f)]
        [InlineData(0x2A)]
        [InlineData(0b_0110_1010)]
        [InlineData("1e400")]
        [InlineData("1e+100000002")]
        [InlineData("184467440737095516150.184467440737095516150")]
        public static void TestJsonNumberConstructor(object value)
        {
            // should not throw any exceptions:
            _ = new JsonNumber(value as dynamic);
        }

        [Fact]
        public static void TestDefaultCtor()
        {
            var jsonNumber = new JsonNumber();
            Assert.Equal(0, jsonNumber.GetInt32());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(17)]
        [InlineData(255)]
        public static void TestByte(byte value)
        {
            var jsonNumber = new JsonNumber();
            jsonNumber.SetByte(value);
            Assert.Equal(value, jsonNumber.GetByte());
            Assert.True(jsonNumber.TryGetByte(out byte result));
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
            var jsonNumber = new JsonNumber();
            jsonNumber.SetInt16(value);
            Assert.Equal(value, jsonNumber.GetInt16());
            Assert.True(jsonNumber.TryGetInt16(out short result));
            Assert.Equal(value, result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-17)]
        [InlineData(17)]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        public static void TestInt(int value)
        {
            var jsonNumber = new JsonNumber();
            jsonNumber.SetInt32(value);
            Assert.Equal(value, jsonNumber.GetInt32());
            Assert.True(jsonNumber.TryGetInt32(out int result));
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
            var jsonNumber = new JsonNumber();
            jsonNumber.SetInt64(value);
            Assert.Equal(value, jsonNumber.GetInt64());
            Assert.True(jsonNumber.TryGetInt64(out long result));
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
            var jsonNumber = new JsonNumber();
            jsonNumber.SetSingle(value);
            Assert.Equal(value, jsonNumber.GetSingle());
            Assert.True(jsonNumber.TryGetSingle(out float result));
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
            var jsonNumber = new JsonNumber();
            jsonNumber.SetDouble(value);
            Assert.Equal(value, jsonNumber.GetDouble());
            Assert.True(jsonNumber.TryGetDouble(out double result));
            Assert.Equal(value, result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(17)]
        [InlineData(-17)]
        [InlineData(sbyte.MinValue)]
        [InlineData(sbyte.MaxValue)]
        public static void TestSbyte(sbyte value)
        {
            var jsonNumber = new JsonNumber();
            jsonNumber.SetSByte(value);
            Assert.Equal(value, jsonNumber.GetSByte());
            Assert.True(jsonNumber.TryGetSByte(out sbyte result));
            Assert.Equal(value, result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(17)]
        [InlineData(ushort.MaxValue)]
        public static void TestUInt16(ushort value)
        {
            var jsonNumber = new JsonNumber();
            jsonNumber.SetUInt16(value);
            Assert.Equal(value, jsonNumber.GetUInt16());
            Assert.True(jsonNumber.TryGetUInt16(out ushort result));
            Assert.Equal(value, result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(17)]
        [InlineData(uint.MaxValue)]
        public static void TestUInt32(uint value)
        {
            var jsonNumber = new JsonNumber();
            jsonNumber.SetUInt32(value);
            Assert.Equal(value, jsonNumber.GetUInt32());
            Assert.True(jsonNumber.TryGetUInt32(out uint result));
            Assert.Equal(value, result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(17)]
        [InlineData(ulong.MaxValue)]
        public static void TestUInt64(ulong value)
        {
            var jsonNumber = new JsonNumber();
            jsonNumber.SetUInt64(value);
            Assert.Equal(value, jsonNumber.GetUInt64());
            Assert.True(jsonNumber.TryGetUInt64(out ulong result));
            Assert.Equal(value, result);
        }

        [Fact]
        public static void TestDecimal()
        {
            var decimalValue = decimal.MaxValue;
            var jsonNumber = new JsonNumber();
            jsonNumber.SetDecimal(decimalValue);
            Assert.Equal(decimalValue, jsonNumber.GetDecimal());
            Assert.True(jsonNumber.TryGetDecimal(out decimal result));
            Assert.Equal(decimalValue, result);
        }

        [Theory]
        [InlineData("3.14")]
        [InlineData("-17")]
        [InlineData("0")]
        [InlineData("189")]
        [InlineData("1e400")]
        [InlineData("1e+100000002")]
        [InlineData("184467440737095516150.184467440737095516150")]
        public static void TestString(string value)
        {
            var jsonNumber = new JsonNumber();
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
        public static void TestInvalidString(string value)
        {
            Assert.Throws<ArgumentException>(() => new JsonNumber(value));
        }

        [Fact]
        public static void TestNullString()
        {
            Assert.Throws<ArgumentNullException>(() => new JsonNumber(null));
        }

        [Fact]
        public static void TestIntFromString()
        {
            var jsonNumber = new JsonNumber("145");
            Assert.Equal(145, jsonNumber.GetInt32());
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

        [Theory]
        [InlineData("0")]
        [InlineData("-17")]
        [InlineData("17")]
        [InlineData("3.14")]
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

        [Fact]
        public static void TestImplicitOperators()
        {
            byte byteValue = byte.MaxValue;
            JsonNumber jsonNumber = byteValue;
            Assert.Equal(byteValue, jsonNumber.GetByte());

            short shortValue = short.MaxValue;
            jsonNumber = shortValue;
            Assert.Equal(shortValue, jsonNumber.GetInt16());

            int intValue = int.MaxValue;
            jsonNumber = intValue;
            Assert.Equal(intValue, jsonNumber.GetInt32());

            long longValue = long.MaxValue;
            jsonNumber = longValue;
            Assert.Equal(longValue, jsonNumber.GetInt64());

            float floatValue = float.MaxValue;
            jsonNumber = floatValue;
            Assert.Equal(floatValue, jsonNumber.GetSingle());

            double doubleValue = double.MaxValue;
            jsonNumber = doubleValue;
            Assert.Equal(doubleValue, jsonNumber.GetDouble());

            sbyte sbyteValue = sbyte.MaxValue;
            jsonNumber = sbyteValue;
            Assert.Equal(sbyteValue, jsonNumber.GetSByte());

            ushort ushortValue = ushort.MaxValue;
            jsonNumber = ushortValue;
            Assert.Equal(ushortValue, jsonNumber.GetUInt16());

            uint uintValue = uint.MaxValue;
            jsonNumber = uintValue;
            Assert.Equal(uintValue, jsonNumber.GetUInt32());

            ulong ulongValue = ulong.MaxValue;
            jsonNumber = ulongValue;
            Assert.Equal(ulongValue, jsonNumber.GetUInt64());

            decimal decimalValue = decimal.MaxValue;
            jsonNumber = decimalValue;
            Assert.Equal(decimalValue, jsonNumber.GetDecimal());
        }
    }
}

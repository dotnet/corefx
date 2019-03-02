﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static partial class ValueTests
    {
        [Fact]
        public static void ReadPrimitives()
        {
            int i = JsonSerializer.Parse<int>(Encoding.UTF8.GetBytes(@"1"));
            Assert.Equal(1, i);

            int i2 = JsonSerializer.Parse<int>("2");
            Assert.Equal(2, i2);

            long l = JsonSerializer.Parse<long>(Encoding.UTF8.GetBytes(long.MaxValue.ToString()));
            Assert.Equal(long.MaxValue, l);

            long l2 = JsonSerializer.Parse<long>(long.MaxValue.ToString());
            Assert.Equal(long.MaxValue, l2);

            string s = JsonSerializer.Parse<string>(Encoding.UTF8.GetBytes(@"""Hello"""));
            Assert.Equal("Hello", s);

            string s2 = JsonSerializer.Parse<string>(@"""Hello""");
            Assert.Equal("Hello", s2);
        }

        [Fact]
        public static void ReadPrimitivesFail()
        {
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<int>(Encoding.UTF8.GetBytes(@"a")));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<int[]>(Encoding.UTF8.GetBytes(@"[1,a]")));
        }

        [Fact]
        public static void ReadPrimitiveArray()
        {
            int[] i = JsonSerializer.Parse<int[]>(Encoding.UTF8.GetBytes(@"[1,2]"));
            Assert.Equal(1, i[0]);
            Assert.Equal(2, i[1]);
        }

        [Fact]
        public static void ReadArrayWithEnums()
        {
            SampleEnum[] i = JsonSerializer.Parse<SampleEnum[]>(Encoding.UTF8.GetBytes(@"[1,2]"));
            Assert.Equal(SampleEnum.One, i[0]);
            Assert.Equal(SampleEnum.Two, i[1]);
        }

        [Fact]
        public static void ReadPrimitiveArrayFail()
        {
            // Invalid data
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<int[]>(Encoding.UTF8.GetBytes(@"[1,""a""]")));

            // Multidimensional arrays currently not supported
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<int[,]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]")));
        }

        [Fact]
        public static void ReadPrimitiveExtraBytesFail()
        {
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<int[]>("[2] {3}"));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<int[]>(Encoding.UTF8.GetBytes(@"[2] {3}")));
        }

        [Fact]
        public static void RangeFail()
        {
            // These have custom code because the reader doesn't natively support:
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<byte>((byte.MinValue - 1).ToString()));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<byte>((byte.MaxValue + 1).ToString()));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<byte?>((byte.MinValue - 1).ToString()));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<byte?>((byte.MaxValue + 1).ToString()));

            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<sbyte>((sbyte.MinValue - 1).ToString()));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<sbyte>((sbyte.MaxValue + 1).ToString()));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<sbyte?>((sbyte.MinValue - 1).ToString()));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<sbyte?>((sbyte.MaxValue + 1).ToString()));

            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<short>((short.MinValue - 1).ToString()));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<short>((short.MaxValue + 1).ToString()));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<short?>((short.MinValue - 1).ToString()));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<short?>((short.MaxValue + 1).ToString()));

            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<ushort>((ushort.MinValue - 1).ToString()));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<ushort>((ushort.MaxValue + 1).ToString()));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<ushort?>((ushort.MinValue - 1).ToString()));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<ushort?>((ushort.MaxValue + 1).ToString()));

            // To ensure range failure, just use double's MinValue and MaxValue (instead of float.MinValue\MaxValue +-1)
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<float>(double.MinValue.ToString()));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<float>(double.MaxValue.ToString()));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<float?>(double.MinValue.ToString()));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<float?>(double.MaxValue.ToString()));

            // These are natively supported by the reader:
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<int>(((long)int.MinValue - 1).ToString()));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<int>(((long)int.MaxValue + 1).ToString()));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<int?>(((long)int.MinValue - 1).ToString()));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<int?>(((long)int.MaxValue + 1).ToString()));

            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<uint>(((long)uint.MinValue - 1).ToString()));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<uint>(((long)uint.MaxValue + 1).ToString()));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<uint?>(((long)uint.MinValue - 1).ToString()));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<uint?>(((long)uint.MaxValue + 1).ToString()));

            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<long>(long.MinValue.ToString() + "0"));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<long>(long.MaxValue.ToString() + "0"));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<long?>(long.MinValue.ToString() + "0"));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<long?>(long.MaxValue.ToString() + "0"));

            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<ulong>(ulong.MinValue.ToString() + "0"));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<ulong>(ulong.MaxValue.ToString() + "0"));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<ulong?>(ulong.MinValue.ToString() + "0"));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<ulong?>(ulong.MaxValue.ToString() + "0"));

            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<decimal>(decimal.MinValue.ToString() + "0"));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<decimal>(decimal.MaxValue.ToString() + "0"));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<decimal?>(decimal.MinValue.ToString() + "0"));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<decimal?>(decimal.MaxValue.ToString() + "0"));

            // todo: determine why these don't throw (issue with reader?)
            //Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<double>(double.MinValue.ToString() + "0"));
            //Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<double>(double.MaxValue.ToString() + "0"));
            //Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<double?>(double.MinValue.ToString() + "0"));
            //Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<double?>(double.MaxValue.ToString() + "0"));
        }

        [Fact]
        public static void RangePass()
        {
            Assert.Equal(byte.MinValue, JsonSerializer.Parse<byte>(byte.MinValue.ToString()));
            Assert.Equal(byte.MaxValue, JsonSerializer.Parse<byte>(byte.MaxValue.ToString()));
            Assert.Equal(byte.MinValue, JsonSerializer.Parse<byte?>(byte.MinValue.ToString()));
            Assert.Equal(byte.MaxValue, JsonSerializer.Parse<byte?>(byte.MaxValue.ToString()));

            Assert.Equal(sbyte.MinValue, JsonSerializer.Parse<sbyte>(sbyte.MinValue.ToString()));
            Assert.Equal(sbyte.MaxValue, JsonSerializer.Parse<sbyte>(sbyte.MaxValue.ToString()));
            Assert.Equal(sbyte.MinValue, JsonSerializer.Parse<sbyte?>(sbyte.MinValue.ToString()));
            Assert.Equal(sbyte.MaxValue, JsonSerializer.Parse<sbyte?>(sbyte.MaxValue.ToString()));

            Assert.Equal(short.MinValue, JsonSerializer.Parse<short>(short.MinValue.ToString()));
            Assert.Equal(short.MaxValue, JsonSerializer.Parse<short>(short.MaxValue.ToString()));
            Assert.Equal(short.MinValue, JsonSerializer.Parse<short?>(short.MinValue.ToString()));
            Assert.Equal(short.MaxValue, JsonSerializer.Parse<short?>(short.MaxValue.ToString()));

            Assert.Equal(ushort.MinValue, JsonSerializer.Parse<ushort>(ushort.MinValue.ToString()));
            Assert.Equal(ushort.MaxValue, JsonSerializer.Parse<ushort>(ushort.MaxValue.ToString()));
            Assert.Equal(ushort.MinValue, JsonSerializer.Parse<ushort?>(ushort.MinValue.ToString()));
            Assert.Equal(ushort.MaxValue, JsonSerializer.Parse<ushort?>(ushort.MaxValue.ToString()));

            // todo: these fail due to double->float conversion
            //Assert.Equal(float.MinValue, JsonSerializer.Parse<float>(float.MinValue.ToString()));
            //Assert.Equal(float.MaxValue, JsonSerializer.Parse<float>(float.MaxValue.ToString()));
            //Assert.Equal(float.MinValue, JsonSerializer.Parse<float?>(float.MinValue.ToString()));
            //Assert.Equal(float.MaxValue, JsonSerializer.Parse<float?>(float.MaxValue.ToString()));

            Assert.Equal(int.MinValue, JsonSerializer.Parse<int>(int.MinValue.ToString()));
            Assert.Equal(int.MaxValue, JsonSerializer.Parse<int>(int.MaxValue.ToString()));
            Assert.Equal(int.MinValue, JsonSerializer.Parse<int?>(int.MinValue.ToString()));
            Assert.Equal(int.MaxValue, JsonSerializer.Parse<int?>(int.MaxValue.ToString()));

            Assert.Equal(uint.MinValue, JsonSerializer.Parse<uint>(uint.MinValue.ToString()));
            Assert.Equal(uint.MaxValue, JsonSerializer.Parse<uint>(uint.MaxValue.ToString()));
            Assert.Equal(uint.MinValue, JsonSerializer.Parse<uint?>(uint.MinValue.ToString()));
            Assert.Equal(uint.MaxValue, JsonSerializer.Parse<uint?>(uint.MaxValue.ToString()));

            Assert.Equal(long.MinValue, JsonSerializer.Parse<long>(long.MinValue.ToString()));
            Assert.Equal(long.MaxValue, JsonSerializer.Parse<long>(long.MaxValue.ToString()));
            Assert.Equal(long.MinValue, JsonSerializer.Parse<long?>(long.MinValue.ToString()));
            Assert.Equal(long.MaxValue, JsonSerializer.Parse<long?>(long.MaxValue.ToString()));

            Assert.Equal(ulong.MinValue, JsonSerializer.Parse<ulong>(ulong.MinValue.ToString()));
            Assert.Equal(ulong.MaxValue, JsonSerializer.Parse<ulong>(ulong.MaxValue.ToString()));
            Assert.Equal(ulong.MinValue, JsonSerializer.Parse<ulong?>(ulong.MinValue.ToString()));
            Assert.Equal(ulong.MaxValue, JsonSerializer.Parse<ulong?>(ulong.MaxValue.ToString()));

            Assert.Equal(decimal.MinValue, JsonSerializer.Parse<decimal>(decimal.MinValue.ToString()));
            Assert.Equal(decimal.MaxValue, JsonSerializer.Parse<decimal>(decimal.MaxValue.ToString()));
            Assert.Equal(decimal.MinValue, JsonSerializer.Parse<decimal?>(decimal.MinValue.ToString()));
            Assert.Equal(decimal.MaxValue, JsonSerializer.Parse<decimal?>(decimal.MaxValue.ToString()));

            // todo: these are failing; do we need round-trip format "R"?
            //Assert.Equal(double.MinValue, JsonSerializer.Parse<double>(double.MinValue.ToString()));
            //Assert.Equal(double.MaxValue, JsonSerializer.Parse<double>(double.MaxValue.ToString()));
            //Assert.Equal(double.MinValue, JsonSerializer.Parse<double?>(double.MinValue.ToString()));
            //Assert.Equal(double.MaxValue, JsonSerializer.Parse<double?>(double.MaxValue.ToString()));
        }

        [Fact]
        public static void ValueFail()
        {
            string unexpectedString = @"""unexpected string""";

            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<byte>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<byte>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<byte?>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<byte?>(unexpectedString));

            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<sbyte>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<sbyte>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<sbyte?>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<sbyte?>(unexpectedString));

            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<short>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<short>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<short?>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<short?>(unexpectedString));

            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<ushort>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<ushort>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<ushort?>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<ushort?>(unexpectedString));

            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<float>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<float>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<float?>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<float?>(unexpectedString));

            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<int>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<int>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<int?>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<int?>(unexpectedString));

            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<uint>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<uint>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<uint?>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<uint?>(unexpectedString));

            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<long>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<long>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<long?>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<long?>(unexpectedString));

            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<ulong>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<ulong>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<ulong?>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<ulong?>(unexpectedString));

            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<decimal>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<decimal>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<decimal?>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<decimal?>(unexpectedString));

            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<double>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<double>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<double?>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<double?>(unexpectedString));

            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<DateTime>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<DateTime>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<DateTime?>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<DateTime?>(unexpectedString));

            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<string>("1"));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<string>("1"));

            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<char>("1"));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<char>("1"));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<char?>("1"));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<char?>("1"));

            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<Enum>(unexpectedString));
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<Enum>(unexpectedString));
        }

        [Fact]
        public static void ReadObjectArray()
        {
            string data =
                "[" +
                SimpleTestClass.s_json +
                "," +
                SimpleTestClass.s_json +
                "]";

            SimpleTestClass[] i = JsonSerializer.Parse<SimpleTestClass[]>(Encoding.UTF8.GetBytes(data));

            i[0].Verify();
            i[1].Verify();
        }

        [Fact]
        public static void ReadPrimitiveJaggedArray()
        {
            int[][] i = JsonSerializer.Parse<int[][]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            Assert.Equal(1, i[0][0]);
            Assert.Equal(2, i[0][1]);
            Assert.Equal(3, i[1][0]);
            Assert.Equal(4, i[1][1]);
        }

        [Fact]
        public static void ReadListOfList()
        {
            List<List<int>> result = JsonSerializer.Parse<List<List<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));

            Assert.Equal(1, result[0][0]);
            Assert.Equal(2, result[0][1]);
            Assert.Equal(3, result[1][0]);
            Assert.Equal(4, result[1][1]);
        }

        [Fact]
        public static void ReadListOfArray()
        {
            List<int[]> result = JsonSerializer.Parse<List<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));

            Assert.Equal(1, result[0][0]);
            Assert.Equal(2, result[0][1]);
            Assert.Equal(3, result[1][0]);
            Assert.Equal(4, result[1][1]);
        }

        [Fact]
        public static void ReadArrayOfList()
        {
            List<int>[] result = JsonSerializer.Parse<List<int>[]> (Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));

            Assert.Equal(1, result[0][0]);
            Assert.Equal(2, result[0][1]);
            Assert.Equal(3, result[1][0]);
            Assert.Equal(4, result[1][1]);
        }

        [Fact]
        public static void ReadPrimitiveList()
        {
            List<int> i = JsonSerializer.Parse<List<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            Assert.Equal(1, i[0]);
            Assert.Equal(2, i[1]);
        }

        public class TestClassWithBadData
        {
            public TestChildClassWithBadData[] Children { get; set; }
        }

        public class TestChildClassWithBadData
        {
            public int MyProperty { get; set; }
        }

        [Fact]
        public static void ReadConversionFails()
        {
            byte[] data = Encoding.UTF8.GetBytes(
                @"{" +
                    @"""Children"":[" +
                        @"{""MyProperty"":""StringButShouldBeInt""}" +
                    @"]" +
                @"}");

            bool exceptionThrown = false;

            try
            {
                JsonSerializer.Parse<TestClassWithBadData>(data);
            }
            catch (JsonReaderException exception)
            {
                exceptionThrown = true;

                // Exception should contain property path.
                Assert.True(exception.ToString().Contains("[System.Text.Json.Serialization.Tests.ValueTests+TestClassWithBadData].Children.MyProperty"));
            }

            Assert.True(exceptionThrown);
        }
    }
}

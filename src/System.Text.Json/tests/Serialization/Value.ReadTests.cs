// Licensed to the .NET Foundation under one or more agreements.
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

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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

            int? i3 = JsonSerializer.Parse<int?>("null");
            Assert.Null(i3);

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
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<int>(Encoding.UTF8.GetBytes(@"a")));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<int[]>(Encoding.UTF8.GetBytes(@"[1,a]")));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<int>(@"null"));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<int>(@""""""));
        }

        [Fact]
        public static void ReadPrimitiveArray()
        {
            int[] i = JsonSerializer.Parse<int[]>(Encoding.UTF8.GetBytes(@"[1,2]"));
            Assert.Equal(1, i[0]);
            Assert.Equal(2, i[1]);

            i = JsonSerializer.Parse<int[]>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, i.Length);
        }

        [Fact]
        public static void ReadArrayWithEnums()
        {
            SampleEnum[] i = JsonSerializer.Parse<SampleEnum[]>(Encoding.UTF8.GetBytes(@"[1,2]"));
            Assert.Equal(SampleEnum.One, i[0]);
            Assert.Equal(SampleEnum.Two, i[1]);
        }

        [Fact]
        public static void EmptyStringInput()
        {
            string obj = JsonSerializer.Parse<string>(@"""""");
            Assert.Equal(string.Empty, obj);
        }

        [Fact]
        public static void ReadPrimitiveArrayFail()
        {
            // Invalid data
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<int[]>(Encoding.UTF8.GetBytes(@"[1,""a""]")));

            // Invalid data
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<List<int?>>(Encoding.UTF8.GetBytes(@"[1,""a""]")));

            // Multidimensional arrays currently not supported
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<int[,]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]")));
        }

        [Fact]
        public static void ReadPrimitiveExtraBytesFail()
        {
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<int[]>("[2] {3}"));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<int[]>(Encoding.UTF8.GetBytes(@"[2] {3}")));
        }

        [Fact]
        public static void RangeFail()
        {
            // These have custom code because the reader doesn't natively support:
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<byte>((byte.MinValue - 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<byte>((byte.MaxValue + 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<byte?>((byte.MinValue - 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<byte?>((byte.MaxValue + 1).ToString()));

            Assert.Throws<JsonException>(() => JsonSerializer.Parse<sbyte>((sbyte.MinValue - 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<sbyte>((sbyte.MaxValue + 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<sbyte?>((sbyte.MinValue - 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<sbyte?>((sbyte.MaxValue + 1).ToString()));

            Assert.Throws<JsonException>(() => JsonSerializer.Parse<short>((short.MinValue - 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<short>((short.MaxValue + 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<short?>((short.MinValue - 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<short?>((short.MaxValue + 1).ToString()));

            Assert.Throws<JsonException>(() => JsonSerializer.Parse<ushort>((ushort.MinValue - 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<ushort>((ushort.MaxValue + 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<ushort?>((ushort.MinValue - 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<ushort?>((ushort.MaxValue + 1).ToString()));

            // To ensure range failure, just use double's MinValue and MaxValue (instead of float.MinValue\MaxValue +-1)
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<float>(double.MinValue.ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<float>(double.MaxValue.ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<float?>(double.MinValue.ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<float?>(double.MaxValue.ToString()));

            // These are natively supported by the reader:
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<int>(((long)int.MinValue - 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<int>(((long)int.MaxValue + 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<int?>(((long)int.MinValue - 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<int?>(((long)int.MaxValue + 1).ToString()));

            Assert.Throws<JsonException>(() => JsonSerializer.Parse<uint>(((long)uint.MinValue - 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<uint>(((long)uint.MaxValue + 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<uint?>(((long)uint.MinValue - 1).ToString()));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<uint?>(((long)uint.MaxValue + 1).ToString()));

            Assert.Throws<JsonException>(() => JsonSerializer.Parse<long>(long.MinValue.ToString() + "0"));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<long>(long.MaxValue.ToString() + "0"));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<long?>(long.MinValue.ToString() + "0"));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<long?>(long.MaxValue.ToString() + "0"));

            Assert.Throws<JsonException>(() => JsonSerializer.Parse<ulong>(ulong.MinValue.ToString() + "0"));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<ulong>(ulong.MaxValue.ToString() + "0"));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<ulong?>(ulong.MinValue.ToString() + "0"));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<ulong?>(ulong.MaxValue.ToString() + "0"));

            Assert.Throws<JsonException>(() => JsonSerializer.Parse<decimal>(decimal.MinValue.ToString() + "0"));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<decimal>(decimal.MaxValue.ToString() + "0"));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<decimal?>(decimal.MinValue.ToString() + "0"));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<decimal?>(decimal.MaxValue.ToString() + "0"));

            // todo: determine why these don't throw (issue with reader?)
            //Assert.Throws<JsonException>(() => JsonSerializer.Parse<double>(double.MinValue.ToString() + "0"));
            //Assert.Throws<JsonException>(() => JsonSerializer.Parse<double>(double.MaxValue.ToString() + "0"));
            //Assert.Throws<JsonException>(() => JsonSerializer.Parse<double?>(double.MinValue.ToString() + "0"));
            //Assert.Throws<JsonException>(() => JsonSerializer.Parse<double?>(double.MaxValue.ToString() + "0"));
        }

        [Fact]
        public static void RangePass()
        {
            Assert.Equal(byte.MaxValue, JsonSerializer.Parse<byte>(byte.MaxValue.ToString()));
            Assert.Equal(byte.MaxValue, JsonSerializer.Parse<byte?>(byte.MaxValue.ToString()));

            Assert.Equal(sbyte.MaxValue, JsonSerializer.Parse<sbyte>(sbyte.MaxValue.ToString()));
            Assert.Equal(sbyte.MaxValue, JsonSerializer.Parse<sbyte?>(sbyte.MaxValue.ToString()));

            Assert.Equal(short.MaxValue, JsonSerializer.Parse<short>(short.MaxValue.ToString()));
            Assert.Equal(short.MaxValue, JsonSerializer.Parse<short?>(short.MaxValue.ToString()));

            Assert.Equal(ushort.MaxValue, JsonSerializer.Parse<ushort>(ushort.MaxValue.ToString()));
            Assert.Equal(ushort.MaxValue, JsonSerializer.Parse<ushort?>(ushort.MaxValue.ToString()));

            // todo: these fail due to double->float conversion
            //Assert.Equal(float.MaxValue, JsonSerializer.Parse<float>(float.MaxValue.ToString()));
            //Assert.Equal(float.MaxValue, JsonSerializer.Parse<float?>(float.MaxValue.ToString()));

            Assert.Equal(int.MaxValue, JsonSerializer.Parse<int>(int.MaxValue.ToString()));
            Assert.Equal(int.MaxValue, JsonSerializer.Parse<int?>(int.MaxValue.ToString()));

            Assert.Equal(uint.MaxValue, JsonSerializer.Parse<uint>(uint.MaxValue.ToString()));
            Assert.Equal(uint.MaxValue, JsonSerializer.Parse<uint?>(uint.MaxValue.ToString()));

            Assert.Equal(long.MaxValue, JsonSerializer.Parse<long>(long.MaxValue.ToString()));
            Assert.Equal(long.MaxValue, JsonSerializer.Parse<long?>(long.MaxValue.ToString()));

            Assert.Equal(ulong.MaxValue, JsonSerializer.Parse<ulong>(ulong.MaxValue.ToString()));
            Assert.Equal(ulong.MaxValue, JsonSerializer.Parse<ulong?>(ulong.MaxValue.ToString()));

            Assert.Equal(decimal.MaxValue, JsonSerializer.Parse<decimal>(decimal.MaxValue.ToString()));
            Assert.Equal(decimal.MaxValue, JsonSerializer.Parse<decimal?>(decimal.MaxValue.ToString()));

            // todo: these are failing; do we need round-trip format "R"?
            //Assert.Equal(double.MaxValue, JsonSerializer.Parse<double>(double.MaxValue.ToString()));
            //Assert.Equal(double.MaxValue, JsonSerializer.Parse<double?>(double.MaxValue.ToString()));
        }

        [Fact]
        public static void ValueFail()
        {
            string unexpectedString = @"""unexpected string""";

            Assert.Throws<JsonException>(() => JsonSerializer.Parse<byte>(unexpectedString));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<byte?>(unexpectedString));

            Assert.Throws<JsonException>(() => JsonSerializer.Parse<sbyte>(unexpectedString));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<sbyte?>(unexpectedString));

            Assert.Throws<JsonException>(() => JsonSerializer.Parse<short>(unexpectedString));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<short?>(unexpectedString));

            Assert.Throws<JsonException>(() => JsonSerializer.Parse<ushort>(unexpectedString));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<ushort?>(unexpectedString));

            Assert.Throws<JsonException>(() => JsonSerializer.Parse<float>(unexpectedString));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<float?>(unexpectedString));

            Assert.Throws<JsonException>(() => JsonSerializer.Parse<int>(unexpectedString));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<int?>(unexpectedString));

            Assert.Throws<JsonException>(() => JsonSerializer.Parse<uint>(unexpectedString));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<uint?>(unexpectedString));

            Assert.Throws<JsonException>(() => JsonSerializer.Parse<long>(unexpectedString));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<long?>(unexpectedString));

            Assert.Throws<JsonException>(() => JsonSerializer.Parse<ulong>(unexpectedString));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<ulong?>(unexpectedString));

            Assert.Throws<JsonException>(() => JsonSerializer.Parse<decimal>(unexpectedString));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<decimal?>(unexpectedString));

            Assert.Throws<JsonException>(() => JsonSerializer.Parse<double>(unexpectedString));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<double?>(unexpectedString));

            Assert.Throws<JsonException>(() => JsonSerializer.Parse<DateTime>(unexpectedString));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<DateTime?>(unexpectedString));

            Assert.Throws<JsonException>(() => JsonSerializer.Parse<DateTimeOffset>(unexpectedString));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<DateTimeOffset?>(unexpectedString));

            Assert.Throws<JsonException>(() => JsonSerializer.Parse<string>("1"));

            Assert.Throws<JsonException>(() => JsonSerializer.Parse<char>("1"));
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<char?>("1"));

            Assert.Throws<JsonException>(() => JsonSerializer.Parse<Enum>(unexpectedString));
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
        public static void ReadEmptyObjectArray()
        {
            SimpleTestClass[] data = JsonSerializer.Parse<SimpleTestClass[]>("[{}]");
            Assert.Equal(1, data.Length);
            Assert.NotNull(data[0]);
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

            i = JsonSerializer.Parse<List<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, i.Count);
        }

        [Fact]
        public static void ReadIEnumerableTOfIEnumerableT()
        {
            IEnumerable<IEnumerable<int>> result = JsonSerializer.Parse<IEnumerable<IEnumerable<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (IEnumerable<int> ie in result)
            {
                foreach (int i in ie)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadIEnumerableTOfArray()
        {
            IEnumerable<int[]> result = JsonSerializer.Parse<IEnumerable<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (int[] arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadArrayOfIEnumerableT()
        {
            IEnumerable<int>[] result = JsonSerializer.Parse<IEnumerable<int>[]> (Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (IEnumerable<int> arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadPrimitiveIEnumerableT()
        {
            IEnumerable<int> result = JsonSerializer.Parse<IEnumerable<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            int expected = 1;

            foreach (int i in result)
            {
                Assert.Equal(expected++, i);
            }

            result = JsonSerializer.Parse<IEnumerable<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public static void ReadIListTOfIListT()
        {
            IList<IList<int>> result = JsonSerializer.Parse<IList<IList<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (IList<int> ie in result)
            {
                foreach (int i in ie)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadIListTOfArray()
        {
            IList<int[]> result = JsonSerializer.Parse<IList<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (int[] arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadArrayOfIListT()
        {
            IList<int>[] result = JsonSerializer.Parse<IList<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (IList<int> arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadPrimitiveIListT()
        {
            IList<int> result = JsonSerializer.Parse<IList<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            int expected = 1;

            foreach (int i in result)
            {
                Assert.Equal(expected++, i);
            }

            result = JsonSerializer.Parse<IList<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public static void ReadICollectionTOfICollectionT()
        {
            ICollection<ICollection<int>> result = JsonSerializer.Parse<ICollection<ICollection<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (ICollection<int> ie in result)
            {
                foreach (int i in ie)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadICollectionTOfArray()
        {
            ICollection<int[]> result = JsonSerializer.Parse<ICollection<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (int[] arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadArrayOfICollectionT()
        {
            ICollection<int>[] result = JsonSerializer.Parse<ICollection<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (ICollection<int> arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadPrimitiveICollectionT()
        {
            ICollection<int> result = JsonSerializer.Parse<ICollection<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            int expected = 1;

            foreach (int i in result)
            {
                Assert.Equal(expected++, i);
            }

            result = JsonSerializer.Parse<ICollection<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public static void ReadIReadOnlyCollectionTOfIReadOnlyCollectionT()
        {
            IReadOnlyCollection<IReadOnlyCollection<int>> result = JsonSerializer.Parse<IReadOnlyCollection<IReadOnlyCollection<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (IReadOnlyCollection<int> ie in result)
            {
                foreach (int i in ie)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadIReadOnlyCollectionTOfArray()
        {
            IReadOnlyCollection<int[]> result = JsonSerializer.Parse<IReadOnlyCollection<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (int[] arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadArrayOfIReadOnlyCollectionT()
        {
            IReadOnlyCollection<int>[] result = JsonSerializer.Parse<IReadOnlyCollection<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (IReadOnlyCollection<int> arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadPrimitiveIReadOnlyCollectionT()
        {
            IReadOnlyCollection<int> result = JsonSerializer.Parse<IReadOnlyCollection<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            int expected = 1;

            foreach (int i in result)
            {
                Assert.Equal(expected++, i);
            }

            result = JsonSerializer.Parse<IReadOnlyCollection<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public static void ReadIReadOnlyListTOfIReadOnlyListT()
        {
            IReadOnlyList<IReadOnlyList<int>> result = JsonSerializer.Parse<IReadOnlyList<IReadOnlyList<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (IReadOnlyList<int> ie in result)
            {
                foreach (int i in ie)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadIReadOnlyListTOfArray()
        {
            IReadOnlyList<int[]> result = JsonSerializer.Parse<IReadOnlyList<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (int[] arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadArrayOfIReadOnlyListT()
        {
            IReadOnlyList<int>[] result = JsonSerializer.Parse<IReadOnlyList<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (IReadOnlyList<int> arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadPrimitiveIReadOnlyListT()
        {
            IReadOnlyList<int> result = JsonSerializer.Parse<IReadOnlyList<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            int expected = 1;

            foreach (int i in result)
            {
                Assert.Equal(expected++, i);
            }

            result = JsonSerializer.Parse<IReadOnlyList<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public static void StackTOfStackT()
        {
            Stack<Stack<int>> result = JsonSerializer.Parse<Stack<Stack<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 4;

            foreach (Stack<int> st in result)
            {
                foreach (int i in st)
                {
                    Assert.Equal(expected--, i);
                }
            }
        }

        [Fact]
        public static void ReadStackTOfArray()
        {
            Stack<int[]> result = JsonSerializer.Parse<Stack<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 3;

            foreach (int[] arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }

                expected = 1;
            }
        }

        [Fact]
        public static void ReadArrayOfStackT()
        {
            Stack<int>[] result = JsonSerializer.Parse<Stack<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 2;

            foreach (Stack<int> st in result)
            {
                foreach (int i in st)
                {
                    Assert.Equal(expected--, i);
                }

                expected = 4;
            }
        }

        [Fact]
        public static void ReadPrimitiveStackT()
        {
            Stack<int> result = JsonSerializer.Parse<Stack<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            int expected = 2;

            foreach (int i in result)
            {
                Assert.Equal(expected--, i);
            }

            result = JsonSerializer.Parse<Stack<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public static void ReadQueueTOfQueueT()
        {
            Queue<Queue<int>> result = JsonSerializer.Parse<Queue<Queue<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (Queue<int> q in result)
            {
                foreach (int i in q)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadQueueTOfArray()
        {
            Queue<int[]> result = JsonSerializer.Parse<Queue<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (int[] arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadArrayOfIQueueT()
        {
            Queue<int>[] result = JsonSerializer.Parse<Queue<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (Queue<int> q in result)
            {
                foreach (int i in q)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadPrimitiveQueueT()
        {
            Queue<int> result = JsonSerializer.Parse<Queue<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            int expected = 1;

            foreach (int i in result)
            {
                Assert.Equal(expected++, i);
            }
            result = JsonSerializer.Parse<Queue<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());

        }

        [Fact]
        public static void ReadHashSetTOfHashSetT()
        {
            HashSet<HashSet<int>> result = JsonSerializer.Parse<HashSet<HashSet<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (HashSet<int> hs in result)
            {
                foreach (int i in hs)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadHashSetTOfArray()
        {
            HashSet<int[]> result = JsonSerializer.Parse<HashSet<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (int[] arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadArrayOfIHashSetT()
        {
            HashSet<int>[] result = JsonSerializer.Parse<HashSet<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (HashSet<int> hs in result)
            {
                foreach (int i in hs)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadPrimitiveHashSetT()
        {
            HashSet<int> result = JsonSerializer.Parse<HashSet<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            int expected = 1;

            foreach (int i in result)
            {
                Assert.Equal(expected++, i);
            }

            result = JsonSerializer.Parse<HashSet<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public static void ReadLinkedListTOfLinkedListT()
        {
            LinkedList<LinkedList<int>> result = JsonSerializer.Parse<LinkedList<LinkedList<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (LinkedList<int> l in result)
            {
                foreach (int i in l)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadLinkedListTOfArray()
        {
            LinkedList<int[]> result = JsonSerializer.Parse<LinkedList<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (int[] arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadArrayOfILinkedListT()
        {
            LinkedList<int>[] result = JsonSerializer.Parse<LinkedList<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (LinkedList<int> l in result)
            {
                foreach (int i in l)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadPrimitiveLinkedListT()
        {
            LinkedList<int> result = JsonSerializer.Parse<LinkedList<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            int expected = 1;

            foreach (int i in result)
            {
                Assert.Equal(expected++, i);
            }

            result = JsonSerializer.Parse<LinkedList<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public static void ReadArrayOfISortedSetT()
        {
            SortedSet<int>[] result = JsonSerializer.Parse<SortedSet<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (SortedSet<int> s in result)
            {
                foreach (int i in s)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadPrimitiveSortedSetT()
        {
            SortedSet<int> result = JsonSerializer.Parse<SortedSet<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            int expected = 1;

            foreach (int i in result)
            {
                Assert.Equal(expected++, i);
            }

            result = JsonSerializer.Parse<SortedSet<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public static void ReadIImmutableListTOfIImmutableListT()
        {
            IImmutableList<IImmutableList<int>> result = JsonSerializer.Parse<IImmutableList<IImmutableList<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (IImmutableList<int> l in result)
            {
                foreach (int i in l)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadIImmutableListTOfArray()
        {
            IImmutableList<int[]> result = JsonSerializer.Parse<IImmutableList<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (int[] arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadArrayOfIIImmutableListT()
        {
            IImmutableList<int>[] result = JsonSerializer.Parse<IImmutableList<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (IImmutableList<int> l in result)
            {
                foreach (int i in l)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadPrimitiveIImmutableListT()
        {
            IImmutableList<int> result = JsonSerializer.Parse<IImmutableList<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            int expected = 1;

            foreach (int i in result)
            {
                Assert.Equal(expected++, i);
            }

            result = JsonSerializer.Parse<IImmutableList<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public static void ReadIImmutableStackTOfIImmutableStackT()
        {
            IImmutableStack<IImmutableStack<int>> result = JsonSerializer.Parse<IImmutableStack<IImmutableStack<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 4;

            foreach (IImmutableStack<int> l in result)
            {
                foreach (int i in l)
                {
                    Assert.Equal(expected--, i);
                }
            }
        }

        [Fact]
        public static void ReadIImmutableStackTOfArray()
        {
            IImmutableStack<int[]> result = JsonSerializer.Parse<IImmutableStack<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 3;

            foreach (int[] arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }

                expected = 1;
            }
        }

        [Fact]
        public static void ReadArrayOfIIImmutableStackT()
        {
            IImmutableStack<int>[] result = JsonSerializer.Parse<IImmutableStack<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 2;

            foreach (IImmutableStack<int> l in result)
            {
                foreach (int i in l)
                {
                    Assert.Equal(expected--, i);
                }

                expected = 4;
            }
        }

        [Fact]
        public static void ReadPrimitiveIImmutableStackT()
        {
            IImmutableStack<int> result = JsonSerializer.Parse<IImmutableStack<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            int expected = 2;

            foreach (int i in result)
            {
                Assert.Equal(expected--, i);
            }

            result = JsonSerializer.Parse<IImmutableStack<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public static void ReadIImmutableQueueTOfIImmutableQueueT()
        {
            IImmutableQueue<IImmutableQueue<int>> result = JsonSerializer.Parse<IImmutableQueue<IImmutableQueue<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (IImmutableQueue<int> l in result)
            {
                foreach (int i in l)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadIImmutableQueueTOfArray()
        {
            IImmutableQueue<int[]> result = JsonSerializer.Parse<IImmutableQueue<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (int[] arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadArrayOfIIImmutableQueueT()
        {
            IImmutableQueue<int>[] result = JsonSerializer.Parse<IImmutableQueue<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (IImmutableQueue<int> l in result)
            {
                foreach (int i in l)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadPrimitiveIImmutableQueueT()
        {
            IImmutableQueue<int> result = JsonSerializer.Parse<IImmutableQueue<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            int expected = 1;

            foreach (int i in result)
            {
                Assert.Equal(expected++, i);
            }

            result = JsonSerializer.Parse<IImmutableQueue<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public static void ReadIImmutableSetTOfIImmutableSetT()
        {
            IImmutableSet<IImmutableSet<int>> result = JsonSerializer.Parse<IImmutableSet<IImmutableSet<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            List<int> expected = new List<int> { 1, 2, 3, 4 };

            foreach (IImmutableSet<int> l in result)
            {
                foreach (int i in l)
                {
                    expected.Remove(i);
                }
            }

            Assert.Equal(0, expected.Count);
        }

        [Fact]
        public static void ReadIImmutableSetTOfArray()
        {
            IImmutableSet<int[]> result = JsonSerializer.Parse<IImmutableSet<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            List<int> expected = new List<int> { 1, 2, 3, 4 };

            foreach (int[] arr in result)
            {
                foreach (int i in arr)
                {
                    expected.Remove(i);
                }
            }

            Assert.Equal(0, expected.Count);
        }

        [Fact]
        public static void ReadArrayOfIIImmutableSetT()
        {
            IImmutableSet<int>[] result = JsonSerializer.Parse<IImmutableSet<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            List<int> expected = new List<int> { 1, 2, 3, 4 };

            foreach (IImmutableSet<int> l in result)
            {
                foreach (int i in l)
                {
                    expected.Remove(i);
                }
            }

            Assert.Equal(0, expected.Count);
        }

        [Fact]
        public static void ReadPrimitiveIImmutableSetT()
        {
            IImmutableSet<int> result = JsonSerializer.Parse<IImmutableSet<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            List<int> expected = new List<int> { 1, 2 };

            foreach (int i in result)
            {
                expected.Remove(i);
            }

            Assert.Equal(0, expected.Count);

            result = JsonSerializer.Parse<IImmutableSet<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public static void ReadImmutableHashSetTOfImmutableHashSetT()
        {
            ImmutableHashSet<ImmutableHashSet<int>> result = JsonSerializer.Parse<ImmutableHashSet<ImmutableHashSet<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            List<int> expected = new List<int> { 1, 2, 3, 4 };

            foreach (ImmutableHashSet<int> l in result)
            {
                foreach (int i in l)
                {
                    expected.Remove(i);
                }
            }

            Assert.Equal(0, expected.Count);
        }

        [Fact]
        public static void ReadImmutableHashSetTOfArray()
        {
            ImmutableHashSet<int[]> result = JsonSerializer.Parse<ImmutableHashSet<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            List<int> expected = new List<int> { 1, 2, 3, 4 };

            foreach (int[] arr in result)
            {
                foreach (int i in arr)
                {
                    expected.Remove(i);
                }
            }

            Assert.Equal(0, expected.Count);
        }

        [Fact]
        public static void ReadArrayOfIImmutableHashSetT()
        {
            ImmutableHashSet<int>[] result = JsonSerializer.Parse<ImmutableHashSet<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            List<int> expected = new List<int> { 1, 2, 3, 4 };

            foreach (ImmutableHashSet<int> l in result)
            {
                foreach (int i in l)
                {
                    expected.Remove(i);
                }
            }

            Assert.Equal(0, expected.Count);
        }

        [Fact]
        public static void ReadPrimitiveImmutableHashSetT()
        {
            ImmutableHashSet<int> result = JsonSerializer.Parse<ImmutableHashSet<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            List<int> expected = new List<int> { 1, 2 };

            foreach (int i in result)
            {
                expected.Remove(i);
            }

            Assert.Equal(0, expected.Count);

            result = JsonSerializer.Parse<ImmutableHashSet<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public static void ReadImmutableListTOfImmutableListT()
        {
            ImmutableList<ImmutableList<int>> result = JsonSerializer.Parse<ImmutableList<ImmutableList<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (ImmutableList<int> l in result)
            {
                foreach (int i in l)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadImmutableListTOfArray()
        {
            ImmutableList<int[]> result = JsonSerializer.Parse<ImmutableList<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (int[] arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadArrayOfIImmutableListT()
        {
            ImmutableList<int>[] result = JsonSerializer.Parse<ImmutableList<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (ImmutableList<int> l in result)
            {
                foreach (int i in l)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadPrimitiveImmutableListT()
        {
            ImmutableList<int> result = JsonSerializer.Parse<ImmutableList<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            int expected = 1;

            foreach (int i in result)
            {
                Assert.Equal(expected++, i);
            }

            result = JsonSerializer.Parse<ImmutableList<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public static void ReadImmutableStackTOfImmutableStackT()
        {
            ImmutableStack<ImmutableStack<int>> result = JsonSerializer.Parse<ImmutableStack<ImmutableStack<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 4;

            foreach (ImmutableStack<int> l in result)
            {
                foreach (int i in l)
                {
                    Assert.Equal(expected--, i);
                }
            }
        }

        [Fact]
        public static void ReadImmutableStackTOfArray()
        {
            ImmutableStack<int[]> result = JsonSerializer.Parse<ImmutableStack<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 3;

            foreach (int[] arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }

                expected = 1;
            }
        }

        [Fact]
        public static void ReadArrayOfIImmutableStackT()
        {
            ImmutableStack<int>[] result = JsonSerializer.Parse<ImmutableStack<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 2;

            foreach (ImmutableStack<int> l in result)
            {
                foreach (int i in l)
                {
                    Assert.Equal(expected--, i);
                }

                expected = 4;
            }
        }

        [Fact]
        public static void ReadPrimitiveImmutableStackT()
        {
            ImmutableStack<int> result = JsonSerializer.Parse<ImmutableStack<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            int expected = 2;

            foreach (int i in result)
            {
                Assert.Equal(expected--, i);
            }

            result = JsonSerializer.Parse<ImmutableStack<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public static void ReadImmutableQueueTOfImmutableQueueT()
        {
            ImmutableQueue<ImmutableQueue<int>> result = JsonSerializer.Parse<ImmutableQueue<ImmutableQueue<int>>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (ImmutableQueue<int> l in result)
            {
                foreach (int i in l)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadImmutableQueueTOfArray()
        {
            ImmutableQueue<int[]> result = JsonSerializer.Parse<ImmutableQueue<int[]>>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (int[] arr in result)
            {
                foreach (int i in arr)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadArrayOfIImmutableQueueT()
        {
            ImmutableQueue<int>[] result = JsonSerializer.Parse<ImmutableQueue<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (ImmutableQueue<int> l in result)
            {
                foreach (int i in l)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadPrimitiveImmutableQueueT()
        {
            ImmutableQueue<int> result = JsonSerializer.Parse<ImmutableQueue<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            int expected = 1;

            foreach (int i in result)
            {
                Assert.Equal(expected++, i);
            }

            result = JsonSerializer.Parse<ImmutableQueue<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public static void ReadArrayOfIImmutableSortedSetT()
        {
            ImmutableSortedSet<int>[] result = JsonSerializer.Parse<ImmutableSortedSet<int>[]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            int expected = 1;

            foreach (ImmutableSortedSet<int> l in result)
            {
                foreach (int i in l)
                {
                    Assert.Equal(expected++, i);
                }
            }
        }

        [Fact]
        public static void ReadPrimitiveImmutableSortedSetT()
        {
            ImmutableSortedSet<int> result = JsonSerializer.Parse<ImmutableSortedSet<int>>(Encoding.UTF8.GetBytes(@"[1,2]"));
            int expected = 1;

            foreach (int i in result)
            {
                Assert.Equal(expected++, i);
            }

            result = JsonSerializer.Parse<ImmutableSortedSet<int>>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, result.Count());
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
            catch (JsonException exception)
            {
                exceptionThrown = true;

                // Exception should contain property path.
                Assert.True(exception.ToString().Contains("[System.Text.Json.Serialization.Tests.ValueTests+TestClassWithBadData].Children.MyProperty"));
            }

            Assert.True(exceptionThrown);
        }
    }
}

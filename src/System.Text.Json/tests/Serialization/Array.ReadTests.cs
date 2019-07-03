﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static partial class ArrayTests
    {
        [Fact]
        public static void ReadObjectArray()
        {
            string data =
                "[" +
                SimpleTestClass.s_json +
                "," +
                SimpleTestClass.s_json +
                "]";

            SimpleTestClass[] i = JsonSerializer.Deserialize<SimpleTestClass[]>(Encoding.UTF8.GetBytes(data));

            i[0].Verify();
            i[1].Verify();
        }

        [Fact]
        public static void ReadNullByteArray()
        {
            string json = @"null";
            byte[] arr = JsonSerializer.Deserialize<byte[]>(json);
            Assert.Null(arr);
        }

        [Fact]
        public static void ReadEmptyByteArray()
        {
            string json = @"""""";
            byte[] arr = JsonSerializer.Deserialize<byte[]>(json);
            Assert.Equal(0, arr.Length);
        }

        [Fact]
        public static void ReadByteArray()
        {
            string json = $"\"{Convert.ToBase64String(new byte[] { 1, 2 })}\"";
            byte[] arr = JsonSerializer.Deserialize<byte[]>(json);

            Assert.Equal(2, arr.Length);
            Assert.Equal(1, arr[0]);
            Assert.Equal(2, arr[1]);
        }

        [Fact]
        public static void Read2dByteArray()
        {
            // Baseline for comparison.
            Assert.Equal("AQI=", Convert.ToBase64String(new byte[] { 1, 2 }));

            string json = "[\"AQI=\",\"AQI=\"]";
            byte[][] arr = JsonSerializer.Deserialize<byte[][]>(json);
            Assert.Equal(2, arr.Length);

            Assert.Equal(2, arr[0].Length);
            Assert.Equal(1, arr[0][0]);
            Assert.Equal(2, arr[0][1]);

            Assert.Equal(2, arr[1].Length);
            Assert.Equal(1, arr[1][0]);
            Assert.Equal(2, arr[1][1]);
        }

        [Theory]
        [InlineData(@"""1""")]
        [InlineData(@"""A===""")]
        [InlineData(@"[1, 2]")]  // Currently not support deserializing JSON arrays as byte[] - only Base64 string.
        public static void ReadByteArrayFail(string json)
        {
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<byte[]>(json));
        }

        [Fact]
        public static void ReadByteListAsJsonArray()
        {
            string json = $"[1, 2]";
            List<byte> list = JsonSerializer.Deserialize<List<byte>>(json);

            Assert.Equal(2, list.Count);
            Assert.Equal(1, list[0]);
            Assert.Equal(2, list[1]);
        }

        [Fact]
        public static void DeserializeObjectArray_36167()
        {
            // https://github.com/dotnet/corefx/issues/36167
            object[] data = JsonSerializer.Deserialize<object[]>("[1]");
            Assert.Equal(1, data.Length);
            Assert.IsType<JsonElement>(data[0]);
            Assert.Equal(1, ((JsonElement)data[0]).GetInt32());
        }

        [Fact]
        public static void ReadEmptyObjectArray()
        {
            SimpleTestClass[] data = JsonSerializer.Deserialize<SimpleTestClass[]>("[{}]");
            Assert.Equal(1, data.Length);
            Assert.NotNull(data[0]);
        }

        [Fact]
        public static void ReadPrimitiveJagged2dArray()
        {
            int[][] i = JsonSerializer.Deserialize<int[][]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]"));
            Assert.Equal(1, i[0][0]);
            Assert.Equal(2, i[0][1]);
            Assert.Equal(3, i[1][0]);
            Assert.Equal(4, i[1][1]);
        }

        [Fact]
        public static void ReadPrimitiveJagged3dArray()
        {
            int[][][] i = JsonSerializer.Deserialize<int[][][]>(Encoding.UTF8.GetBytes(@"[[[11,12],[13,14]], [[21,22],[23,24]]]"));
            Assert.Equal(11, i[0][0][0]);
            Assert.Equal(12, i[0][0][1]);
            Assert.Equal(13, i[0][1][0]);
            Assert.Equal(14, i[0][1][1]);

            Assert.Equal(21, i[1][0][0]);
            Assert.Equal(22, i[1][0][1]);
            Assert.Equal(23, i[1][1][0]);
            Assert.Equal(24, i[1][1][1]);
        }

        [Fact]
        public static void ReadArrayWithInterleavedComments()
        {
            var options = new JsonSerializerOptions();
            options.ReadCommentHandling = JsonCommentHandling.Skip;

            int[][] i = JsonSerializer.Deserialize<int[][]>(Encoding.UTF8.GetBytes("[[1,2] // Inline [\n,[3, /* Multi\n]] Line*/4]]"), options);
            Assert.Equal(1, i[0][0]);
            Assert.Equal(2, i[0][1]);
            Assert.Equal(3, i[1][0]);
            Assert.Equal(4, i[1][1]);
        }

        [Fact]
        public static void ReadEmpty()
        {
            SimpleTestClass[] arr = JsonSerializer.Deserialize<SimpleTestClass[]>("[]");
            Assert.Equal(0, arr.Length);

            List<SimpleTestClass> list = JsonSerializer.Deserialize<List<SimpleTestClass>>("[]");
            Assert.Equal(0, list.Count);
        }

        [Fact]
        public static void ReadPrimitiveArray()
        {
            int[] i = JsonSerializer.Deserialize<int[]>(Encoding.UTF8.GetBytes(@"[1,2]"));
            Assert.Equal(1, i[0]);
            Assert.Equal(2, i[1]);

            i = JsonSerializer.Deserialize<int[]>(Encoding.UTF8.GetBytes(@"[]"));
            Assert.Equal(0, i.Length);
        }

        [ActiveIssue(38435)]
        [Fact]
        public static void ReadInitializedArrayTest()
        {
            string serialized = "{\"Values\":[1,2,3]}";
            TestClassWithInitializedArray testClassWithInitializedArray = JsonSerializer.Deserialize<TestClassWithInitializedArray>(serialized);

            Assert.Equal(1, testClassWithInitializedArray.Values[0]);
            Assert.Equal(2, testClassWithInitializedArray.Values[1]);
            Assert.Equal(3, testClassWithInitializedArray.Values[2]);
        }

        [Fact]
        public static void ReadArrayWithEnums()
        {
            SampleEnum[] i = JsonSerializer.Deserialize<SampleEnum[]>(Encoding.UTF8.GetBytes(@"[1,2]"));
            Assert.Equal(SampleEnum.One, i[0]);
            Assert.Equal(SampleEnum.Two, i[1]);
        }

        [Fact]
        public static void ReadPrimitiveArrayFail()
        {
            // Invalid data
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<int[]>(Encoding.UTF8.GetBytes(@"[1,""a""]")));

            // Invalid data
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<List<int?>>(Encoding.UTF8.GetBytes(@"[1,""a""]")));

            // Multidimensional arrays currently not supported
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<int[,]>(Encoding.UTF8.GetBytes(@"[[1,2],[3,4]]")));
        }

        public static IEnumerable<object[]> ReadNullJson
        {
            get
            {
                yield return new object[] { $"[null, null, null]", true, true, true };
                yield return new object[] { $"[null, null, {SimpleTestClass.s_json}]", true, true, false };
                yield return new object[] { $"[null, {SimpleTestClass.s_json}, null]", true, false, true };
                yield return new object[] { $"[null, {SimpleTestClass.s_json}, {SimpleTestClass.s_json}]", true, false, false };
                yield return new object[] { $"[{SimpleTestClass.s_json}, {SimpleTestClass.s_json}, {SimpleTestClass.s_json}]", false, false, false };
                yield return new object[] { $"[{SimpleTestClass.s_json}, {SimpleTestClass.s_json}, null]", false, false, true };
                yield return new object[] { $"[{SimpleTestClass.s_json}, null, {SimpleTestClass.s_json}]", false, true, false };
                yield return new object[] { $"[{SimpleTestClass.s_json}, null, null]", false, true, true };
            }
        }

        [Theory]
        [MemberData(nameof(ReadNullJson))]
        public static void ReadNull(string json, bool element0Null, bool element1Null, bool element2Null)
        {
            SimpleTestClass[] arr = JsonSerializer.Deserialize<SimpleTestClass[]>(json);
            Assert.Equal(3, arr.Length);
            VerifyReadNull(arr[0], element0Null);
            VerifyReadNull(arr[1], element1Null);
            VerifyReadNull(arr[2], element2Null);

            List<SimpleTestClass> list = JsonSerializer.Deserialize<List<SimpleTestClass>>(json);
            Assert.Equal(3, list.Count);
            VerifyReadNull(list[0], element0Null);
            VerifyReadNull(list[1], element1Null);
            VerifyReadNull(list[2], element2Null);

            static void VerifyReadNull(SimpleTestClass obj, bool isNull)
            {
                if (isNull)
                {
                    Assert.Null(obj);
                }
                else
                {
                    obj.Verify();
                }
            }
        }

        [Fact]
        public static void ReadClassWithStringArray()
        {
            TestClassWithStringArray obj = JsonSerializer.Deserialize<TestClassWithStringArray>(TestClassWithStringArray.s_data);
            obj.Verify();
        }

        [Fact]
        public static void ReadClassWithObjectList()
        {
            TestClassWithObjectList obj = JsonSerializer.Deserialize<TestClassWithObjectList>(TestClassWithObjectList.s_data);
            obj.Verify();
        }

        [Fact]
        public static void ReadClassWithObjectArray()
        {
            TestClassWithObjectArray obj = JsonSerializer.Deserialize<TestClassWithObjectArray>(TestClassWithObjectArray.s_data);
            obj.Verify();
        }

        [Fact]
        public static void ReadClassWithGenericList()
        {
            TestClassWithGenericList obj = JsonSerializer.Deserialize<TestClassWithGenericList>(TestClassWithGenericList.s_data);
            obj.Verify();
        }

        [Fact]
        public static void ReadClassWithObjectIEnumerable()
        {
            TestClassWithObjectIEnumerable obj = JsonSerializer.Deserialize<TestClassWithObjectIEnumerable>(TestClassWithObjectIEnumerable.s_data);
            obj.Verify();
        }

        [Fact]
        public static void ReadClassWithObjectIList()
        {
            TestClassWithObjectIList obj = JsonSerializer.Deserialize<TestClassWithObjectIList>(TestClassWithObjectIList.s_data);
            obj.Verify();
        }

        [Fact]
        public static void ReadClassWithObjectICollection()
        {
            TestClassWithObjectICollection obj = JsonSerializer.Deserialize<TestClassWithObjectICollection>(TestClassWithObjectICollection.s_data);
            obj.Verify();
        }

        [Fact]
        public static void ReadClassWithObjectIEnumerableT()
        {
            TestClassWithObjectIEnumerableT obj = JsonSerializer.Deserialize<TestClassWithObjectIEnumerableT>(TestClassWithObjectIEnumerableT.s_data);
            obj.Verify();
        }

        [Fact]
        public static void ReadClassWithObjectIListT()
        {
            TestClassWithObjectIListT obj = JsonSerializer.Deserialize<TestClassWithObjectIListT>(TestClassWithObjectIListT.s_data);
            obj.Verify();
        }

        [Fact]
        public static void ReadClassWithObjectICollectionT()
        {
            TestClassWithObjectICollectionT obj = JsonSerializer.Deserialize<TestClassWithObjectICollectionT>(TestClassWithObjectICollectionT.s_data);
            obj.Verify();
        }

        [Fact]
        public static void ReadClassWithObjectIReadOnlyCollectionT()
        {
            TestClassWithObjectIReadOnlyCollectionT obj = JsonSerializer.Deserialize<TestClassWithObjectIReadOnlyCollectionT>(TestClassWithObjectIReadOnlyCollectionT.s_data);
            obj.Verify();
        }

        [Fact]
        public static void ReadClassWithObjectIReadOnlyListT()
        {
            TestClassWithObjectIReadOnlyListT obj = JsonSerializer.Deserialize<TestClassWithObjectIReadOnlyListT>(TestClassWithObjectIReadOnlyListT.s_data);
            obj.Verify();
        }

        [Fact]
        public static void ReadClassWithGenericIEnumerable()
        {
            TestClassWithGenericIEnumerable obj = JsonSerializer.Deserialize<TestClassWithGenericIEnumerable>(TestClassWithGenericIEnumerable.s_data);
            obj.Verify();
        }

        [Fact]
        public static void ReadClassWithGenericIList()
        {
            TestClassWithGenericIList obj = JsonSerializer.Deserialize<TestClassWithGenericIList>(TestClassWithGenericIList.s_data);
            obj.Verify();
        }

        [Fact]
        public static void ReadClassWithGenericICollection()
        {
            TestClassWithGenericICollection obj = JsonSerializer.Deserialize<TestClassWithGenericICollection>(TestClassWithGenericICollection.s_data);
        }

        [Fact]
        public static void ReadClassWithObjectISetT()
        {
            TestClassWithObjectISetT obj = JsonSerializer.Deserialize<TestClassWithObjectISetT>(TestClassWithObjectISetT.s_data);
            obj.Verify();
        }

        [Fact]
        public static void ReadClassWithGenericIEnumerableT()
        {
            TestClassWithGenericIEnumerableT obj = JsonSerializer.Deserialize<TestClassWithGenericIEnumerableT>(TestClassWithGenericIEnumerableT.s_data);
            obj.Verify();
        }

        [Fact]
        public static void ReadClassWithGenericIListT()
        {
            TestClassWithGenericIListT obj = JsonSerializer.Deserialize<TestClassWithGenericIListT>(TestClassWithGenericIListT.s_data);
            obj.Verify();
        }

        [Fact]
        public static void ReadClassWithGenericICollectionT()
        {
            TestClassWithGenericICollectionT obj = JsonSerializer.Deserialize<TestClassWithGenericICollectionT>(TestClassWithGenericICollectionT.s_data);
            obj.Verify();
        }

        [Fact]
        public static void ReadClassWithGenericIReadOnlyCollectionT()
        {
            TestClassWithGenericIReadOnlyCollectionT obj = JsonSerializer.Deserialize<TestClassWithGenericIReadOnlyCollectionT>(TestClassWithGenericIReadOnlyCollectionT.s_data);
            obj.Verify();
        }

        [Fact]
        public static void ReadClassWithGenericIReadOnlyListT()
        {
            TestClassWithGenericIReadOnlyListT obj = JsonSerializer.Deserialize<TestClassWithGenericIReadOnlyListT>(TestClassWithGenericIReadOnlyListT.s_data);
            obj.Verify();
        }

        [Fact]
        public static void ReadClassWithGenericISetT()
        {
            TestClassWithGenericISetT obj = JsonSerializer.Deserialize<TestClassWithGenericISetT>(TestClassWithGenericISetT.s_data);
            obj.Verify();
        }

        [Fact]
        public static void ReadClassWithObjectIEnumerableConstructibleTypes()
        {
            TestClassWithObjectIEnumerableConstructibleTypes obj = JsonSerializer.Deserialize<TestClassWithObjectIEnumerableConstructibleTypes>(TestClassWithObjectIEnumerableConstructibleTypes.s_data);
            obj.Verify();
        }

        [Fact]
        public static void ReadClassWithObjectImmutableTypes()
        {
            TestClassWithObjectImmutableTypes obj = JsonSerializer.Deserialize<TestClassWithObjectImmutableTypes>(TestClassWithObjectImmutableTypes.s_data);
            obj.Verify();
        }

        [Fact]
        public static void ClassWithNoSetter()
        {
            string json = @"{""MyList"":[1]}";
            ClassWithListButNoSetter obj = JsonSerializer.Deserialize<ClassWithListButNoSetter>(json);
            Assert.Equal(1, obj.MyList[0]);
        }

        public class ClassWithListButNoSetter
        {
            public List<int> MyList { get; } = new List<int>();
        }
    }
}

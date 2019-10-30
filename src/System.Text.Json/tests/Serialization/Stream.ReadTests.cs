﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static partial class StreamTests
    {
        [Fact]
        public static async Task NullArgumentFail()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await JsonSerializer.DeserializeAsync<string>((Stream)null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await JsonSerializer.DeserializeAsync(new MemoryStream(), (Type)null));
        }

        [Fact]
        public static async Task ReadSimpleObjectAsync()
        {
            using (MemoryStream stream = new MemoryStream(SimpleTestClass.s_data))
            {
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    DefaultBufferSize = 1
                };

                SimpleTestClass obj = await JsonSerializer.DeserializeAsync<SimpleTestClass>(stream, options);
                obj.Verify();
            }
        }

        [Fact]
        public static async Task ReadSimpleObjectWithTrailingTriviaAsync()
        {
            byte[] data = Encoding.UTF8.GetBytes(SimpleTestClass.s_json + " /* Multi\r\nLine Comment */\t");
            using (MemoryStream stream = new MemoryStream(data))
            {
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    DefaultBufferSize = 1,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                };

                SimpleTestClass obj = await JsonSerializer.DeserializeAsync<SimpleTestClass>(stream, options);
                obj.Verify();
            }
        }

        [Fact]
        public static async Task ReadPrimitivesAsync()
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(@"1")))
            {
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    DefaultBufferSize = 1
                };

                int i = await JsonSerializer.DeserializeAsync<int>(stream, options);
                Assert.Equal(1, i);
            }
        }

        [Fact]
        public static async Task ReadPrimitivesWithTrailingTriviaAsync()
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(" 1\t// Comment\r\n/* Multi\r\nLine */")))
            {
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    DefaultBufferSize = 1,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                };

                int i = await JsonSerializer.DeserializeAsync<int>(stream, options);
                Assert.Equal(1, i);
            }
        }

        [Fact]
        public static async Task ReadReferenceTypeCollectionPassingNullValueAsync()
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes("null")))
            {
                IList<object> referenceTypeCollection = await JsonSerializer.DeserializeAsync<IList<object>>(stream);
                Assert.Null(referenceTypeCollection);
            }
        }

        [Fact]
        public static async Task ReadValueTypeCollectionPassingNullValueAsync()
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes("null")))
            {
                IList<int> valueTypeCollection = await JsonSerializer.DeserializeAsync<IList<int>>(stream);
                Assert.Null(valueTypeCollection);
            }
        }

        public static IEnumerable<object[]> BOMTestData =>
            new List<object[]>
            {
                new object[] {new byte[] { 0xEF, 0xBB, 0xBF, 49 }, default, 1 },
                new object[] {new byte[] { 0xEF, 0xBB, 0xBF, 49 }, new JsonSerializerOptions { DefaultBufferSize = 1 }, 1 },
                new object[] {new byte[] { 0xEF, 0xBB, 0xBF, 49 }, new JsonSerializerOptions { DefaultBufferSize = 2 }, 1 },
                new object[] {new byte[] { 0xEF, 0xBB, 0xBF, 49 }, new JsonSerializerOptions { DefaultBufferSize = 3 }, 1 },
                new object[] {new byte[] { 0xEF, 0xBB, 0xBF, 49 }, new JsonSerializerOptions { DefaultBufferSize = 4 }, 1 },
                new object[] {new byte[] { 0xEF, 0xBB, 0xBF, 49 }, new JsonSerializerOptions { DefaultBufferSize = 15 }, 1 },
                new object[] {new byte[] { 0xEF, 0xBB, 0xBF, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49 }, new JsonSerializerOptions { DefaultBufferSize = 15 }, 1111111111111111111 },
                new object[] {new byte[] { 0xEF, 0xBB, 0xBF, 49 }, new JsonSerializerOptions { DefaultBufferSize = 16 }, 1 },
                new object[] {new byte[] { 0xEF, 0xBB, 0xBF, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49 }, new JsonSerializerOptions { DefaultBufferSize = 16 }, 1111111111111111111 },
                new object[] {new byte[] { 0xEF, 0xBB, 0xBF, 49 }, new JsonSerializerOptions { DefaultBufferSize = 17 }, 1 },
                new object[] {new byte[] { 0xEF, 0xBB, 0xBF, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49, 49 }, new JsonSerializerOptions { DefaultBufferSize = 17 }, 1111111111111111111 },
            };

        [Theory]
        [MemberData(nameof(BOMTestData))]
        public static async Task TestBOMWithSingleJsonValue(byte[] utf8BomAndValueArray, JsonSerializerOptions options, ulong expected)
        {
            ulong value;
            using (Stream stream = new MemoryStream(utf8BomAndValueArray))
            {
                value = await JsonSerializer.DeserializeAsync<ulong>(stream, options);
            }
            Assert.Equal(expected, value);
        }

        [Fact]
        public static async Task TestBOMWithNoJsonValue()
        {
            byte[] utf8BomAndValueArray = new byte[] { 0xEF, 0xBB, 0xBF };
            using (Stream stream = new MemoryStream(utf8BomAndValueArray))
            {
                await Assert.ThrowsAsync<JsonException>(
                    async () => await JsonSerializer.DeserializeAsync<byte>(stream));
            }
        }

        public static IEnumerable<object[]> BOMWithStreamTestData
        {
            get
            {
                foreach (object[] testData in Yield(100, 6601)) yield return testData;
                foreach (object[] testData in Yield(200, 13201)) yield return testData;
                foreach (object[] testData in Yield(400, 26401)) yield return testData;
                foreach (object[] testData in Yield(800, 52801)) yield return testData;
                foreach (object[] testData in Yield(1600, 105601)) yield return testData;

                IEnumerable<object[]> Yield(int count, int expectedStreamLength)
                {
                    // Use the same stream instance so the tests run faster.
                    Stream stream = CreateStream(count);

                    // Test with both small (1 byte) and default (16K) buffer sizes to encourage
                    // different code paths dealing with buffer re-use and growing.
                    yield return new object[] { stream, count, expectedStreamLength, 1 };
                    yield return new object[] { stream, count, expectedStreamLength, 16 * 1024 };
                }

                static Stream CreateStream(int count)
                {
                    byte[] objBytes = Encoding.UTF8.GetBytes(
                        @"{""Test"":{},""Test2"":[],""Test3"":{""Value"":{}},""PersonType"":0,""Id"":2}");

                    byte[] utf8Bom = Encoding.UTF8.GetPreamble();

                    var stream = new MemoryStream();

                    stream.Write(utf8Bom, 0, utf8Bom.Length);
                    stream.WriteByte((byte)'[');

                    for (int i = 1; i <= count; i++)
                    {
                        stream.Write(objBytes, 0, objBytes.Length);

                        if (i < count)
                        {
                            stream.WriteByte((byte)',');
                        }
                    }

                    stream.WriteByte((byte)']');
                    return stream;
                }
            }
        }

        [Theory]
        [MemberData(nameof(BOMWithStreamTestData))]
        public static async Task TestBOMWithShortAndLongBuffers(Stream stream, int count, int expectedStreamLength, int bufferSize)
        {
            JsonElement[] value;

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                DefaultBufferSize = bufferSize
            };

            stream.Position = 0;
            value = await JsonSerializer.DeserializeAsync<JsonElement[]>(stream, options);

            // Verify first and last elements.
            VerifyElement(0);
            VerifyElement(count - 1);

            // Round trip and verify.
            stream.Position = 3; // Skip the BOM.
            string originalString = new StreamReader(stream).ReadToEnd();
            Assert.Equal(expectedStreamLength, originalString.Length);

            string roundTrippedString = JsonSerializer.Serialize(value);
            Assert.Equal(originalString, roundTrippedString);

            void VerifyElement(int index)
            {
                Assert.Equal(JsonValueKind.Object, value[index].GetProperty("Test").ValueKind);
                Assert.Equal(JsonValueKind.Array, value[index].GetProperty("Test2").ValueKind);
                Assert.Equal(0, value[index].GetProperty("Test2").GetArrayLength());
                Assert.Equal(JsonValueKind.Object, value[index].GetProperty("Test3").ValueKind);
                Assert.Equal(JsonValueKind.Object, value[index].GetProperty("Test3").GetProperty("Value").ValueKind);
                Assert.Equal(0, value[index].GetProperty("PersonType").GetInt32());
                Assert.Equal(2, value[index].GetProperty("Id").GetInt32());
            }
        }
    }
}

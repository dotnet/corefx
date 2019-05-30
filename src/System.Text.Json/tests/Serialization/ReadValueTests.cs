// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static partial class ReadValueTests
    {
        [Fact]
        public static void EnableComments()
        {
            string json = "3";

            JsonReaderOptions options = new JsonReaderOptions
            {
                CommentHandling = JsonCommentHandling.Allow,
            };

            byte[] utf8 = Encoding.UTF8.GetBytes(json);

            AssertExtensions.Throws<ArgumentException>(
                "reader",
                () =>
                {
                    var state = new JsonReaderState(options);
                    var reader = new Utf8JsonReader(utf8, isFinalBlock: false, state);
                    JsonSerializer.ReadValue(ref reader, typeof(int));
                });

            AssertExtensions.Throws<ArgumentException>(
                "reader",
                () =>
                {
                    var state = new JsonReaderState(options);
                    var reader = new Utf8JsonReader(utf8, isFinalBlock: true, state);
                    JsonSerializer.ReadValue(ref reader, typeof(int));
                });
        }

        [Fact]
        public static void ReadDefaultReader()
        {
            Assert.ThrowsAny<JsonException>(() =>
            {
                Utf8JsonReader reader = default;
                JsonSerializer.ReadValue(ref reader, typeof(int));
            });
        }

        [Fact]
        public static void ReadSimpleStruct()
        {
            byte[] utf8 = Encoding.UTF8.GetBytes(SimpleTestStruct.s_json);
            var reader = new Utf8JsonReader(utf8, isFinalBlock: true, state: default);
            SimpleTestStruct obj = JsonSerializer.ReadValue<SimpleTestStruct>(ref reader);
            obj.Verify();
        }

        [Fact]
        public static void ReadPartial()
        {
            byte[] utf8 = Encoding.UTF8.GetBytes("[1, 2, 3]");
            var reader = new Utf8JsonReader(utf8, isFinalBlock: true, state: default);
            reader.Read();
            int[] obj = JsonSerializer.ReadValue<int[]>(ref reader);
            var expected = new int[3] { 1, 2, 3 };
            Assert.Equal(expected, obj);

            reader = new Utf8JsonReader(utf8, isFinalBlock: true, state: default);
            reader.Read();
            reader.Read();
            int number = JsonSerializer.ReadValue<int>(ref reader);
            Assert.Equal(1, number);
        }
    }
}

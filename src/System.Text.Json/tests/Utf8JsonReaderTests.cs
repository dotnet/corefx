// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace System.Text.Json.Tests
{
    public static partial class Utf8JsonReaderTests
    {
        // TestCaseType is only used to give the json strings a descriptive name.
        [Theory]
        [MemberData(nameof(TestCases))]
        public static void TestJsonReaderUtf8(bool compactData, TestCaseType type, string jsonString)
        {
            // Remove all formatting/indendation
            if (compactData)
            {
                using (JsonTextReader jsonReader = new JsonTextReader(new StringReader(jsonString)))
                {
                    jsonReader.FloatParseHandling = FloatParseHandling.Decimal;
                    JToken jtoken = JToken.ReadFrom(jsonReader);
                    var stringWriter = new StringWriter();
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter))
                    {
                        jtoken.WriteTo(jsonWriter);
                        jsonString = stringWriter.ToString();
                    }
                }
            }

            byte[] dataUtf8 = Encoding.UTF8.GetBytes(jsonString);
            byte[] result = JsonTestHelper.ReturnBytesHelper(dataUtf8, out int length);
            string actualStr = Encoding.UTF8.GetString(result.AsSpan(0, length));

            Stream stream = new MemoryStream(dataUtf8);
            TextReader reader = new StreamReader(stream, Encoding.UTF8, false, 1024, true);
            string expectedStr = JsonTestHelper.NewtonsoftReturnStringHelper(reader);

            Assert.Equal(expectedStr, actualStr);

            // Json payload contains numbers that are too large for .NET (need BigInteger+)
            if (type != TestCaseType.FullSchema1 && type != TestCaseType.BasicLargeNum)
            {
                object jsonValues = JsonTestHelper.ReturnObjectHelper(dataUtf8);
                string str = JsonTestHelper.ObjectToString(jsonValues);
                ReadOnlySpan<char> expectedSpan = expectedStr.AsSpan(0, expectedStr.Length - 2);
                ReadOnlySpan<char> actualSpan = str.AsSpan(0, str.Length - 2);
                Assert.True(expectedSpan.SequenceEqual(actualSpan));
            }

            result = JsonTestHelper.ReturnBytesHelper(dataUtf8, out length, JsonCommentHandling.SkipComments);
            actualStr = Encoding.UTF8.GetString(result.AsSpan(0, length));

            Assert.Equal(expectedStr, actualStr);

            result = JsonTestHelper.ReturnBytesHelper(dataUtf8, out length, JsonCommentHandling.AllowComments);
            actualStr = Encoding.UTF8.GetString(result.AsSpan(0, length));

            Assert.Equal(expectedStr, actualStr);
        }

        [Theory]
        [MemberData(nameof(TestCases))]
        public static void TestPartialJsonReader(bool compactData, TestCaseType type, string jsonString)
        {
            // Skipping really large JSON since slicing them (O(n^2)) is too slow.
            if (type == TestCaseType.Json40KB || type == TestCaseType.Json400KB || type == TestCaseType.ProjectLockJson)
            {
                return;
            }

            // Remove all formatting/indendation
            if (compactData)
            {
                using (JsonTextReader jsonReader = new JsonTextReader(new StringReader(jsonString)))
                {
                    jsonReader.FloatParseHandling = FloatParseHandling.Decimal;
                    JToken jtoken = JToken.ReadFrom(jsonReader);
                    var stringWriter = new StringWriter();
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter))
                    {
                        jtoken.WriteTo(jsonWriter);
                        jsonString = stringWriter.ToString();
                    }
                }
            }

            byte[] dataUtf8 = Encoding.UTF8.GetBytes(jsonString);

            byte[] result = JsonTestHelper.ReturnBytesHelper(dataUtf8, out int outputLength);
            Span<byte> outputSpan = new byte[outputLength];

            Stream stream = new MemoryStream(dataUtf8);
            TextReader reader = new StreamReader(stream, Encoding.UTF8, false, 1024, true);
            string expectedStr = JsonTestHelper.NewtonsoftReturnStringHelper(reader);

            for (int i = 0; i < dataUtf8.Length; i++)
            {
                JsonReaderState state = default;
                var json = new Utf8JsonReader(dataUtf8.AsSpan(0, i), isFinalBlock: false, state);
                byte[] output = JsonTestHelper.ReaderLoop(outputSpan.Length, out int firstLength, ref json);
                output.AsSpan(0, firstLength).CopyTo(outputSpan);
                int written = firstLength;

                long consumed = json.BytesConsumed;
                Assert.Equal(consumed, json.CurrentState.BytesConsumed);
                JsonReaderState jsonState = json.CurrentState;

                // Skipping large JSON since slicing them (O(n^3)) is too slow.
                if (type == TestCaseType.DeepTree || type == TestCaseType.BroadTree || type == TestCaseType.LotsOfNumbers
                    || type == TestCaseType.LotsOfStrings || type == TestCaseType.Json4KB)
                {
                    json = new Utf8JsonReader(dataUtf8.AsSpan((int)consumed), isFinalBlock: true, jsonState);
                    output = JsonTestHelper.ReaderLoop(outputSpan.Length - written, out int length, ref json);
                    output.AsSpan(0, length).CopyTo(outputSpan.Slice(written));
                    written += length;
                    Assert.Equal(dataUtf8.Length - consumed, json.BytesConsumed);
                    Assert.Equal(json.BytesConsumed, json.CurrentState.BytesConsumed);

                    Assert.Equal(outputSpan.Length, written);
                    string actualStr = Encoding.UTF8.GetString(outputSpan);
                    Assert.Equal(expectedStr, actualStr);
                }
                else
                {
                    for (long j = consumed; j < dataUtf8.Length - consumed; j++)
                    {
                        written = firstLength;
                        json = new Utf8JsonReader(dataUtf8.AsSpan((int)consumed, (int)j), isFinalBlock: false, jsonState);
                        output = JsonTestHelper.ReaderLoop(outputSpan.Length - written, out int length, ref json);
                        output.AsSpan(0, length).CopyTo(outputSpan.Slice(written));
                        written += length;

                        long consumedInner = json.BytesConsumed;
                        Assert.Equal(consumedInner, json.CurrentState.BytesConsumed);
                        json = new Utf8JsonReader(dataUtf8.AsSpan((int)(consumed + consumedInner)), isFinalBlock: true, json.CurrentState);
                        output = JsonTestHelper.ReaderLoop(outputSpan.Length - written, out length, ref json);
                        output.AsSpan(0, length).CopyTo(outputSpan.Slice(written));
                        written += length;
                        Assert.Equal(dataUtf8.Length - consumedInner - consumed, json.BytesConsumed);
                        Assert.Equal(json.BytesConsumed, json.CurrentState.BytesConsumed);

                        Assert.Equal(outputSpan.Length, written);
                        string actualStr = Encoding.UTF8.GetString(outputSpan);
                        Assert.Equal(expectedStr, actualStr);
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(SpecialNumTestCases))]
        public static void TestPartialJsonReaderSpecialNumbers(TestCaseType type, string jsonString)
        {
            byte[] dataUtf8 = Encoding.UTF8.GetBytes(jsonString);

            foreach (JsonCommentHandling commentHandling in Enum.GetValues(typeof(JsonCommentHandling)))
            {
                for (int i = 0; i < dataUtf8.Length; i++)
                {
                    var state = new JsonReaderState(options: new JsonReaderOptions { CommentHandling = commentHandling });
                    var json = new Utf8JsonReader(dataUtf8.AsSpan(0, i), isFinalBlock: false, state);
                    while (json.Read())
                        ;

                    long consumed = json.BytesConsumed;
                    Assert.Equal(consumed, json.CurrentState.BytesConsumed);

                    for (long j = consumed; j < dataUtf8.Length - consumed; j++)
                    {
                        // Need to re-initialize the state and reader to avoid using the previous state stack.
                        state = new JsonReaderState(options: new JsonReaderOptions { CommentHandling = commentHandling });
                        json = new Utf8JsonReader(dataUtf8.AsSpan(0, i), isFinalBlock: false, state);
                        while (json.Read())
                            ;

                        JsonReaderState jsonState = json.CurrentState;

                        json = new Utf8JsonReader(dataUtf8.AsSpan((int)consumed, (int)j), isFinalBlock: false, jsonState);
                        while (json.Read())
                            ;

                        long consumedInner = json.BytesConsumed;
                        Assert.Equal(consumedInner, json.CurrentState.BytesConsumed);
                        json = new Utf8JsonReader(dataUtf8.AsSpan((int)(consumed + consumedInner)), isFinalBlock: true, json.CurrentState);
                        while (json.Read())
                            ;
                        Assert.Equal(dataUtf8.Length - consumedInner - consumed, json.BytesConsumed);
                        Assert.Equal(json.BytesConsumed, json.CurrentState.BytesConsumed);
                    }
                }
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(62)]
        [InlineData(63)]
        [InlineData(64)]
        [InlineData(65)]
        [InlineData(66)]
        [InlineData(128)]
        [InlineData(256)]
        [InlineData(512)]
        public static void TestDepth(int depth)
        {
            foreach (JsonCommentHandling commentHandling in Enum.GetValues(typeof(JsonCommentHandling)))
            {
                for (int i = 0; i < depth; i++)
                {
                    string jsonStr = JsonTestHelper.WriteDepthObject(i, commentHandling == JsonCommentHandling.AllowComments);
                    Span<byte> data = Encoding.UTF8.GetBytes(jsonStr);

                    var state = new JsonReaderState(maxDepth: depth, options: new JsonReaderOptions { CommentHandling = commentHandling });
                    var json = new Utf8JsonReader(data, isFinalBlock: true, state);

                    int actualDepth = 0;
                    while (json.Read())
                    {
                        if (json.TokenType >= JsonTokenType.String && json.TokenType <= JsonTokenType.Null)
                            actualDepth = json.CurrentDepth;
                    }

                    int expectedDepth = 0;
                    var newtonJson = new JsonTextReader(new StringReader(jsonStr))
                    {
                        MaxDepth = depth
                    };
                    while (newtonJson.Read())
                    {
                        if (newtonJson.TokenType == JsonToken.String)
                        {
                            expectedDepth = newtonJson.Depth;
                        }
                    }

                    Assert.Equal(expectedDepth, actualDepth);
                    Assert.Equal(i + 1, actualDepth);
                }
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(62)]
        [InlineData(63)]
        [InlineData(64)]
        [InlineData(65)]
        [InlineData(66)]
        [InlineData(128)]
        [InlineData(256)]
        [InlineData(512)]
        public static void TestDepthWithObjectArrayMismatch(int depth)
        {
            foreach (JsonCommentHandling commentHandling in Enum.GetValues(typeof(JsonCommentHandling)))
            {
                for (int i = 0; i < depth; i++)
                {
                    string jsonStr = JsonTestHelper.WriteDepthObjectWithArray(i, commentHandling == JsonCommentHandling.AllowComments);
                    Span<byte> data = Encoding.UTF8.GetBytes(jsonStr);

                    var state = new JsonReaderState(maxDepth: depth + 1, options: new JsonReaderOptions { CommentHandling = commentHandling });
                    var json = new Utf8JsonReader(data, isFinalBlock: true, state);

                    int actualDepth = 0;
                    while (json.Read())
                    {
                        if (json.TokenType >= JsonTokenType.String && json.TokenType <= JsonTokenType.Null)
                            actualDepth = json.CurrentDepth;
                    }

                    int expectedDepth = 0;
                    var newtonJson = new JsonTextReader(new StringReader(jsonStr))
                    {
                        MaxDepth = depth + 1
                    };
                    while (newtonJson.Read())
                    {
                        if (newtonJson.TokenType == JsonToken.String)
                        {
                            expectedDepth = newtonJson.Depth;
                        }
                    }

                    Assert.Equal(expectedDepth, actualDepth);
                    Assert.Equal(i + 2, actualDepth);
                }
            }
        }

        [Theory]
        [InlineData("[123, 456]", "123456", "123456")]
        [InlineData("/*a*/[{\"testA\":[{\"testB\":[{\"testC\":123}]}]}]", "testAtestBtestC123", "atestAtestBtestC123")]
        [InlineData("{\"testA\":[1/*hi*//*bye*/, 2, 3], \"testB\": 4}", "testA123testB4", "testA1hibye23testB4")]
        [InlineData("{\"test\":[[[123,456]]]}", "test123456", "test123456")]
        [InlineData("/*a*//*z*/[/*b*//*z*/123/*c*//*z*/,/*d*//*z*/456/*e*//*z*/]/*f*//*z*/", "123456", "azbz123czdz456ezfz")]
        [InlineData("[123,/*hi*/456/*bye*/]", "123456", "123hi456bye")]
        [InlineData("/*a*//*z*/{/*b*//*z*/\"test\":/*c*//*z*/[/*d*//*z*/[/*e*//*z*/[/*f*//*z*/123/*g*//*z*/,/*h*//*z*/456/*i*//*z*/]/*j*//*z*/]/*k*//*z*/]/*l*//*z*/}/*m*//*z*/",
    "test123456", "azbztestczdzezfz123gzhz456izjzkzlzmz")]
        [InlineData("//a\n//z\n{//b\n//z\n\"test\"://c\n//z\n[//d\n//z\n[//e\n//z\n[//f\n//z\n123//g\n//z\n,//h\n//z\n456//i\n//z\n]//j\n//z\n]//k\n//z\n]//l\n//z\n}//m\n//z\n",
    "test123456", "azbztestczdzezfz123gzhz456izjzkzlzmz")]
        public static void AllowCommentStackMismatch(string jsonString, string expectedWithoutComments, string expectedWithComments)
        {
            byte[] data = Encoding.UTF8.GetBytes(jsonString);

            var builder = new StringBuilder();
            using (var newtonJson = new JsonTextReader(new StringReader(jsonString)))
            {
                while (newtonJson.Read())
                {
                    if (newtonJson.TokenType == JsonToken.Integer || newtonJson.TokenType == JsonToken.Comment || newtonJson.TokenType == JsonToken.PropertyName)
                        builder.Append(newtonJson.Value.ToString());
                }
            }
            Assert.Equal(expectedWithComments, builder.ToString());

            var state = new JsonReaderState(options: new JsonReaderOptions { CommentHandling = JsonCommentHandling.AllowComments });
            var json = new Utf8JsonReader(data, isFinalBlock: true, state);

            builder = new StringBuilder();
            while (json.Read())
            {
                if (json.TokenType == JsonTokenType.Number || json.TokenType == JsonTokenType.Comment || json.TokenType == JsonTokenType.PropertyName)
                    builder.Append(Encoding.UTF8.GetString(json.ValueSpan));
            }

            Assert.Equal(expectedWithComments, builder.ToString());

            state = new JsonReaderState(options: new JsonReaderOptions { CommentHandling = JsonCommentHandling.SkipComments });
            json = new Utf8JsonReader(data, isFinalBlock: true, state);

            builder = new StringBuilder();
            while (json.Read())
            {
                if (json.TokenType == JsonTokenType.Number || json.TokenType == JsonTokenType.Comment || json.TokenType == JsonTokenType.PropertyName)
                    builder.Append(Encoding.UTF8.GetString(json.ValueSpan));
            }

            Assert.Equal(expectedWithoutComments, builder.ToString());
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(62)]
        [InlineData(63)]
        [InlineData(64)]
        [InlineData(65)]
        [InlineData(66)]
        [InlineData(128)]
        [InlineData(256)]
        [InlineData(512)]
        public static void TestDepthBeyondLimit(int depth)
        {
            string jsonStr = JsonTestHelper.WriteDepthObject(depth - 1);
            Span<byte> data = Encoding.UTF8.GetBytes(jsonStr);

            var state = new JsonReaderState(maxDepth: depth - 1);
            var json = new Utf8JsonReader(data, isFinalBlock: true, state);

            try
            {
                int maxDepth = 0;
                while (json.Read())
                {
                    if (maxDepth < json.CurrentDepth)
                        maxDepth = json.CurrentDepth;
                }
                Assert.True(false, $"Expected JsonReaderException was not thrown. Max depth allowed = {json.CurrentState.MaxDepth} | Max depth reached = {maxDepth}");
            }
            catch (JsonReaderException)
            { }

            jsonStr = JsonTestHelper.WriteDepthArray(depth - 1);
            data = Encoding.UTF8.GetBytes(jsonStr);

            state = new JsonReaderState(maxDepth: depth - 1);
            json = new Utf8JsonReader(data, isFinalBlock: true, state);

            try
            {
                int maxDepth = 0;
                while (json.Read())
                {
                    if (maxDepth < json.CurrentDepth)
                        maxDepth = json.CurrentDepth;
                }
                Assert.True(false, $"Expected JsonReaderException was not thrown. Max depth allowed = {json.CurrentState.MaxDepth} | Max depth reached = {maxDepth}");
            }
            catch (JsonReaderException)
            { }
        }

        [Theory]
        [InlineData("{\"nam\\\"e\":\"ah\\\"son\"}", JsonCommentHandling.Default, "nam\\\"e, ah\\\"son, ")]
        [InlineData("{\"Here is a string: \\\"\\\"\":\"Here is a\",\"Here is a back slash\\\\\":[\"Multiline\\r\\n String\\r\\n\",\"\\tMul\\r\\ntiline String\",\"\\\"somequote\\\"\\tMu\\\"\\\"l\\r\\ntiline\\\"another\\\" String\\\\\"],\"str\":\"\\\"\\\"\"}",
            JsonCommentHandling.Default,
            "Here is a string: \\\"\\\", Here is a, Here is a back slash\\\\, Multiline\\r\\n String\\r\\n, \\tMul\\r\\ntiline String, \\\"somequote\\\"\\tMu\\\"\\\"l\\r\\ntiline\\\"another\\\" String\\\\, str, \\\"\\\", ")]

        [InlineData("{\"nam\\\"e\":\"ah\\\"son\"}", JsonCommentHandling.AllowComments, "nam\\\"e, ah\\\"son, ")]
        [InlineData("{\"Here is a string: \\\"\\\"\":\"Here is a\",\"Here is a back slash\\\\\":[\"Multiline\\r\\n String\\r\\n\",\"\\tMul\\r\\ntiline String\",\"\\\"somequote\\\"\\tMu\\\"\\\"l\\r\\ntiline\\\"another\\\" String\\\\\"],\"str\":\"\\\"\\\"\"}",
            JsonCommentHandling.AllowComments,
            "Here is a string: \\\"\\\", Here is a, Here is a back slash\\\\, Multiline\\r\\n String\\r\\n, \\tMul\\r\\ntiline String, \\\"somequote\\\"\\tMu\\\"\\\"l\\r\\ntiline\\\"another\\\" String\\\\, str, \\\"\\\", ")]

        [InlineData("{\"nam\\\"e\":\"ah\\\"son\"}", JsonCommentHandling.SkipComments, "nam\\\"e, ah\\\"son, ")]
        [InlineData("{\"Here is a string: \\\"\\\"\":\"Here is a\",\"Here is a back slash\\\\\":[\"Multiline\\r\\n String\\r\\n\",\"\\tMul\\r\\ntiline String\",\"\\\"somequote\\\"\\tMu\\\"\\\"l\\r\\ntiline\\\"another\\\" String\\\\\"],\"str\":\"\\\"\\\"\"}",
            JsonCommentHandling.SkipComments,
            "Here is a string: \\\"\\\", Here is a, Here is a back slash\\\\, Multiline\\r\\n String\\r\\n, \\tMul\\r\\ntiline String, \\\"somequote\\\"\\tMu\\\"\\\"l\\r\\ntiline\\\"another\\\" String\\\\, str, \\\"\\\", ")]
        public static void TestJsonReaderUtf8SpecialString(string jsonString, JsonCommentHandling commentHandling, string expectedStr)
        {
            byte[] dataUtf8 = Encoding.UTF8.GetBytes(jsonString);
            byte[] result = JsonTestHelper.ReturnBytesHelper(dataUtf8, out int length, commentHandling);
            string actualStr = Encoding.UTF8.GetString(result.AsSpan(0, length));

            Assert.Equal(expectedStr, actualStr);

            object jsonValues = JsonTestHelper.ReturnObjectHelper(dataUtf8, commentHandling);
            string str = JsonTestHelper.ObjectToString(jsonValues);
            ReadOnlySpan<char> expectedSpan = expectedStr.AsSpan(0, expectedStr.Length - 2);
            ReadOnlySpan<char> actualSpan = str.AsSpan(0, str.Length - 2);
            Assert.True(expectedSpan.SequenceEqual(actualSpan));
        }

        [Theory]
        [InlineData("  \"h漢字ello\"  ")]
        [InlineData("  \"he\\r\\n\\\"l\\\\\\\"lo\\\\\"  ")]
        [InlineData("  12345  ")]
        [InlineData("  null  ")]
        [InlineData("  true  ")]
        [InlineData("  false  ")]
        public static void SingleJsonValue(string jsonString)
        {
            byte[] dataUtf8 = Encoding.UTF8.GetBytes(jsonString);

            foreach (JsonCommentHandling commentHandling in Enum.GetValues(typeof(JsonCommentHandling)))
            {
                for (int i = 0; i < dataUtf8.Length; i++)
                {
                    var state = new JsonReaderState(options: new JsonReaderOptions { CommentHandling = commentHandling });
                    var json = new Utf8JsonReader(dataUtf8.AsSpan(0, i), false, state);
                    while (json.Read())
                        ;

                    long consumed = json.BytesConsumed;
                    Assert.Equal(consumed, json.CurrentState.BytesConsumed);
                    json = new Utf8JsonReader(dataUtf8.AsSpan((int)consumed), true, json.CurrentState);
                    while (json.Read())
                        ;
                    Assert.Equal(dataUtf8.Length - consumed, json.BytesConsumed);
                    Assert.Equal(json.BytesConsumed, json.CurrentState.BytesConsumed);
                }
            }
        }

        [Theory]
        [InlineData("\"h漢字ello\"", 1, 0)] // "\""
        [InlineData("12345", 3, 0)]   // "123"
        [InlineData("null", 3, 0)]   // "nul"
        [InlineData("true", 3, 0)]   // "tru"
        [InlineData("false", 4, 0)]  // "fals"
        [InlineData("   {\"a漢字ge\":30}   ", 16, 16)] // "   {\"a漢字ge\":"
        [InlineData("{\"n漢字ame\":\"A漢字hson\"}", 15, 14)]  // "{\"n漢字ame\":\"A漢字hso"
        [InlineData("-123456789", 1, 0)] // "-"
        [InlineData("0.5", 2, 0)]    // "0."
        [InlineData("10.5e+3", 5, 0)] // "10.5e"
        [InlineData("10.5e-1", 6, 0)]    // "10.5e-"
        [InlineData("{\"i漢字nts\":[1, 2, 3, 4, 5]}", 27, 25)]    // "{\"i漢字nts\":[1, 2, 3, 4, "
        [InlineData("{\"s漢字trings\":[\"a漢字bc\", \"def\"], \"ints\":[1, 2, 3, 4, 5]}", 36, 36)]  // "{\"s漢字trings\":[\"a漢字bc\", \"def\""
        [InlineData("{\"a漢字ge\":30, \"name\":\"test}:[]\", \"another 漢字string\" : \"tests\"}", 25, 24)]   // "{\"a漢字ge\":30, \"name\":\"test}"
        [InlineData("   [[[[{\r\n\"t漢字emp1\":[[[[{\"t漢字emp2:[]}]]]]}]]]]\":[]}]]]]}]]]]   ", 54, 29)] // "   [[[[{\r\n\"t漢字emp1\":[[[[{\"t漢字emp2:[]}]]]]}]]]]"
        [InlineData("{\r\n\"is漢字Active\": false, \"in漢字valid\"\r\n : \"now its 漢字valid\"}", 26, 26)]  // "{\r\n\"is漢字Active\": false, \"in漢字valid\"\r\n}"
        [InlineData("{\"property\\u1234Name\": \"String value with hex: \\uABCD in the middle.\"}", 51, 23)]  // "{\"property\\u1234Name\": \"String value with hex: \\uAB"
        [InlineData("{ \"number\": 0}", 13, 12)]    // "{ \"number\": 0"
        public static void PartialJson(string jsonString, int splitLocation, int consumed)
        {
            byte[] dataUtf8 = Encoding.UTF8.GetBytes(jsonString);

            foreach (JsonCommentHandling commentHandling in Enum.GetValues(typeof(JsonCommentHandling)))
            {
                var state = new JsonReaderState(options: new JsonReaderOptions { CommentHandling = commentHandling });
                var json = new Utf8JsonReader(dataUtf8.AsSpan(0, splitLocation), false, state);
                while (json.Read())
                    ;
                Assert.Equal(consumed, json.BytesConsumed);
                Assert.Equal(consumed, json.CurrentState.BytesConsumed);

                json = new Utf8JsonReader(dataUtf8.AsSpan((int)json.BytesConsumed), true, json.CurrentState);
                while (json.Read())
                    ;
                Assert.Equal(dataUtf8.Length - consumed, json.BytesConsumed);
                Assert.Equal(json.BytesConsumed, json.CurrentState.BytesConsumed);
            }
        }

        [Theory]
        [InlineData("{\r\n\"is\\r\\nAct漢字ive\": false \"in漢字valid\"\r\n}", 30, 30, 2, 21)]
        [InlineData("{\r\n\"is\\r\\nAct漢字ive\": false \"in漢字valid\"\r\n}", 31, 31, 2, 21)]
        [InlineData("{\r\n\"is\\r\\nAct漢字ive\": false, \"in漢字valid\"\r\n}", 30, 30, 3, 0)]
        [InlineData("{\r\n\"is\\r\\nAct漢字ive\": false, \"in漢字valid\"\r\n}", 31, 30, 3, 0)]
        [InlineData("{\r\n\"is\\r\\nAct漢字ive\": false, \"in漢字valid\"\r\n}", 32, 30, 3, 0)]
        [InlineData("{\r\n\"is\\r\\nAct漢字ive\": false, 5\r\n}", 30, 30, 2, 22)]
        [InlineData("{\r\n\"is\\r\\nAct漢字ive\": false, 5\r\n}", 31, 30, 2, 22)]
        [InlineData("{\r\n\"is\\r\\nAct漢字ive\": false, 5\r\n}", 32, 30, 2, 22)]
        public static void InvalidJsonSplitRemainsInvalid(string jsonString, int splitLocation, int consumed, int expectedlineNumber, int expectedBytePosition)
        {
            foreach (JsonCommentHandling commentHandling in Enum.GetValues(typeof(JsonCommentHandling)))
            {
                var state = new JsonReaderState(options: new JsonReaderOptions { CommentHandling = commentHandling });
                byte[] dataUtf8 = Encoding.UTF8.GetBytes(jsonString);
                var json = new Utf8JsonReader(dataUtf8.AsSpan(0, splitLocation), false, state);
                while (json.Read())
                    ;
                Assert.Equal(consumed, json.BytesConsumed);
                Assert.Equal(consumed, json.CurrentState.BytesConsumed);

                json = new Utf8JsonReader(dataUtf8.AsSpan((int)json.BytesConsumed), true, json.CurrentState);
                try
                {
                    while (json.Read())
                        ;
                    Assert.True(false, "Expected JsonReaderException was not thrown.");
                }
                catch (JsonReaderException ex)
                {
                    Assert.Equal(expectedlineNumber, ex.LineNumber);
                    Assert.Equal(expectedBytePosition, ex.BytePositionInLine);
                }
            }
        }

        [Theory]
        [InlineData("{]", 0, 1)]
        [InlineData("[}", 0, 1)]
        [InlineData("nulz", 0, 3)]
        [InlineData("truz", 0, 3)]
        [InlineData("falsz", 0, 4)]
        [InlineData("\"a漢字ge\":", 0, 11)]
        [InlineData("12345.1.", 0, 7)]
        [InlineData("-f", 0, 1)]
        [InlineData("1.f", 0, 2)]
        [InlineData("0.1f", 0, 3)]
        [InlineData("0.1e1f", 0, 5)]
        [InlineData("123,", 0, 3)]
        [InlineData("01", 0, 1)]
        [InlineData("-01", 0, 2)]
        [InlineData("10.5e-0.2", 0, 7)]
        [InlineData("{\"a漢字ge\":30, \"ints\":[1, 2, 3, 4, 5.1e7.3]}", 0, 42)]
        [InlineData("{\"a漢字ge\":30, \r\n \"num\":-0.e, \r\n \"ints\":[1, 2, 3, 4, 5]}", 1, 10)]
        [InlineData("{{}}", 0, 1)]
        [InlineData("[[{{}}]]", 0, 3)]
        [InlineData("[1, 2, 3, ]", 0, 10)]
        [InlineData("{\"a漢字ge\":30, \"ints\":[1, 2, 3, 4, 5}}", 0, 38)]
        [InlineData("{\r\n\"isActive\": false \"\r\n}", 1, 18)]
        [InlineData("[[[[{\r\n\"t漢字emp1\":[[[[{\"temp2\":[}]]]]}]]]]", 1, 28)]
        [InlineData("[[[[{\r\n\"t漢字emp1\":[[[[{\"temp2\":[]},[}]]]]}]]]]", 1, 32)]
        [InlineData("{\r\n\t\"isActive\": false,\r\n\t\"array\": [\r\n\t\t[{\r\n\t\t\t\"id\": 1\r\n\t\t}]\r\n\t]\r\n}", 3, 3, 3)]
        [InlineData("{\"Here is a 漢字string: \\\"\\\"\":\"Here is 漢字a\",\"Here is a back slash\\\\\":[\"Multiline\\r\\n String\\r\\n\",\"\\tMul\\r\\ntiline String\",\"\\\"somequote\\\"\\tMu\\\"\\\"l\\r\\ntiline\\\"another\\\" String\\\\\"],\"str:\"\\\"\\\"\"}", 4, 35)]
        public static void InvalidJsonWhenPartial(string jsonString, int expectedlineNumber, int expectedBytePosition, int maxDepth = 64)
        {
            byte[] dataUtf8 = Encoding.UTF8.GetBytes(jsonString);

            foreach (JsonCommentHandling commentHandling in Enum.GetValues(typeof(JsonCommentHandling)))
            {
                var state = new JsonReaderState(maxDepth: maxDepth, options: new JsonReaderOptions { CommentHandling = commentHandling });
                var json = new Utf8JsonReader(dataUtf8, false, state);

                try
                {
                    while (json.Read())
                        ;
                    Assert.True(false, "Expected JsonReaderException was not thrown.");
                }
                catch (JsonReaderException ex)
                {
                    Assert.Equal(expectedlineNumber, ex.LineNumber);
                    Assert.Equal(expectedBytePosition, ex.BytePositionInLine);
                }
            }
        }

        [Theory]
        [InlineData("{\"text\": \"๏ แผ่นดินฮั่นเสื่อมโทรมแสนสังเวช\\uABCZ พระปกเกศกองบู๊กู้ขึ้นใหม่\"}", 0, 109)]
        [InlineData("{\"text\": \"๏ แผ่นดินฮั่นเสื่อมโ\\nทรมแสนสังเวช\\uABCZ พระปกเกศกองบู๊กู้ขึ้นใหม่\"}", 1, 41)]
        public static void PositionInCodeUnits(string jsonString, int expectedlineNumber, int expectedBytePosition)
        {
            byte[] dataUtf8 = Encoding.UTF8.GetBytes(jsonString);

            foreach (JsonCommentHandling commentHandling in Enum.GetValues(typeof(JsonCommentHandling)))
            {
                var state = new JsonReaderState(options: new JsonReaderOptions { CommentHandling = commentHandling });
                var json = new Utf8JsonReader(dataUtf8, false, state);

                try
                {
                    while (json.Read())
                        ;
                    Assert.True(false, "Expected JsonReaderException was not thrown.");
                }
                catch (JsonReaderException ex)
                {
                    Assert.Equal(expectedlineNumber, ex.LineNumber);
                    Assert.Equal(expectedBytePosition, ex.BytePositionInLine);
                }
            }
        }

        [Theory]
        [InlineData("\"", 0, 0)]
        [InlineData("{]", 0, 1)]
        [InlineData("[}", 0, 1)]
        [InlineData("nul", 0, 3)]
        [InlineData("tru", 0, 3)]
        [InlineData("fals", 0, 4)]
        [InlineData("\"a漢字ge\":", 0, 11)]
        [InlineData("{\"a漢字ge\":", 0, 13)]
        [InlineData("{\"name\":\"A漢字hso", 0, 8)]
        [InlineData("12345.1.", 0, 7)]
        [InlineData("-", 0, 1)]
        [InlineData("-f", 0, 1)]
        [InlineData("1.f", 0, 2)]
        [InlineData("0.", 0, 2)]
        [InlineData("0.1f", 0, 3)]
        [InlineData("0.1e1f", 0, 5)]
        [InlineData("123,", 0, 3)]
        [InlineData("false,", 0, 5)]
        [InlineData("true,", 0, 4)]
        [InlineData("null,", 0, 4)]
        [InlineData("\"h漢字ello\",", 0, 13)]
        [InlineData("\"\\u12z3\"", 0, 5)]
        [InlineData("\"\\u12]3\"", 0, 5)]
        [InlineData("\"\\u12=3\"", 0, 5)]
        [InlineData("\"\\u12$3\"", 0, 5)]
        [InlineData("\"\\u12\"", 0, 5)]
        [InlineData("\"\\u120\"", 0, 6)]
        [InlineData("01", 0, 1)]
        [InlineData("1a", 0, 1)]
        [InlineData("-01", 0, 2)]
        [InlineData("10.5e", 0, 5)]
        [InlineData("10.5e-", 0, 6)]
        [InlineData("10.5e-0.2", 0, 7)]
        [InlineData("{\"age\":30, \"ints\":[1, 2, 3, 4, 5.1e7.3]}", 0, 36)]
        [InlineData("{\"age\":30, \r\n \"num\":-0.e, \r\n \"ints\":[1, 2, 3, 4, 5]}", 1, 10)]
        [InlineData("{ \"number\": 00", 0, 13)]
        [InlineData("{{}}", 0, 1)]
        [InlineData("[[]", 0, 3)]
        [InlineData("[[{{}}]]", 0, 3)]
        [InlineData("[1, 2, 3, ]", 0, 10)]
        [InlineData("{\"ints\":[1, 2, 3, 4, 5", 0, 22)]
        [InlineData("{\"s漢字trings\":[\"a漢字bc\", \"def\"", 0, 36)]
        [InlineData("{\"age\":30, \"ints\":[1, 2, 3, 4, 5}}", 0, 32)]
        [InlineData("{\"age\":30, \"name\":\"test}", 0, 18)]
        [InlineData("{\r\n\"isActive\": false \"\r\n}", 1, 18)]
        [InlineData("[[[[{\r\n\"t漢字emp1\":[[[[{\"temp2\":[}]]]]}]]]]", 1, 28)]
        [InlineData("[[[[{\r\n\"t漢字emp1\":[[[[{\"temp2:[]}]]]]}]]]]", 1, 19)]
        [InlineData("[[[[{\r\n\"t漢字emp1\":[[[[{\"temp2\":[]},[}]]]]}]]]]", 1, 32)]
        [InlineData("{\r\n\t\"isActive\": false,\r\n\t\"array\": [\r\n\t\t[{\r\n\t\t\t\"id\": 1\r\n\t\t}]\r\n\t]\r\n}", 3, 3, 3)]
        [InlineData("{\"Here is a 漢字string: \\\"\\\"\":\"Here is 漢字a\",\"Here is a back slash\\\\\":[\"Multiline\\r\\n String\\r\\n\",\"\\tMul\\r\\ntiline String\",\"\\\"somequote\\\"\\tMu\\\"\\\"l\\r\\ntiline\\\"another\\\" String\\\\\"],\"str:\"\\\"\\\"\"}", 4, 35)]
        [InlineData("\"hel\rlo\"", 0, 4)]
        [InlineData("\"hel\nlo\"", 0, 4)]
        [InlineData("\"hel\\uABCXlo\"", 0, 9)]
        [InlineData("\"hel\\\tlo\"", 0, 5)]
        [InlineData("\"hel\rlo\\\"\"", 0, 4)]
        [InlineData("\"hel\nlo\\\"\"", 0, 4)]
        [InlineData("\"hel\\uABCXlo\\\"\"", 0, 9)]
        [InlineData("\"hel\\\tlo\\\"\"", 0, 5)]
        [InlineData("\"he\\nl\rlo\\\"\"", 1, 1)]
        [InlineData("\"he\\nl\nlo\\\"\"", 1, 1)]
        [InlineData("\"he\\nl\\uABCXlo\\\"\"", 1, 6)]
        [InlineData("\"he\\nl\\\tlo\\\"\"", 1, 2)]
        [InlineData("\"he\\nl\rlo", 1, 1)]
        [InlineData("\"he\\nl\nlo", 1, 1)]
        [InlineData("\"he\\nl\\uABCXlo", 1, 6)]
        [InlineData("\"he\\nl\\\tlo", 1, 2)]
        public static void InvalidJson(string jsonString, int expectedlineNumber, int expectedBytePosition, int maxDepth = 64)
        {
            byte[] dataUtf8 = Encoding.UTF8.GetBytes(jsonString);

            foreach (JsonCommentHandling commentHandling in Enum.GetValues(typeof(JsonCommentHandling)))
            {
                var state = new JsonReaderState(maxDepth: maxDepth, options: new JsonReaderOptions { CommentHandling = commentHandling });
                var json = new Utf8JsonReader(dataUtf8, isFinalBlock: true, state);

                try
                {
                    while (json.Read())
                        ;
                    Assert.True(false, "Expected JsonReaderException was not thrown with single-segment data.");
                }
                catch (JsonReaderException ex)
                {
                    Assert.Equal(expectedlineNumber, ex.LineNumber);
                    Assert.Equal(expectedBytePosition, ex.BytePositionInLine);
                }
            }
        }

        [Theory]
        [InlineData("\"", 0, 0)]
        [InlineData("{]", 0, 1)]
        [InlineData("[}", 0, 1)]
        [InlineData("nul", 0, 3)]
        [InlineData("tru", 0, 3)]
        [InlineData("fals", 0, 4)]
        [InlineData("\"a漢字ge\":", 0, 11)]
        [InlineData("{\"a漢字ge\":", 0, 13)]
        [InlineData("{\"name\":\"A漢字hso", 0, 8)]
        [InlineData("12345.1.", 0, 7)]
        [InlineData("-", 0, 1)]
        [InlineData("-f", 0, 1)]
        [InlineData("1.f", 0, 2)]
        [InlineData("0.", 0, 2)]
        [InlineData("0.1f", 0, 3)]
        [InlineData("0.1e1f", 0, 5)]
        [InlineData("123,", 0, 3)]
        [InlineData("false,", 0, 5)]
        [InlineData("true,", 0, 4)]
        [InlineData("null,", 0, 4)]
        [InlineData("\"h漢字ello\",", 0, 13)]
        [InlineData("01", 0, 1)]
        [InlineData("1a", 0, 1)]
        [InlineData("-01", 0, 2)]
        [InlineData("10.5e", 0, 5)]
        [InlineData("10.5e-", 0, 6)]
        [InlineData("10.5e-0.2", 0, 7)]
        [InlineData("{\"age\":30, \"ints\":[1, 2, 3, 4, 5.1e7.3]}", 0, 36)]
        [InlineData("{\"age\":30, \r\n \"num\":-0.e, \r\n \"ints\":[1, 2, 3, 4, 5]}", 1, 10)]
        [InlineData("{{}}", 0, 1, 1)]
        [InlineData("[[{{}}]]", 0, 3)]
        [InlineData("[1, 2, 3, ]", 0, 10)]
        [InlineData("{\"ints\":[1, 2, 3, 4, 5", 0, 22)]
        [InlineData("{\"s漢字trings\":[\"a漢字bc\", \"def\"", 0, 36)]
        [InlineData("{\"age\":30, \"ints\":[1, 2, 3, 4, 5}}", 0, 32)]
        [InlineData("{\"age\":30, \"name\":\"test}", 0, 18)]
        [InlineData("{\r\n\"isActive\": false \"\r\n}", 1, 18)]
        [InlineData("[[[[{\r\n\"t漢字emp1\":[[[[{\"temp2\":[}]]]]}]]]]", 1, 28)]
        [InlineData("[[[[{\r\n\"t漢字emp1\":[[[[{\"temp2:[]}]]]]}]]]]", 1, 19)]
        [InlineData("[[[[{\r\n\"t漢字emp1\":[[[[{\"temp2\":[]},[}]]]]}]]]]", 1, 32)]
        [InlineData("{\r\n\t\"isActive\": false,\r\n\t\"array\": [\r\n\t\t[{\r\n\t\t\t\"id\": 1\r\n\t\t}]\r\n\t]\r\n}", 3, 3, 3)]
        [InlineData("{\"Here is a 漢字string: \\\"\\\"\":\"Here is 漢字a\",\"Here is a back slash\\\\\":[\"Multiline\\r\\n String\\r\\n\",\"\\tMul\\r\\ntiline String\",\"\\\"somequote\\\"\\tMu\\\"\\\"l\\r\\ntiline\\\"another\\\" String\\\\\"],\"str:\"\\\"\\\"\"}", 4, 35)]
        [InlineData("\"hel\rlo\"", 0, 4)]
        [InlineData("\"hel\nlo\"", 0, 4)]
        [InlineData("\"hel\\uABCXlo\"", 0, 9)]
        [InlineData("\"hel\\\tlo\"", 0, 5)]
        [InlineData("\"hel\rlo\\\"\"", 0, 4)]
        [InlineData("\"hel\nlo\\\"\"", 0, 4)]
        [InlineData("\"hel\\uABCXlo\\\"\"", 0, 9)]
        [InlineData("\"hel\\\tlo\\\"\"", 0, 5)]
        [InlineData("\"he\\nl\rlo\\\"\"", 1, 1)]
        [InlineData("\"he\\nl\nlo\\\"\"", 1, 1)]
        [InlineData("\"he\\nl\\uABCXlo\\\"\"", 1, 6)]
        [InlineData("\"he\\nl\\\tlo\\\"\"", 1, 2)]
        [InlineData("\"he\\nl\rlo", 1, 1)]
        [InlineData("\"he\\nl\nlo", 1, 1)]
        [InlineData("\"he\\nl\\uABCXlo", 1, 6)]
        [InlineData("\"he\\nl\\\tlo", 1, 2)]
        public static void InvalidJsonSingleSegment(string jsonString, int expectedlineNumber, int expectedBytePosition, int maxDepth = 64)
        {
            byte[] dataUtf8 = Encoding.UTF8.GetBytes(jsonString);

            foreach (JsonCommentHandling commentHandling in Enum.GetValues(typeof(JsonCommentHandling)))
            {
                var state = new JsonReaderState(maxDepth: maxDepth, options: new JsonReaderOptions { CommentHandling = commentHandling });
                var json = new Utf8JsonReader(dataUtf8, isFinalBlock: true, state);

                try
                {
                    while (json.Read())
                        ;
                    Assert.True(false, "Expected JsonReaderException was not thrown with single-segment data.");
                }
                catch (JsonReaderException ex)
                {
                    Assert.Equal(expectedlineNumber, ex.LineNumber);
                    Assert.Equal(expectedBytePosition, ex.BytePositionInLine);
                }

                for (int i = 0; i < dataUtf8.Length; i++)
                {
                    try
                    {
                        var stateInner = new JsonReaderState(maxDepth: maxDepth, options: new JsonReaderOptions { CommentHandling = commentHandling });
                        var jsonSlice = new Utf8JsonReader(dataUtf8.AsSpan(0, i), isFinalBlock: false, stateInner);
                        while (jsonSlice.Read())
                            ;

                        long consumed = jsonSlice.BytesConsumed;
                        Assert.Equal(consumed, jsonSlice.CurrentState.BytesConsumed);
                        JsonReaderState jsonState = jsonSlice.CurrentState;

                        jsonSlice = new Utf8JsonReader(dataUtf8.AsSpan((int)consumed), isFinalBlock: true, jsonState);
                        while (jsonSlice.Read())
                            ;

                        Assert.True(false, "Expected JsonReaderException was not thrown with multi-segment data.");
                    }
                    catch (JsonReaderException ex)
                    {
                        string errorMessage = $"expectedLineNumber: {expectedlineNumber} | actual: {ex.LineNumber} | index: {i} | option: {commentHandling}";
                        string firstSegmentString = Encoding.UTF8.GetString(dataUtf8.AsSpan(0, i));
                        string secondSegmentString = Encoding.UTF8.GetString(dataUtf8.AsSpan(i));
                        errorMessage += " | " + firstSegmentString + " | " + secondSegmentString;
                        Assert.True(expectedlineNumber == ex.LineNumber, errorMessage);
                        errorMessage = $"expectedBytePosition: {expectedBytePosition} | actual: {ex.BytePositionInLine} | index: {i} | option: {commentHandling}";
                        errorMessage += " | " + firstSegmentString + " | " + secondSegmentString;
                        Assert.True(expectedBytePosition == ex.BytePositionInLine, errorMessage);
                    }
                }
            }
        }

        [Theory]
        [InlineData("//", "", 2)]
        [InlineData("//\n", "", 3)]
        [InlineData("/**/", "", 4)]
        [InlineData("/*/*/", "/", 5)]

        [InlineData("//T漢字his is a 漢字comment before json\n\"hello\"", "T漢字his is a 漢字comment before json", 44)]
        [InlineData("\"h漢字ello\"//This is a 漢字comment after json", "This is a 漢字comment after json", 49)]
        [InlineData("\"h漢字ello\"//This is a 漢字comment after json\n", "This is a 漢字comment after json", 50)]
        [InlineData("\"a漢字lpha\" \r\n//This is a 漢字comment after json\n//Here is another comment/*and a multi-line comment*///Another single-line comment", "This is a 漢字comment after json", 53)]
        [InlineData("\"b漢字eta\" \r\n//This is a 漢字comment after json\n//Here is another comment/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", "This is a 漢字comment after json", 52)]
        [InlineData("\"g漢字amma\" \r\n//This is a 漢字comment after json\n//Here is another comment\n/*and a multi-line comment*///Another single-line comment", "This is a 漢字comment after json", 53)]
        [InlineData("\"d漢字elta\" \r\n//This is a 漢字comment after json\n//Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", "This is a 漢字comment after json", 53)]
        [InlineData("\"h漢字ello\"//This is a 漢字comment after json with new line\n", "This is a 漢字comment after json with new line", 64)]
        [InlineData("{\"a漢字ge\" : \n//This is a 漢字comment between key-value pairs\n 30}", "This is a 漢字comment between key-value pairs", 66)]
        [InlineData("{\"a漢字ge\" : 30//This is a 漢字comment between key-value pairs on the same line\n}", "This is a 漢字comment between key-value pairs on the same line", 84)]

        [InlineData("/*T漢字his is a multi-line 漢字comment before json*/\"hello\"", "T漢字his is a multi-line 漢字comment before json", 56)]
        [InlineData("\"h漢字ello\"/*This is a multi-line 漢字comment after json*/", "This is a multi-line 漢字comment after json", 62)]
        [InlineData("\"a漢字lpha\" \r\n/*This is a multi-line 漢字comment after json*///Here is another comment/*and a multi-line comment*///Another single-line comment", "This is a multi-line 漢字comment after json", 65)]
        [InlineData("\"b漢字eta\" \r\n/*This is a multi-line 漢字comment after json*///Here is another comment/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", "This is a multi-line 漢字comment after json", 64)]
        [InlineData("\"g漢字amma\" \r\n/*This is a multi-line 漢字comment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment", "This is a multi-line 漢字comment after json", 65)]
        [InlineData("\"d漢字elta\" \r\n/*This is a multi-line 漢字comment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", "This is a multi-line 漢字comment after json", 65)]
        [InlineData("{\"a漢字ge\" : \n/*This is a 漢字comment between key-value pairs*/ 30}", "This is a 漢字comment between key-value pairs", 67)]
        [InlineData("{\"a漢字ge\" : 30/*This is a 漢字comment between key-value pairs on the same line*/}", "This is a 漢字comment between key-value pairs on the same line", 85)]

        [InlineData("/*T漢字his is a split multi-line \n漢字comment before json*/\"hello\"", "T漢字his is a split multi-line \n漢字comment before json", 63)]
        [InlineData("\"h漢字ello\"/*This is a split multi-line \n漢字comment after json*/", "This is a split multi-line \n漢字comment after json", 69)]
        [InlineData("\"a漢字lpha\" \r\n/*This is a split multi-line \n漢字comment after json*///Here is another comment/*and a multi-line comment*///Another single-line comment", "This is a split multi-line \n漢字comment after json", 72)]
        [InlineData("\"b漢字eta\" \r\n/*This is a split multi-line \n漢字comment after json*///Here is another comment/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", "This is a split multi-line \n漢字comment after json", 71)]
        [InlineData("\"g漢字amma\" \r\n/*This is a split multi-line \n漢字comment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment", "This is a split multi-line \n漢字comment after json", 72)]
        [InlineData("\"d漢字elta\" \r\n/*This is a split multi-line \n漢字comment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", "This is a split multi-line \n漢字comment after json", 72)]
        [InlineData("{\"a漢字ge\" : \n/*This is a split multi-line \n漢字comment between key-value pairs*/ 30}", "This is a split multi-line \n漢字comment between key-value pairs", 85)]
        [InlineData("{\"a漢字ge\" : 30/*This is a split multi-line \n漢字comment between key-value pairs on the same line*/}", "This is a split multi-line \n漢字comment between key-value pairs on the same line", 103)]
        public static void AllowComments(string jsonString, string expectedComment, int expectedIndex)
        {
            byte[] dataUtf8 = Encoding.UTF8.GetBytes(jsonString);
            var state = new JsonReaderState(options: new JsonReaderOptions { CommentHandling = JsonCommentHandling.AllowComments });
            var json = new Utf8JsonReader(dataUtf8, isFinalBlock: true, state);

            bool foundComment = false;
            long indexAfterFirstComment = 0;
            while (json.Read())
            {
                JsonTokenType tokenType = json.TokenType;
                switch (tokenType)
                {
                    case JsonTokenType.Comment:
                        if (foundComment)
                            break;
                        foundComment = true;
                        indexAfterFirstComment = json.BytesConsumed;
                        Assert.Equal(indexAfterFirstComment, json.CurrentState.BytesConsumed);
                        Assert.True(json.TryGetValueAsString(out string actualComment));
                        Assert.Equal(expectedComment, actualComment);
                        break;
                }
            }
            Assert.True(foundComment);
            Assert.Equal(expectedIndex, indexAfterFirstComment);
        }

        [Theory]
        [InlineData("//", "", 2)]
        [InlineData("//\n", "", 3)]
        [InlineData("/**/", "", 4)]
        [InlineData("/*/*/", "/", 5)]

        [InlineData("//T漢字his is a 漢字comment before json\n\"hello\"", "T漢字his is a 漢字comment before json", 44)]
        [InlineData("\"h漢字ello\"//This is a 漢字comment after json", "This is a 漢字comment after json", 49)]
        [InlineData("\"h漢字ello\"//This is a 漢字comment after json\n", "This is a 漢字comment after json", 50)]
        [InlineData("\"a漢字lpha\" \r\n//This is a 漢字comment after json\n//Here is another comment/*and a multi-line comment*///Another single-line comment", "This is a 漢字comment after json", 53)]
        [InlineData("\"b漢字eta\" \r\n//This is a 漢字comment after json\n//Here is another comment/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", "This is a 漢字comment after json", 52)]
        [InlineData("\"g漢字amma\" \r\n//This is a 漢字comment after json\n//Here is another comment\n/*and a multi-line comment*///Another single-line comment", "This is a 漢字comment after json", 53)]
        [InlineData("\"d漢字elta\" \r\n//This is a 漢字comment after json\n//Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", "This is a 漢字comment after json", 53)]
        [InlineData("\"h漢字ello\"//This is a 漢字comment after json with new line\n", "This is a 漢字comment after json with new line", 64)]
        [InlineData("{\"a漢字ge\" : \n//This is a 漢字comment between key-value pairs\n 30}", "This is a 漢字comment between key-value pairs", 66)]
        [InlineData("{\"a漢字ge\" : 30//This is a 漢字comment between key-value pairs on the same line\n}", "This is a 漢字comment between key-value pairs on the same line", 84)]

        [InlineData("/*T漢字his is a multi-line 漢字comment before json*/\"hello\"", "T漢字his is a multi-line 漢字comment before json", 56)]
        [InlineData("\"h漢字ello\"/*This is a multi-line 漢字comment after json*/", "This is a multi-line 漢字comment after json", 62)]
        [InlineData("\"a漢字lpha\" \r\n/*This is a multi-line 漢字comment after json*///Here is another comment/*and a multi-line comment*///Another single-line comment", "This is a multi-line 漢字comment after json", 65)]
        [InlineData("\"b漢字eta\" \r\n/*This is a multi-line 漢字comment after json*///Here is another comment/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", "This is a multi-line 漢字comment after json", 64)]
        [InlineData("\"g漢字amma\" \r\n/*This is a multi-line 漢字comment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment", "This is a multi-line 漢字comment after json", 65)]
        [InlineData("\"d漢字elta\" \r\n/*This is a multi-line 漢字comment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", "This is a multi-line 漢字comment after json", 65)]
        [InlineData("{\"a漢字ge\" : \n/*This is a 漢字comment between key-value pairs*/ 30}", "This is a 漢字comment between key-value pairs", 67)]
        [InlineData("{\"a漢字ge\" : 30/*This is a 漢字comment between key-value pairs on the same line*/}", "This is a 漢字comment between key-value pairs on the same line", 85)]

        [InlineData("/*T漢字his is a split multi-line \n漢字comment before json*/\"hello\"", "T漢字his is a split multi-line \n漢字comment before json", 63)]
        [InlineData("\"h漢字ello\"/*This is a split multi-line \n漢字comment after json*/", "This is a split multi-line \n漢字comment after json", 69)]
        [InlineData("\"a漢字lpha\" \r\n/*This is a split multi-line \n漢字comment after json*///Here is another comment/*and a multi-line comment*///Another single-line comment", "This is a split multi-line \n漢字comment after json", 72)]
        [InlineData("\"b漢字eta\" \r\n/*This is a split multi-line \n漢字comment after json*///Here is another comment/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", "This is a split multi-line \n漢字comment after json", 71)]
        [InlineData("\"g漢字amma\" \r\n/*This is a split multi-line \n漢字comment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment", "This is a split multi-line \n漢字comment after json", 72)]
        [InlineData("\"d漢字elta\" \r\n/*This is a split multi-line \n漢字comment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", "This is a split multi-line \n漢字comment after json", 72)]
        [InlineData("{\"a漢字ge\" : \n/*This is a split multi-line \n漢字comment between key-value pairs*/ 30}", "This is a split multi-line \n漢字comment between key-value pairs", 85)]
        [InlineData("{\"a漢字ge\" : 30/*This is a split multi-line \n漢字comment between key-value pairs on the same line*/}", "This is a split multi-line \n漢字comment between key-value pairs on the same line", 103)]
        public static void AllowCommentsSingleSegment(string jsonString, string expectedComment, int expectedIndex)
        {
            byte[] dataUtf8 = Encoding.UTF8.GetBytes(jsonString);
            var state = new JsonReaderState(options: new JsonReaderOptions { CommentHandling = JsonCommentHandling.AllowComments });
            var json = new Utf8JsonReader(dataUtf8, isFinalBlock: true, state);

            bool foundComment = false;
            long indexAfterFirstComment = 0;
            while (json.Read())
            {
                JsonTokenType tokenType = json.TokenType;
                switch (tokenType)
                {
                    case JsonTokenType.Comment:
                        if (foundComment)
                            break;
                        foundComment = true;
                        indexAfterFirstComment = json.BytesConsumed;
                        Assert.Equal(indexAfterFirstComment, json.CurrentState.BytesConsumed);
                        Assert.True(json.TryGetValueAsString(out string actualComment));
                        Assert.Equal(expectedComment, actualComment);
                        break;
                }
            }
            Assert.True(foundComment);
            Assert.Equal(expectedIndex, indexAfterFirstComment);

            for (int i = 0; i < dataUtf8.Length; i++)
            {
                var stateInner = new JsonReaderState(options: new JsonReaderOptions { CommentHandling = JsonCommentHandling.AllowComments });
                var jsonSlice = new Utf8JsonReader(dataUtf8.AsSpan(0, i), isFinalBlock: false, stateInner);

                foundComment = false;
                indexAfterFirstComment = 0;
                while (jsonSlice.Read())
                {
                    JsonTokenType tokenType = jsonSlice.TokenType;
                    switch (tokenType)
                    {
                        case JsonTokenType.Comment:
                            if (foundComment)
                                break;
                            foundComment = true;
                            indexAfterFirstComment = jsonSlice.BytesConsumed;
                            Assert.Equal(indexAfterFirstComment, jsonSlice.CurrentState.BytesConsumed);
                            Assert.True(jsonSlice.TryGetValueAsString(out string actualComment));
                            Assert.Equal(expectedComment, actualComment);
                            break;
                    }
                }

                int consumed = (int)jsonSlice.BytesConsumed;
                Assert.Equal(consumed, jsonSlice.CurrentState.BytesConsumed);
                jsonSlice = new Utf8JsonReader(dataUtf8.AsSpan(consumed), isFinalBlock: true, jsonSlice.CurrentState);

                if (!foundComment)
                {
                    while (jsonSlice.Read())
                    {
                        JsonTokenType tokenType = jsonSlice.TokenType;
                        switch (tokenType)
                        {
                            case JsonTokenType.Comment:
                                if (foundComment)
                                    break;
                                foundComment = true;
                                indexAfterFirstComment = jsonSlice.BytesConsumed;
                                Assert.Equal(indexAfterFirstComment, jsonSlice.CurrentState.BytesConsumed);
                                Assert.True(jsonSlice.TryGetValueAsString(out string actualComment));
                                Assert.Equal(expectedComment, actualComment);
                                break;
                        }
                    }
                    indexAfterFirstComment += consumed;
                }

                Assert.True(foundComment);
                Assert.Equal(expectedIndex, indexAfterFirstComment);
            }
        }

        [Theory]
        [InlineData("//", 2)]
        [InlineData("//\n", 3)]
        [InlineData("/**/", 4)]
        [InlineData("/*/*/", 5)]

        [InlineData("//T漢字his is a 漢字comment before json\n\"hello\"", 32)]
        [InlineData("\"h漢字ello\"//This is a 漢字comment after json", 37)]
        [InlineData("\"h漢字ello\"//This is a 漢字comment after json\n", 38)]
        [InlineData("\"a漢字lpha\" \r\n//This is a 漢字comment after json\n//Here is another comment/*and a multi-line comment*///Another single-line comment", 41)]
        [InlineData("\"b漢字eta\" \r\n//This is a 漢字comment after json\n//Here is another comment/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", 40)]
        [InlineData("\"g漢字amma\" \r\n//This is a 漢字comment after json\n//Here is another comment\n/*and a multi-line comment*///Another single-line comment", 41)]
        [InlineData("\"d漢字elta\" \r\n//This is a 漢字comment after json\n//Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", 41)]
        [InlineData("\"h漢字ello\"//This is a 漢字comment after json with new line\n", 52)]
        [InlineData("{\"a漢字ge\" : \n//This is a 漢字comment between key-value pairs\n 30}", 54)]
        [InlineData("{\"a漢字ge\" : 30//This is a 漢字comment between key-value pairs on the same line\n}", 72)]

        [InlineData("/*T漢字his is a multi-line 漢字comment before json*/\"hello\"", 44)]
        [InlineData("\"h漢字ello\"/*This is a multi-line 漢字comment after json*/", 50)]
        [InlineData("\"a漢字lpha\" \r\n/*This is a multi-line 漢字comment after json*///Here is another comment/*and a multi-line comment*///Another single-line comment", 53)]
        [InlineData("\"b漢字eta\" \r\n/*This is a multi-line 漢字comment after json*///Here is another comment/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", 52)]
        [InlineData("\"g漢字amma\" \r\n/*This is a multi-line 漢字comment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment", 53)]
        [InlineData("\"d漢字elta\" \r\n/*This is a multi-line 漢字comment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", 53)]
        [InlineData("{\"a漢字ge\" : \n/*This is a 漢字comment between key-value pairs*/ 30}", 55)]
        [InlineData("{\"a漢字ge\" : 30/*This is a 漢字comment between key-value pairs on the same line*/}", 73)]

        [InlineData("/*T漢字his is a split multi-line \n漢字comment before json*/\"hello\"", 51)]
        [InlineData("\"h漢字ello\"/*This is a split multi-line \n漢字comment after json*/", 57)]
        [InlineData("\"a漢字lpha\" \r\n/*This is a split multi-line \n漢字comment after json*///Here is another comment/*and a multi-line comment*///Another single-line comment", 60)]
        [InlineData("\"b漢字eta\" \r\n/*This is a split multi-line \n漢字comment after json*///Here is another comment/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", 59)]
        [InlineData("\"g漢字amma\" \r\n/*This is a split multi-line \n漢字comment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment", 60)]
        [InlineData("\"d漢字elta\" \r\n/*This is a split multi-line \n漢字comment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", 60)]
        [InlineData("{\"a漢字ge\" : \n/*This is a split multi-line \n漢字comment between key-value pairs*/ 30}", 73)]
        [InlineData("{\"a漢字ge\" : 30/*This is a split multi-line \n漢字comment between key-value pairs on the same line*/}", 91)]
        public static void SkipComments(string jsonString, int expectedConsumed)
        {
            byte[] dataUtf8 = Encoding.UTF8.GetBytes(jsonString);
            var state = new JsonReaderState(options: new JsonReaderOptions { CommentHandling = JsonCommentHandling.SkipComments });
            var json = new Utf8JsonReader(dataUtf8, isFinalBlock: true, state);

            JsonTokenType prevTokenType = JsonTokenType.None;
            while (json.Read())
            {
                JsonTokenType tokenType = json.TokenType;
                switch (tokenType)
                {
                    case JsonTokenType.Comment:
                        Assert.True(false, "TokenType should never be Comment when we are skipping them.");
                        break;
                }
                Assert.NotEqual(tokenType, prevTokenType);
                prevTokenType = tokenType;
            }
            Assert.Equal(dataUtf8.Length, json.BytesConsumed);
            Assert.Equal(dataUtf8.Length, json.CurrentState.BytesConsumed);
        }

        [Theory]
        [InlineData("//", 2)]
        [InlineData("//\n", 3)]
        [InlineData("/**/", 4)]
        [InlineData("/*/*/", 5)]

        [InlineData("//T漢字his is a 漢字comment before json\n\"hello\"", 32)]
        [InlineData("\"h漢字ello\"//This is a 漢字comment after json", 37)]
        [InlineData("\"h漢字ello\"//This is a 漢字comment after json\n", 38)]
        [InlineData("\"a漢字lpha\" \r\n//This is a 漢字comment after json\n//Here is another comment/*and a multi-line comment*///Another single-line comment", 41)]
        [InlineData("\"b漢字eta\" \r\n//This is a 漢字comment after json\n//Here is another comment/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", 40)]
        [InlineData("\"g漢字amma\" \r\n//This is a 漢字comment after json\n//Here is another comment\n/*and a multi-line comment*///Another single-line comment", 41)]
        [InlineData("\"d漢字elta\" \r\n//This is a 漢字comment after json\n//Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", 41)]
        [InlineData("\"h漢字ello\"//This is a 漢字comment after json with new line\n", 52)]
        [InlineData("{\"a漢字ge\" : \n//This is a 漢字comment between key-value pairs\n 30}", 54)]
        [InlineData("{\"a漢字ge\" : 30//This is a 漢字comment between key-value pairs on the same line\n}", 72)]

        [InlineData("/*T漢字his is a multi-line 漢字comment before json*/\"hello\"", 44)]
        [InlineData("\"h漢字ello\"/*This is a multi-line 漢字comment after json*/", 50)]
        [InlineData("\"a漢字lpha\" \r\n/*This is a multi-line 漢字comment after json*///Here is another comment/*and a multi-line comment*///Another single-line comment", 53)]
        [InlineData("\"b漢字eta\" \r\n/*This is a multi-line 漢字comment after json*///Here is another comment/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", 52)]
        [InlineData("\"g漢字amma\" \r\n/*This is a multi-line 漢字comment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment", 53)]
        [InlineData("\"d漢字elta\" \r\n/*This is a multi-line 漢字comment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", 53)]
        [InlineData("{\"a漢字ge\" : \n/*This is a 漢字comment between key-value pairs*/ 30}", 55)]
        [InlineData("{\"a漢字ge\" : 30/*This is a 漢字comment between key-value pairs on the same line*/}", 73)]

        [InlineData("/*T漢字his is a split multi-line \n漢字comment before json*/\"hello\"", 51)]
        [InlineData("\"h漢字ello\"/*This is a split multi-line \n漢字comment after json*/", 57)]
        [InlineData("\"a漢字lpha\" \r\n/*This is a split multi-line \n漢字comment after json*///Here is another comment/*and a multi-line comment*///Another single-line comment", 60)]
        [InlineData("\"b漢字eta\" \r\n/*This is a split multi-line \n漢字comment after json*///Here is another comment/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", 59)]
        [InlineData("\"g漢字amma\" \r\n/*This is a split multi-line \n漢字comment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment", 60)]
        [InlineData("\"d漢字elta\" \r\n/*This is a split multi-line \n漢字comment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", 60)]
        [InlineData("{\"a漢字ge\" : \n/*This is a split multi-line \n漢字comment between key-value pairs*/ 30}", 73)]
        [InlineData("{\"a漢字ge\" : 30/*This is a split multi-line \n漢字comment between key-value pairs on the same line*/}", 91)]
        public static void SkipCommentsSingleSegment(string jsonString, int expectedConsumed)
        {
            byte[] dataUtf8 = Encoding.UTF8.GetBytes(jsonString);
            var state = new JsonReaderState(options: new JsonReaderOptions { CommentHandling = JsonCommentHandling.SkipComments });
            var json = new Utf8JsonReader(dataUtf8, isFinalBlock: true, state);

            JsonTokenType prevTokenType = JsonTokenType.None;
            while (json.Read())
            {
                JsonTokenType tokenType = json.TokenType;
                switch (tokenType)
                {
                    case JsonTokenType.Comment:
                        Assert.True(false, "TokenType should never be Comment when we are skipping them.");
                        break;
                }
                Assert.NotEqual(tokenType, prevTokenType);
                prevTokenType = tokenType;
            }
            Assert.Equal(dataUtf8.Length, json.BytesConsumed);
            Assert.Equal(dataUtf8.Length, json.CurrentState.BytesConsumed);

            for (int i = 0; i < dataUtf8.Length; i++)
            {
                var stateInner = new JsonReaderState(options: new JsonReaderOptions { CommentHandling = JsonCommentHandling.SkipComments });
                var jsonSlice = new Utf8JsonReader(dataUtf8.AsSpan(0, i), isFinalBlock: false, stateInner);

                prevTokenType = JsonTokenType.None;
                while (jsonSlice.Read())
                {
                    JsonTokenType tokenType = jsonSlice.TokenType;
                    switch (tokenType)
                    {
                        case JsonTokenType.Comment:
                            Assert.True(false, "TokenType should never be Comment when we are skipping them.");
                            break;
                    }
                    Assert.NotEqual(tokenType, prevTokenType);
                    prevTokenType = tokenType;
                }

                int prevConsumed = (int)jsonSlice.BytesConsumed;
                Assert.Equal(prevConsumed, jsonSlice.CurrentState.BytesConsumed);
                jsonSlice = new Utf8JsonReader(dataUtf8.AsSpan(prevConsumed), isFinalBlock: true, jsonSlice.CurrentState);

                while (jsonSlice.Read())
                {
                    JsonTokenType tokenType = jsonSlice.TokenType;
                    switch (tokenType)
                    {
                        case JsonTokenType.Comment:
                            Assert.True(false, "TokenType should never be Comment when we are skipping them.");
                            break;
                    }
                    Assert.NotEqual(tokenType, prevTokenType);
                    prevTokenType = tokenType;
                }

                Assert.Equal(dataUtf8.Length - prevConsumed, jsonSlice.BytesConsumed);
                Assert.Equal(jsonSlice.BytesConsumed, jsonSlice.CurrentState.BytesConsumed);
            }
        }

        [Theory]
        [InlineData("//", 0, 0)]
        [InlineData("//\n", 0, 0)]
        [InlineData("/**/", 0, 0)]
        [InlineData("/*/*/", 0, 0)]

        [InlineData("//T漢字his is a 漢字comment before json\n\"hello\"", 0, 0)]
        [InlineData("\"h漢字ello\"//This is a 漢字comment after json", 0, 13)]
        [InlineData("\"h漢字ello\"//This is a 漢字comment after json\n", 0, 13)]
        [InlineData("\"a漢字lpha\" \r\n//This is a 漢字comment after json\n//Here is another comment/*and a multi-line comment*///Another single-line comment", 1, 0)]
        [InlineData("\"b漢字eta\" \r\n//This is a 漢字comment after json\n//Here is another comment/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", 1, 0)]
        [InlineData("\"g漢字amma\" \r\n//This is a 漢字comment after json\n//Here is another comment\n/*and a multi-line comment*///Another single-line comment", 1, 0)]
        [InlineData("\"d漢字elta\" \r\n//This is a 漢字comment after json\n//Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", 1, 0)]
        [InlineData("\"h漢字ello\"//This is a 漢字comment after json with new line\n", 0, 13)]
        [InlineData("{\"a漢字ge\" : \n//This is a 漢字comment between key-value pairs\n 30}", 1, 0)]
        [InlineData("{\"a漢字ge\" : 30//This is a 漢字comment between key-value pairs on the same line\n}", 0, 17)]

        [InlineData("/*T漢字his is a multi-line 漢字comment before json*/\"hello\"", 0, 0)]
        [InlineData("\"h漢字ello\"/*This is a multi-line 漢字comment after json*/", 0, 13)]
        [InlineData("\"a漢字lpha\" \r\n/*This is a multi-line 漢字comment after json*///Here is another comment/*and a multi-line comment*///Another single-line comment", 1, 0)]
        [InlineData("\"b漢字eta\" \r\n/*This is a multi-line 漢字comment after json*///Here is another comment/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", 1, 0)]
        [InlineData("\"g漢字amma\" \r\n/*This is a multi-line 漢字comment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment", 1, 0)]
        [InlineData("\"d漢字elta\" \r\n/*This is a multi-line 漢字comment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", 1, 0)]
        [InlineData("{\"a漢字ge\" : \n/*This is a 漢字comment between key-value pairs*/ 30}", 1, 0)]
        [InlineData("{\"a漢字ge\" : 30/*This is a 漢字comment between key-value pairs on the same line*/}", 0, 17)]

        [InlineData("/*T漢字his is a split multi-line \n漢字comment before json*/\"hello\"", 0, 0)]
        [InlineData("\"h漢字ello\"/*This is a split multi-line \n漢字comment after json*/", 0, 13)]
        [InlineData("\"a漢字lpha\" \r\n/*This is a split multi-line \n漢字comment after json*///Here is another comment/*and a multi-line comment*///Another single-line comment", 1, 0)]
        [InlineData("\"b漢字eta\" \r\n/*This is a split multi-line \n漢字comment after json*///Here is another comment/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", 1, 0)]
        [InlineData("\"g漢字amma\" \r\n/*This is a split multi-line \n漢字comment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment", 1, 0)]
        [InlineData("\"d漢字elta\" \r\n/*This is a split multi-line \n漢字comment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", 1, 0)]
        [InlineData("{\"a漢字ge\" : \n/*This is a split multi-line \n漢字comment between key-value pairs*/ 30}", 1, 0)]
        [InlineData("{\"a漢字ge\" : 30/*This is a split multi-line \n漢字comment between key-value pairs on the same line*/}", 0, 17)]
        public static void CommentsAreInvalidByDefault(string jsonString, int expectedlineNumber, int expectedPosition)
        {
            byte[] dataUtf8 = Encoding.UTF8.GetBytes(jsonString);
            var json = new Utf8JsonReader(dataUtf8, isFinalBlock: true, default);

            try
            {
                while (json.Read())
                {
                    JsonTokenType tokenType = json.TokenType;
                    switch (tokenType)
                    {
                        case JsonTokenType.Comment:
                            Assert.True(false, "TokenType should never be Comment when we are skipping them.");
                            break;
                    }
                }
                Assert.True(false, "Expected JsonReaderException was not thrown with single-segment data.");
            }
            catch (JsonReaderException ex)
            {
                Assert.Equal(expectedlineNumber, ex.LineNumber);
                Assert.Equal(expectedPosition, ex.BytePositionInLine);
            }
        }

        [Theory]
        [InlineData("//", 0, 0)]
        [InlineData("//\n", 0, 0)]
        [InlineData("/**/", 0, 0)]
        [InlineData("/*/*/", 0, 0)]

        [InlineData("//T漢字his is a 漢字comment before json\n\"hello\"", 0, 0)]
        [InlineData("\"h漢字ello\"//This is a 漢字comment after json", 0, 13)]
        [InlineData("\"h漢字ello\"//This is a 漢字comment after json\n", 0, 13)]
        [InlineData("\"a漢字lpha\" \r\n//This is a 漢字comment after json\n//Here is another comment/*and a multi-line comment*///Another single-line comment", 1, 0)]
        [InlineData("\"b漢字eta\" \r\n//This is a 漢字comment after json\n//Here is another comment/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", 1, 0)]
        [InlineData("\"g漢字amma\" \r\n//This is a 漢字comment after json\n//Here is another comment\n/*and a multi-line comment*///Another single-line comment", 1, 0)]
        [InlineData("\"d漢字elta\" \r\n//This is a 漢字comment after json\n//Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", 1, 0)]
        [InlineData("\"h漢字ello\"//This is a 漢字comment after json with new line\n", 0, 13)]
        [InlineData("{\"a漢字ge\" : \n//This is a 漢字comment between key-value pairs\n 30}", 1, 0)]
        [InlineData("{\"a漢字ge\" : 30//This is a 漢字comment between key-value pairs on the same line\n}", 0, 17)]

        [InlineData("/*T漢字his is a multi-line 漢字comment before json*/\"hello\"", 0, 0)]
        [InlineData("\"h漢字ello\"/*This is a multi-line 漢字comment after json*/", 0, 13)]
        [InlineData("\"a漢字lpha\" \r\n/*This is a multi-line 漢字comment after json*///Here is another comment/*and a multi-line comment*///Another single-line comment", 1, 0)]
        [InlineData("\"b漢字eta\" \r\n/*This is a multi-line 漢字comment after json*///Here is another comment/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", 1, 0)]
        [InlineData("\"g漢字amma\" \r\n/*This is a multi-line 漢字comment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment", 1, 0)]
        [InlineData("\"d漢字elta\" \r\n/*This is a multi-line 漢字comment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", 1, 0)]
        [InlineData("{\"a漢字ge\" : \n/*This is a 漢字comment between key-value pairs*/ 30}", 1, 0)]
        [InlineData("{\"a漢字ge\" : 30/*This is a 漢字comment between key-value pairs on the same line*/}", 0, 17)]

        [InlineData("/*T漢字his is a split multi-line \n漢字comment before json*/\"hello\"", 0, 0)]
        [InlineData("\"h漢字ello\"/*This is a split multi-line \n漢字comment after json*/", 0, 13)]
        [InlineData("\"a漢字lpha\" \r\n/*This is a split multi-line \n漢字comment after json*///Here is another comment/*and a multi-line comment*///Another single-line comment", 1, 0)]
        [InlineData("\"b漢字eta\" \r\n/*This is a split multi-line \n漢字comment after json*///Here is another comment/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", 1, 0)]
        [InlineData("\"g漢字amma\" \r\n/*This is a split multi-line \n漢字comment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment", 1, 0)]
        [InlineData("\"d漢字elta\" \r\n/*This is a split multi-line \n漢字comment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/", 1, 0)]
        [InlineData("{\"a漢字ge\" : \n/*This is a split multi-line \n漢字comment between key-value pairs*/ 30}", 1, 0)]
        [InlineData("{\"a漢字ge\" : 30/*This is a split multi-line \n漢字comment between key-value pairs on the same line*/}", 0, 17)]
        public static void CommentsAreInvalidByDefaultSingleSegment(string jsonString, int expectedlineNumber, int expectedPosition)
        {
            byte[] dataUtf8 = Encoding.UTF8.GetBytes(jsonString);
            var json = new Utf8JsonReader(dataUtf8, isFinalBlock: true, default);

            try
            {
                while (json.Read())
                {
                    JsonTokenType tokenType = json.TokenType;
                    switch (tokenType)
                    {
                        case JsonTokenType.Comment:
                            Assert.True(false, "TokenType should never be Comment when we are skipping them.");
                            break;
                    }
                }
                Assert.True(false, "Expected JsonReaderException was not thrown with single-segment data.");
            }
            catch (JsonReaderException ex)
            {
                Assert.Equal(expectedlineNumber, ex.LineNumber);
                Assert.Equal(expectedPosition, ex.BytePositionInLine);
            }

            for (int i = 0; i < dataUtf8.Length; i++)
            {
                var jsonSlice = new Utf8JsonReader(dataUtf8.AsSpan(0, i), isFinalBlock: false, default);
                try
                {
                    while (jsonSlice.Read())
                    {
                        JsonTokenType tokenType = jsonSlice.TokenType;
                        switch (tokenType)
                        {
                            case JsonTokenType.Comment:
                                Assert.True(false, "TokenType should never be Comment when we are skipping them.");
                                break;
                        }
                    }

                    Assert.Equal(jsonSlice.BytesConsumed, jsonSlice.CurrentState.BytesConsumed);
                    jsonSlice = new Utf8JsonReader(dataUtf8.AsSpan((int)jsonSlice.BytesConsumed), isFinalBlock: true, jsonSlice.CurrentState);
                    while (jsonSlice.Read())
                    {
                        JsonTokenType tokenType = jsonSlice.TokenType;
                        switch (tokenType)
                        {
                            case JsonTokenType.Comment:
                                Assert.True(false, "TokenType should never be Comment when we are skipping them.");
                                break;
                        }
                    }

                    Assert.True(false, "Expected JsonReaderException was not thrown with multi-segment data.");
                }
                catch (JsonReaderException ex)
                {
                    Assert.Equal(expectedlineNumber, ex.LineNumber);
                    Assert.Equal(expectedPosition, ex.BytePositionInLine);
                }
            }
        }

        [Theory]
        [InlineData("//\n}", 1, 0)]
        [InlineData("//comment\n}", 1, 0)]
        [InlineData("/**/}", 0, 4)]
        [InlineData("/*\n*/}", 1, 2)]
        [InlineData("/*comment\n*/}", 1, 2)]
        [InlineData("/*/*/}", 0, 5)]
        [InlineData("//This is a comment before json\n\"hello\"{", 1, 7)]
        [InlineData("\"hello\"//This is a comment after json\n{", 1, 0)]
        [InlineData("\"gamma\" \r\n//This is a comment after json\n//Here is another comment\n/*and a multi-line comment*/{//Another single-line comment", 3, 28)]
        [InlineData("\"delta\" \r\n//This is a comment after json\n//Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/{", 4, 18)]
        [InlineData("\"hello\"//This is a comment after json with new line\n{", 1, 0)]
        [InlineData("{\"age\" : \n//This is a comment between key-value pairs\n 30}{", 2, 4)]
        [InlineData("{\"age\" : 30//This is a comment between key-value pairs on the same line\n}{", 1, 1)]
        [InlineData("/*This is a multi-line comment before json*/\"hello\"{", 0, 51)]
        [InlineData("\"hello\"/*This is a multi-line comment after json*/{", 0, 50)]
        [InlineData("\"gamma\" \r\n/*This is a multi-line comment after json*///Here is another comment\n/*and a multi-line comment*/{//Another single-line comment", 2, 28)]
        [InlineData("\"delta\" \r\n/*This is a multi-line comment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/{", 3, 18)]
        [InlineData("{\"age\" : \n/*This is a comment between key-value pairs*/ 30}{", 1, 49)]
        [InlineData("{\"age\" : 30/*This is a comment between key-value pairs on the same line*/}{", 0, 74)]
        [InlineData("/*This is a split multi-line \ncomment before json*/\"hello\"{", 1, 28)]
        [InlineData("\"hello\"/*This is a split multi-line \ncomment after json*/{", 1, 20)]
        [InlineData("\"gamma\" \r\n/*This is a split multi-line \ncomment after json*///Here is another comment\n/*and a multi-line comment*/{//Another single-line comment", 3, 28)]
        [InlineData("\"delta\" \r\n/*This is a split multi-line \ncomment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/{", 4, 18)]
        [InlineData("{\"age\" : \n/*This is a split multi-line \ncomment between key-value pairs*/ 30}{", 2, 37)]
        [InlineData("{\"age\" : 30/*This is a split multi-line \ncomment between key-value pairs on the same line*/}{", 1, 51)]

        [InlineData("//\n漢字}", 1, 0)]
        [InlineData("//c漢字omment\n漢字}", 1, 0)]
        [InlineData("/**/漢字}", 0, 4)]
        [InlineData("/*\n*/漢字}", 1, 2)]
        [InlineData("/*c漢字omment\n*/漢字}", 1, 2)]
        [InlineData("/*/*/漢字}", 0, 5)]
        [InlineData("//T漢字his is a comment before json\n\"hello\"漢字{", 1, 7)]
        [InlineData("\"h漢字ello\"//This is a comment after json\n漢字{", 1, 0)]
        [InlineData("\"g漢字amma\" \r\n//This is a comment after json\n//Here is another comment\n/*and a multi-line comment*/漢字{//Another single-line comment", 3, 28)]
        [InlineData("\"d漢字elta\" \r\n//This is a comment after json\n//Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/漢字{", 4, 18)]
        [InlineData("\"h漢字ello\"//This is a comment after json with new line\n漢字{", 1, 0)]
        [InlineData("{\"a漢字ge\" : \n//This is a comment between key-value pairs\n 30}漢字{", 2, 4)]
        [InlineData("{\"a漢字ge\" : 30//This is a comment between key-value pairs on the same line\n}漢字{", 1, 1)]
        [InlineData("/*T漢字his is a multi-line comment before json*/\"hello\"漢字{", 0, 57)]
        [InlineData("\"h漢字ello\"/*This is a multi-line comment after json*/漢字{", 0, 56)]
        [InlineData("\"g漢字amma\" \r\n/*This is a multi-line comment after json*///Here is another comment\n/*and a multi-line comment*/漢字{//Another single-line comment", 2, 28)]
        [InlineData("\"d漢字elta\" \r\n/*This is a multi-line comment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/漢字{", 3, 18)]
        [InlineData("{\"a漢字ge\" : \n/*This is a comment between key-value pairs*/ 30}漢字{", 1, 49)]
        [InlineData("{\"a漢字ge\" : 30/*This is a comment between key-value pairs on the same line*/}漢字{", 0, 80)]
        [InlineData("/*T漢字his is a split multi-line \ncomment before json*/\"hello\"漢字{", 1, 28)]
        [InlineData("\"h漢字ello\"/*This is a split multi-line \ncomment after json*/漢字{", 1, 20)]
        [InlineData("\"g漢字amma\" \r\n/*This is a split multi-line \ncomment after json*///Here is another comment\n/*and a multi-line comment*/漢字{//Another single-line comment", 3, 28)]
        [InlineData("\"d漢字elta\" \r\n/*This is a split multi-line \ncomment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/漢字{", 4, 18)]
        [InlineData("{\"a漢字ge\" : \n/*This is a split multi-line \ncomment between key-value pairs*/ 30}漢字{", 2, 37)]
        [InlineData("{\"a漢字ge\" : 30/*This is a split multi-line \ncomment between key-value pairs on the same line*/}漢字{", 1, 51)]

        [InlineData("{   // comment \n   ]", 1, 3)]
        [InlineData("[   // comment \n   }", 1, 3)]
        [InlineData("{   /* comment */   ]", 0, 20)]
        [InlineData("[   /* comment */   }", 0, 20)]
        public static void InvalidJsonWithComments(string jsonString, int expectedlineNumber, int expectedPosition)
        {
            byte[] dataUtf8 = Encoding.UTF8.GetBytes(jsonString);
            var state = new JsonReaderState(options: new JsonReaderOptions { CommentHandling = JsonCommentHandling.AllowComments });
            var json = new Utf8JsonReader(dataUtf8, isFinalBlock: true, state);

            try
            {
                while (json.Read())
                    ;
                Assert.True(false, "Expected JsonReaderException was not thrown with single-segment data.");
            }
            catch (JsonReaderException ex)
            {
                Assert.Equal(expectedlineNumber, ex.LineNumber);
                Assert.Equal(expectedPosition, ex.BytePositionInLine);
            }
        }

        [Theory]
        [InlineData("//\n}", 1, 0)]
        [InlineData("//comment\n}", 1, 0)]
        [InlineData("/**/}", 0, 4)]
        [InlineData("/*\n*/}", 1, 2)]
        [InlineData("/*comment\n*/}", 1, 2)]
        [InlineData("/*/*/}", 0, 5)]
        [InlineData("//This is a comment before json\n\"hello\"{", 1, 7)]
        [InlineData("\"hello\"//This is a comment after json\n{", 1, 0)]
        [InlineData("\"gamma\" \r\n//This is a comment after json\n//Here is another comment\n/*and a multi-line comment*/{//Another single-line comment", 3, 28)]
        [InlineData("\"delta\" \r\n//This is a comment after json\n//Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/{", 4, 18)]
        [InlineData("\"hello\"//This is a comment after json with new line\n{", 1, 0)]
        [InlineData("{\"age\" : \n//This is a comment between key-value pairs\n 30}{", 2, 4)]
        [InlineData("{\"age\" : 30//This is a comment between key-value pairs on the same line\n}{", 1, 1)]
        [InlineData("/*This is a multi-line comment before json*/\"hello\"{", 0, 51)]
        [InlineData("\"hello\"/*This is a multi-line comment after json*/{", 0, 50)]
        [InlineData("\"gamma\" \r\n/*This is a multi-line comment after json*///Here is another comment\n/*and a multi-line comment*/{//Another single-line comment", 2, 28)]
        [InlineData("\"delta\" \r\n/*This is a multi-line comment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/{", 3, 18)]
        [InlineData("{\"age\" : \n/*This is a comment between key-value pairs*/ 30}{", 1, 49)]
        [InlineData("{\"age\" : 30/*This is a comment between key-value pairs on the same line*/}{", 0, 74)]
        [InlineData("/*This is a split multi-line \ncomment before json*/\"hello\"{", 1, 28)]
        [InlineData("\"hello\"/*This is a split multi-line \ncomment after json*/{", 1, 20)]
        [InlineData("\"gamma\" \r\n/*This is a split multi-line \ncomment after json*///Here is another comment\n/*and a multi-line comment*/{//Another single-line comment", 3, 28)]
        [InlineData("\"delta\" \r\n/*This is a split multi-line \ncomment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/{", 4, 18)]
        [InlineData("{\"age\" : \n/*This is a split multi-line \ncomment between key-value pairs*/ 30}{", 2, 37)]
        [InlineData("{\"age\" : 30/*This is a split multi-line \ncomment between key-value pairs on the same line*/}{", 1, 51)]

        [InlineData("//\n漢字}", 1, 0)]
        [InlineData("//c漢字omment\n漢字}", 1, 0)]
        [InlineData("/**/漢字}", 0, 4)]
        [InlineData("/*\n*/漢字}", 1, 2)]
        [InlineData("/*c漢字omment\n*/漢字}", 1, 2)]
        [InlineData("/*/*/漢字}", 0, 5)]
        [InlineData("//T漢字his is a comment before json\n\"hello\"漢字{", 1, 7)]
        [InlineData("\"h漢字ello\"//This is a comment after json\n漢字{", 1, 0)]
        [InlineData("\"g漢字amma\" \r\n//This is a comment after json\n//Here is another comment\n/*and a multi-line comment*/漢字{//Another single-line comment", 3, 28)]
        [InlineData("\"d漢字elta\" \r\n//This is a comment after json\n//Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/漢字{", 4, 18)]
        [InlineData("\"h漢字ello\"//This is a comment after json with new line\n漢字{", 1, 0)]
        [InlineData("{\"a漢字ge\" : \n//This is a comment between key-value pairs\n 30}漢字{", 2, 4)]
        [InlineData("{\"a漢字ge\" : 30//This is a comment between key-value pairs on the same line\n}漢字{", 1, 1)]
        [InlineData("/*T漢字his is a multi-line comment before json*/\"hello\"漢字{", 0, 57)]
        [InlineData("\"h漢字ello\"/*This is a multi-line comment after json*/漢字{", 0, 56)]
        [InlineData("\"g漢字amma\" \r\n/*This is a multi-line comment after json*///Here is another comment\n/*and a multi-line comment*/漢字{//Another single-line comment", 2, 28)]
        [InlineData("\"d漢字elta\" \r\n/*This is a multi-line comment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/漢字{", 3, 18)]
        [InlineData("{\"a漢字ge\" : \n/*This is a comment between key-value pairs*/ 30}漢字{", 1, 49)]
        [InlineData("{\"a漢字ge\" : 30/*This is a comment between key-value pairs on the same line*/}漢字{", 0, 80)]
        [InlineData("/*T漢字his is a split multi-line \ncomment before json*/\"hello\"漢字{", 1, 28)]
        [InlineData("\"h漢字ello\"/*This is a split multi-line \ncomment after json*/漢字{", 1, 20)]
        [InlineData("\"g漢字amma\" \r\n/*This is a split multi-line \ncomment after json*///Here is another comment\n/*and a multi-line comment*/漢字{//Another single-line comment", 3, 28)]
        [InlineData("\"d漢字elta\" \r\n/*This is a split multi-line \ncomment after json*///Here is another comment\n/*and a multi-line comment*///Another single-line comment\n\t  /*blah * blah*/漢字{", 4, 18)]
        [InlineData("{\"a漢字ge\" : \n/*This is a split multi-line \ncomment between key-value pairs*/ 30}漢字{", 2, 37)]
        [InlineData("{\"a漢字ge\" : 30/*This is a split multi-line \ncomment between key-value pairs on the same line*/}漢字{", 1, 51)]
        public static void InvalidJsonWithCommentsSingleSegment(string jsonString, int expectedlineNumber, int expectedPosition)
        {
            byte[] dataUtf8 = Encoding.UTF8.GetBytes(jsonString);
            var state = new JsonReaderState(options: new JsonReaderOptions { CommentHandling = JsonCommentHandling.AllowComments });
            var json = new Utf8JsonReader(dataUtf8, isFinalBlock: true, state);

            try
            {
                while (json.Read())
                    ;
                Assert.True(false, "Expected JsonReaderException was not thrown with single-segment data.");
            }
            catch (JsonReaderException ex)
            {
                Assert.Equal(expectedlineNumber, ex.LineNumber);
                Assert.Equal(expectedPosition, ex.BytePositionInLine);
            }
        }

        [Theory]
        [MemberData(nameof(LotsOfCommentsTests))]
        public static void SkipLotsOfComments(string valueString, bool insideArray, string expectedString)
        {
            var builder = new StringBuilder();
            if (insideArray)
            {
                builder.Append("[");
            }
            for (int i = 0; i < 100_000; i++)
            {
                builder.Append("// comment ").Append(i).Append("\n");
            }
            builder.Append(valueString);
            if (insideArray)
            {
                builder.Append("]");
            }
            string jsonString = builder.ToString();
            byte[] dataUtf8 = Encoding.UTF8.GetBytes(jsonString);

            var state = new JsonReaderState(options: new JsonReaderOptions { CommentHandling = JsonCommentHandling.SkipComments });
            var json = new Utf8JsonReader(dataUtf8, isFinalBlock: true, state);

            if (insideArray)
            {
                Assert.True(json.Read());
                Assert.True(json.TokenType == JsonTokenType.StartArray);
            }

            if (json.Read())
            {
                bool isTokenPrimitive = json.TokenType >= JsonTokenType.String && json.TokenType <= JsonTokenType.Null;
                Assert.True(isTokenPrimitive);
                switch (json.TokenType)
                {
                    case JsonTokenType.Null:
                        Assert.Equal(expectedString, Encoding.UTF8.GetString(json.ValueSpan));
                        break;
                    case JsonTokenType.Number:
                        if (json.ValueSpan.Contains((byte)'.'))
                        {
                            Assert.True(json.TryGetValueAsDouble(out double numberValue));
                            Assert.Equal(expectedString, numberValue.ToString());
                        }
                        else
                        {
                            Assert.True(json.TryGetValueAsInt32(out int numberValue));
                            Assert.Equal(expectedString, numberValue.ToString());
                        }
                        break;
                    case JsonTokenType.String:
                        Assert.True(json.TryGetValueAsString(out string stringValue));
                        Assert.Equal(expectedString, stringValue);
                        break;
                    case JsonTokenType.False:
                    case JsonTokenType.True:
                        Assert.True(json.TryGetValueAsBoolean(out bool boolValue));
                        Assert.Equal(expectedString, boolValue.ToString());
                        break;
                }
            }

            if (insideArray)
            {
                Assert.True(json.Read());
                Assert.True(json.TokenType == JsonTokenType.EndArray);
            }

            Assert.False(json.Read());
            Assert.Equal(dataUtf8.Length, json.BytesConsumed);
        }

        [Theory]
        [MemberData(nameof(LotsOfCommentsTests))]
        public static void ConsumeLotsOfComments(string valueString, bool insideArray, string expectedString)
        {
            var builder = new StringBuilder();
            if (insideArray)
            {
                builder.Append("[");
            }
            for (int i = 0; i < 100_000; i++)
            {
                builder.Append("// comment ").Append(i).Append("\n");
            }
            builder.Append(valueString);
            if (insideArray)
            {
                builder.Append("]");
            }
            string jsonString = builder.ToString();
            byte[] dataUtf8 = Encoding.UTF8.GetBytes(jsonString);

            var state = new JsonReaderState(options: new JsonReaderOptions { CommentHandling = JsonCommentHandling.AllowComments });
            var json = new Utf8JsonReader(dataUtf8, isFinalBlock: true, state);

            bool foundPrimitiveValue = false;
            while (json.Read())
            {
                bool isTokenPrimitive = json.TokenType >= JsonTokenType.String && json.TokenType <= JsonTokenType.Null;

                if (insideArray)
                {
                    Assert.True(isTokenPrimitive || json.TokenType == JsonTokenType.Comment || json.TokenType == JsonTokenType.StartArray || json.TokenType == JsonTokenType.EndArray);
                }
                else
                {
                    Assert.True(isTokenPrimitive || json.TokenType == JsonTokenType.Comment);
                }
                
                switch (json.TokenType)
                {
                    case JsonTokenType.Null:
                        Assert.Equal(expectedString, Encoding.UTF8.GetString(json.ValueSpan));
                        foundPrimitiveValue = true;
                        break;
                    case JsonTokenType.Number:
                        if (json.ValueSpan.Contains((byte)'.'))
                        {
                            Assert.True(json.TryGetValueAsDouble(out double numberValue));
                            Assert.Equal(expectedString, numberValue.ToString());
                        }
                        else
                        {
                            Assert.True(json.TryGetValueAsInt32(out int numberValue));
                            Assert.Equal(expectedString, numberValue.ToString());
                        }
                        foundPrimitiveValue = true;
                        break;
                    case JsonTokenType.String:
                        Assert.True(json.TryGetValueAsString(out string stringValue));
                        Assert.Equal(expectedString, stringValue);
                        foundPrimitiveValue = true;
                        break;
                    case JsonTokenType.False:
                    case JsonTokenType.True:
                        Assert.True(json.TryGetValueAsBoolean(out bool boolValue));
                        Assert.Equal(expectedString, boolValue.ToString());
                        foundPrimitiveValue = true;
                        break;
                }
            }
            Assert.True(foundPrimitiveValue);
            Assert.Equal(dataUtf8.Length, json.BytesConsumed);
        }

        public static IEnumerable<object[]> TestCases
        {
            get
            {
                return new List<object[]>
                {
                    new object[] { true, TestCaseType.Basic, SR.BasicJson},
                    new object[] { true, TestCaseType.BasicLargeNum, SR.BasicJsonWithLargeNum}, // Json.NET treats numbers starting with 0 as octal (0425 becomes 277)
                    new object[] { true, TestCaseType.BroadTree, SR.BroadTree}, // \r\n behavior is different between Json.NET and System.Text.Json
                    new object[] { true, TestCaseType.DeepTree, SR.DeepTree},
                    new object[] { true, TestCaseType.FullSchema1, SR.FullJsonSchema1},
                    new object[] { true, TestCaseType.HelloWorld, SR.HelloWorld},
                    new object[] { true, TestCaseType.LotsOfNumbers, SR.LotsOfNumbers},
                    new object[] { true, TestCaseType.LotsOfStrings, SR.LotsOfStrings},
                    new object[] { true, TestCaseType.ProjectLockJson, SR.ProjectLockJson},
                    new object[] { true, TestCaseType.Json400B, SR.Json400B},
                    new object[] { true, TestCaseType.Json4KB, SR.Json4KB},
                    new object[] { true, TestCaseType.Json40KB, SR.Json40KB},
                    new object[] { true, TestCaseType.Json400KB, SR.Json400KB},

                    new object[] { false, TestCaseType.Basic, SR.BasicJson},
                    new object[] { false, TestCaseType.BasicLargeNum, SR.BasicJsonWithLargeNum}, // Json.NET treats numbers starting with 0 as octal (0425 becomes 277)
                    new object[] { false, TestCaseType.BroadTree, SR.BroadTree}, // \r\n behavior is different between Json.NET and System.Text.Json
                    new object[] { false, TestCaseType.DeepTree, SR.DeepTree},
                    new object[] { false, TestCaseType.FullSchema1, SR.FullJsonSchema1},
                    new object[] { false, TestCaseType.HelloWorld, SR.HelloWorld},
                    new object[] { false, TestCaseType.LotsOfNumbers, SR.LotsOfNumbers},
                    new object[] { false, TestCaseType.LotsOfStrings, SR.LotsOfStrings},
                    new object[] { false, TestCaseType.ProjectLockJson, SR.ProjectLockJson},
                    new object[] { false, TestCaseType.Json400B, SR.Json400B},
                    new object[] { false, TestCaseType.Json4KB, SR.Json4KB},
                    new object[] { false, TestCaseType.Json40KB, SR.Json40KB},
                    new object[] { false, TestCaseType.Json400KB, SR.Json400KB}
                };
            }
        }

        public static IEnumerable<object[]> SpecialNumTestCases
        {
            get
            {
                return new List<object[]>
                {
                    new object[] { TestCaseType.FullSchema2, SR.FullJsonSchema2},
                    new object[] { TestCaseType.SpecialNumForm, SR.JsonWithSpecialNumFormat},
                };
            }
        }
        
        public static IEnumerable<object[]> LotsOfCommentsTests
        {
            get
            {
                return new List<object[]>
                {
                    new object[] {"   12345   ", true, "12345"},
                    new object[] {"   12345.67890e-12   ", true, "1.23456789E-08"},
                    new object[] {"   true  ", true, "True"},
                    new object[] {"   false   ", true, "False"},
                    new object[] {"   null   ", true, "null"},
                    new object[] {"   \" Test string with \\\"nested quotes \\\" and hex: \\uABCD values! \"   ", true, " Test string with \\\"nested quotes \\\" and hex: \\uABCD values! "},

                    new object[] {"   12345   ", false, "12345"},
                    new object[] {"   12345.67890e-12   ", false, "1.23456789E-08"},
                    new object[] {"   true  ", false, "True"},
                    new object[] {"   false   ", false, "False"},
                    new object[] {"   null   ", false, "null"},
                    new object[] {"   \" Test string with \\\"nested quotes \\\" and hex: \\uABCD values! \"   ", false, " Test string with \\\"nested quotes \\\" and hex: \\uABCD values! "},
                };
            }
        }
    }
}

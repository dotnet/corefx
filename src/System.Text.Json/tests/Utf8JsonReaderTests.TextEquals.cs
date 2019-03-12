// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using Xunit;

namespace System.Text.Json.Tests
{
    public static partial class Utf8JsonReaderTests
    {
        [Fact]
        public static void TestTextEqualsBasic()
        {
            byte[] connectionId = Encoding.UTF8.GetBytes("connectionId");
            byte[] availableTransports = Encoding.UTF8.GetBytes("availableTransports");
            byte[] value123 = Encoding.UTF8.GetBytes("123");
            byte[] embeddedQuotes = Encoding.UTF8.GetBytes("My name is \"Ahson\"");
            bool foundId = false;
            bool foundTransports = false;
            bool foundValue = false;
            bool foundArrayValue = false;

            string jsonString = "{\"conne\\u0063tionId\":\"123\",\"availableTransports\":[\"My name is \\\"Ahson\\\"\"]}";
            byte[] utf8Data = Encoding.UTF8.GetBytes(jsonString);

            var json = new Utf8JsonReader(utf8Data, isFinalBlock: true, state: default);
            while (json.Read())
            {
                if (json.TokenType == JsonTokenType.PropertyName)
                {
                    if (json.TextEquals(connectionId))
                    {
                        foundId = true;
                    }
                    else if (json.TextEquals(availableTransports))
                    {
                        foundTransports = true;
                    }
                }
                else if (json.TokenType == JsonTokenType.String)
                {
                    if (json.TextEquals(value123))
                    {
                        foundValue = true;
                    }
                    else if (json.TextEquals(embeddedQuotes))
                    {
                        foundArrayValue = true;
                    }
                }
            }

            Assert.True(foundId);
            Assert.True(foundTransports);
            Assert.True(foundValue);
            Assert.True(foundArrayValue);
        }

        [Theory]
        [InlineData("{\"name\": 1234}", "name", true)]
        [InlineData("{\"name\": 1234}", "namee", false)]
        [InlineData("{\"name\": 1234}", "na\\u006de", false)]
        [InlineData("{\"name\": 1234}", "", false)]
        [InlineData("{\"\": 1234}", "name", false)]
        [InlineData("{\"\": 1234}", "na\\u006de", false)]
        [InlineData("{\"\": 1234}", "", true)]
        [InlineData("{\"na\\u006de\": 1234}", "name", true)]
        [InlineData("{\"na\\u006de\": 1234}", "namee", false)]
        [InlineData("{\"na\\u006de\": 1234}", "na\\u006de", false)]
        [InlineData("{\"na\\u006de\": 1234}", "", false)]
        public static void TestTextEquals(string jsonString, string lookUpString, bool expectedFound)
        {
            byte[] lookup = Encoding.UTF8.GetBytes(lookUpString);
            byte[] utf8Data = Encoding.UTF8.GetBytes(jsonString);
            bool found = false;

            var json = new Utf8JsonReader(utf8Data, isFinalBlock: true, state: default);
            while (json.Read())
            {
                if (json.TokenType == JsonTokenType.PropertyName)
                {
                    if (json.TextEquals(lookup))
                    {
                        found = true;
                    }
                }
            }

            Assert.Equal(expectedFound, found);

            ReadOnlySequence<byte> sequence = JsonTestHelper.GetSequence(utf8Data, 1);
            found = false;

            json = new Utf8JsonReader(sequence, isFinalBlock: true, state: default);
            while (json.Read())
            {
                if (json.TokenType == JsonTokenType.PropertyName)
                {
                    if (json.TextEquals(lookup))
                    {
                        found = true;
                    }
                }
            }

            Assert.Equal(expectedFound, found);
        }

        [Theory]
        [InlineData("{\"name\": \"John\"}", "John", true)]
        [InlineData("{\"name\": \"John\"}", "Johna", false)]
        [InlineData("{\"name\": \"John\"}", "Joh\\u006e", false)]
        [InlineData("{\"name\": \"John\"}", "", false)]
        [InlineData("{\"name\": \"\"}", "John", false)]
        [InlineData("{\"name\": \"\"}", "Joh\\u006e", false)]
        [InlineData("{\"name\": \"\"}", "", true)]
        [InlineData("{\"name\": \"Joh\\u006e\"}", "John", true)]
        [InlineData("{\"name\": \"Joh\\u006e\"}", "Johna", false)]
        [InlineData("{\"name\": \"Joh\\u006e\"}", "Joh\\u006e", false)]
        [InlineData("{\"name\": \"Joh\\u006e\"}", "", false)]
        public static void TestTextEqualsValue(string jsonString, string lookUpString, bool expectedFound)
        {
            byte[] lookup = Encoding.UTF8.GetBytes(lookUpString);
            byte[] utf8Data = Encoding.UTF8.GetBytes(jsonString);
            bool found = false;

            var json = new Utf8JsonReader(utf8Data, isFinalBlock: true, state: default);
            while (json.Read())
            {
                if (json.TokenType == JsonTokenType.String)
                {
                    if (json.TextEquals(lookup))
                    {
                        found = true;
                    }
                }
            }

            Assert.Equal(expectedFound, found);

            ReadOnlySequence<byte> sequence = JsonTestHelper.GetSequence(utf8Data, 1);
            found = false;

            json = new Utf8JsonReader(sequence, isFinalBlock: true, state: default);
            while (json.Read())
            {
                if (json.TokenType == JsonTokenType.String)
                {
                    if (json.TextEquals(lookup))
                    {
                        found = true;
                    }
                }
            }

            Assert.Equal(expectedFound, found);
        }

        [Fact]
        public static void TestTextEqualsLargeMatch()
        {
            var jsonChars = new char[320];
            Array.Fill(jsonChars, 'a');
            byte[] lookup = Encoding.UTF8.GetBytes(jsonChars);
            ReadOnlySpan<byte> lookupSpan = lookup.AsSpan(0, lookup.Length - 5);   // remove extra characters that were replaced by escaped bytes

            ReadOnlySpan<char> unEscapedA = new char[6] { '\\', 'u', '0', '0', '6', '1' };

            for (int i = 0; i < jsonChars.Length - 5; i++)
            {
                jsonChars.AsSpan().Fill('a');
                unEscapedA.CopyTo(jsonChars.AsSpan(i));
                string jsonString = "\"" + new string(jsonChars) + "\"";
                byte[] utf8Data = Encoding.UTF8.GetBytes(jsonString);

                bool found = false;

                var json = new Utf8JsonReader(utf8Data, isFinalBlock: true, state: default);
                while (json.Read())
                {
                    if (json.TokenType == JsonTokenType.String)
                    {
                        if (json.TextEquals(lookupSpan))
                        {
                            found = true;
                        }
                    }
                }

                Assert.True(found, $"Json String: {jsonString}");

                ReadOnlySequence<byte> sequence = JsonTestHelper.GetSequence(utf8Data, 1);
                found = false;

                json = new Utf8JsonReader(sequence, isFinalBlock: true, state: default);
                while (json.Read())
                {
                    if (json.TokenType == JsonTokenType.String)
                    {
                        if (json.TextEquals(lookupSpan))
                        {
                            found = true;
                        }
                    }
                }

                Assert.True(found, $"Json String: {jsonString}  | Look up: {Encoding.UTF8.GetString(lookupSpan.ToArray())}");
            }
        }

        [Fact]
        public static void TestTextEqualsLargeMismatch()
        {
            var jsonChars = new char[320];
            Array.Fill(jsonChars, 'a');
            ReadOnlySpan<char> unEscapedA = new char[6] { '\\', 'u', '0', '0', '6', '1' };

            byte[] originalLookup = Encoding.UTF8.GetBytes(jsonChars);

            for (int i = 1; i < jsonChars.Length - 6; i++)
            {
                jsonChars.AsSpan().Fill('a');
                unEscapedA.CopyTo(jsonChars.AsSpan(i));
                string jsonString = "\"" + new string(jsonChars) + "\"";
                byte[] utf8Data = Encoding.UTF8.GetBytes(jsonString);

                for (int j = 0; j < 3; j++)
                {
                    Span<byte> lookup = new byte[originalLookup.Length];
                    originalLookup.CopyTo(lookup);
                    lookup = lookup.Slice(0, lookup.Length - 5);    // remove extra characters that were replaced by escaped bytes

                    switch (j)
                    {
                        case 0:
                            lookup[i] = (byte)'b';
                            break;
                        case 1:
                            lookup[i + 1] = (byte)'b';
                            break;
                        case 2:
                            lookup[i - 1] = (byte)'b';
                            break;
                    }

                    bool found = false;

                    var json = new Utf8JsonReader(utf8Data, isFinalBlock: true, state: default);
                    while (json.Read())
                    {
                        if (json.TokenType == JsonTokenType.String)
                        {
                            if (json.TextEquals(lookup))      
                            {
                                found = true;
                            }
                        }
                    }

                    Assert.False(found, $"Json String: {jsonString}");

                    ReadOnlySequence<byte> sequence = JsonTestHelper.GetSequence(utf8Data, 1);
                    found = false;

                    json = new Utf8JsonReader(sequence, isFinalBlock: true, state: default);
                    while (json.Read())
                    {
                        if (json.TokenType == JsonTokenType.String)
                        {
                            if (json.TextEquals(lookup))
                            {
                                found = true;
                            }
                        }
                    }

                    Assert.False(found);
                }
            }
        }

        [Fact]
        public static void TestTextEqualsMismatchMultiSegment()
        {
            string jsonString = "\"Hi, \\\"Ahson\\\"!\"";
            byte[] lookup = Encoding.UTF8.GetBytes("Hello, \"Ahson\"");
            byte[] utf8Data = Encoding.UTF8.GetBytes(jsonString);
            bool found = false;

            // Segment 1: "Hi, \"A
            // Segment 2: hson\"!"
            ReadOnlySequence<byte> sequence = JsonTestHelper.CreateSegments(utf8Data);

            var json = new Utf8JsonReader(sequence, isFinalBlock: true, state: default);
            while (json.Read())
            {
                if (json.TokenType == JsonTokenType.String)
                {
                    if (json.TextEquals(lookup))
                    {
                        found = true;
                    }
                }
            }

            Assert.False(found);
        }

        [Theory]
        [InlineData("/*comment*/[1234, true, false, /*comment*/ null, {}]/*comment*/")]
        public static void TestTextEqualsInvalid(string jsonString)
        {
            byte[] utf8Data = Encoding.UTF8.GetBytes(jsonString);

            var state = new JsonReaderState(options: new JsonReaderOptions { CommentHandling = JsonCommentHandling.Allow });
            var json = new Utf8JsonReader(utf8Data, isFinalBlock: true, state);

            try
            {
                json.TextEquals(default(ReadOnlySpan<byte>));
                Assert.True(false, $"Expected InvalidOperationException was not thrown when calling TextEquals with TokenType = {json.TokenType}");
            }
            catch (InvalidOperationException)
            { }

            while (json.Read())
            {
                try
                {
                    json.TextEquals(default(ReadOnlySpan<byte>));
                    Assert.True(false, $"Expected InvalidOperationException was not thrown when calling TextEquals with TokenType = {json.TokenType}");
                }
                catch (InvalidOperationException)
                { }
            }

            Assert.Equal(utf8Data.Length, json.BytesConsumed);
        }
    }
}

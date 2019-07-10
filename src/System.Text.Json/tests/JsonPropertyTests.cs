// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Collections;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using Xunit;
using System.Buffers.Text;
using System.IO.Tests;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace System.Text.Json.Tests
{
    public static class JsonPropertyTests
    {
        private const string CompiledNewline = @"
";

        private static readonly JsonDocumentOptions s_options =
            new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip,
            };

        private static readonly bool s_replaceNewlines =
            !StringComparer.Ordinal.Equals(CompiledNewline, Environment.NewLine);

        [Fact]
        public static void CheckUseAfterDispose()
        {
            using (JsonDocument doc = JsonDocument.Parse("{\"First\":1}", default))
            {
                JsonElement root = doc.RootElement;
                JsonProperty property = root.EnumerateObject().First();
                doc.Dispose();

                Assert.Throws<ObjectDisposedException>(() =>
                {
                    Utf8JsonWriter writer = default;
                    property.WriteTo(writer);
                });
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static void WriteObjectValidations(bool skipValidation)
        {
            var buffer = new ArrayBufferWriter<byte>(1024);
            using (JsonDocument doc = JsonDocument.Parse("{\"First\":1}", default))
            {
                JsonElement root = doc.RootElement;
                var options = new JsonWriterOptions
                {
                    SkipValidation = skipValidation,
                };
                var writer = new Utf8JsonWriter(buffer, options);
                if (skipValidation)
                {
                    foreach (JsonProperty property in root.EnumerateObject())
                    {
                        property.WriteTo(writer);
                    }
                    writer.Flush();
                    AssertContents("\"First\":1", buffer);
                }
                else
                {
                    foreach (JsonProperty property in root.EnumerateObject())
                    {
                        Assert.Throws<InvalidOperationException>(() =>
                        {
                            property.WriteTo(writer);
                        });
                    }
                    writer.Flush();
                    AssertContents("", buffer);
                }
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static void WriteEmptyObject(bool indented)
        {
            WriteValue(
                indented,
                "{     }",
                "{}",
                "{}");
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static void WriteEmptyCommentedObject(bool indented)
        {
            WriteValue(
                indented,
                "{ /* Technically empty */ }",
                "{}",
                "{}");
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static void WriteSimpleObject(bool indented)
        {
            WriteValue(
                indented,
                @"{ ""r""   : 2,
// Comments make everything more interesting.
            ""d"":
2
}",
                @"{
  ""r"": 2,
  ""d"": 2
}",
                "{\"r\":2,\"d\":2}");
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public static void WriteEverythingObject(bool indented)
        {
            WriteValue(
                indented,
                "{" +
                    "\"int\": 42," +
                    "\"quadratic googol\": 1e400," +
                    "\"precisePi\": 3.141592653589793238462643383279," +
                    "\"lit0\": null,\"lit1\":  false,/*guess next*/\"lit2\": true," +
                    "\"ascii\": \"pizza\"," +
                    "\"escaped\": \"p\\u0069zza\"," +
                    "\"utf8\": \"p\u00CDzza\"," +
                    "\"utf8ExtraEscape\": \"p\u00CDz\\u007Aa\"," +
                    "\"arr\": [\"hello\", \"sa\\u0069lor\", 21, \"blackjack!\" ]," +
                    "\"obj\": {" +
                        "\"arr\": [ 1, 3, 5, 7, /*9,*/ 11] " +
                    "}}",
                @"{
  ""int"": 42,
  ""quadratic googol"": 1e400,
  ""precisePi"": 3.141592653589793238462643383279,
  ""lit0"": null,
  ""lit1"": false,
  ""lit2"": true,
  ""ascii"": ""pizza"",
  ""escaped"": ""pizza"",
  ""utf8"": ""p\u00cdzza"",
  ""utf8ExtraEscape"": ""p\u00cdzza"",
  ""arr"": [
    ""hello"",
    ""sailor"",
    21,
    ""blackjack!""
  ],
  ""obj"": {
    ""arr"": [
      1,
      3,
      5,
      7,
      11
    ]
  }
}",
                "{\"int\":42,\"quadratic googol\":1e400,\"precisePi\":3.141592653589793238462643383279," +
                    "\"lit0\":null,\"lit1\":false,\"lit2\":true,\"ascii\":\"pizza\",\"escaped\":\"pizza\"," +
                    "\"utf8\":\"p\\u00cdzza\",\"utf8ExtraEscape\":\"p\\u00cdzza\"," +
                    "\"arr\":[\"hello\",\"sailor\",21,\"blackjack!\"]," +
                    "\"obj\":{\"arr\":[1,3,5,7,11]}}");
        }

        private static void WriteValue(
            bool indented,
            string jsonIn,
            string expectedIndent,
            string expectedMinimal)
        {
            var buffer = new ArrayBufferWriter<byte>(1024);
            using (JsonDocument doc = JsonDocument.Parse(jsonIn, s_options))
            {
                var options = new JsonWriterOptions
                {
                    Indented = indented,
                };

                var writer = new Utf8JsonWriter(buffer, options);
                writer.WriteStartObject();
                foreach (JsonProperty prop in doc.RootElement.EnumerateObject())
                {
                    prop.WriteTo(writer);
                }
                writer.WriteEndObject();
                writer.Flush();

                if (indented && s_replaceNewlines)
                {
                    AssertContents(
                        expectedIndent.Replace(CompiledNewline, Environment.NewLine),
                        buffer);
                }

                AssertContents(indented ? expectedIndent : expectedMinimal, buffer);
            }
        }

        private static void AssertContents(string expectedValue, ArrayBufferWriter<byte> buffer)
        {
            Assert.Equal(
                expectedValue,
                Encoding.UTF8.GetString(
                    buffer.WrittenSpan
#if netfx
                        .ToArray()
#endif
                    ));
        }

        [Theory]
        [InlineData("hello")]
        [InlineData("")]
        [InlineData(null)]
        public static void NameEquals_InvalidInstance_Throws(string text)
        {
            const string ErrorMessage = "Operation is not valid due to the current state of the object.";
            JsonProperty prop = default;
            AssertExtensions.Throws<InvalidOperationException>(() => prop.NameEquals(text), ErrorMessage);
            AssertExtensions.Throws<InvalidOperationException>(() => prop.NameEquals(text.AsSpan()), ErrorMessage);
            byte[] expectedGetBytes = text == null ? null : Encoding.UTF8.GetBytes(text);
            AssertExtensions.Throws<InvalidOperationException>(() => prop.NameEquals(expectedGetBytes), ErrorMessage);
        }

        [Theory]
        [InlineData("conne\\u0063tionId", "connectionId")]
        [InlineData("connectionId", "connectionId")]
        [InlineData("123", "123")]
        [InlineData("My name is \\\"Ahson\\\"", "My name is \"Ahson\"")]
        public static void NameEquals_UseGoodMatches_True(string propertyName, string otherText)
        {
            string jsonString = $"{{ \"{propertyName}\" : \"itsValue\" }}";
            using (JsonDocument doc = JsonDocument.Parse(jsonString))
            {
                JsonElement jElement = doc.RootElement;
                JsonProperty property = jElement.EnumerateObject().First();
                byte[] expectedGetBytes = Encoding.UTF8.GetBytes(otherText);
                Assert.True(property.NameEquals(otherText));
                Assert.True(property.NameEquals(otherText.AsSpan()));
                Assert.True(property.NameEquals(expectedGetBytes));
            }
        }

        [Fact]
        public static void NameEquals_GivenPropertyAndValue_TrueForPropertyName()
        {
            string jsonString = $"{{ \"aPropertyName\" : \"itsValue\" }}";
            using (JsonDocument doc = JsonDocument.Parse(jsonString))
            {
                JsonElement jElement = doc.RootElement;
                JsonProperty property = jElement.EnumerateObject().First();

                string text = "aPropertyName";
                byte[] expectedGetBytes = Encoding.UTF8.GetBytes(text);
                Assert.True(property.NameEquals(text));
                Assert.True(property.NameEquals(text.AsSpan()));
                Assert.True(property.NameEquals(expectedGetBytes));

                text = "itsValue";
                expectedGetBytes = Encoding.UTF8.GetBytes(text);
                Assert.False(property.NameEquals(text));
                Assert.False(property.NameEquals(text.AsSpan()));
                Assert.False(property.NameEquals(expectedGetBytes));
            }
        }
    }
}

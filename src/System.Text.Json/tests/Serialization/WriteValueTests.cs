// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Text.Encodings.Web;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static partial class WriteValueTests
    {
        [Fact]
        public static void NullWriterThrows()
        {
            Assert.Throws<ArgumentNullException>(() => JsonSerializer.Serialize(null, 1));
            Assert.Throws<ArgumentNullException>(() => JsonSerializer.Serialize(null, 1, typeof(int)));
        }

        [Fact]
        public static void CanWriteValueToJsonArray()
        {
            using MemoryStream memoryStream = new MemoryStream();
            using Utf8JsonWriter writer = new Utf8JsonWriter(memoryStream);

            writer.WriteStartObject();
            writer.WriteStartArray("test");
            JsonSerializer.Serialize<int>(writer, 1);
            writer.WriteEndArray();
            writer.WriteEndObject();
            writer.Flush();

            string json = Encoding.UTF8.GetString(memoryStream.ToArray());
            Assert.Equal("{\"test\":[1]}", json);
        }

        [Fact]
        public static void WriterOptionsWinIndented()
        {
            int[] input = new int[3] { 1, 2, 3 };

            var serializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream))
                {
                    JsonSerializer.Serialize(writer, input, serializerOptions);
                }
                Assert.Equal("[1,2,3]", Encoding.UTF8.GetString(stream.ToArray()));
            }

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = false }))
                {
                    JsonSerializer.Serialize(writer, input, serializerOptions);
                }
                Assert.Equal("[1,2,3]", Encoding.UTF8.GetString(stream.ToArray()));
            }

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }))
                {
                    JsonSerializer.Serialize(writer, input);
                }
                Assert.Equal($"[{Environment.NewLine}  1,{Environment.NewLine}  2,{Environment.NewLine}  3{Environment.NewLine}]", Encoding.UTF8.GetString(stream.ToArray()));
            }
        }

        [Fact]
        public static void WriterOptionsWinEncoder()
        {
            string input = "abcd+<>&";

            var serializerOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream))
                {
                    JsonSerializer.Serialize(writer, input, serializerOptions);
                }
                Assert.Equal("\"abcd\\u002B\\u003C\\u003E\\u0026\"", Encoding.UTF8.GetString(stream.ToArray()));
            }

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Encoder = JavaScriptEncoder.Default }))
                {
                    JsonSerializer.Serialize(writer, input, serializerOptions);
                }
                Assert.Equal("\"abcd\\u002B\\u003C\\u003E\\u0026\"", Encoding.UTF8.GetString(stream.ToArray()));
            }

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping }))
                {
                    JsonSerializer.Serialize(writer, input);
                }
                Assert.Equal("\"abcd+<>&\"", Encoding.UTF8.GetString(stream.ToArray()));
            }
        }

        [Fact]
        public static void WriterOptionsSkipValidation()
        {
            int[] input = new int[3] { 1, 2, 3 };

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream))
                {
                    writer.WriteStartObject();
                    Assert.Throws<JsonException>(() => JsonSerializer.Serialize(writer, input));
                }
            }

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { SkipValidation = true }))
                {
                    writer.WriteStartObject();
                    JsonSerializer.Serialize(writer, input);
                }
                Assert.Equal("{[1,2,3]", Encoding.UTF8.GetString(stream.ToArray()));
            }

            var serializerOptions = new JsonSerializerOptions
            {
                Converters = { new InvalidArrayConverter() },
            };

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream))
                {
                    Assert.Throws<JsonException>(() => JsonSerializer.Serialize(writer, input, serializerOptions));
                }
            }

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { SkipValidation = true }))
                {
                    JsonSerializer.Serialize(writer, input, serializerOptions);
                }
                Assert.Equal("[}", Encoding.UTF8.GetString(stream.ToArray()));
            }
        }

        public class InvalidArrayConverter : JsonConverter<int[]>
        {
            public override int[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }

            public override void Write(Utf8JsonWriter writer, int[] value, JsonSerializerOptions options)
            {
                writer.WriteStartArray();
                writer.WriteEndObject();
            }
        }

        [Fact]
        public static void OptionsFollowToConverter()
        {
            string json = "{\"type\":\"array\",\"array\":[1]}";
            string jsonFormatted =
@"{
  ""type"": ""array"",
  ""array"": [
    1
  ]
}";
            string expectedInner = "{\"array\":[1]}";

            var tempOptions = new JsonSerializerOptions();
            tempOptions.Converters.Add(new CustomConverter());
            DeepArray direct = JsonSerializer.Deserialize<DeepArray>(json, tempOptions);
            IContent custom = JsonSerializer.Deserialize<IContent>(json, tempOptions);

            {
                var options = new JsonSerializerOptions();
                options.Converters.Add(new CustomConverter());

                Assert.Equal(expectedInner, JsonSerializer.Serialize(direct, options));
                Assert.Equal(json, JsonSerializer.Serialize(custom, options));
            }

            {
                var options = new JsonSerializerOptions
                {
                    Converters = { new CustomConverter() }
                };
                WriteAndValidate(direct, typeof(DeepArray), expectedInner, options, writerOptions: default);
                WriteAndValidate(custom, typeof(IContent), json, options, writerOptions: default);
            }

            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Converters = { new CustomConverter() }
                };
                var writerOptions = new JsonWriterOptions { Indented = false };
                WriteAndValidate(direct, typeof(DeepArray), expectedInner, options, writerOptions);
                WriteAndValidate(custom, typeof(IContent), json, options, writerOptions);
            }

            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Converters = { new CustomConverter() }
                };
                WriteAndValidate(direct, typeof(DeepArray), expectedInner, options, writerOptions: default);
                WriteAndValidate(custom, typeof(IContent), json, options, writerOptions: default);
            }

            {
                var options = new JsonSerializerOptions
                {
                    Converters = { new CustomConverter() }
                };
                var writerOptions = new JsonWriterOptions { Indented = true };
                WriteAndValidate(direct, typeof(DeepArray), $"{{{Environment.NewLine}  \"array\": [{Environment.NewLine}    1{Environment.NewLine}  ]{Environment.NewLine}}}", options, writerOptions);
                WriteAndValidate(custom, typeof(IContent), jsonFormatted, options, writerOptions);
            }

            static void WriteAndValidate(object input, Type type, string expected, JsonSerializerOptions options, JsonWriterOptions writerOptions)
            {
                using (var stream = new MemoryStream())
                {
                    using (var writer = new Utf8JsonWriter(stream, writerOptions))
                    {
                        JsonSerializer.Serialize(writer, input, type, options);
                    }
                    Assert.Equal(expected, Encoding.UTF8.GetString(stream.ToArray()));
                }
            }
        }
    }
}

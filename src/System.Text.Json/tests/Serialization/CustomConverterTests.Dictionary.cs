// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static partial class CustomConverterTests
    {
        // Demonstrates custom Dictionary<string, long>; Adds offset to each integer or long to verify converter ran.
        private class DictionaryConverter : JsonConverter<Dictionary<string, long>>
        {
            private long _offset;

            public DictionaryConverter(long offset)
            {
                _offset = offset;
            }

            public override Dictionary<string, long> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException();
                }

                var value = new Dictionary<string, long>();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        return value;
                    }

                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException();
                    }

                    string key = reader.GetString();

                    reader.Read();
                    if (reader.TokenType != JsonTokenType.Number)
                    {
                        throw new JsonException();
                    }

                    long longValue = reader.GetInt64() + _offset;

                    value.Add(key, longValue);
                }

                throw new JsonException();
            }

            public override void Write(Utf8JsonWriter writer, Dictionary<string, long> value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();

                foreach (KeyValuePair<string, long> item in value)
                {
                    writer.WriteNumber(item.Key, item.Value - _offset);
                }

                writer.WriteEndObject();
            }
        }

        [Fact]
        public static void CustomDictionaryConverter()
        {
            const string json = @"{""Key1"":1,""Key2"":2}";

            var options = new JsonSerializerOptions();
            options.Converters.Add(new DictionaryConverter(10));

            {
                Dictionary<string, long> dictionary = JsonSerializer.Deserialize<Dictionary<string, long>>(json, options);
                Assert.Equal(11, dictionary["Key1"]);
                Assert.Equal(12, dictionary["Key2"]);

                string jsonSerialized = JsonSerializer.Serialize(dictionary, options);
                Assert.Equal(json, jsonSerialized);
            }
        }

        [Fact]
        public static void CustomDictionaryConverterContravariant()
        {
            const string json = @"{""Key1"":1,""Key2"":2}";

            var options = new JsonSerializerOptions();
            options.Converters.Add(new IDictionaryConverter(10));

            {
                Dictionary<string, long> dictionary = JsonSerializer.Deserialize<Dictionary<string, long>>(json, options);
                Assert.Equal(11, dictionary["Key1"]);
                Assert.Equal(12, dictionary["Key2"]);

                string jsonSerialized = JsonSerializer.Serialize(dictionary, options);
                Assert.Equal(json, jsonSerialized);
            }
        }

        [Fact]
        public static void ClassWithCustomDictionaryConverterTest()
        {
            const string json = @"{""MyInt"":32,""MyDictionary"":{""Key1"":1,""Key2"":2},""MyString"":""Hello""}";

            var options = new JsonSerializerOptions();
            options.Converters.Add(new IDictionaryConverter(10));

            {
                ClassWithCustomDictionaryConverter dictionary = JsonSerializer.Deserialize<ClassWithCustomDictionaryConverter>(json, options);
                Assert.Equal(11, dictionary.MyDictionary["Key1"]);
                Assert.Equal(12, dictionary.MyDictionary["Key2"]);
                Assert.Equal(32, dictionary.MyInt);
                Assert.Equal("Hello", dictionary.MyString);

                string jsonSerialized = JsonSerializer.Serialize(dictionary, options);
                Assert.Equal(json, jsonSerialized);
            }
        }

        private class ClassWithCustomDictionaryConverter
        {
            public int MyInt { get; set; }
            public Dictionary<string, long> MyDictionary { get; set; }
            public string MyString { get; set; }
        }

        private class IDictionaryConverter : JsonConverter<IDictionary<string, long>>
        {
            private long _offset;

            public IDictionaryConverter(long offset)
            {
                _offset = offset;
            }

            public override bool CanConvert(Type typeToConvert)
            {
                if (!typeToConvert.IsGenericType)
                    return false;

                Type generic = typeToConvert.GetGenericTypeDefinition();
                if (generic != typeof(Dictionary<,>))
                    return false;

                Type arg1 = typeToConvert.GetGenericArguments()[0];
                Type arg2 = typeToConvert.GetGenericArguments()[1];
                return arg1 == typeof(string) && arg2 == typeof(long);
            }

            public override IDictionary<string, long> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException();
                }

                var value = new Dictionary<string, long>();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        return value;
                    }

                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException();
                    }

                    string key = reader.GetString();

                    reader.Read();
                    if (reader.TokenType != JsonTokenType.Number)
                    {
                        throw new JsonException();
                    }

                    long longValue = reader.GetInt64() + _offset;

                    value.Add(key, longValue);
                }

                throw new JsonException();
            }

            public override void Write(Utf8JsonWriter writer, IDictionary<string, long> value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();

                foreach (KeyValuePair<string, long> item in value)
                {
                    writer.WriteNumber(item.Key, item.Value - _offset);
                }

                writer.WriteEndObject();
            }
        }
    }
}

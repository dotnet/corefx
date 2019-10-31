// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static class DictionaryKeyPolicyTests
    {
        [Fact]
        public static void CamelCaseDeserialize()
        {
            var options = new JsonSerializerOptions
            {
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase // e.g. Key1 -> key1.
            };

            const string JsonString = @"[{""Key1"":1,""Key2"":2},{""Key1"":3,""Key2"":4}]";

            // Without key policy, deserialize keys as they are.
            Dictionary<string, int>[] obj = JsonSerializer.Deserialize<Dictionary<string, int>[]>(JsonString);

            Assert.Equal(2, obj.Length);

            Assert.Equal(2, obj[0].Count);
            Assert.Equal(1, obj[0]["Key1"]);
            Assert.Equal(2, obj[0]["Key2"]);

            Assert.Equal(2, obj[1].Count);
            Assert.Equal(3, obj[1]["Key1"]);
            Assert.Equal(4, obj[1]["Key2"]);

            // Ensure we ignore key policy and deserialize keys as they are.
            obj = JsonSerializer.Deserialize<Dictionary<string, int>[]>(JsonString, options);

            Assert.Equal(2, obj.Length);

            Assert.Equal(2, obj[0].Count);
            Assert.Equal(1, obj[0]["Key1"]);
            Assert.Equal(2, obj[0]["Key2"]);

            Assert.Equal(2, obj[1].Count);
            Assert.Equal(3, obj[1]["Key1"]);
            Assert.Equal(4, obj[1]["Key2"]);
        }

        [Fact]
        public static void IgnoreKeyPolicyForExtensionData()
        {
            var options = new JsonSerializerOptions
            {
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase // e.g. Key1 -> key1.
            };

            // Ensure we ignore key policy for extension data and deserialize keys as they are.
            ClassWithExtensionData myClass = JsonSerializer.Deserialize<ClassWithExtensionData>(@"{""Key1"":1, ""Key2"":2}", options);
            Assert.Equal(1, (myClass.ExtensionData["Key1"]).GetInt32());
            Assert.Equal(2, (myClass.ExtensionData["Key2"]).GetInt32());

            // Ensure we ignore key policy for extension data and serialize keys as they are.
            const string expectedJson = @"{""Key1"":1,""Key2"":2}";
            const string expectedJsonReversed = @"{""Key2"":2,""Key1"":1}";

            string serialized = JsonSerializer.Serialize(myClass);
            Assert.True(expectedJson == serialized || expectedJsonReversed == serialized);
        }

        public class ClassWithExtensionData
        {
            [JsonExtensionData]
            public Dictionary<string, JsonElement> ExtensionData { get; set; }
        }

        [Fact]
        public static void CamelCaseSerialize()
        {
            var options = new JsonSerializerOptions()
            {
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase // e.g. Key1 -> key1.
            };

            Dictionary<string, int>[] obj = new Dictionary<string, int>[]
            {
                new Dictionary<string, int>() { { "Key1", 1 }, { "Key2", 2 } },
            };

            const string Json = @"[{""Key1"":1,""Key2"":2}]";
            const string JsonReversed = @"[{""Key2"":2,""Key1"":1}]";

            const string JsonCamel = @"[{""key1"":1,""key2"":2}]";
            const string JsonCamelReversed = @"[{""key2"":2,""key1"":1}]";

            // Without key policy option, serialize keys as they are.
            string json = JsonSerializer.Serialize<object>(obj);
            Assert.True(Json == json || JsonReversed == json);

            // With key policy option, serialize keys with camel casing.
            json = JsonSerializer.Serialize<object>(obj, options);
            Assert.True(JsonCamel == json || JsonCamelReversed == json);
        }

        [Fact]
        public static void CamelCaseSerialize_Null_Values()
        {
            var options = new JsonSerializerOptions()
            {
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase // e.g. Key1 -> key1.
            };

            Dictionary<string, string>[] obj = new Dictionary<string, string>[]
            {
                new Dictionary<string, string>() { { "Key1", null }, { "Key2", null } },
            };

            const string Json = @"[{""Key1"":null,""Key2"":null}]";
            const string JsonReversed = @"[{""Key2"":null,""Key1"":null}]";

            const string JsonCamel = @"[{""key1"":null,""key2"":null}]";
            const string JsonCamelReversed = @"[{""key2"":null,""key1"":null}]";

            // Without key policy option, serialize keys as they are.
            string json = JsonSerializer.Serialize<object>(obj);
            Assert.True(Json == json || JsonReversed == json);

            // With key policy option, serialize keys with camel casing.
            json = JsonSerializer.Serialize<object>(obj, options);
            Assert.True(JsonCamel == json || JsonCamelReversed == json);
        }

        [Fact]
        public static void CamelCaseSerialize_Null_Nullable_Values()
        {
            var options = new JsonSerializerOptions()
            {
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase // e.g. Key1 -> key1.
            };

            Dictionary<string, int?>[] obj = new Dictionary<string, int?>[]
            {
                new Dictionary<string, int?>() { { "Key1", null }, { "Key2", null } },
            };

            const string Json = @"[{""Key1"":null,""Key2"":null}]";
            const string JsonReversed = @"[{""Key2"":null,""Key1"":null}]";

            const string JsonCamel = @"[{""key1"":null,""key2"":null}]";
            const string JsonCamelReversed = @"[{""key2"":null,""key1"":null}]";

            // Without key policy option, serialize keys as they are.
            string json = JsonSerializer.Serialize<object>(obj);
            Assert.True(Json == json || JsonReversed == json);

            // With key policy option, serialize keys with camel casing.
            json = JsonSerializer.Serialize<object>(obj, options);
            Assert.True(JsonCamel == json || JsonCamelReversed == json);
        }

        [Fact]
        public static void CustomNameDeserialize()
        {
            var options = new JsonSerializerOptions
            {
                DictionaryKeyPolicy = new UppercaseNamingPolicy() // e.g. myint -> MYINT.
            };


            // Without key policy, deserialize keys as they are.
            Dictionary<string, int> obj = JsonSerializer.Deserialize<Dictionary<string, int>>(@"{""myint"":1}");
            Assert.Equal(1, obj["myint"]);

            // Ensure we ignore key policy and deserialize keys as they are.
            obj = JsonSerializer.Deserialize<Dictionary<string, int>>(@"{""myint"":1}", options);
            Assert.Equal(1, obj["myint"]);
        }

        [Fact]
        public static void CustomNameSerialize()
        {
            var options = new JsonSerializerOptions
            {
                DictionaryKeyPolicy = new UppercaseNamingPolicy() // e.g. myint -> MYINT.
            };

            Dictionary<string, int> obj = new Dictionary<string, int> { { "myint1", 1 } };

            const string Json = @"{""myint1"":1}";
            const string JsonCustomKey = @"{""MYINT1"":1}";

            // Without key policy option, serialize keys as they are.
            string json = JsonSerializer.Serialize<object>(obj);
            Assert.Equal(Json, json);

            // With key policy option, serialize keys honoring the custom key policy.
            json = JsonSerializer.Serialize<object>(obj, options);
            Assert.Equal(JsonCustomKey, json);
        }

        [Fact]
        public static void NullNamePolicy()
        {
            var options = new JsonSerializerOptions
            {
                DictionaryKeyPolicy = new NullNamingPolicy()
            };

            // A naming policy that returns null is not allowed.
            Assert.Throws<InvalidOperationException>(() => JsonSerializer.Serialize(new Dictionary<string, int> { { "onlyKey", 1 } }, options));

            // We don't use policy on deserialize, so we populate dictionary.
            Dictionary<string, int> obj = JsonSerializer.Deserialize<Dictionary<string, int>>(@"{""onlyKey"": 1}", options);

            Assert.Equal(1, obj.Count);
            Assert.Equal(1, obj["onlyKey"]);
        }

        [Fact]
        public static void CustomNameSerialize_NullableValue()
        {
            var options = new JsonSerializerOptions
            {
                DictionaryKeyPolicy = new UppercaseNamingPolicy() // e.g. myint -> MYINT.
            };

            Dictionary<string, int?> obj = new Dictionary<string, int?> { { "myint1", 1 } };

            const string Json = @"{""myint1"":1}";
            const string JsonCustomKey = @"{""MYINT1"":1}";

            // Without key policy option, serialize keys as they are.
            string json = JsonSerializer.Serialize<object>(obj);
            Assert.Equal(Json, json);

            // With key policy option, serialize keys honoring the custom key policy.
            json = JsonSerializer.Serialize<object>(obj, options);
            Assert.Equal(JsonCustomKey, json);
        }

        [Fact]
        public static void NullNamePolicy_NullableValue()
        {
            var options = new JsonSerializerOptions
            {
                DictionaryKeyPolicy = new NullNamingPolicy()
            };

            // A naming policy that returns null is not allowed.
            Assert.Throws<InvalidOperationException>(() => JsonSerializer.Serialize(new Dictionary<string, int?> { { "onlyKey", 1 } }, options));

            // We don't use policy on deserialize, so we populate dictionary.
            Dictionary<string, int?> obj = JsonSerializer.Deserialize<Dictionary<string, int?>>(@"{""onlyKey"": 1}", options);

            Assert.Equal(1, obj.Count);
            Assert.Equal(1, obj["onlyKey"]);
        }

        [Fact]
        public static void KeyConflict_Serialize_WriteAll()
        {
            var options = new JsonSerializerOptions
            {
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
            };

            // The camel case policy resolves two keys to the same output key.
            Dictionary<string, int> obj = new Dictionary<string, int> { { "myInt", 1 }, { "MyInt", 2 } };
            string json = JsonSerializer.Serialize(obj, options);

            // Check that we write all.
            string expectedJson = @"{""myInt"":1,""myInt"":2}";
            string expectedJsonReversed = @"{""myInt"":2,""myInt"":1}";

            Assert.True(expectedJson == json || expectedJsonReversed == json);
        }
    }
}

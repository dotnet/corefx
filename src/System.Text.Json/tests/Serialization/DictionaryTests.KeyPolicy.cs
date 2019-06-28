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
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
            };

            const string JsonString = @"[{""Key1"":1,""Key2"":2},{""Key1"":3,""Key2"":4}]";
            Dictionary<string, int>[] obj = JsonSerializer.Deserialize<Dictionary<string, int>[]>(JsonString, options);

            Assert.Equal(2, obj.Length);
            Assert.Equal(1, obj[0]["key1"]);
            Assert.Equal(2, obj[0]["key2"]);
            Assert.Equal(3, obj[1]["key1"]);
            Assert.Equal(4, obj[1]["key2"]);
        }

        [Fact]
        public static void CamelCaseSerialize()
        {
            var options = new JsonSerializerOptions()
            {
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
            };

            Dictionary<string, int>[] obj = new Dictionary<string, int>[]
            {
                new Dictionary<string, int>() { { "Key1", 1 }, { "Key2", 2 } },
                new Dictionary<string, int>() { { "Key1", 3 }, { "Key2", 4 } },
            };

            const string Json = @"[{""Key1"":1,""Key2"":2},{""Key1"":3,""Key2"":4}]";
            const string JsonCamel = @"[{""key1"":1,""key2"":2},{""key1"":3,""key2"":4}]";

            // Without key policy option, serialize keys as they are.
            string json = JsonSerializer.Serialize<object>(obj);
            Assert.Equal(Json, json);

            // With key policy option, serialize keys with camel casing.
            json = JsonSerializer.Serialize<object>(obj, options);
            Assert.Equal(JsonCamel, json);
        }

        [Fact]
        public static void CustomNameDeserialize()
        {
            var options = new JsonSerializerOptions
            {
                DictionaryKeyPolicy = new UppercaseNamingPolicy()
            };

            Dictionary<string, int> obj = JsonSerializer.Deserialize<Dictionary<string, int>>(@"{""myint"":1}", options);
            Assert.Equal(1, obj["MYINT"]);
        }

        [Fact]
        public static void CustomNameSerialize()
        {
            var options = new JsonSerializerOptions
            {
                DictionaryKeyPolicy = new UppercaseNamingPolicy()
            };

            Dictionary<string, int> obj = new Dictionary<string, int> { { "myint1", 1 }, { "myint2", 2 } };

            const string Json = @"{""myint1"":1,""myint2"":2}";
            const string JsonCustomKey = @"{""MYINT1"":1,""MYINT2"":2}";

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

            // A policy that returns null is not allowed.
            Assert.Throws<InvalidOperationException>(() => JsonSerializer.Deserialize<Dictionary<string, int>>(@"{""onlyKey"": 1}", options));
            Assert.Throws<InvalidOperationException>(() => JsonSerializer.Serialize(new Dictionary<string, int> { { "onlyKey", 1 } }, options));
        }
    }
}

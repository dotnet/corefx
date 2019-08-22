// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.IO;
using Xunit;

namespace System.Text.Json.Tests
{
    public static class JsonNodeTransformationsTests
    {
        private static string jsonSampleString = @"
            {
                ""text"": ""property value"",
                ""boolean true"": true,
                ""boolean false"": false,
                ""null"": null,
                ""int"": 17,
                ""double"": 3.14,
                ""scientific"": 3e100,
                ""array"" : [1,2,3],
                ""inner object"" : 
                {
                    ""inner property"" : ""value""
                }
            }";

        private static void CheckNodeFromSampleString(JsonNode node)
        {
            var jsonObject = (JsonObject)node;
            Assert.Equal(9, jsonObject.PropertyNames.Count);
            Assert.Equal(9, jsonObject.PropertyValues.Count);
            Assert.Equal("property value", (JsonString)jsonObject["text"]);
            Assert.True(((JsonBoolean)jsonObject["boolean true"]).Value);
            Assert.False(((JsonBoolean)jsonObject["boolean false"]).Value);
            Assert.Null(jsonObject["null"]);
            Assert.Equal(17, ((JsonNumber)jsonObject["int"]).GetInt32());
            Assert.Equal(3.14, ((JsonNumber)jsonObject["double"]).GetDouble());
            Assert.Equal("3e100", ((JsonNumber)jsonObject["scientific"]).ToString());

            var innerArray = (JsonArray)jsonObject["array"];
            Assert.Equal(3, innerArray.Count);
            Assert.Equal(1, ((JsonNumber)innerArray[0]).GetInt32());
            Assert.Equal(2, ((JsonNumber)innerArray[1]).GetInt32());
            Assert.Equal(3, ((JsonNumber)innerArray[2]).GetInt32());

            var innerObject = (JsonObject)jsonObject["inner object"];
            Assert.Equal(1, innerObject.PropertyNames.Count);
            Assert.Equal(1, innerObject.PropertyValues.Count);
            Assert.Equal("value", (JsonString)innerObject["inner property"]);
        }

        [Fact]
        public static void TestParseStringToJsonNode()
        {
            JsonNode node = JsonNode.Parse(jsonSampleString);
            CheckNodeFromSampleString(node);
        }

        [Fact]
        public static void TestCloneJsonArray()
        {
            var jsonArray = new JsonArray { "value1", "value2" };
            var jsonArrayCopy = jsonArray.Clone() as JsonArray;
            Assert.Equal(2, jsonArrayCopy.Count);
            jsonArray.Add("value3");
            Assert.Equal(2, jsonArrayCopy.Count);
        }

        [Fact]
        public static void TestCloneJsonNode()
        {
            var jsonObject = new JsonObject
            {
                { "text", "property value" },
                { "boolean", true },
                { "number", 15 },
                { "array", new JsonString[] { "value1", "value2"} }
            };

            var jsonObjectCopy = (JsonObject)jsonObject.Clone();

            jsonObject["text"] = (JsonString)"something different";
            Assert.Equal("property value", (JsonString)jsonObjectCopy["text"]);

            ((JsonBoolean)jsonObject["boolean"]).Value = false;
            Assert.True(((JsonBoolean)jsonObjectCopy["boolean"]).Value);

            Assert.Equal(2, jsonObjectCopy.GetJsonArrayPropertyValue("array").Count);
            jsonObject.GetJsonArrayPropertyValue("array").Add("value3");
            Assert.Equal(2, jsonObjectCopy.GetJsonArrayPropertyValue("array").Count);

            jsonObject.Add("new one", 123);
            Assert.Equal(4, jsonObjectCopy.PropertyNames.Count);
        }

        [Fact]
        public static void TestAsJsonElement()
        {
            var jsonObject = new JsonObject
            {
                { "text", "property value" },
                { "boolean", true },
                { "number", 15 },
                { "null node", (JsonNode) null },
                { "array", new JsonString[] { "value1", "value2"} }
            };

            JsonElement jsonElement = jsonObject.AsJsonElement();
            Assert.False(jsonElement.IsImmutable);

            JsonElement.ObjectEnumerator enumerator = jsonElement.EnumerateObject();

            enumerator.MoveNext();
            Assert.Equal("text", enumerator.Current.Name);
            Assert.Equal(JsonValueKind.String, enumerator.Current.Value.ValueKind);
            Assert.Equal("property value", enumerator.Current.Value.GetString());
           
            enumerator.MoveNext();
            Assert.Equal("boolean", enumerator.Current.Name);
            Assert.Equal(JsonValueKind.True, enumerator.Current.Value.ValueKind);
            Assert.True(enumerator.Current.Value.GetBoolean());
            
            enumerator.MoveNext();
            Assert.Equal("number", enumerator.Current.Name);
            Assert.Equal(15, enumerator.Current.Value.GetInt32());
            Assert.Equal(JsonValueKind.Number, enumerator.Current.Value.ValueKind);

            enumerator.MoveNext();
            Assert.Equal("null node", enumerator.Current.Name);
            Assert.Equal(JsonValueKind.Null, enumerator.Current.Value.ValueKind);

            enumerator.MoveNext();
            Assert.Equal("array", enumerator.Current.Name);
            Assert.Equal(2, enumerator.Current.Value.GetArrayLength());
            Assert.Equal(JsonValueKind.Array, enumerator.Current.Value.ValueKind);
            JsonElement.ArrayEnumerator innerEnumerator = enumerator.Current.Value.EnumerateArray();

            innerEnumerator.MoveNext();
            Assert.Equal(JsonValueKind.String, innerEnumerator.Current.ValueKind);
            Assert.Equal("value1", innerEnumerator.Current.GetString());
            
            innerEnumerator.MoveNext();
            Assert.Equal(JsonValueKind.String, innerEnumerator.Current.ValueKind);
            Assert.Equal("value2", innerEnumerator.Current.GetString());

            innerEnumerator.Dispose();
            enumerator.Dispose();

            // Modifying JsonObject will change JsonElement:

            jsonObject["text"] = new JsonNumber("123");
            Assert.Equal(123, jsonElement.GetProperty("text").GetInt32());
        }

        [Fact]
        public static void TestGetNode()
        {
            var jsonObject = new JsonObject
            {
                { "text", "property value" },
                { "boolean", true },
                { "number", 15 },
                { "null node", (JsonNode) null },
                { "array", new JsonString[] { "value1", "value2"} }
            };

            JsonElement jsonElement = jsonObject.AsJsonElement();
            JsonObject jsonObjectFromElement = (JsonObject)JsonNode.GetNode(jsonElement);

            Assert.Equal("property value", (JsonString)jsonObjectFromElement["text"]);

            // Modifying JsonObject will change JsonObjectFromElement:

            jsonObject["text"] = new JsonString("something different");
            Assert.Equal("something different", (JsonString)jsonObjectFromElement["text"]);

            // Modifying JsonObjectFromElement will change JsonObject:

            ((JsonBoolean)jsonObjectFromElement["boolean"]).Value = false;
            Assert.False(((JsonBoolean)jsonObject["boolean"]).Value);
        }
    }
}

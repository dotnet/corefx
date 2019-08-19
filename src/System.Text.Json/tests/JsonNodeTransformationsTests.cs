// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Text.Json.Tests
{
    public static class JsonNodeTransformationsTests
    {
        [Fact]
        public static void TestJsonDocumentToJsonNode()
        {
            var jsonString = @"
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

            JsonDocument jsonDocument = JsonDocument.Parse(jsonString);
            JsonNode node = JsonNode.DeepCopy(jsonDocument);

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
        public static void TestCloneJsonArray()
        {
            var jsonArray = new JsonArray { "value1", "value2" };
            var jsonArrayCopy = JsonNode.DeepCopy(jsonArray) as JsonArray;
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

            var jsonObjectCopy = (JsonObject)JsonNode.DeepCopy(jsonObject);
            
            jsonObject["text"] = (JsonString)"something different";
            Assert.Equal("property value", (JsonString)jsonObjectCopy["text"]);

            ((JsonBoolean)jsonObject["boolean"]).Value = false;
            Assert.True(((JsonBoolean)jsonObjectCopy["boolean"]).Value);

            Assert.Equal(2, jsonObjectCopy.GetJsonArrayProperty("array").Count);
            jsonObject.GetJsonArrayProperty("array").Add("value3");
            Assert.Equal(2, jsonObjectCopy.GetJsonArrayProperty("array").Count);

            jsonObject.Add("new one", 123);
            Assert.Equal(4, jsonObjectCopy.PropertyNames.Count);            
        }
    }
}

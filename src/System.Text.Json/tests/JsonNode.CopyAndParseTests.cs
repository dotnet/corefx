// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Json.Tests
{
    public static partial class JsonNodeTests
    {
        private static string jsonSampleString = @"
            {
                ""text"": ""property value"",
                ""boolean true"": true,
                ""boolean false"": false,
                ""null"": null,
                ""int"": 17,
                ""combo array"": 
                [                
                    {
                        ""inner property"" : ""value"",
                        ""simple array"" : [0, 2.2, 3.14],
                        ""empty object"": {},
                        ""nested object"":
                        {
                            ""empty array"" : [],
                            ""nested empty array"" : [[],[]]
                        }
                    }
                ],
                ""double"": 3.14,
                ""scientific"": 3e100,
                ""simple array"" : [1,2,3],
                ""inner object"" : 
                {
                    ""inner property"" : ""value""
                }
            }";

        [Fact]
        public static void TestParseStringToJsonNode()
        {
            JsonNode node = JsonNode.Parse(jsonSampleString);
            CheckNode(node);
        }

        [Fact]
        public static void TestDeepCopy()
        {
            using (JsonDocument document = JsonDocument.Parse(jsonSampleString))
            {
                JsonNode node = JsonNode.DeepCopy(document.RootElement);
                CheckNode(node);
            }
        }

        private static void CheckNode(JsonNode node)
        {
            var jsonObject = (JsonObject)node;
            Assert.Equal(10, jsonObject.PropertyNames.Count);
            Assert.Equal(10, jsonObject.PropertyValues.Count);
            Assert.Equal("property value", jsonObject["text"]);
            Assert.True(((JsonBoolean)jsonObject["boolean true"]).Value);
            Assert.False(((JsonBoolean)jsonObject["boolean false"]).Value);
            Assert.IsType<JsonNull>(jsonObject["null"]);
            Assert.Equal(17, jsonObject["int"]);
            Assert.Equal(3.14, jsonObject["double"]);
            Assert.Equal("3e100", ((JsonNumber)jsonObject["scientific"]).ToString());

            var innerArray = (JsonArray)jsonObject["simple array"];
            Assert.Equal(3, innerArray.Count);
            Assert.Equal(1, ((JsonNumber)innerArray[0]).GetInt32());
            Assert.Equal(2, ((JsonNumber)innerArray[1]).GetInt32());
            Assert.Equal(3, ((JsonNumber)innerArray[2]).GetInt32());

            var innerObject = (JsonObject)jsonObject["inner object"];
            Assert.Equal(1, innerObject.PropertyNames.Count);
            Assert.Equal(1, innerObject.PropertyValues.Count);
            Assert.Equal("value", innerObject["inner property"]);

            var comboObject = (JsonObject)jsonObject.GetJsonArrayPropertyValue("combo array")[0];
            Assert.Equal(4, comboObject.PropertyNames.Count);
            Assert.Equal(4, comboObject.PropertyValues.Count);
            Assert.Equal("value", comboObject["inner property"]);
            Assert.Equal(0, comboObject.GetJsonObjectPropertyValue("empty object").PropertyNames.Count);
            Assert.Equal(3, comboObject.GetJsonArrayPropertyValue("simple array").Count);
            var nestedObject = (JsonObject)comboObject["nested object"];
            Assert.Equal(0, nestedObject.GetJsonArrayPropertyValue("empty array").Count);
            Assert.Equal(2, nestedObject.GetJsonArrayPropertyValue("nested empty array").Count);
        }

        [Fact]
        public static void TestParseDoesNotOverflow()
        {
            var builder = new StringBuilder();
            for (int i = 0; i < 2_000; i++)
            {
                builder.Append("[");
            }

            for (int i = 0; i < 2_000; i++)
            {
                builder.Append("]");
            }

            JsonNode jsonNode = JsonNode.Parse(builder.ToString());
        }

        [Fact]
        public static void TestDeepCopyDoesNotOverflow()
        {
            var builder = new StringBuilder();
            for (int i = 0; i < 2_000; i++)
            {
                builder.Append("[");
            }

            for (int i = 0; i < 2_000; i++)
            {
                builder.Append("]");
            }

            var options = new JsonDocumentOptions { MaxDepth = 5_000 };
            using (JsonDocument dom = JsonDocument.Parse(builder.ToString(), options))
            {
                JsonNode node = JsonNode.DeepCopy(dom.RootElement);
            }
        }
    }
}

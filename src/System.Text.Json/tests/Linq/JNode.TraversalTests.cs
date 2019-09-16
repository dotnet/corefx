// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Json.Linq.Tests
{
    public static partial class JNodeTests
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
        public static void TestParseStringToJNode()
        {
            JNode node = JNode.Parse(jsonSampleString);
            CheckNode(node);
        }

        [Fact]
        public static void TestDeepCopy()
        {
            using (JsonDocument document = JsonDocument.Parse(jsonSampleString))
            {
                JNode node = JNode.DeepCopy(document.RootElement);
                CheckNode(node);
            }
        }

        private static void CheckNode(JNode node)
        {
            var jsonObject = (JObject)node;
            Assert.Equal(10, jsonObject.GetPropertyNames().Count);
            Assert.Equal(10, jsonObject.GetPropertyValues().Count);
            Assert.Equal("property value", jsonObject["text"]);
            Assert.True(((JBoolean)jsonObject["boolean true"]).Value);
            Assert.False(((JBoolean)jsonObject["boolean false"]).Value);
            Assert.IsType<JNull>(jsonObject["null"]);
            Assert.Equal(17, jsonObject["int"]);
            Assert.Equal(3.14, jsonObject["double"]);
            Assert.Equal("3e100", ((JNumber)jsonObject["scientific"]).ToString());

            var innerArray = (JArray)jsonObject["simple array"];
            Assert.Equal(3, innerArray.Count);
            Assert.Equal(1, ((JNumber)innerArray[0]).GetInt32());
            Assert.Equal(2, ((JNumber)innerArray[1]).GetInt32());
            Assert.Equal(3, ((JNumber)innerArray[2]).GetInt32());

            var innerObject = (JObject)jsonObject["inner object"];
            Assert.Equal(1, innerObject.GetPropertyNames().Count);
            Assert.Equal(1, innerObject.GetPropertyValues().Count);
            Assert.Equal("value", innerObject["inner property"]);

            var comboObject = (JObject)jsonObject.GetJsonArrayPropertyValue("combo array")[0];
            Assert.Equal(4, comboObject.GetPropertyNames().Count);
            Assert.Equal(4, comboObject.GetPropertyValues().Count);
            Assert.Equal("value", comboObject["inner property"]);
            Assert.Equal(0, comboObject.GetJsonObjectPropertyValue("empty object").GetPropertyNames().Count);
            Assert.Equal(3, comboObject.GetJsonArrayPropertyValue("simple array").Count);
            var nestedObject = (JObject)comboObject["nested object"];
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

            var options = new JNodeOptions { MaxDepth = 5_000 };
            JNode jsonNode = JNode.Parse(builder.ToString(), options);
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
                JNode node = JNode.DeepCopy(dom.RootElement);
            }
        }

        [Fact]
        public static void TestToJsonString()
        {
            var jsonObject = new JObject()
            {
                { "text", "property value" },
                { "boolean true", true },
                {  "boolean false", false },
                {  "null", null },
                {  "int", 17 },
                {
                    "combo array", new JArray()
                    {
                        new JObject()
                        {
                            { "inner property", "value" },
                            { "simple array", new JArray() { 0, 2.2, 3.14 } },
                            { "empty object", new JObject() },
                            { "nested object", new JObject
                                {
                                    {  "empty array", new JArray() },
                                    {  "nested empty array", new JArray() { new JArray(), new JArray()} }
                                }
                            }
                        }
                    }
                },
                { "double", 3.14 },
                { "scientific", new JNumber("3e100") },
                { "simple array", new JArray() { 1,2,3 } },
                { "inner object", new JObject()
                    {
                        { "inner property", "value" }
                    }
                }
            };

            string json = jsonObject.ToJsonString();
            JNode node = JNode.Parse(json);
            CheckNode(node);
        }

        [Fact]
        public static void TestParseWithDuplicates()
        {
            var stringWithDuplicates = @"
            {
                ""property"": ""first value"",
                ""different property"": ""value"",
                ""property"": ""duplicate value"",
                ""property"": ""last duplicate value""
            }";

            var jsonObject = (JObject)JNode.Parse(stringWithDuplicates);
            Assert.Equal(2, jsonObject.GetPropertyNames().Count);
            Assert.Equal(2, jsonObject.GetPropertyValues().Count);
            Assert.Equal("last duplicate value", jsonObject["property"]);

            jsonObject = (JObject) JNode.Parse(stringWithDuplicates, new JNodeOptions() { DuplicatePropertyNameHandling = DuplicatePropertyNameHandlingStrategy.Replace });
            Assert.Equal(2, jsonObject.GetPropertyNames().Count);
            Assert.Equal(2, jsonObject.GetPropertyValues().Count);
            Assert.Equal("last duplicate value", jsonObject["property"]);

            jsonObject = (JObject)JNode.Parse(stringWithDuplicates, new JNodeOptions() { DuplicatePropertyNameHandling = DuplicatePropertyNameHandlingStrategy.Ignore });
            Assert.Equal(2, jsonObject.GetPropertyNames().Count);
            Assert.Equal(2, jsonObject.GetPropertyValues().Count);
            Assert.Equal("first value", jsonObject["property"]);

            Assert.Throws<ArgumentException>(() => JNode.Parse(stringWithDuplicates, new JNodeOptions() { DuplicatePropertyNameHandling = DuplicatePropertyNameHandlingStrategy.Error }));
        }
    }
}

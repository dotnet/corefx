﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public class JsonElementTests
    {
        [Fact]
        public void SerializeJsonElement()
        {
            var obj = JsonSerializer.Parse<JsonElementClass>(JsonElementClass.s_json);
            obj.Verify();
            string reserialized = JsonSerializer.ToString(obj);

            // Properties in the exported json will be in the order that they were reflected, doing a quick check to see that
            // we end up with the same length (i.e. same amount of data) to start.
            Assert.Equal(JsonElementClass.s_json.Replace(" ", string.Empty).Length, reserialized.Length);

            // Shoving it back through the parser should validate round tripping.
            obj = JsonSerializer.Parse<JsonElementClass>(reserialized);
            obj.Verify();
        }

        public class JsonElementClass : ITestClass
        {
            public JsonElement Number { get; set; }
            public JsonElement True { get; set; }
            public JsonElement False { get; set; }
            public JsonElement String { get; set; }

            // Currently does not work
            // public JsonElement Array { get; set; }
            // public JsonElement Object { get; set; }

            public static readonly string s_json =
                @"{" +
                    @"""Number"" : 1," +
                    @"""True"" : true," +
                    @"""False"" : false," +
                    @"""String"" : ""Hello""" +
                    // @"""Array"" : [2, false, true, ""Goodbye""]," +
                    // @"""Object"" : {}" +
                @"}";

            public static readonly byte[] s_data = Encoding.UTF8.GetBytes(s_json);

            public void Initialize()
            {
                Number = JsonDocument.Parse(@"1").RootElement.Clone();
                True = JsonDocument.Parse(@"true").RootElement.Clone();
                False = JsonDocument.Parse(@"false").RootElement.Clone();
                String = JsonDocument.Parse(@"""Hello""").RootElement.Clone();
                // Array = JsonDocument.Parse(@"[2, false, true, ""Goodbye""]").RootElement.Clone();
                // Object = JsonDocument.Parse(@"{}").RootElement.Clone();
            }

            public void Verify()
            {
                Assert.Equal(JsonValueType.Number, Number.Type);
                Assert.Equal("1", Number.ToString());
                Assert.Equal(JsonValueType.True, True.Type);
                Assert.Equal("True", True.ToString());
                Assert.Equal(JsonValueType.False, False.Type);
                Assert.Equal("False", False.ToString());
                Assert.Equal(JsonValueType.String, String.Type);
                Assert.Equal("Hello", String.ToString());
            }
        }

        [Fact]
        public void SerializeJsonElementArray()
        {
            var obj = JsonSerializer.Parse<JsonElementArrayClass>(JsonElementArrayClass.s_json);
            obj.Verify();
            string reserialized = JsonSerializer.ToString(obj);

            // Properties in the exported json will be in the order that they were reflected, doing a quick check to see that
            // we end up with the same length (i.e. same amount of data) to start.
            Assert.Equal(JsonElementArrayClass.s_json.Replace(" ", string.Empty).Length, reserialized.Length);

            // Shoving it back through the parser should validate round tripping.
            obj = JsonSerializer.Parse<JsonElementArrayClass>(reserialized);
            obj.Verify();
        }

        public class JsonElementArrayClass : ITestClass
        {
            public JsonElement[] Array { get; set; }

            public static readonly string s_json =
                @"{" +
                    @"""Array"" : [" +
                        @"1, " +
                        @"true, " +
                        @"false, " +
                        @"""Hello""" +
                    @"]" +
                    // @"""Array"" : [2, false, true, ""Goodbye""]," +
                    // @"""Object"" : {}" +
                @"}";

            public static readonly byte[] s_data = Encoding.UTF8.GetBytes(s_json);

            public void Initialize()
            {
                Array = new JsonElement[]
                {
                    JsonDocument.Parse(@"1").RootElement.Clone(),
                    JsonDocument.Parse(@"true").RootElement.Clone(),
                    JsonDocument.Parse(@"false").RootElement.Clone(),
                    JsonDocument.Parse(@"""Hello""").RootElement.Clone()
                };
            }

            public void Verify()
            {
                Assert.Equal(JsonValueType.Number, Array[0].Type);
                Assert.Equal("1", Array[0].ToString());
                Assert.Equal(JsonValueType.True, Array[1].Type);
                Assert.Equal("True", Array[1].ToString());
                Assert.Equal(JsonValueType.False, Array[2].Type);
                Assert.Equal("False", Array[2].ToString());
                Assert.Equal(JsonValueType.String, Array[3].Type);
                Assert.Equal("Hello", Array[3].ToString());
            }
        }
    }
}

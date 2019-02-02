// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace System.Text.Json.Tests
{
    public partial class Utf8JsonWriterTests
    {
        // {"":[],"":{}}
        // TODO: Check coverage and add more & negative test cases
        [Theory]
        [InlineData("12345")]
        [InlineData("true")]
        [InlineData("false")]
        [InlineData("null")]
        [InlineData("\"abcde\"")]
        public void WriteJsonElementSingleValue(string jsonString)
        {
            var output = new FixedSizedBufferWriter(1_024);
            var document = JsonDocument.Parse(jsonString);
            var jsonUtf8 = new Utf8JsonWriter(output, state: default);

            jsonUtf8.WriteElementValue(document.RootElement);
            jsonUtf8.Flush(isFinalBlock: true);

            string actualStr = Encoding.UTF8.GetString(output.Formatted);
            Assert.Equal(jsonString, actualStr);

            output.Clear();
            jsonUtf8 = new Utf8JsonWriter(output, state: default);

            JsonElement root = document.RootElement;
            switch (root.Type)
            {
                case JsonValueType.False:
                    jsonUtf8.WriteBooleanValue(false);
                    break;
                case JsonValueType.Null:
                    jsonUtf8.WriteNullValue();
                    break;
                case JsonValueType.Number:
                    jsonUtf8.WriteNumberValue(root.GetInt32());
                    break;
                case JsonValueType.String:
                    jsonUtf8.WriteStringValue(root.GetString());
                    break;
                case JsonValueType.True:
                    jsonUtf8.WriteBooleanValue(true);
                    break;
                case JsonValueType.Undefined:
                case JsonValueType.Array:
                case JsonValueType.Object:
                    Debug.Fail($"Unespected value type `{root.Type}` for single value json.");
                    break;
            }
            jsonUtf8.Flush(isFinalBlock: true);

            actualStr = Encoding.UTF8.GetString(output.Formatted);
            Assert.Equal(jsonString, actualStr);
        }

        [Fact]
        public void WriteJsonElementSingleComplexValue()
        {
            {
                string jsonString = "{}";
                var output = new FixedSizedBufferWriter(1_024);
                var document = JsonDocument.Parse(jsonString);
                var jsonUtf8 = new Utf8JsonWriter(output, state: default);

                jsonUtf8.WriteElementValue(document.RootElement);
                jsonUtf8.Flush(isFinalBlock: true);

                string actualStr = Encoding.UTF8.GetString(output.Formatted);
                Assert.Equal(jsonString, actualStr);
            }

            {
                string jsonString = "[]";
                var output = new FixedSizedBufferWriter(1_024);
                var document = JsonDocument.Parse(jsonString);
                var jsonUtf8 = new Utf8JsonWriter(output, state: default);

                jsonUtf8.WriteElementValue(document.RootElement);
                jsonUtf8.Flush(isFinalBlock: true);

                string actualStr = Encoding.UTF8.GetString(output.Formatted);
                Assert.Equal(jsonString, actualStr);
            }
        }

        [Fact]
        public void WriteJsonElementRoot()
        {
            string jsonString = "[42, {\"property\": \"value\"}, null, true, false, \"string\", {\"property\": [], \"another\": [1, 2, 3]}]";
            var output = new FixedSizedBufferWriter(1_024);
            var document = JsonDocument.Parse(jsonString);
            var jsonUtf8 = new Utf8JsonWriter(output, state: default);

            jsonUtf8.WriteElementValue(document.RootElement);
            jsonUtf8.Flush(isFinalBlock: true);

            string actualStr = Encoding.UTF8.GetString(output.Formatted);
            Assert.Equal(jsonString, actualStr);
        }

        [Fact]
        public void WriteJsonElementEnumerateArray()
        {
            string jsonString = "[42,{\"property\": \"value\"},null,true,false,\"string\",{\"property\":[], \"another\": [1, 2,3]}]";

            var output = new FixedSizedBufferWriter(1_024);
            var document = JsonDocument.Parse(jsonString);
            var jsonUtf8 = new Utf8JsonWriter(output, state: default);

            JsonElement root = document.RootElement;
            jsonUtf8.WriteStartArray();
            foreach (JsonElement element in root.EnumerateArray())
            {
                jsonUtf8.WriteElementValue(element);
            }
            jsonUtf8.WriteEndArray();
            jsonUtf8.Flush(isFinalBlock: true);

            string actualStr = Encoding.UTF8.GetString(output.Formatted);
            Assert.Equal(jsonString, actualStr);
        }

        [Fact]
        public void WriteJsonElementEnumerateObject()
        {
            string jsonString = "{\"number\":42,\"nested\":{\"property\": \"value\" },\"null\":null,\"true\":true,\"false\":false,\"string\":\"string\",\"more nests\":{\"property\": [], \"another\": [1, 2, 3]}}";

            var output = new FixedSizedBufferWriter(1_024);
            var document = JsonDocument.Parse(jsonString);
            var jsonUtf8 = new Utf8JsonWriter(output, state: default);

            JsonElement root = document.RootElement;
            jsonUtf8.WriteStartObject();
            foreach (JsonProperty element in root.EnumerateObject())
            {
                jsonUtf8.WriteElement(element.Name, element.Value);
            }
            jsonUtf8.WriteEndObject();
            jsonUtf8.Flush(isFinalBlock: true);

            string actualStr = Encoding.UTF8.GetString(output.Formatted);
            Assert.Equal(jsonString, actualStr);
        }

        [Fact]
        public void WriteJsonElementEnumerateNested()
        {
            string jsonString = "[42,{\"property\":\"value\"},null,true,false,\"string\",{\"property\":[],\"another\":[1,2,3]}]";
            var output = new FixedSizedBufferWriter(1_024);
            var document = JsonDocument.Parse(jsonString);
            var jsonUtf8 = new Utf8JsonWriter(output, state: default);

            JsonElement root = document.RootElement;
            jsonUtf8.WriteStartArray();
            jsonUtf8.WriteElementValue(root[0]);
            JsonElement subObject = root[1];
            jsonUtf8.WriteStartObject();
            jsonUtf8.WriteElement("property", subObject.GetProperty("property"), escape: false);
            jsonUtf8.WriteEndObject();
            jsonUtf8.WriteElementValue(root[2]);
            jsonUtf8.WriteElementValue(root[3]);
            jsonUtf8.WriteElementValue(root[4]);
            jsonUtf8.WriteElementValue(root[5]);
            JsonElement anotherSubObject = root[6];
            jsonUtf8.WriteStartObject();
            jsonUtf8.WriteElement("property", anotherSubObject.GetProperty("property"), escape: true);
            JsonElement subArray = anotherSubObject.GetProperty("another");
            jsonUtf8.WriteStartArray("another");
            jsonUtf8.WriteElementValue(subArray[0]);
            jsonUtf8.WriteElementValue(subArray[1]);
            jsonUtf8.WriteElementValue(subArray[2]);
            jsonUtf8.WriteEndArray();
            jsonUtf8.WriteEndObject();
            jsonUtf8.WriteEndArray();
            jsonUtf8.Flush(isFinalBlock: true);

            string actualStr = Encoding.UTF8.GetString(output.Formatted);
            Assert.Equal(jsonString, actualStr);
        }

        private static string ExpectedString(string inputString, bool prettyPrint)
        {
            using (var jsonReader = new JsonTextReader(new StringReader(inputString)))
            {
                JToken obj = JToken.ReadFrom(jsonReader);
                var stringWriter = new StringWriter();

                using (var jsonWriter = new JsonTextWriter(stringWriter))
                {
                    jsonWriter.Formatting = prettyPrint ? Formatting.Indented : Formatting.None;
                    obj.WriteTo(jsonWriter);
                    return stringWriter.ToString();
                }
            }
        }
    }
}

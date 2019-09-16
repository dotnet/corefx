// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Json.Linq.Tests
{
    public static partial class JNodeTests
    { 
        [Fact]
        public static void TestCloneJArray()
        {
            var jsonArray = new JArray { "value1", "value2" };
            var jsonArrayCopy = jsonArray.Clone() as JArray;
            Assert.Equal(2, jsonArrayCopy.Count);
            jsonArray.Add("value3");
            Assert.Equal(2, jsonArrayCopy.Count);
        }

        [Fact]
        public static void TestDeepCloneJArray()
        {
            JArray inner = new JArray { 1, 2, 3 };
            JArray outer = new JArray { inner };
            JArray outerClone = (JArray)outer.Clone();
            ((JArray) outerClone[0]).Add(4);

            Assert.Equal(3, inner.Count);
        }

        [Fact]
        public static void TestCloneJNode()
        {
            var jsonObject = new JObject
            {
                { "text", "property value" },
                { "boolean", true },
                { "number", 15 },
                { "array", new JArray { "value1", "value2"} },
                { "null", null }
            };

            var jsonObjectCopy = (JObject)jsonObject.Clone();
            Assert.Equal(5, jsonObjectCopy.GetPropertyNames().Count);
            Assert.Equal(5, jsonObjectCopy.GetPropertyValues().Count);

            jsonObject["text"] = "something different";
            Assert.Equal("property value", jsonObjectCopy["text"]);

            ((JBoolean)jsonObject["boolean"]).Value = false;
            Assert.True(((JBoolean)jsonObjectCopy["boolean"]).Value);

            Assert.Equal(2, jsonObjectCopy.GetJsonArrayPropertyValue("array").Count);
            jsonObject.GetJsonArrayPropertyValue("array").Add("value3");
            Assert.Equal(2, jsonObjectCopy.GetJsonArrayPropertyValue("array").Count);

            Assert.IsType<JNull>(jsonObjectCopy["null"]);

            jsonObject.Add("new one", 123);
            Assert.Equal(5, jsonObjectCopy.GetPropertyNames().Count);
        }

        [Fact]
        public static void TestCloneJNodeInJsonElement()
        {
            var jsonObject = new JObject
            {
                { "text", "value" },
                { "boolean", true },
                { "array", new JArray { "value1", "value2"} }
            };

            JsonElement jsonElement = jsonObject.AsJsonElement();

            var jsonObjectCloneFromElement = (JObject)JNode.DeepCopy(jsonElement);

            Assert.Equal(3, jsonObjectCloneFromElement.GetPropertyNames().Count);
            Assert.Equal(3, jsonObjectCloneFromElement.GetPropertyValues().Count);

            Assert.Equal("value", jsonObjectCloneFromElement["text"]);
            Assert.Equal(true, jsonObjectCloneFromElement["boolean"]);

            // Modifying should not change the clone and vice versa:
            
            jsonObjectCloneFromElement["boolean"] = false;
            Assert.Equal(false, jsonObjectCloneFromElement["boolean"]);
            Assert.Equal(true, jsonObject["boolean"]);

            jsonObjectCloneFromElement.GetJsonArrayPropertyValue("array").Add("value3");
            Assert.Equal(3, jsonObjectCloneFromElement.GetJsonArrayPropertyValue("array").Count);
            Assert.Equal(2, jsonObject.GetJsonArrayPropertyValue("array").Count);

            jsonObject["text"] = "different value";
            Assert.Equal("different value", jsonObject["text"]);
            Assert.Equal("value", jsonObjectCloneFromElement["text"]);

            jsonObject.GetJsonArrayPropertyValue("array").Remove("value2");
            Assert.Equal(1, jsonObject.GetJsonArrayPropertyValue("array").Count);
            Assert.Equal(3, jsonObjectCloneFromElement.GetJsonArrayPropertyValue("array").Count);
        }
    }
}

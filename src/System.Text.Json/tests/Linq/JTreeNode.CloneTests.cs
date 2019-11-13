// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Json.Linq.Tests
{
    public static partial class JTreeNodeTests
    { 
        [Fact]
        public static void TestCloneJTreeArray()
        {
            var jsonArray = new JTreeArray { "value1", "value2" };
            var jsonArrayCopy = jsonArray.Clone() as JTreeArray;
            Assert.Equal(2, jsonArrayCopy.Count);
            jsonArray.Add("value3");
            Assert.Equal(2, jsonArrayCopy.Count);
        }

        [Fact]
        public static void TestDeepCloneJTreeArray()
        {
            JTreeArray inner = new JTreeArray { 1, 2, 3 };
            JTreeArray outer = new JTreeArray { inner };
            JTreeArray outerClone = (JTreeArray)outer.Clone();
            ((JTreeArray) outerClone[0]).Add(4);

            Assert.Equal(3, inner.Count);
        }

        [Fact]
        public static void TestCloneJTreeNode()
        {
            var jsonObject = new JTreeObject
            {
                { "text", "property value" },
                { "boolean", true },
                { "number", 15 },
                { "array", new JTreeArray { "value1", "value2"} },
                { "null", null }
            };

            var jsonObjectCopy = (JTreeObject)jsonObject.Clone();
            Assert.Equal(5, jsonObjectCopy.GetPropertyNames().Count);
            Assert.Equal(5, jsonObjectCopy.GetPropertyValues().Count);

            jsonObject["text"] = "something different";
            Assert.Equal("property value", jsonObjectCopy["text"]);

            ((JTreeBoolean)jsonObject["boolean"]).Value = false;
            Assert.True(((JTreeBoolean)jsonObjectCopy["boolean"]).Value);

            Assert.Equal(2, jsonObjectCopy.GetJsonArrayPropertyValue("array").Count);
            jsonObject.GetJsonArrayPropertyValue("array").Add("value3");
            Assert.Equal(2, jsonObjectCopy.GetJsonArrayPropertyValue("array").Count);

            Assert.IsType<JTreeNull>(jsonObjectCopy["null"]);

            jsonObject.Add("new one", 123);
            Assert.Equal(5, jsonObjectCopy.GetPropertyNames().Count);
        }

        [Fact]
        public static void TestCloneJTreeNodeInJsonElement()
        {
            var jsonObject = new JTreeObject
            {
                { "text", "value" },
                { "boolean", true },
                { "array", new JTreeArray { "value1", "value2"} }
            };

            JsonElement jsonElement = jsonObject.AsJsonElement();

            var jsonObjectCloneFromElement = (JTreeObject)JTreeNode.DeepCopy(jsonElement);

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

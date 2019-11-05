// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static partial class DefaultTests
    {
        [Fact]
        public static void DefaultIgnoreDefaultValuesOnWrite()
        {
            var obj = new TestClassWithInitializedDefaultValueProperties
            {
                MyString = "Hello",
                MyInt = 1,
                MyDateTime = null,
                MyIntArray = null,
                MyIntList = null,
                MyStringDictionary = new Dictionary<string, string>() { ["key"] = "value" }
            };

            string json = JsonSerializer.Serialize(obj);
            Assert.Contains(@"""MyString"":""Hello""", json);
            Assert.Contains(@"""MyInt"":1", json);
            Assert.Contains(@"""MyDateTime"":null", json);
            Assert.Contains(@"""MyIntArray"":null", json);
            Assert.Contains(@"""MyIntList"":null", json);
        }

        [Fact]
        public static void EnableIgnoreDefaultValuesOnWrite()
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                IgnoreDefaultValues = true
            };

            var obj = new TestClassWithInitializedDefaultValueProperties
            {
                MyInt = 1,
                MyDateTime = null,
                MyIntArray = new int[] { 1 },
                MyIntList = new List<int> { 1 },
                MyStringDictionary = new Dictionary<string, string>() { ["key"] = null }
            };

            string json = JsonSerializer.Serialize(obj, options);

            // Roundtrip to verify serialize is accurate.
            TestClassWithInitializedProperties newObj = JsonSerializer.Deserialize<TestClassWithInitializedProperties>(json);
            // Assert.Null(newObj.MyDateTime);
            Assert.Equal(1, newObj.MyIntArray[0]);
            Assert.Equal(1, newObj.MyIntList[0]);
            Assert.Null(newObj.MyStringDictionary["key"]);

            var parentObj = new WrapperForTestClassWithInitializedDefaultValueProperties
            {
                MyClass = obj
            };
            json = JsonSerializer.Serialize(parentObj, options);

            // Roundtrip to ensure serialize is accurate.
            WrapperForTestClassWithInitializedDefaultValueProperties newParentObj = JsonSerializer.Deserialize<WrapperForTestClassWithInitializedDefaultValueProperties>(json);
            TestClassWithInitializedDefaultValueProperties nestedObj = newParentObj.MyClass;
            Assert.Null(nestedObj.MyString);
            Assert.Null(nestedObj.MyInt);
            Assert.Null(nestedObj.MyDateTime);
            Assert.Equal(1, nestedObj.MyIntArray[0]);
            Assert.Equal(1, nestedObj.MyIntList[0]);
            Assert.Null(nestedObj.MyStringDictionary["key"]);
        }

        class WrapperForTestClassWithInitializedDefaultValueProperties
        {
            public TestClassWithInitializedDefaultValueProperties MyClass { get; set; }
        }
    }
}

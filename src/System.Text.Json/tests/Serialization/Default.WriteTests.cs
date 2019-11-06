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
            Assert.Contains(@"""MyStringDictionary"":{""key"":""value""}", json);
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
                MyString = "Hello",
                MyInt = 1,
                MyDateTime = null,
                MyIntArray = new int[] { 1 },
                MyIntList = new List<int> { 1 },
                MyStringDictionary = new Dictionary<string, string>() { ["key"] = null }
            };

            string json = JsonSerializer.Serialize(obj, options);
                
            Assert.DoesNotContain(@"""MyString"":""Hello""", json);
            Assert.DoesNotContain(@"""MyInt"":1", json);
            Assert.DoesNotContain(@"""MyDateTime"":null", json);
            Assert.Contains(@"""MyIntArray"":[1]", json);
            Assert.Contains(@"""MyIntList"":[1]", json);
            Assert.Contains(@"""MyStringDictionary"":{""key"":null}", json);

            var parentObj = new WrapperForTestClassWithInitializedDefaultValueProperties
            {
                MyClass = obj
            };
            json = JsonSerializer.Serialize(parentObj, options);

            Assert.DoesNotContain(@"""MyString"":""Hello""", json);
            Assert.DoesNotContain(@"""MyInt"":1", json);
            Assert.DoesNotContain(@"""MyDateTime"":null", json);
            Assert.Contains(@"""MyIntArray"":[1]", json);
            Assert.Contains(@"""MyIntList"":[1]", json);
            Assert.Contains(@"""MyStringDictionary"":{""key"":null}", json);
        }

        class WrapperForTestClassWithInitializedDefaultValueProperties
        {
            public TestClassWithInitializedDefaultValueProperties MyClass { get; set; }
        }
    }
}

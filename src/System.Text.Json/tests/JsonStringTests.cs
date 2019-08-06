// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Json.Tests
{
    public static class JsonStringTests
    {
        [Fact]
        public static void TestDefaultCtor()
        {
            var jsonString = new JsonString();
            Assert.Equal("", jsonString.Value);
        }

        [Fact]
        public static void TestValueCtor()
        {
            var jsonString = new JsonString("property value");
            Assert.Equal("property value", jsonString.Value);
        }

        [Fact]
        public static void TestChangingValue()
        {
            var jsonString = new JsonString();
            jsonString.Value = "property value";
            Assert.Equal("property value", jsonString.Value);
            jsonString.Value = "different property value";
            Assert.Equal("different property value", jsonString.Value);
        }
    }
}

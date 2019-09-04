// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Json.Tests
{
    public static class JsonElementWithNodeParentTests
    {
        [Fact]
        public static void TestArray()
        {
            var jsonArray = new JsonArray() { 1, 2, 3 };
            JsonElement jsonArrayElement = jsonArray.AsJsonElement();

            Assert.Equal(2, jsonArrayElement[1].GetInt32());
            Assert.Equal(3, jsonArrayElement.GetArrayLength());

            var notJsonArray = new JsonString();
            JsonElement notJsonArrayElement = notJsonArray.AsJsonElement();

            Assert.Throws<InvalidOperationException>(() => notJsonArrayElement[1]);
            Assert.Throws<InvalidOperationException>(() => notJsonArrayElement.GetArrayLength());
        }

        [Fact]
        public static void TestObject()
        {
            var jsonObject = new JsonObject()
            {
                ["existing property"] = "value",
                ["different property"] = 14
            };

            JsonElement jsonObjectElement = jsonObject.AsJsonElement();
            Assert.True(jsonObjectElement.TryGetProperty("existing property", out JsonElement propertyValue));
            Assert.Equal("value", propertyValue.GetString());
            Assert.True(jsonObjectElement.TryGetProperty(Encoding.UTF8.GetBytes("existing property"), out propertyValue));
            Assert.Equal("value", propertyValue.GetString());

            Assert.Throws<InvalidOperationException>(() => propertyValue.TryGetProperty("property of not JsonObject", out _));
            Assert.Throws<InvalidOperationException>(() => propertyValue.TryGetProperty(Encoding.UTF8.GetBytes("property of not JsonObject"), out _));

            Assert.False(jsonObjectElement.TryGetProperty("not existing property", out propertyValue));
            Assert.Equal(default, propertyValue);

            Assert.False(jsonObjectElement.TryGetProperty(Encoding.UTF8.GetBytes("not existing property"), out propertyValue));
            Assert.Equal(default, propertyValue);
        }

        [Fact]
        public static void TestBoolean()
        {
            Assert.True(new JsonBoolean(true).AsJsonElement().GetBoolean());
            Assert.Throws<InvalidOperationException>(() => new JsonString().AsJsonElement().GetBoolean());
        }

        [Fact]
        public static void TestString()
        {
            Assert.Equal("value", new JsonString("value").AsJsonElement().GetString());
            Assert.Throws<InvalidOperationException>(() => new JsonBoolean().AsJsonElement().GetString());
        }

        [Fact]
        public static void TestByte()
        {
            Assert.Equal(byte.MaxValue, new JsonNumber(byte.MaxValue).AsJsonElement().GetByte());
            Assert.Throws<InvalidOperationException>(() => new JsonString().AsJsonElement().GetByte());
        }

        [Fact]
        public static void TestInt16()
        {
            Assert.Equal(short.MaxValue, new JsonNumber(short.MaxValue).AsJsonElement().GetInt16());
            Assert.Throws<InvalidOperationException>(() => new JsonString().AsJsonElement().GetInt16());
        }

        [Fact]
        public static void TestInt32()
        {
            Assert.Equal(int.MaxValue, new JsonNumber(int.MaxValue).AsJsonElement().GetInt32());
            Assert.Throws<InvalidOperationException>(() => new JsonString().AsJsonElement().GetInt32());
        }

        [Fact]
        public static void TestInt64()
        {
            Assert.Equal(long.MaxValue, new JsonNumber(long.MaxValue).AsJsonElement().GetInt64());
            Assert.Throws<InvalidOperationException>(() => new JsonString().AsJsonElement().GetInt64());
        }

        [Fact]
        public static void TestSingle()
        {
            Assert.Equal(3.14f, new JsonNumber(3.14f).AsJsonElement().GetSingle());
            Assert.Throws<InvalidOperationException>(() => new JsonString().AsJsonElement().GetSingle());
        }

        [Fact]
        public static void TestDouble()
        {
            Assert.Equal(3.14, new JsonNumber(3.14).AsJsonElement().GetDouble());
            Assert.Throws<InvalidOperationException>(() => new JsonString().AsJsonElement().GetDouble());
        }

        [Fact]
        public static void TestSByte()
        {
            Assert.Equal(sbyte.MaxValue, new JsonNumber(sbyte.MaxValue).AsJsonElement().GetSByte());
            Assert.Throws<InvalidOperationException>(() => new JsonString().AsJsonElement().GetSByte());
        }

        [Fact]
        public static void TestUInt16()
        {
            Assert.Equal(ushort.MaxValue, new JsonNumber(ushort.MaxValue).AsJsonElement().GetUInt16());
            Assert.Throws<InvalidOperationException>(() => new JsonString().AsJsonElement().GetUInt16());
        }

        [Fact]
        public static void TestUInt32()
        {
            Assert.Equal(uint.MaxValue, new JsonNumber(uint.MaxValue).AsJsonElement().GetUInt32());
            Assert.Throws<InvalidOperationException>(() => new JsonString().AsJsonElement().GetUInt32());
        }

        [Fact]
        public static void TestUInt64()
        {
            Assert.Equal(ulong.MaxValue, new JsonNumber(ulong.MaxValue).AsJsonElement().GetUInt64());
            Assert.Throws<InvalidOperationException>(() => new JsonString().AsJsonElement().GetUInt64());
        }
    }
}

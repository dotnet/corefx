// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.IO;
using Xunit;

namespace System.Text.Json.Linq.Tests
{
    public static class JsonElementWithNodeParentTests
    {
        [Fact]
        public static void TestArray()
        {
            var jsonArray = new JArray() { 1, 2, 3 };
            JsonElement jsonArrayElement = jsonArray.AsJsonElement();

            Assert.Equal(2, jsonArrayElement[1].GetInt32());
            Assert.Equal(3, jsonArrayElement.GetArrayLength());

            var notJArray = new JString();
            JsonElement notJArrayElement = notJArray.AsJsonElement();

            Assert.Throws<InvalidOperationException>(() => notJArrayElement[1]);
            Assert.Throws<InvalidOperationException>(() => notJArrayElement.GetArrayLength());
        }

        [Fact]
        public static void TestArrayEnumerator()
        {
            var jsonArray = new JArray() { 1, 2, 3 };
            JsonElement jsonArrayElement = jsonArray.AsJsonElement();

            JsonElement.ArrayEnumerator arrayEnumerator = jsonArrayElement.EnumerateArray();

            for (int i = 1; i <= 3; i++)
            {
                Assert.True(arrayEnumerator.MoveNext());
                Assert.Equal(i, arrayEnumerator.Current.GetInt32());
            }

            Assert.False(arrayEnumerator.MoveNext());
            Assert.False(arrayEnumerator.MoveNext());

            JsonElement notArray = new JObject().AsJsonElement();
            Assert.Throws<InvalidOperationException>(() => notArray.EnumerateArray());
        }

        [Fact]
        public static void TestObject()
        {
            var jsonObject = new JObject()
            {
                ["existing property"] = "value",
                ["different property"] = 14
            };

            JsonElement jsonObjectElement = jsonObject.AsJsonElement();
            Assert.True(jsonObjectElement.TryGetProperty("existing property", out JsonElement propertyValue));
            Assert.Equal("value", propertyValue.GetString());
            Assert.True(jsonObjectElement.TryGetProperty(Encoding.UTF8.GetBytes("existing property"), out propertyValue));
            Assert.Equal("value", propertyValue.GetString());

            Assert.Throws<InvalidOperationException>(() => propertyValue.TryGetProperty("property of not JObject", out _));
            Assert.Throws<InvalidOperationException>(() => propertyValue.TryGetProperty(Encoding.UTF8.GetBytes("property of not JObject"), out _));

            Assert.False(jsonObjectElement.TryGetProperty("not existing property", out propertyValue));
            Assert.Equal(default, propertyValue);

            Assert.False(jsonObjectElement.TryGetProperty(Encoding.UTF8.GetBytes("not existing property"), out propertyValue));
            Assert.Equal(default, propertyValue);
        }

        [Fact]
        public static void TestObjectEnumerator()
        {
            var jsonObject = new JObject()
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3
            };
            JsonElement jsonObjectElement = jsonObject.AsJsonElement();

            JsonElement.ObjectEnumerator objectEnumerator = jsonObjectElement.EnumerateObject();

            for (int i = 1; i <= 3; i++)
            {
                Assert.True(objectEnumerator.MoveNext());
                Assert.Equal(i.ToString(), objectEnumerator.Current.Name);
                Assert.Equal(i, objectEnumerator.Current.Value.GetInt32());
            }

            Assert.False(objectEnumerator.MoveNext());
            Assert.False(objectEnumerator.MoveNext());

            jsonObject["2"] = 4;
            Assert.Throws<InvalidOperationException>(() => objectEnumerator.MoveNext());


            JsonElement notObject = new JArray().AsJsonElement();
            Assert.Throws<InvalidOperationException>(() => notObject.EnumerateObject());
        }

        [Fact]
        public static void TestBoolean()
        {
            Assert.True(new JBoolean(true).AsJsonElement().GetBoolean());
            Assert.Throws<InvalidOperationException>(() => new JString().AsJsonElement().GetBoolean());
        }

        [Fact]
        public static void TestString()
        {
            Assert.Equal("value", new JString("value").AsJsonElement().GetString());
            Assert.Throws<InvalidOperationException>(() => new JBoolean().AsJsonElement().GetString());
        }

        [Fact]
        public static void TestByte()
        {
            Assert.Equal(byte.MaxValue, new JNumber(byte.MaxValue).AsJsonElement().GetByte());
            Assert.Throws<InvalidOperationException>(() => new JString().AsJsonElement().GetByte());
        }

        [Fact]
        public static void TestInt16()
        {
            Assert.Equal(short.MaxValue, new JNumber(short.MaxValue).AsJsonElement().GetInt16());
            Assert.Throws<InvalidOperationException>(() => new JString().AsJsonElement().GetInt16());
        }

        [Fact]
        public static void TestInt32()
        {
            Assert.Equal(int.MaxValue, new JNumber(int.MaxValue).AsJsonElement().GetInt32());
            Assert.Throws<InvalidOperationException>(() => new JString().AsJsonElement().GetInt32());
        }

        [Fact]
        public static void TestInt64()
        {
            Assert.Equal(long.MaxValue, new JNumber(long.MaxValue).AsJsonElement().GetInt64());
            Assert.Throws<InvalidOperationException>(() => new JString().AsJsonElement().GetInt64());
        }

        [Fact]
        public static void TestSingle()
        {
            Assert.Equal(3.14f, new JNumber(3.14f).AsJsonElement().GetSingle());
            Assert.Throws<InvalidOperationException>(() => new JString().AsJsonElement().GetSingle());
        }

        [Fact]
        public static void TestDouble()
        {
            Assert.Equal(3.14, new JNumber(3.14).AsJsonElement().GetDouble());
            Assert.Throws<InvalidOperationException>(() => new JString().AsJsonElement().GetDouble());
        }

        [Fact]
        public static void TestSByte()
        {
            Assert.Equal(sbyte.MaxValue, new JNumber(sbyte.MaxValue).AsJsonElement().GetSByte());
            Assert.Throws<InvalidOperationException>(() => new JString().AsJsonElement().GetSByte());
        }

        [Fact]
        public static void TestUInt16()
        {
            Assert.Equal(ushort.MaxValue, new JNumber(ushort.MaxValue).AsJsonElement().GetUInt16());
            Assert.Throws<InvalidOperationException>(() => new JString().AsJsonElement().GetUInt16());
        }

        [Fact]
        public static void TestUInt32()
        {
            Assert.Equal(uint.MaxValue, new JNumber(uint.MaxValue).AsJsonElement().GetUInt32());
            Assert.Throws<InvalidOperationException>(() => new JString().AsJsonElement().GetUInt32());
        }

        [Fact]
        public static void TestUInt64()
        {
            Assert.Equal(ulong.MaxValue, new JNumber(ulong.MaxValue).AsJsonElement().GetUInt64());
            Assert.Throws<InvalidOperationException>(() => new JString().AsJsonElement().GetUInt64());
        }

        [Fact]
        public static void TestDecimal()
        {
            Assert.Equal(decimal.One, new JNumber(decimal.One).AsJsonElement().GetDecimal());
            Assert.Throws<InvalidOperationException>(() => new JString().AsJsonElement().GetDecimal());
        }

        [Fact]
        public static void TestDateTime()
        {
            var dateTime = new DateTime(2019, 1, 1);
            Assert.Equal(dateTime, new JString(dateTime).AsJsonElement().GetDateTime());
            Assert.Throws<InvalidOperationException>(() => new JBoolean().AsJsonElement().GetDateTime());
        }

        [Fact]
        public static void TestDateOffset()
        {
            var dateTimeOffset = DateTimeOffset.ParseExact("2019-01-01T00:00:00", "s", CultureInfo.InvariantCulture);
            Assert.Equal(dateTimeOffset, new JString(dateTimeOffset).AsJsonElement().GetDateTimeOffset());
            Assert.Throws<InvalidOperationException>(() => new JBoolean().AsJsonElement().GetDateTimeOffset());
        }

        [Fact]
        public static void TestGuid()
        {
            Guid guid = Guid.ParseExact("ca761232-ed42-11ce-bacd-00aa0057b223", "D");
            Assert.Equal(guid, new JString(guid).AsJsonElement().GetGuid());
            Assert.Throws<InvalidOperationException>(() => new JBoolean().AsJsonElement().GetGuid());
        }

        [Fact]
        public static void TestGetRawText()
        {
            string jsonToParse = @"{""property name"":""value""}";
            string rawText = JNode.Parse(jsonToParse).AsJsonElement().GetRawText();
            Assert.Equal(jsonToParse, rawText);
        }

        [Fact]
        public static void TestWriteTo()
        {
            var jsonObject = new JObject()
            {
                ["property"] = "value",
                ["array"] = new JArray() { 1, 2 }
            };

            var stream = new MemoryStream();
            using (var writer = new Utf8JsonWriter(stream))
            {
                jsonObject.AsJsonElement().WriteTo(writer);
                string result = Encoding.UTF8.GetString(stream.ToArray());
                Assert.Equal(jsonObject.ToJsonString(), result);
            }
        }

        [Fact]
        public static void TestToString()
        {
            var jsonObject = new JObject()
            {
                ["text"] = "value",
                ["boolean"] = true,
                ["null"] = null,
                ["array"] = new JArray() { 1, 2 }
            };

            string toStringResult = jsonObject.AsJsonElement().ToString();
            Assert.Equal(jsonObject.ToJsonString(), toStringResult);
        }
        
        [Fact]
        public static void TestClone()
        {
            var jsonObject = new JObject
            {
                ["text"] = "value",
                ["boolean"] = true,
                ["null"] = null,
                ["array"] = new JArray() { 1, 2 }
            };

            JsonElement jsonElementCopy = jsonObject.AsJsonElement().Clone();
            Assert.Equal("value", jsonElementCopy.GetProperty("text").GetString());
            Assert.True(jsonElementCopy.GetProperty("boolean").GetBoolean());
            Assert.Equal(JsonValueKind.Null, jsonElementCopy.GetProperty("null").ValueKind);

            JsonElement.ArrayEnumerator arrayEnumerator = jsonElementCopy.GetProperty("array").EnumerateArray();

            Assert.True(arrayEnumerator.MoveNext());
            Assert.Equal(1, arrayEnumerator.Current.GetInt32());
            Assert.True(arrayEnumerator.MoveNext());
            Assert.Equal(2, arrayEnumerator.Current.GetInt32());
            Assert.False(arrayEnumerator.MoveNext());

            var jsonObjectFromCopy = (JObject)JNode.GetOriginatingNode(jsonElementCopy);
            
            jsonObject["text"] = "different value";
            Assert.Equal("value", jsonObjectFromCopy["text"]);

            jsonObjectFromCopy["boolean"] = false;
            Assert.Equal(true, jsonObject["boolean"]);
        }
    }
}

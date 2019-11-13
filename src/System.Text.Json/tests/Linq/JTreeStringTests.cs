// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.Text.Json.Tests;
using Xunit;

namespace System.Text.Json.Linq.Tests
{
    public static class JTreeStringTests
    {
        [Fact]
        public static void TestDefaultCtor()
        {
            var jsonString = new JTreeString();
            Assert.Equal("", jsonString.Value);
        }

        [InlineData("value")]
        [InlineData("value with some spaces")]
        [InlineData("     leading spaces")]
        [InlineData("trailing spaces")]
        [InlineData("new lines\r\n")]
        [InlineData("tabs\ttabs\t")]
        [InlineData("\\u003e\\u003e\\u003e\\u003e\\u003e")]
        [InlineData("za\u017C\u00F3\u0142\u0107 g\u0119\u015Bl\u0105 ja\u017A\u0144")]
        [InlineData("\u6f22\u5b57 \u6f22\u5b57")]
        [InlineData(">><++>>>\">>\\>>&>>>\u6f22\u5B57>>>")]
        [InlineData("Here is a string: \\\"\\\"\":\"Here is a\",\"Here is a back slash\\\\\":[\"Multiline\\r\\n String\\r\\n\",\"\\tMul\\r\\ntiline String\",\"\\\"somequote\\\"\\tMu\\\"\\\"l\\r\\ntiline\\\"another\\\" String\\\\\"],\"str")]
        [InlineData("Hello / a / b / c \\/ \\r\\b\\n\\f\\t\\/")]
        [Theory]
        public static void TestInitialization(string value)
        {
            // Default constructor:
            var defaultlyInitializedJTreeString = new JTreeString();
            defaultlyInitializedJTreeString.Value = value;
            Assert.Equal(value, defaultlyInitializedJTreeString.Value);

            // To string:
            Assert.Equal(value, defaultlyInitializedJTreeString.ToString());

            // Value constructor:
            Assert.Equal(value, new JTreeString(value).Value);

            // Implicit operator:
            JTreeNode jsonNode = value;
            JTreeString implicitlyInitializiedJsonString = (JTreeString)jsonNode;
            Assert.Equal(value, implicitlyInitializiedJsonString.Value);

            // Casted to span:
            ReadOnlySpan<char> spanValue = value.AsSpan();
            Assert.Equal(value, new JTreeString(spanValue).Value);
        }

        [Fact]
        public static void TestNulls()
        {
            string property = null;
            Assert.Throws<ArgumentNullException>(() => new JTreeString(property));
            Assert.Throws<ArgumentNullException>(() => new JTreeString().Value = null);
        }

        [Fact]
        public static void TestReadonlySpan()
        {
            var spanValue = new ReadOnlySpan<char>(new char[] { 's', 'p', 'a', 'n' });
            Assert.Equal("span", new JTreeString(spanValue).Value);

            string property = null;
            spanValue = property.AsSpan();
            var jsonString = new JTreeString(spanValue);
            Assert.Equal("", jsonString.Value);
        }

        [Fact]
        public static void TestGuid()
        {
            var guidString = "ca761232-ed42-11ce-bacd-00aa0057b223";
            Guid guid = Guid.ParseExact(guidString, "D");
            var jsonString = new JTreeString(guid);
            Assert.Equal(guidString, jsonString.Value);
            Assert.Equal(guid, jsonString.GetGuid());
            Assert.True(jsonString.TryGetGuid(out Guid guid2));
            Assert.Equal(guid, guid2);
        }

        [Theory]
        [MemberData(nameof(JsonDateTimeTestData.DateTimeFractionTrimBaseTests), MemberType = typeof(JsonDateTimeTestData))]
        [MemberData(nameof(JsonDateTimeTestData.DateTimeFractionTrimUtcOffsetTests), MemberType = typeof(JsonDateTimeTestData))]
        public static void TestDateTime(string testStr, string expectedStr)
        {
            var dateTime = DateTime.ParseExact(testStr, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            var jsonString = new JTreeString(dateTime);
            Assert.Equal(expectedStr, jsonString.Value);
            Assert.Equal(dateTime, jsonString.GetDateTime());
            Assert.True(jsonString.TryGetDateTime(out DateTime dateTime2));
            Assert.Equal(dateTime, dateTime2);
        }

        [Theory]
        [MemberData(nameof(JsonDateTimeTestData.DateTimeOffsetFractionTrimTests), MemberType = typeof(JsonDateTimeTestData))]
        public static void TestDateTimeOffset(string testStr, string expectedStr)
        {
            var dateTimeOffset = DateTimeOffset.ParseExact(testStr, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            var jsonString = new JTreeString(dateTimeOffset);
            Assert.Equal(expectedStr, jsonString.Value);
            Assert.Equal(dateTimeOffset, jsonString.GetDateTimeOffset());
            Assert.True(jsonString.TryGetDateTimeOffset(out DateTimeOffset dateTimeOffset2));
            Assert.Equal(dateTimeOffset, dateTimeOffset2);
        }

        [Fact]
        public static void TestChangingValue()
        {
            var jsonString = new JTreeString();
            jsonString.Value = "property value";
            Assert.Equal("property value", jsonString.Value);
            jsonString.Value = "different property value";
            Assert.Equal("different property value", jsonString.Value);
        }

        [Fact]
        public static void TestGettersFail()
        {
            var jsonString = new JTreeString("value");

            Assert.Throws<FormatException>(() => jsonString.GetDateTime());
            Assert.Throws<FormatException>(() => jsonString.GetDateTimeOffset());
            Assert.Throws<FormatException>(() => jsonString.GetGuid());

            Assert.False(jsonString.TryGetDateTime(out DateTime _));
            Assert.False(jsonString.TryGetDateTimeOffset(out DateTimeOffset _));
            Assert.False(jsonString.TryGetGuid(out Guid _));
        }

        [Fact]
        public static void TestEquals()
        {
            var jsonString = new JTreeString("json property value");

            Assert.True(jsonString.Equals(new JTreeString("json property value")));
            Assert.True(new JTreeString("json property value").Equals(jsonString));

            Assert.False(jsonString.Equals(new JTreeString("jsonpropertyvalue")));
            Assert.False(jsonString.Equals(new JTreeString("Json Property Value")));
            Assert.False(new JTreeString("jsonpropertyvalue").Equals(jsonString));
            Assert.False(new JTreeString("Json Property Value").Equals(jsonString));

            Assert.True(jsonString == new JTreeString("json property value"));
            Assert.True(jsonString != new JTreeString("something different"));

            JTreeNode jsonNode = new JTreeString("json property value");
            Assert.True(jsonString.Equals(jsonNode));

            IEquatable<JTreeString> jsonStringIEquatable = jsonString;
            Assert.True(jsonStringIEquatable.Equals(jsonString));
            Assert.True(jsonString.Equals(jsonStringIEquatable));

            Assert.False(jsonString.Equals(null));

            object jsonStringCopy = jsonString;
            object jsonStringObject = new JTreeString("json property value");
            Assert.True(jsonString.Equals(jsonStringObject));
            Assert.True(jsonStringCopy.Equals(jsonStringObject));
            Assert.True(jsonStringObject.Equals(jsonString));

            jsonString = new JTreeString();
            Assert.True(jsonString.Equals(new JTreeString()));
            Assert.True(jsonString.Equals(new JTreeString("")));
            Assert.False(jsonString.Equals(new JTreeString("something not empty")));

            Assert.False(jsonString.Equals(new Exception()));
            Assert.False(jsonString.Equals(new JTreeBoolean()));
            Assert.False(new JTreeString("true").Equals(new JTreeBoolean(true)));
            Assert.False(new JTreeString("123").Equals(new JTreeNumber("123")));

            JTreeString jsonStringNull = null;
            Assert.False(jsonString == jsonStringNull);
            Assert.False(jsonStringNull == jsonString);

            Assert.True(jsonString != jsonStringNull);
            Assert.True(jsonStringNull != jsonString);

            JTreeString otherJTreeStringNull = null;
            Assert.True(jsonStringNull == otherJTreeStringNull);
        }

        [Fact]
        public static void TestGetHashCode()
        {
            var jsonString = new JTreeString("json property value");

            int expectedHashCode = jsonString.GetHashCode();
            Assert.Equal(expectedHashCode, new JTreeString("json property value").GetHashCode());
            Assert.NotEqual(jsonString.GetHashCode(), new JTreeString("json property value ").GetHashCode());
            Assert.NotEqual(jsonString.GetHashCode(), new JTreeString("jsonpropertyvalue").GetHashCode());
            Assert.NotEqual(jsonString.GetHashCode(), new JTreeString("Json Property Value").GetHashCode());
            Assert.NotEqual(jsonString.GetHashCode(), new JTreeString("SOMETHING COMPLETELY DIFFERENT").GetHashCode());
            Assert.NotEqual(jsonString.GetHashCode(), new JTreeString("").GetHashCode());
            Assert.NotEqual(jsonString.GetHashCode(), new JTreeString().GetHashCode());

            JTreeNode jsonNode = new JTreeString("json property value");
            Assert.Equal(jsonString.GetHashCode(), jsonNode.GetHashCode());

            IEquatable<JTreeString> jsonStringIEquatable = jsonString;
            Assert.Equal(jsonString.GetHashCode(), jsonStringIEquatable.GetHashCode());

            object jsonNumberCopy = jsonString;
            object jsonNumberObject = new JTreeString("something different");

            Assert.Equal(jsonString.GetHashCode(), jsonNumberCopy.GetHashCode());
            Assert.NotEqual(jsonString.GetHashCode(), jsonNumberObject.GetHashCode());

            Assert.Equal(new JTreeString().GetHashCode(), new JTreeString().GetHashCode());
        }

        [Fact]
        public static void TestValueKind()
        {
            Assert.Equal(JsonValueKind.String, new JTreeString().ValueKind);
        }
    }
}

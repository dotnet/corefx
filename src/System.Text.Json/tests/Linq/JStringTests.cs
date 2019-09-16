// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace System.Text.Json.Linq.Tests
{
    public static class JStringTests
    {
        [Fact]
        public static void TestDefaultCtor()
        {
            var jsonString = new JString();
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
            var defaultlyInitializedJString = new JString();
            defaultlyInitializedJString.Value = value;
            Assert.Equal(value, defaultlyInitializedJString.Value);

            // To string:
            Assert.Equal(value, defaultlyInitializedJString.ToString());

            // Value constructor:
            Assert.Equal(value, new JString(value).Value);

            // Implicit operator:            
            JNode jsonNode = value;
            JString implicitlyInitializiedJString = (JString)jsonNode;
            Assert.Equal(value, implicitlyInitializiedJString.Value);

            // Casted to span:
            ReadOnlySpan<char> spanValue = value.AsSpan();
            Assert.Equal(value, new JString(spanValue).Value);
        }

        [Fact]
        public static void TestNulls()
        {
            string property = null;
            Assert.Throws<ArgumentNullException>(() => new JString(property));
            Assert.Throws<ArgumentNullException>(() => new JString().Value = null);
        }

        [Fact]
        public static void TestReadonlySpan()
        {
            var spanValue = new ReadOnlySpan<char>(new char[] { 's', 'p', 'a', 'n' });
            Assert.Equal("span", new JString(spanValue).Value);

            string property = null;
            spanValue = property.AsSpan();
            var jsonString = new JString(spanValue);
            Assert.Equal("", jsonString.Value);
        }

        [Fact]
        public static void TestGuid()
        {
            var guidString = "ca761232-ed42-11ce-bacd-00aa0057b223";
            Guid guid = Guid.ParseExact(guidString, "D");
            var jsonString = new JString(guid);
            Assert.Equal(guidString, jsonString.Value);
            Assert.Equal(guid, jsonString.GetGuid());
            Assert.True(jsonString.TryGetGuid(out Guid guid2));
            Assert.Equal(guid, guid2);
        }

        public static IEnumerable<object[]> DateTimeData =>
           new List<object[]>
           {
                new object[] { new DateTime(DateTime.MinValue.Ticks, DateTimeKind.Utc) },
                new object[] { new DateTime(2019, 1, 1) },
                new object[] { new DateTime(2019, 1, 1, new GregorianCalendar()) },
                new object[] { new DateTime(2019, 1, 1, new ChineseLunisolarCalendar()) }
           };

        [Theory]
        [MemberData(nameof(DateTimeData))]
        public static void TestDateTime(DateTime dateTime)
        {
            var jsonString = new JString(dateTime);
            Assert.Equal(dateTime.ToString("s", CultureInfo.InvariantCulture), jsonString.Value);
            Assert.Equal(dateTime, jsonString.GetDateTime());
            Assert.True(jsonString.TryGetDateTime(out DateTime dateTime2));
            Assert.Equal(dateTime, dateTime2);
        }

        [Theory]
        [MemberData(nameof(DateTimeData))]
        public static void TestDateTimeOffset(DateTime dateTime)
        {
            var dateTimeOffset = DateTimeOffset.ParseExact(dateTime.ToString("s"), "s", CultureInfo.InvariantCulture);
            var jsonString = new JString(dateTimeOffset);
            Assert.Equal(dateTimeOffset.ToString("s", CultureInfo.InvariantCulture), jsonString.Value);
            Assert.Equal(dateTimeOffset, jsonString.GetDateTimeOffset());
            Assert.True(jsonString.TryGetDateTimeOffset(out DateTimeOffset dateTimeOffset2));
            Assert.Equal(dateTimeOffset, dateTimeOffset2);
        }

        [Fact]
        public static void TestChangingValue()
        {
            var jsonString = new JString();
            jsonString.Value = "property value";
            Assert.Equal("property value", jsonString.Value);
            jsonString.Value = "different property value";
            Assert.Equal("different property value", jsonString.Value);
        }
        
        [Fact]
        public static void TestGettersFail()
        {
            var jsonString = new JString("value");

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
            var jsonString = new JString("json property value");

            Assert.True(jsonString.Equals(new JString("json property value")));
            Assert.True(new JString("json property value").Equals(jsonString));

            Assert.False(jsonString.Equals(new JString("jsonpropertyvalue")));
            Assert.False(jsonString.Equals(new JString("Json Property Value")));
            Assert.False(new JString("jsonpropertyvalue").Equals(jsonString));
            Assert.False(new JString("Json Property Value").Equals(jsonString));

            Assert.True(jsonString == new JString("json property value"));
            Assert.True(jsonString != new JString("something different"));

            JNode jsonNode = new JString("json property value");
            Assert.True(jsonString.Equals(jsonNode));

            IEquatable<JString> jsonStringIEquatable = jsonString;
            Assert.True(jsonStringIEquatable.Equals(jsonString));
            Assert.True(jsonString.Equals(jsonStringIEquatable));

            Assert.False(jsonString.Equals(null));

            object jsonStringCopy = jsonString;
            object jsonStringObject = new JString("json property value");
            Assert.True(jsonString.Equals(jsonStringObject));
            Assert.True(jsonStringCopy.Equals(jsonStringObject));
            Assert.True(jsonStringObject.Equals(jsonString));

            jsonString = new JString();
            Assert.True(jsonString.Equals(new JString()));
            Assert.True(jsonString.Equals(new JString("")));
            Assert.False(jsonString.Equals(new JString("something not empty")));

            Assert.False(jsonString.Equals(new Exception()));
            Assert.False(jsonString.Equals(new JBoolean()));
            Assert.False(new JString("true").Equals(new JBoolean(true)));
            Assert.False(new JString("123").Equals(new JNumber("123")));

            JString jsonStringNull = null;
            Assert.False(jsonString == jsonStringNull);
            Assert.False(jsonStringNull == jsonString);

            Assert.True(jsonString != jsonStringNull);
            Assert.True(jsonStringNull != jsonString);

            JString otherJStringNull = null;
            Assert.True(jsonStringNull == otherJStringNull);
        }

        [Fact]
        public static void TestGetHashCode()
        {
            var jsonString = new JString("json property value");

            int expectedHashCode = jsonString.GetHashCode();
            Assert.Equal(expectedHashCode, new JString("json property value").GetHashCode());
            Assert.NotEqual(jsonString.GetHashCode(), new JString("json property value ").GetHashCode());
            Assert.NotEqual(jsonString.GetHashCode(), new JString("jsonpropertyvalue").GetHashCode());
            Assert.NotEqual(jsonString.GetHashCode(), new JString("Json Property Value").GetHashCode());
            Assert.NotEqual(jsonString.GetHashCode(), new JString("SOMETHING COMPLETELY DIFFERENT").GetHashCode());
            Assert.NotEqual(jsonString.GetHashCode(), new JString("").GetHashCode());
            Assert.NotEqual(jsonString.GetHashCode(), new JString().GetHashCode());

            JNode jsonNode = new JString("json property value");
            Assert.Equal(jsonString.GetHashCode(), jsonNode.GetHashCode());

            IEquatable<JString> jsonStringIEquatable = jsonString;
            Assert.Equal(jsonString.GetHashCode(), jsonStringIEquatable.GetHashCode());

            object jsonNumberCopy = jsonString;
            object jsonNumberObject = new JString("something different");

            Assert.Equal(jsonString.GetHashCode(), jsonNumberCopy.GetHashCode());
            Assert.NotEqual(jsonString.GetHashCode(), jsonNumberObject.GetHashCode());

            Assert.Equal(new JString().GetHashCode(), new JString().GetHashCode());
        }

        [Fact]
        public static void TestValueKind()
        {
            Assert.Equal(JsonValueKind.String, new JString().ValueKind);
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static class PropertyNameTests
    {
        [Fact]
        public static void CamelCaseDeserializeNoMatch()
        {
            var options = new JsonSerializerOptions();
            options.PropertyNamingPolicy = JsonPropertyNamingPolicy.CamelCase;

            SimpleTestClass obj = JsonSerializer.Parse<SimpleTestClass>(@"{""MyInt16"":1}", options);

            // This is 0 (default value) because the data does not match the property "MyInt16" that is assuming camel-casing of "myInt16".
            Assert.Equal(0, obj.MyInt16);
        }

        [Fact]
        public static void CamelCaseDeserializeMatch()
        {
            var options = new JsonSerializerOptions();
            options.PropertyNamingPolicy = JsonPropertyNamingPolicy.CamelCase;

            SimpleTestClass obj = JsonSerializer.Parse<SimpleTestClass>(@"{""myInt16"":1}", options);

            // This is 1 because the data matches the property "MyInt16" that is assuming camel-casing of "myInt16".
            Assert.Equal(1, obj.MyInt16);
        }

        [Fact]
        public static void CamelCaseSerialize()
        {
            var options = new JsonSerializerOptions();
            options.PropertyNamingPolicy = JsonPropertyNamingPolicy.CamelCase;

            SimpleTestClass obj = JsonSerializer.Parse<SimpleTestClass>(@"{}", options);
            string json = JsonSerializer.ToString(obj, options);
            Assert.Contains(@"""myInt16"":0", json);
            Assert.Contains(@"""myInt32"":0", json);
        }

        [Fact]
        public static void CustomNamePolicy()
        {
            var options = new JsonSerializerOptions();
            options.PropertyNamingPolicy = new UppercaseNamingPolicy();

            SimpleTestClass obj = JsonSerializer.Parse<SimpleTestClass>(@"{""MYINT16"":1}", options);

            // This is 1 because the data matches the property "MYINT16" that is uppercase of "myInt16".
            Assert.Equal(1, obj.MyInt16);
        }

        [Fact]
        public static void NullNamePolicy()
        {
            var options = new JsonSerializerOptions();
            options.PropertyNamingPolicy = new NullNamingPolicy();

            SimpleTestClass obj = JsonSerializer.Parse<SimpleTestClass>(@"{""MyInt16"":1}", options);

            // This is 1 because returning a null for a name policy indicates use the actual property name.
            Assert.Equal(1, obj.MyInt16);
        }

        [Fact]
        public static void IgnoreCase()
        {
            {
                // A non-match scenario (case-sensitive by default).
                SimpleTestClass obj = JsonSerializer.Parse<SimpleTestClass>(@"{ ""myint16"":1}");
                Assert.Equal(0, obj.MyInt16);
            }

            {
                // A non-match scenario (case-sensitive by default).
                var options = new JsonSerializerOptions();
                SimpleTestClass obj = JsonSerializer.Parse<SimpleTestClass>(@"{""myint16"":1}", options);
                Assert.Equal(0, obj.MyInt16);
            }

            {
                var options = new JsonSerializerOptions();
                options.PropertyNameCaseInsensitive = true;
                SimpleTestClass obj = JsonSerializer.Parse<SimpleTestClass>(@"{""myint16"":1}", options);
                Assert.Equal(1, obj.MyInt16);
            }
        }

        [Fact]
        public static void JsonNameAttribute()
        {
            {
                OverridePropertyNameDesignTime_TestClass obj = JsonSerializer.Parse<OverridePropertyNameDesignTime_TestClass>(@"{""Blah"":1}");
                Assert.Equal(1, obj.myInt);

                string json = JsonSerializer.ToString(obj);
                Assert.Contains(@"""Blah"":1", json);
            }

            // The JsonNameAttribute should be unaffected by JsonPropertyNamingPolicy and PropertyNameCaseInsensitive.
            {
                var options = new JsonSerializerOptions();
                options.PropertyNamingPolicy = JsonPropertyNamingPolicy.CamelCase;
                options.PropertyNameCaseInsensitive = true;

                OverridePropertyNameDesignTime_TestClass obj = JsonSerializer.Parse<OverridePropertyNameDesignTime_TestClass>(@"{""Blah"":1}", options);
                Assert.Equal(1, obj.myInt);

                string json = JsonSerializer.ToString(obj);
                Assert.Contains(@"""Blah"":1", json);
            }
        }

        [Fact]
        public static void JsonNameAttributeDuplicateDesignTimeFail()
        {
            {
                var options = new JsonSerializerOptions();
                Assert.Throws<InvalidOperationException>(() => JsonSerializer.Parse<DuplicatePropertyNameDesignTime_TestClass>("{}", options));
            }

            {
                var options = new JsonSerializerOptions();
                Assert.Throws<InvalidOperationException>(() => JsonSerializer.ToString(new DuplicatePropertyNameDesignTime_TestClass(), options));
            }
        }

        [Fact]
        public static void JsonNullNameAttribute()
        {
            var options = new JsonSerializerOptions();
            options.PropertyNamingPolicy = JsonPropertyNamingPolicy.CamelCase;
            options.PropertyNameCaseInsensitive = true;

            // A null JsonNameAttribute should use the property name and be unaffected by other policies.
            NullPropertyName_TestClass obj = JsonSerializer.Parse<NullPropertyName_TestClass>(@"{""MyInt1"":1}", options);
            Assert.Equal(1, obj.MyInt1);

            string json = JsonSerializer.ToString(obj);
            Assert.Contains(@"""MyInt1"":1", json);
        }

        [Fact]
        public static void JsonNameConflictOnCamelCasingFail()
        {
            string json = @"{""myInt"":1,""MyInt"":2}";

            {
                // Baseline comparison - no options set.
                PropertyNamesDifferentByCaseOnly_TestClass obj = JsonSerializer.Parse<PropertyNamesDifferentByCaseOnly_TestClass>(json);
                Assert.Equal(1, obj.myInt);
                Assert.Equal(2, obj.MyInt);

                string jsonOut = JsonSerializer.ToString(obj);
                Assert.Equal(json, jsonOut);
            }

            {
                var options = new JsonSerializerOptions();
                options.PropertyNamingPolicy = JsonPropertyNamingPolicy.CamelCase;

                Assert.Throws<InvalidOperationException>(() => JsonSerializer.Parse<PropertyNamesDifferentByCaseOnly_TestClass>(json, options));
                Assert.Throws<InvalidOperationException>(() => JsonSerializer.ToString(new PropertyNamesDifferentByCaseOnly_TestClass(), options));
            }
        }

        [Fact]
        public static void JsonNameConflictOnCaseInsensitiveFail()
        {
            string json = @"{""myInt"":1,""MyInt"":2}";

            {
                var options = new JsonSerializerOptions();
                options.PropertyNameCaseInsensitive = true;

                Assert.Throws<InvalidOperationException>(() => JsonSerializer.Parse<PropertyNamesDifferentByCaseOnly_TestClass>(json, options));
                Assert.Throws<InvalidOperationException>(() => JsonSerializer.ToString(new PropertyNamesDifferentByCaseOnly_TestClass(), options));
            }
        }

        [Fact]
        public static void JsonOutputNotAffectedByCasingPolicy()
        {
            {
                // Baseline.
                string json = JsonSerializer.ToString(new SimpleTestClass());
                Assert.Contains(@"""MyInt16"":0", json);
            }

            // The JSON output should be unaffected by PropertyNameCaseInsensitive.
            {
                var options = new JsonSerializerOptions();
                options.PropertyNameCaseInsensitive = true;

                string json = JsonSerializer.ToString(new SimpleTestClass(), options);
                Assert.Contains(@"""MyInt16"":0", json);
            }
        }

        [Fact]
        public static void EmptyPropertyName()
        {
            string json = @"{"""":1}";

            {
                var obj = new EmptyPropertyName_TestClass();
                obj.MyInt1 = 1;

                string jsonOut = JsonSerializer.ToString(obj);
                Assert.Equal(json, jsonOut);
            }

            {
                EmptyPropertyName_TestClass obj = JsonSerializer.Parse<EmptyPropertyName_TestClass>(json);
                Assert.Equal(1, obj.MyInt1);
            }
        }
    }

    public class OverridePropertyNameDesignTime_TestClass
    {
        [JsonName("Blah")]
        public int myInt { get; set; }
        public static readonly byte[] s_dataMatchingAttribute = Encoding.UTF8.GetBytes(
            @"{" +
            @"""blah"" : 1" +
            @"}"
        );

        public static readonly byte[] s_dataNotMatchingAttribute = Encoding.UTF8.GetBytes(
            @"{" +
            @"""blah2"" : 1" +
            @"}"
        );
    }

    public class DuplicatePropertyNameDesignTime_TestClass
    {
        [JsonName("Blah")]
        public int MyInt1 { get; set; }

        [JsonName("Blah")]
        public int MyInt2 { get; set; }
    }

    public class EmptyPropertyName_TestClass
    {
        [JsonName("")]
        public int MyInt1 { get; set; }
    }

    public class NullPropertyName_TestClass
    {
        [JsonName(null)]
        public int MyInt1 { get; set; }
    }

    public class PropertyNamesDifferentByCaseOnly_TestClass
    {
        public int myInt { get; set; }
        public int MyInt { get; set; }
    }

    public class UppercaseNamingPolicy : JsonPropertyNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return name.ToUpperInvariant();
        }
    }

    public class NullNamingPolicy : JsonPropertyNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return null;
        }
    }
}

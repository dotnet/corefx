﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static class EnumTests
    {
        private static readonly string s_jsonStringEnum =
                @"{" +
                @"""MyEnum"" : ""Two""" +
                @"}";

        private static readonly string s_jsonIntEnum =
                @"{" +
                @"""MyEnum"" : 2" +
                @"}";

        [Fact]
        public static void EnumAsStringFail()
        {
            Assert.Throws<JsonReaderException>(() => JsonSerializer.Parse<SimpleTestClass>(s_jsonStringEnum));
        }

#if !MAKE_UNREVIEWED_APIS_INTERNAL
        [Fact]
        public static void EnumAsString()
        {
            var options = new JsonSerializerOptions();
            options.AddAttribute(typeof(SimpleTestClass), new JsonEnumConverterAttribute(treatAsString: true));

            SimpleTestClass obj = JsonSerializer.Parse<SimpleTestClass>(s_jsonStringEnum, options);
            Assert.Equal(SampleEnum.Two, obj.MyEnum);
        }
#endif
        [Fact]
        public static void EnumAsInt()
        {
            SimpleTestClass obj = JsonSerializer.Parse<SimpleTestClass>(s_jsonIntEnum);
            Assert.Equal(SampleEnum.Two, obj.MyEnum);
        }
    }
}

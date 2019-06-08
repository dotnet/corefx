// Licensed to the .NET Foundation under one or more agreements.
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

        private static readonly string s_jsonInt64EnumMin =
                @"{" +
                @"""MyInt64Enum"" : " + long.MinValue +
                @"}";

        private static readonly string s_jsonInt64EnumMax =
                @"{" +
                @"""MyInt64Enum"" : " + long.MaxValue +
                @"}";

        private static readonly string s_jsonUInt64EnumMax =
                @"{" +
                @"""MyUInt64Enum"" : " + ulong.MaxValue +
                @"}";

        [Fact]
        public static void EnumAsStringFail()
        {
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<SimpleTestClass>(s_jsonStringEnum));
        }

        [Fact]
        public static void EnumAsInt()
        {
            SimpleTestClass obj = JsonSerializer.Parse<SimpleTestClass>(s_jsonIntEnum);
            Assert.Equal(SampleEnum.Two, obj.MyEnum);
        }

        [Fact]
        public static void EnumAsInt64Min()
        {
            SimpleTestClass obj = JsonSerializer.Parse<SimpleTestClass>(s_jsonInt64EnumMin);
            Assert.Equal(SampleInt64Enum.Min, obj.MyInt64Enum);
        }

        [Fact]
        public static void EnumAsInt64Max()
        {
            SimpleTestClass obj = JsonSerializer.Parse<SimpleTestClass>(s_jsonInt64EnumMax);
            Assert.Equal(SampleInt64Enum.Max, obj.MyInt64Enum);
        }

        [Fact]
        public static void EnumAsUInt64Max()
        {
            SimpleTestClass obj = JsonSerializer.Parse<SimpleTestClass>(s_jsonUInt64EnumMax);
            Assert.Equal(SampleUInt64Enum.Max, obj.MyUInt64Enum);
        }

        [Fact]
        public static void Tmp_Test()
        {
            string json = "{ \"MyByteEnum\" : -1, \"MyUInt32Enum\" : -1, \"MyUInt64Enum\" : -1 }";
            SimpleTestClass obj = JsonSerializer.Parse<SimpleTestClass>(json);
            Assert.Equal(SampleUInt64Enum.Max, obj.MyUInt64Enum);
            Assert.Equal(SampleUInt32Enum.Max, obj.MyUInt32Enum);
            Assert.Equal(SampleByteEnum.Max, obj.MyByteEnum);
        }
    }
}

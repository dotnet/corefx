// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;
using Newtonsoft.Json;

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

        private static readonly string s_jsonByteEnum =
                @"{" +
                @"""MyByteEnum"" : " + byte.MaxValue +
                @"}";


        public static IEnumerable<object[]> JsonWithInvalidOutOfBoundNumber
        {
            get
            {
                const string InvalidNumber = "18446744073709551616"; // ulong.MaxValue + 1;
                foreach(var i in new dynamic[] { InvalidNumber, ulong.MaxValue })
                {
                    yield return new object[] { $"{{ \"MyEnum\" : \"{i}\" }}" };
                    yield return new object[] { $"{{ \"MyEnum\" : {i} }}" };
                    yield return new object[] { $"{{ \"MyByteEnum\" : \"{i}\" }}" };
                    yield return new object[] { $"{{ \"MyByteEnum\" : {i} }}" };
                    yield return new object[] { $"{{ \"MyUInt32Enum\" : \"{i}\" }}" };
                    yield return new object[] { $"{{ \"MyUInt32Enum\" : {i} }}" };
                }
                foreach(var i in new dynamic[] { InvalidNumber })
                {
                    yield return new object[] { $"{{ \"MyUInt64Enum\" : \"{i}\" }}" };
                    yield return new object[] { $"{{ \"MyUInt64Enum\" : {i} }}" };
                }
                foreach(var i in new dynamic[] { -1, (ulong)byte.MaxValue + 2 })
                {
                    yield return new object[] { $"{{ \"MyByteEnum\" : \"{i}\" }}" };
                }
                foreach(var i in new dynamic[] { -1, (ulong)UInt32.MaxValue + 2 })
                {
                    yield return new object[] { $"{{ \"MyUInt32Enum\" : \"{i}\" }}" };
                }
            }
        }

        public static IEnumerable<object[]> JsonWithAcceptableInvalidNumber
        {
            get
            {
                foreach(var (i,j) in new (dynamic, dynamic)[] { (-1, -1), ((ulong)byte.MaxValue + 2, (ulong)UInt32.MaxValue + 2) })
                {
                    yield return new object[] { 
                        $"{{ \"MyByteEnum\" : {i}, \"MyUInt32Enum\" : {j} }}",
                        (SampleByteEnum)(i % (byte.MaxValue + 1)), (SampleUInt32Enum)(j % ((long)UInt32.MaxValue + 1))
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(JsonWithAcceptableInvalidNumber))]
        public static void Parse_JsonWithAcceptableInvalidNumber_Success(string json, SampleByteEnum e1, SampleUInt32Enum e2)
        {
            SimpleTestClass result = JsonSerializer.Parse<SimpleTestClass>(json);
            Assert.Equal(e1, result.MyByteEnum);
            Assert.Equal(e2, result.MyUInt32Enum);
        }

        [Theory]
        [MemberData(nameof(JsonWithInvalidOutOfBoundNumber))]
        public static void Parse_InvalidOutOfBoundsNumber_Throws(string json)
        {
            Assert.Throws<JsonException>(() => JsonSerializer.Parse<SimpleTestClass>(json));
        }

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
        public static void EnumAsByteMax()
        {
            SimpleTestClass obj = JsonSerializer.Parse<SimpleTestClass>(s_jsonByteEnum);
            Assert.Equal(SampleByteEnum.Max, obj.MyByteEnum);
        }
    }
}

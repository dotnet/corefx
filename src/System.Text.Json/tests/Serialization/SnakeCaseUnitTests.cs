// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static class SnakeCaseUnitTests
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        //
        [InlineData("i", "i")]
        [InlineData("i", "I")]
        //
        [InlineData("ii", "ii")]
        [InlineData("i_i", "iI")]
        [InlineData("ii", "Ii")]
        [InlineData("ii", "II")]
        //
        [InlineData("iii", "iii")]
        [InlineData("ii_i", "iiI")]
        [InlineData("i_ii", "iIi")]
        [InlineData("i_ii", "iII")]
        [InlineData("iii", "Iii")]
        [InlineData("ii_i", "IiI")]
        [InlineData("i_ii", "IIi")]
        [InlineData("iii", "III")]
        //
        [InlineData("i_phone", "iPhone")]
        [InlineData("i_phone", "IPhone")]
        [InlineData("ip_hone", "IPHone")]
        [InlineData("iph_one", "IPHOne")]
        [InlineData("ipho_ne", "IPHONe")]
        [InlineData("iphone", "IPHONE")]
        //
        [InlineData("id", "id")]
        [InlineData("id", "ID")]
        //
        [InlineData("url", "url")]
        [InlineData("url", "URL")]
        [InlineData("url_value", "url_value")]
        [InlineData("url_value", "URLValue")]
        //
        [InlineData("xml2json", "xml2json")]
        [InlineData("xml2json", "Xml2Json")]
        //
        [InlineData("already_snake_case", "already_snake_case")]
        [InlineData("_already_snake_case", "_already_snake_case")]
        [InlineData("__already_snake_case", "__already_snake_case")]
        [InlineData("already_snake_case_", "already_snake_case_")]
        [InlineData("already_snake_case__", "already_snake_case__")]
        //
        [InlineData("sn_a__k_ec_as_e", "sn_a__k_ec_as_e")]
        [InlineData("sn_ak_ec_as_e", "sn_ak_ec_as_e")]
        [InlineData("sn_a__k_ec_as_e", "SnA__ kEcAsE")]
        [InlineData("sn_a__k_ec_as_e", "SnA__kEcAsE")]
        [InlineData("sn_ak_ec_as_e", "SnAkEcAsE")]
        //
        [InlineData("spaces", "spaces ")]
        [InlineData("spaces", "spaces  ")]
        [InlineData("spaces", "spaces   ")]
        [InlineData("spaces", "   spaces")]
        [InlineData("spaces", "  spaces")]
        [InlineData("spaces", " spaces")]
        //
        [InlineData("9999_12_31t23_59_59_9999999z", "9999-12-31T23:59:59.9999999Z")]
        [InlineData("hi_this_is_text_time_to_test", "Hi!! This is text. Time to test.")]
        //
        [InlineData("is_cia", "IsCIA")]
        [InlineData("is_json_property", "IsJSONProperty")]
        //
        [InlineData("lower_case", "lower case")]
        [InlineData("lower_case", "lower Case")]
        [InlineData("lower_case", "lowerCase")]
        [InlineData("lower_c_a_se", "lower cASe")]
        [InlineData("lowe_r_case", "loweR case")]
        [InlineData("lowe_r_case", "loweR Case")]
        [InlineData("lowe_r_c_a_se", "loweR cASe")]
        [InlineData("upper_case", "Upper case")]
        [InlineData("upper_case", "Upper Case")]
        [InlineData("upper_case", "UPPER CASE")]
        [InlineData("upper_case", "UpperCase")]
        [InlineData("upper_c_a_se", "Upper cASe")]
        [InlineData("uppe_r_case", "UppeR case")]
        [InlineData("uppe_r_case", "UppeR Case")]
        [InlineData("uppe_r_c_a_se", "UppeR cASe")]
        [InlineData("u_pper_case", "UPper case")]
        [InlineData("u_pper_case", "UPper Case")]
        [InlineData("u_pper_case", "UPperCase")]
        [InlineData("u_pper_c_a_se", "UPper cASe")]
        [InlineData("u_ppe_r_case", "UPpeR case")]
        [InlineData("u_ppe_r_case", "UPpeR Case")]
        [InlineData("u_ppe_r_c_a_se", "UPpeR cASe")]
        // Valid unicode
        [InlineData("\u00E4", "\u00E4")]
        [InlineData("\uD835\uDFDC", "\uD835\uDFDC")]
        // Invalid unicode
        [InlineData("\uDC01", "\uDC01")]
        [InlineData("\uD801", "\uD801")]
        public static void Convert_SpecifiedName_MatchesExpected(string expected, string name) =>
            Assert.Equal(expected, JsonNamingPolicy.SnakeCase.ConvertName(name));

        [Fact]
        public static void SerializeType_RoundTipping_MatchesOriginal()
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCase };

            string expected = @"{""some_int_property"":42}";
            string actual = JsonSerializer.Serialize(JsonSerializer.Deserialize<NamingPolictyTestClass>(expected, options), options);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void DeserializeType_RoundTipping_MatchesOriginal()
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCase };

            var expected = new NamingPolictyTestClass { SomeIntProperty = 42 };
            var actual = JsonSerializer.Deserialize<NamingPolictyTestClass>(JsonSerializer.Serialize(expected, options), options);

            Assert.Equal(
                expected.SomeIntProperty,
                actual.SomeIntProperty);
        }

        private class NamingPolictyTestClass
        {
            public int SomeIntProperty { get; set; }
        }

        [Fact]
        public static void SerializeDictionary_RoundTipping_MatchesOriginal()
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCase };

            string expected = @"{""some_int_property"":42}";
            string actual = JsonSerializer.Serialize(JsonSerializer.Deserialize<Dictionary<string, int>>(expected, options), options);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void DeserializeDictionary_RoundTipping_MatchesOriginal()
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCase };

            var expected = new Dictionary<string, int> { ["SomeIntProperty"] = 42 };
            var actual = JsonSerializer.Deserialize<Dictionary<string, int>>(JsonSerializer.Serialize(expected, options), options);

            Assert.Equal(
                expected["SomeIntProperty"],
                actual["SomeIntProperty"]);
        }
    }
}

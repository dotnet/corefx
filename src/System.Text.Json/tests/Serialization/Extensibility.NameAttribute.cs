// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Json.Serialization.Tests
{
#if !MAKE_UNREVIEWED_APIS_INTERNAL
    public static partial class ExtensibilityTests
    {
        [JsonCamelCasingConverter]
        public class SimpleTestClassWithCamelCase : SimpleTestClass
        {
            new public static readonly byte[] s_data = Encoding.UTF8.GetBytes(
                @"{" +
                    @"""myInt16"" : 1" +
                @"}");
        }

        [Fact]
        public static void CamelCaseAttributeDesignTimeNonMatch()
        {
            SimpleTestClassWithCamelCase obj = JsonSerializer.Parse<SimpleTestClassWithCamelCase>(SimpleTestClass.s_data);

            // This is 0 (default value) because the provided data is pascal-cased
            Assert.Equal(obj.MyInt16, 0);
        }

        [Fact]
        public static void CamelCaseAttributeDesignTime()
        {
            SimpleTestClassWithCamelCase obj = JsonSerializer.Parse<SimpleTestClassWithCamelCase>(SimpleTestClassWithCamelCase.s_data);
            Assert.Equal(obj.MyInt16, 1);
        }

        [Fact]
        public static void CamelCaseAttributeRuntime()
        {
            var options = new JsonSerializerOptions();
            options.AddAttribute(typeof(SimpleTestClass), new JsonCamelCasingConverterAttribute());

            SimpleTestClass obj = JsonSerializer.Parse<SimpleTestClass>(SimpleTestClassWithCamelCase.s_data, options);
            Assert.Equal(obj.MyInt16, 1);
        }

        [Fact]
        public static void CamelCaseAttributeInheritanceRuntime()
        {
            var options = new JsonSerializerOptions();
            
            // Add attibute to base class
            options.AddAttribute(typeof(SimpleTestClass), new JsonCamelCasingConverterAttribute());

            SimpleDerivedTestClass obj = JsonSerializer.Parse<SimpleDerivedTestClass>(SimpleTestClassWithCamelCase.s_data, options);
            Assert.Equal(obj.MyInt16, 1);
        }

        [Fact]
        public static void OverridePropertyNameAtDesignTime()
        {
            OverridePropertyNameDesignTime_TestClass x = JsonSerializer.Parse<OverridePropertyNameDesignTime_TestClass>(OverridePropertyNameDesignTime_TestClass.s_dataMatchingAttribute);

            Assert.Equal(x.MyInt16, 1);
        }

        [Fact]
        public static void OverridePropertyNameAtRuntime()
        {
            var options = new JsonSerializerOptions();
            options.AddAttribute(typeof(OverridePropertyNameRuntime_TestClass), new JsonPropertyNameAttribute("blah"));

            OverridePropertyNameRuntime_TestClass x = JsonSerializer.Parse<OverridePropertyNameRuntime_TestClass>(OverridePropertyNameRuntime_TestClass.s_data, options);

            Assert.Equal(x.MyInt16, 1);
        }

        [Fact]
        public static void OverridePropertyNameAndDesignTimeAttributeAtRuntimeIgnoresNonMatch()
        {
            OverridePropertyNameDesignTime_TestClass x = JsonSerializer.Parse<OverridePropertyNameDesignTime_TestClass>(OverridePropertyNameDesignTime_TestClass.s_dataNotMatchingAttribute);
            Assert.Equal(x.MyInt16, 0);
        }

        [Fact]
        public static void OverridePropertyNameAndDesignTimeAttributeAtRuntime()
        {
            var options = new JsonSerializerOptions();
            options.AddAttribute(typeof(OverridePropertyNameDesignTime_TestClass).GetProperty("MyInt16"), new JsonPropertyNameAttribute("blah2"));

            OverridePropertyNameDesignTime_TestClass x = JsonSerializer.Parse<OverridePropertyNameDesignTime_TestClass>(OverridePropertyNameDesignTime_TestClass.s_dataNotMatchingAttribute, options);

            Assert.Equal(x.MyInt16, 1);
        }
    }
#endif
}

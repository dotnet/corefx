// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.ComponentModel.Tests
{
    public class VersionConverterTests : ConverterTestBase
    {
        private static VersionConverter s_converter = new VersionConverter();

        [Fact]
        public static void CanConvertFrom_WithContext()
        {
            CanConvertFrom_WithContext(new object[2, 2]
                {
                    { typeof(string), true },
                    { typeof(Version), true }
                },
                VersionConverterTests.s_converter);
        }

        [Fact]
        public static void ConvertFrom_WithContext()
        {
            ConvertFrom_WithContext(new object[3, 3]
                {
                    {"1.2", new Version(1, 2), null},
                    {"1.2.3", new Version(1, 2, 3), null},
                    {"1.2.3.4", new Version(1, 2, 3, 4), null}
                },
                VersionConverterTests.s_converter);
        }

        [Fact]
        public static void ConvertFrom_WithContext_Negative()
        {
            Assert.Throws<NotSupportedException>(
                () => VersionConverterTests.s_converter.ConvertFrom(TypeConverterTests.s_context, null, null));
            Assert.Throws<ArgumentException>(
                () => VersionConverterTests.s_converter.ConvertFrom(TypeConverterTests.s_context, null, ""));
            Assert.Throws<ArgumentException>(
                () => VersionConverterTests.s_converter.ConvertFrom(TypeConverterTests.s_context, null, "1"));
        }
    }
}

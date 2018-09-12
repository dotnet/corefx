// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using Xunit;

namespace System.ComponentModel.Tests
{
    public class SingleConverterTests : ConverterTestBase
    {
        private static TypeConverter s_converter = new SingleConverter();

        [Fact]
        public static void ConvertFrom_WithContext()
        {
            ConvertFrom_WithContext(new object[2, 3]
                {
                    { 1.1f + " ", (float)1.1, null },
                    { "+7", (float)7, CultureInfo.InvariantCulture }
                },
                SingleConverterTests.s_converter);
        }

        [Fact]
        public static void ConvertFrom_WithContext_Negative()
        {
            AssertExtensions.Throws<ArgumentException, Exception>(
                () => SingleConverterTests.s_converter.ConvertFrom(TypeConverterTests.s_context, null, "0x8"));
        }

        [Fact]
        [SkipOnTargetFramework(~TargetFrameworkMonikers.NetFramework)]
        public static void ConvertTo_WithContext_NetFramework()
        {
            ConvertTo_WithContext(new object[2, 3]
                {
                    { (float)1.1, "1.1", null },
                    { (float)1.1, 1, CultureInfo.InvariantCulture }
                },
                SingleConverterTests.s_converter);
        }

        [Fact]
        [SkipOnTargetFramework(TargetFrameworkMonikers.NetFramework)]
        public static void ConvertTo_WithContext_NotNetFramework()
        {
            ConvertTo_WithContext(new object[2, 3]
                {
                    { (float)1.1, "1.10000002", null },
                    { (float)1.1, 1, CultureInfo.InvariantCulture }
                },
                SingleConverterTests.s_converter);
        }
    }
}

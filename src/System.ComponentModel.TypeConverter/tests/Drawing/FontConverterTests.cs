// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using Xunit;

namespace MonoTests.System.Drawing
{
    public class FontNameConverterTest
    {
        [Fact]
        public void TestConvertFrom()
        {
            FontConverter.FontNameConverter f = new FontConverter.FontNameConverter();
            // returns "Times" under Linux and "Times New Roman" under Windows
            Assert.True((f.ConvertFrom("Times") as string).StartsWith("Times"), "string test");
            Assert.True(f.GetStandardValuesSupported(), "standard values supported");
            Assert.False(f.GetStandardValuesExclusive(), "standard values exclusive");
        }

        [Fact]
        public void ExTestConvertFrom()
        {
            FontConverter.FontNameConverter f = new FontConverter.FontNameConverter();
            Assert.Throws<NotSupportedException>(() => f.ConvertFrom(null));
        }

        [Fact]
        public void ExTestConvertFrom2()
        {
            FontConverter.FontNameConverter f = new FontConverter.FontNameConverter();
            Assert.Throws<NotSupportedException>(() => f.ConvertFrom(1));
        }
    }
}

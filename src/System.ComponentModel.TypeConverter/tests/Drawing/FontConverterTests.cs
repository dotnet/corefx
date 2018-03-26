// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;

namespace System.ComponentModel.TypeConverterTests
{
    public class FontNameConverterTest
    {
        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void TestConvertFrom()
        {
            FontConverter.FontNameConverter converter = new FontConverter.FontNameConverter();
            // returns "Times" under Linux and "Times New Roman" under Windows
            Assert.True((converter.ConvertFrom("Times") as string).StartsWith("Times"), "string test");
            Assert.True(converter.GetStandardValuesSupported(), "standard values supported");
            Assert.False(converter.GetStandardValuesExclusive(), "standard values exclusive");
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ExTestConvertFrom()
        {
            FontConverter.FontNameConverter converter = new FontConverter.FontNameConverter();
            Assert.Throws<NotSupportedException>(() => converter.ConvertFrom(null));
        }

        [ConditionalFact(Helpers.GdiplusIsAvailable)]
        public void ExTestConvertFrom2()
        {
            FontConverter.FontNameConverter converter = new FontConverter.FontNameConverter();
            Assert.Throws<NotSupportedException>(() => converter.ConvertFrom(1));
        }
    }
}

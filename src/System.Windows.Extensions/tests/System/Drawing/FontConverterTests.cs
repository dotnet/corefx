// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Globalization;
using Xunit;
using static System.Drawing.FontConverter;

namespace System.ComponentModel.TypeConverterTests
{
    public class FontNameConverterTest
    {
        [ConditionalFact(Helpers.IsDrawingSupported)]
        public void TestConvertFrom()
        {
            FontConverter.FontNameConverter converter = new FontConverter.FontNameConverter();
            // returns "Times" under Linux and "Times New Roman" under Windows
            if (PlatformDetection.IsWindows)
            {
                Assert.Equal("Times New Roman", converter.ConvertFrom("Times") as string);
            }
            else
            {
                Assert.Equal("Times", converter.ConvertFrom("Times") as string);
            }
            Assert.True(converter.GetStandardValuesSupported(), "standard values supported");
            Assert.False(converter.GetStandardValuesExclusive(), "standard values exclusive");
        }

        [ConditionalFact(Helpers.IsDrawingSupported)]
        public void ExTestConvertFrom_ThrowsNotSupportedException()
        {
            FontConverter.FontNameConverter converter = new FontConverter.FontNameConverter();
            Assert.Throws<NotSupportedException>(() => converter.ConvertFrom(null));
            Assert.Throws<NotSupportedException>(() => converter.ConvertFrom(1));
        }
    }

    public class FontConverterTest
    {
        public static char sep = CultureInfo.CurrentCulture.TextInfo.ListSeparator[0];

        [ConditionalTheory(Helpers.IsDrawingSupported)]
        [MemberData(nameof(TestConvertFormData))]
        public void TestConvertFrom(string input, string expectedName, float expectedSize, GraphicsUnit expectedUnits, FontStyle expectedFontStyle)
        {
            FontConverter converter = new FontConverter();
            Font font = (Font)converter.ConvertFrom(input);
            Assert.Equal(expectedName, font.Name);
            Assert.Equal(expectedSize, font.Size);
            Assert.Equal(expectedUnits, font.Unit);
            Assert.Equal(expectedFontStyle, font.Style);
        }

        [ConditionalTheory(Helpers.IsDrawingSupported)]
        [MemberData(nameof(ArgumentExceptionFontConverterData))]
        public void InvalidInputThrowsArgumentException(string input)
        {
            FontConverter converter = new FontConverter();
            Assert.Throws<ArgumentException>(() => converter.ConvertFrom(input));
        }

        [ConditionalTheory(Helpers.IsDrawingSupported)]
        [MemberData(nameof(InvalidEnumArgumentExceptionFontConverterData))]
        public void InvalidInputThrowsInvalidEnumArgumentException(string input)
        {
            FontConverter converter = new FontConverter();
            Assert.Throws<InvalidEnumArgumentException>(() => converter.ConvertFrom(input));
        }

        [ConditionalFact(Helpers.IsDrawingSupported)]
        public void EmptyStringInput()
        {
            FontConverter converter = new FontConverter();
            Font font = (Font)converter.ConvertFrom(string.Empty);
            Assert.Null(font);
        }

        public static TheoryData<string, string, float, GraphicsUnit, FontStyle> TestConvertFormData()
        {
            var data = new TheoryData<string, string, float, GraphicsUnit, FontStyle>()
            {
                { $"Courier New", "Courier New", 8.25f, GraphicsUnit.Point, FontStyle.Regular },
                { $"Courier New, 11", "Courier New", 11f, GraphicsUnit.Point, FontStyle.Regular },
                { $"Arial{sep} 11px", "Arial", 11f, GraphicsUnit.Pixel, FontStyle.Regular },
                { $"Courier New{sep} 11 px", "Courier New", 11f, GraphicsUnit.Pixel, FontStyle.Regular },
                { $"Courier New{sep} 11 px{sep} style=Regular", "Courier New", 11f, GraphicsUnit.Pixel, FontStyle.Regular },
                { $"Courier New{sep} style=Bold", "Courier New", 8.25f, GraphicsUnit.Point, FontStyle.Bold },
                { $"Courier New{sep} 11 px{sep} style=Bold{sep} Italic", "Courier New", 11f, GraphicsUnit.Pixel, FontStyle.Bold | FontStyle.Italic },
                { $"Courier New{sep} 11 px{sep} style=Regular, Italic", "Courier New", 11f, GraphicsUnit.Pixel, FontStyle.Regular | FontStyle.Italic },
                { $"Courier New{sep} 11 px{sep} style=Bold{sep} Italic{sep} Strikeout", "Courier New", 11f, GraphicsUnit.Pixel, FontStyle.Bold | FontStyle.Italic | FontStyle.Strikeout },
                { $"Arial{sep} 11 px{sep} style=Bold, Italic, Strikeout", "Arial", 11f, GraphicsUnit.Pixel, FontStyle.Bold | FontStyle.Italic | FontStyle.Strikeout },
                { $"11px", "Microsoft Sans Serif", 8.25f, GraphicsUnit.Point, FontStyle.Regular },
                { $"Style=Bold", "Microsoft Sans Serif", 8.25f, GraphicsUnit.Point, FontStyle.Regular },
                { $"arIAL{sep} 10{sep} style=bold", "Arial", 10f, GraphicsUnit.Point, FontStyle.Bold },
                { $"Arial{sep} 10{sep}", "Arial", 10f, GraphicsUnit.Point, FontStyle.Regular },
                { $"Arial{sep}", "Arial", 8.25f, GraphicsUnit.Point, FontStyle.Regular },
                { $"Arial{sep} 10{sep} style=12", "Arial", 10f, GraphicsUnit.Point, FontStyle.Underline | FontStyle.Strikeout },
            };

            if (!PlatformDetection.IsFullFramework)
            {
                // FullFramework style keyword is case sensitive.
                data.Add($"Courier New{sep} Style=Bold", "Courier New", 8.25f, GraphicsUnit.Point, FontStyle.Bold);
                data.Add($"11px{sep} Style=Bold", "Microsoft Sans Serif", 8.25f, GraphicsUnit.Point, FontStyle.Bold);

                // FullFramework disregards all arguments if the font name is an empty string.
                if (PlatformDetection.IsWindows10Version1607OrGreater)
                {
                    data.Add($"{sep} 10{sep} style=bold", "", 10f, GraphicsUnit.Point, FontStyle.Bold); // empty string is not a installed font on Windows 7 and windows 8.
                }
            }

            return data;
        }

        public static TheoryData<string> ArgumentExceptionFontConverterData() => new TheoryData<string>()
        {
            { $"Courier New{sep} 11 px{sep} type=Bold{sep} Italic" },
            { $"Courier New{sep} {sep} Style=Bold" },
            { $"Courier New{sep} 11{sep} Style=" },
            { $"Courier New{sep} 11{sep} Style=RandomEnum" },
            { $"Arial{sep} 10{sep} style=bold{sep}" },
            { $"Arial{sep} 10{sep} style=null" },
            { $"Arial{sep} 10{sep} style=abc#" },
            { $"Arial{sep} 10{sep} style=##" },
            { $"Arial{sep} 10display{sep} style=bold" },
            { $"Arial{sep} 10style{sep} style=bold" },
        };

        public static TheoryData<string> InvalidEnumArgumentExceptionFontConverterData() => new TheoryData<string>()
        {
            { $"Arial{sep} 10{sep} style=56" },
            { $"Arial{sep} 10{sep} style=-1" },
        };
    }

    public class FontUnitConverterTest
    {
        [ConditionalFact(Helpers.IsDrawingSupported)]
        public void GetStandardValuesTest()
        {
            FontUnitConverter converter = new FontUnitConverter();
            var values = converter.GetStandardValues();
            Assert.Equal(6, values.Count); // The six supported values of Graphics unit: World, Pixel, Point, Inch, Document, Millimeter.

            foreach (var item in values)
            {
                Assert.NotEqual(GraphicsUnit.Display, (GraphicsUnit)item);
            }
        }
    }
}

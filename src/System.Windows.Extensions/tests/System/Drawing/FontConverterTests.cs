// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
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
        [ConditionalTheory(Helpers.IsDrawingSupported)]
        [MemberData(nameof(TestConvertFormData))]
        public void TestConvertFrom(string input, string name, float size, GraphicsUnit units, FontStyle fontStyle)
        {
            FontConverter converter = new FontConverter();
            Font font = (Font)converter.ConvertFrom(input);
            Assert.Equal(name, font.Name);
            Assert.Equal(size, font.Size);
            Assert.Equal(units, font.Unit);
            Assert.Equal(fontStyle, font.Style);
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
                { "Courier New", "Courier New", 8.25f, GraphicsUnit.Point, FontStyle.Regular },
                { "Courier New, 11", "Courier New", 11f, GraphicsUnit.Point, FontStyle.Regular },
                { "Arial, 11px", "Arial", 11f, GraphicsUnit.Pixel, FontStyle.Regular },
                { "Courier New, 11 px", "Courier New", 11f, GraphicsUnit.Pixel, FontStyle.Regular },
                { "Courier New, 11 px, style=Regular", "Courier New", 11f, GraphicsUnit.Pixel, FontStyle.Regular },
                { "Courier New, style=Bold", "Courier New", 8.25f, GraphicsUnit.Point, FontStyle.Bold },
                { "Courier New, 11 px, style=Bold, Italic", "Courier New", 11f, GraphicsUnit.Pixel, FontStyle.Bold | FontStyle.Italic },
                { "Courier New, 11 px, style=Regular, Italic", "Courier New", 11f, GraphicsUnit.Pixel, FontStyle.Regular | FontStyle.Italic },
                { "Courier New, 11 px, style=Bold, Italic, Strikeout", "Courier New", 11f, GraphicsUnit.Pixel, FontStyle.Bold | FontStyle.Italic | FontStyle.Strikeout },
                { "Arial, 11 px, style=Bold, Italic, Strikeout", "Arial", 11f, GraphicsUnit.Pixel, FontStyle.Bold | FontStyle.Italic | FontStyle.Strikeout },
                { "11px", "Microsoft Sans Serif", 8.25f, GraphicsUnit.Point, FontStyle.Regular },
                { "Style=Bold", "Microsoft Sans Serif", 8.25f, GraphicsUnit.Point, FontStyle.Regular },
                { "arIAL, 10, style=bold", "Arial", 10f, GraphicsUnit.Point, FontStyle.Bold },
                { "Arial, 10,", "Arial", 10f, GraphicsUnit.Point, FontStyle.Regular },
                { "Arial,", "Arial", 8.25f, GraphicsUnit.Point, FontStyle.Regular },
                { "Arial, 10, style=12", "Arial", 10f, GraphicsUnit.Point, FontStyle.Underline | FontStyle.Strikeout },
            };

            if (!PlatformDetection.IsFullFramework)
            {
                // FullFramework style keyword is case sensitive.
                data.Add("Courier New, Style=Bold", "Courier New", 8.25f, GraphicsUnit.Point, FontStyle.Bold);
                data.Add("11px, Style=Bold", "Microsoft Sans Serif", 8.25f, GraphicsUnit.Point, FontStyle.Bold);

                // FullFramework disregards all arguments if the font name is an empty string.
                data.Add(", 10, style=bold", "", 10f, GraphicsUnit.Point, FontStyle.Bold);
            }

            return data;
        }

        public static TheoryData<string> ArgumentExceptionFontConverterData() => new TheoryData<string>()
        {
            { "Courier New, 11 px, type=Bold, Italic" },
            { "Courier New, , Style=Bold" },
            { "Courier New, 11, Style=" },
            { "Courier New, 11, Style=RandomEnum" },
            { "Arial, 10, style=bold," },
            { "Arial, 10, style=null" },
            { "Arial, 10, style=abc#" },
            { "Arial, 10, style=##" },
            { "Arial, 10display, style=bold" },
            { "Arial, 10style, style=bold" },
        };

        public static TheoryData<string> InvalidEnumArgumentExceptionFontConverterData() => new TheoryData<string>()
        {
            { "Arial, 10, style=56" },
            { "Arial, 10, style=-1" },
        };
    }

    public class FontUnitConverterTest
    {
        [ConditionalFact(Helpers.IsDrawingSupported)]
        public void GetStandardValuesTest()
        {
            FontUnitConverter converter = new FontUnitConverter();
            var values = converter.GetStandardValues();
            Assert.Equal(6, values.Count);

            foreach (var item in values)
            {
                Assert.NotEqual(GraphicsUnit.Display, (GraphicsUnit)item);
            }
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Framework.WebEncoders;
using System;
using System.IO;
using Xunit;
using System.Text.Unicode;

namespace System.Text.Encodings.Web
{
    public class EncoderExtensionsTests
    {
        [Fact]
        public void HtmlEncode_PositiveTestCase()
        {
            // Arrange
            HtmlEncoder encoder = HtmlEncoder.Create(UnicodeRanges.All);
            StringWriter writer = new StringWriter();

            // Act
            encoder.Encode(writer, "Hello+there!");

            // Assert
            Assert.Equal("Hello&#x2B;there!", writer.ToString());
        }

        [Fact]
        public void HtmlEncode_PositiveTestCase_CreateWithSettings()
        {
            // Arrange
            TextEncoderSettings settings = new TextEncoderSettings(UnicodeRanges.All);
            HtmlEncoder encoder = HtmlEncoder.Create(settings);
            StringWriter writer = new StringWriter();

            // Act
            encoder.Encode(writer, "Hello+there!");

            // Assert
            Assert.Equal("Hello&#x2B;there!", writer.ToString());
        }

        [Fact]
        public void HtmlEncode_CreateNullRanges()
        {
            Assert.Throws<ArgumentNullException>("allowedRanges", () => HtmlEncoder.Create(default(UnicodeRange[])));
        }

        [Fact]
        public void HtmlEncode_CreateNullSettings()
        {
            Assert.Throws<ArgumentNullException>("settings", () => HtmlEncoder.Create(default(TextEncoderSettings)));
        }

        [Fact]
        public void EncodeSingleRune_InsufficientRoom()
        {
            Span<char> buffer = stackalloc char[1];
            int numberWritten = HtmlEncoder.Default.EncodeSingleRune(new Rune(0x10000), buffer);
            Assert.Equal(-1, numberWritten);
        }

        [Fact]
        public void JavaScriptStringEncode_PositiveTestCase()
        {
            // Arrange
            JavaScriptEncoder encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            StringWriter writer = new StringWriter();

            // Act
            encoder.Encode(writer, "Hello+there!");

            // Assert
            Assert.Equal(@"Hello\u002Bthere!", writer.ToString());
        }

        [Fact]
        public void UrlEncode_PositiveTestCase()
        {
            // Arrange
            UrlEncoder encoder = UrlEncoder.Create(UnicodeRanges.All);
            StringWriter writer = new StringWriter();

            // Act
            encoder.Encode(writer, "Hello+there!");

            // Assert
            Assert.Equal("Hello%2Bthere!", writer.ToString());
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Text.Unicode;
using Xunit;

namespace System.Text.Encodings.Web.Tests
{
    public class EncoderExtensionsTests
    {
        [Fact]
        public void HtmlEncode_ParameterChecks()
        {
            Assert.Throws<ArgumentNullException>(() => EncoderExtensions.HtmlEncode(null, "Hello!", new StringWriter()));
        }

        [Fact]
        public void HtmlEncode_PositiveTestCase()
        {
            HtmlEncoder encoder = HtmlEncoder.Create(UnicodeRanges.All);
            StringWriter writer = new StringWriter();

            encoder.Encode(writer, "Hello+there!");
            Assert.Equal("Hello&#x2B;there!", writer.ToString());
        }

        [Fact]
        public void JavaScriptStringEncode_ParameterChecks()
        {
            Assert.Throws<ArgumentNullException>(() => EncoderExtensions.JavaScriptStringEncode(null, "Hello!", new StringWriter()));
        }

        [Fact]
        public void JavaScriptStringEncode_PositiveTestCase()
        {
            IJavaScriptStringEncoder encoder = new JavaScriptStringEncoder(UnicodeRanges.All);
            StringWriter writer = new StringWriter();

            encoder.JavaScriptStringEncode("Hello+there!", writer);
            Assert.Equal(@"Hello\u002Bthere!", writer.ToString());
        }

        [Fact]
        public void UrlEncode_ParameterChecks()
        {
            Assert.Throws<ArgumentNullException>(() => EncoderExtensions.UrlEncode(null, "Hello!", new StringWriter()));
        }

        [Fact]
        public void UrlEncode_PositiveTestCase()
        {
            UrlEncoder encoder = UrlEncoder.Create(UnicodeRanges.All);
            StringWriter writer = new StringWriter();
            
            encoder.Encode(writer, "Hello+there!");
            Assert.Equal("Hello%2Bthere!", writer.ToString());
        }
    }
}

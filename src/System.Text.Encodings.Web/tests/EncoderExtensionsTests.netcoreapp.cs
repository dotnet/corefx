// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Text.Encodings.Web
{
    public partial class EncoderExtensionsTests
    {
        [Fact]
        public void EncodeSingleRune_InsufficientRoom()
        {
            Span<char> buffer = stackalloc char[1];
            int numberWritten = HtmlEncoder.Default.EncodeSingleRune(new Rune(0x10000), buffer);
            Assert.Equal(-1, numberWritten);
        }
    }
}

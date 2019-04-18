// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Internal;
using System.Text.Unicode;
using Xunit;

namespace Microsoft.Framework.WebEncoders
{
    // A text encoder that escapes each char as [U+ABCD] (hex).
    public sealed class CustomTextEncoder : TextEncoder
    {
        private static readonly CustomTextEncoder _default = new CustomTextEncoder(UnicodeRanges.BasicLatin);

        private readonly HtmlEncoder _innerEncoder;

        public CustomTextEncoder(TextEncoderSettings settings)
        {
            _innerEncoder = HtmlEncoder.Create(settings);
        }

        public CustomTextEncoder(params UnicodeRange[] allowedRanges)
            : this(new TextEncoderSettings(allowedRanges ?? Array.Empty<UnicodeRange>()))
        {
        }

        public override int EncodeSingleRune(Rune value, Span<char> buffer)
        {
            Span<char> scratchBuffer = stackalloc char[10]; // "[U+10FFFF]" is max length

            scratchBuffer[0] = '[';
            scratchBuffer[1] = 'U';
            scratchBuffer[2] = '+';

            bool success = value.Value.TryFormat(scratchBuffer.Slice(3), out int charsWritten, "X4");
            Assert.True(success);

            scratchBuffer[charsWritten + 3] = ']';
            scratchBuffer = scratchBuffer.Slice(0, charsWritten + 4);

            return scratchBuffer.TryCopyTo(buffer) ? scratchBuffer.Length : -1;
        }

        public override bool RuneMustBeEncoded(Rune value)
        {
            // Use the inner encoder as a proxy for what values are allowed to go through unescaped
            return _innerEncoder.RuneMustBeEncoded(value);
        }
    }
}

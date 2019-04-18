// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Text.Encodings.Web;

namespace Microsoft.Framework.WebEncoders
{
    /// <summary>
    /// Dummy encoder used for unit testing.
    /// </summary>
    public sealed class ScalarTestEncoder : TextEncoder
    {
        private const int Int32Length = 8;

        /// <summary>
        /// Returns 0.
        /// </summary>
        public override int FindFirstCharacterToEncode(ReadOnlySpan<char> text) => text.IsEmpty ? -1 : 0;

        /// <summary>
        /// Returns true.
        /// </summary>
        public override bool RuneMustBeEncoded(Rune value) => true;

        /// <summary>
        /// Encodes scalar as a hexadecimal number.
        /// </summary>
        public override int EncodeSingleRune(Rune value, Span<char> buffer)
        {
            string valueAsHex = ((uint)value.Value).ToString("X8");
            return valueAsHex.AsSpan().TryCopyTo(buffer) ? valueAsHex.Length : -1;
        }
    }
}

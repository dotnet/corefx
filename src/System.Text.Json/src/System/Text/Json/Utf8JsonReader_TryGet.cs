// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Buffers.Text;

namespace System.Text.Json
{
    public ref partial struct Utf8JsonReader
    {
        /// <summary>
        /// Reads the next JSON token value from the source as a <see cref="string"/>.
        /// Returns true if the UTF-8 encoded token value can be successfully transcoded
        /// to a UTF-16 string.
        /// Returns false if the token type is not JsonTokenType.String or
        /// JsonTokenType.PropertyName.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown if invalid byte sequence are detected while transcoding.
        /// </exception>
        public bool TryGetValueAsString(out string value)
        {
            value = default;
            if (TokenType != JsonTokenType.String && TokenType != JsonTokenType.PropertyName)
                return false;

            byte[] valueArray = IsValueMultiSegment ? ValueSequence.ToArray() : ValueSpan.ToArray();

            var utf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

            //TODO: Perform additional validation and unescaping if necessary
            value = utf8.GetString(valueArray);
            return true;
        }

        /// <summary>
        /// Reads the next JSON token value from the source as a <see cref="double"/>.
        /// Returns true if the entire UTF-8 encoded token value can be successfully 
        /// parsed to a double value.
        /// Returns false if the token type is not JsonTokenType.Number.
        /// </summary>
        public bool TryGetValueAsDouble(out double value)
        {
            value = default;
            if (TokenType != JsonTokenType.Number)
                return false;

            ReadOnlySpan<byte> valueSpan = IsValueMultiSegment ? ValueSequence.ToArray() : ValueSpan;

            if (valueSpan.IndexOfAny((byte)'e', (byte)'E') == -1)
            {
                return Utf8Parser.TryParse(valueSpan, out value, out int bytesConsumed) && valueSpan.Length == bytesConsumed;
            }
            else
            {
                return Utf8Parser.TryParse(valueSpan, out value, out int bytesConsumed, standardFormat: 'e') && valueSpan.Length == bytesConsumed;
            }
        }
    }
}

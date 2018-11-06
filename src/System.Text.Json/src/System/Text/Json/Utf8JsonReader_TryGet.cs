// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers.Text;
using System.Diagnostics;

namespace System.Text.Json
{
    public ref partial struct Utf8JsonReader
    {
        // Reject any invalid UTF-8 data rather than silently replacing.
        private static readonly UTF8Encoding s_utf8Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

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
            if (TokenType != JsonTokenType.String && TokenType != JsonTokenType.PropertyName && TokenType != JsonTokenType.Comment)
            {
                return false;
            }

            //TODO: Perform additional validation and unescaping if necessary
            value = s_utf8Encoding.GetString(ValueSpan);
            return true;
        }

        /// <summary>
        /// Reads the next JSON token value from the source as a <see cref="bool"/>.
        /// Returns true if the entire UTF-8 encoded token value can be successfully 
        /// parsed to a <see cref="bool"/> value.
        /// Returns false otherwise, or if the token type is not JsonTokenType.Number.
        /// </summary>
        public bool TryGetValueAsBoolean(out bool value)
        {
            bool result = true;
            if (TokenType == JsonTokenType.True)
            {
                Debug.Assert(ValueSpan.Length == 4);
                value = true;
            }
            else if (TokenType == JsonTokenType.False)
            {
                Debug.Assert(ValueSpan.Length == 5);
                value = false;
            }
            else
            {
                value = default;
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Reads the next JSON token value from the source as a <see cref="int"/>.
        /// Returns true if the entire UTF-8 encoded token value can be successfully 
        /// parsed to a <see cref="int"/> value.
        /// Returns false otherwise, or if the token type is not JsonTokenType.Number.
        /// </summary>
        public bool TryGetValueAsInt32(out int value)
        {
            value = default;
            if (TokenType != JsonTokenType.Number)
            {
                return false;
            }

            return Utf8Parser.TryParse(ValueSpan, out value, out int bytesConsumed) && ValueSpan.Length == bytesConsumed;
        }

        /// <summary>
        /// Reads the next JSON token value from the source as a <see cref="long"/>.
        /// Returns true if the entire UTF-8 encoded token value can be successfully 
        /// parsed to a <see cref="long"/> value.
        /// Returns false otherwise, or if the token type is not JsonTokenType.Number.
        /// </summary>
        public bool TryGetValueAsInt64(out long value)
        {
            value = default;
            if (TokenType != JsonTokenType.Number)
            {
                return false;
            }

            return Utf8Parser.TryParse(ValueSpan, out value, out int bytesConsumed) && ValueSpan.Length == bytesConsumed;
        }

        /// <summary>
        /// Reads the next JSON token value from the source as a <see cref="float"/>.
        /// Returns true if the entire UTF-8 encoded token value can be successfully 
        /// parsed to a <see cref="float"/> value.
        /// Returns false otherwise, or if the token type is not JsonTokenType.Number.
        /// </summary>
        public bool TryGetValueAsSingle(out float value)
        {
            value = default;
            if (TokenType != JsonTokenType.Number)
            {
                return false;
            }

            if (ValueSpan.IndexOfAny((byte)'e', (byte)'E') == -1)
            {
                return Utf8Parser.TryParse(ValueSpan, out value, out int bytesConsumed) && ValueSpan.Length == bytesConsumed;
            }
            else
            {
                return Utf8Parser.TryParse(ValueSpan, out value, out int bytesConsumed, standardFormat: 'e') && ValueSpan.Length == bytesConsumed;
            }
        }

        /// <summary>
        /// Reads the next JSON token value from the source as a <see cref="double"/>.
        /// Returns true if the entire UTF-8 encoded token value can be successfully 
        /// parsed to a <see cref="double"/> value.
        /// Returns false otherwise, or if the token type is not JsonTokenType.Number.
        /// </summary>
        public bool TryGetValueAsDouble(out double value)
        {
            value = default;
            if (TokenType != JsonTokenType.Number)
            {
                return false;
            }

            if (ValueSpan.IndexOfAny((byte)'e', (byte)'E') == -1)
            {
                return Utf8Parser.TryParse(ValueSpan, out value, out int bytesConsumed) && ValueSpan.Length == bytesConsumed;
            }
            else
            {
                return Utf8Parser.TryParse(ValueSpan, out value, out int bytesConsumed, standardFormat: 'e') && ValueSpan.Length == bytesConsumed;
            }
        }

        /// <summary>
        /// Reads the next JSON token value from the source as a <see cref="decimal"/>.
        /// Returns true if the entire UTF-8 encoded token value can be successfully 
        /// parsed to a <see cref="decimal"/> value.
        /// Returns false otherwise, or if the token type is not JsonTokenType.Number.
        /// </summary>
        public bool TryGetValueAsDecimal(out decimal value)
        {
            value = default;
            if (TokenType != JsonTokenType.Number)
            {
                return false;
            }

            if (ValueSpan.IndexOfAny((byte)'e', (byte)'E') == -1)
            {
                return Utf8Parser.TryParse(ValueSpan, out value, out int bytesConsumed) && ValueSpan.Length == bytesConsumed;
            }
            else
            {
                return Utf8Parser.TryParse(ValueSpan, out value, out int bytesConsumed, standardFormat: 'e') && ValueSpan.Length == bytesConsumed;
            }
        }
    }
}

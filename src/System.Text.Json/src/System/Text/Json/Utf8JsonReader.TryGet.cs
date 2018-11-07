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
        /// </summary>
        /// <remarks>
        /// This method never returns false. If the JSON token is a string and the data is valid UTF-8, the cast is always successful.
        /// Otherwise, it throws.
        /// </remarks>
        /// <exception cref="InvalidCastException">
        /// Thrown if trying to get the value of JSON token that is not a string
        /// (i.e. JsonTokenType.String, JsonTokenType.PropertyName, or JsonTokenType.Comment).
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if invalid byte sequence are detected while transcoding.
        /// </exception>
        public bool TryGetValueAsString(out string value)
        {
            if (TokenType != JsonTokenType.String && TokenType != JsonTokenType.PropertyName && TokenType != JsonTokenType.Comment)
            {
                throw ThrowHelper.GetInvalidCastException_ExpectedString(TokenType);
            }

            // TODO: https://github.com/dotnet/corefx/issues/33292
            value = s_utf8Encoding.GetString(ValueSpan);
            return true;
        }

        /// <summary>
        /// Reads the next JSON token value from the source as a <see cref="bool"/>.
        /// Returns true if the entire UTF-8 encoded token value can be successfully 
        /// parsed to a <see cref="bool"/> value.
        /// </summary>
        /// <remarks>
        /// This method never returns false. If the JSON token is a boolean, the cast is always successful.
        /// Otherwise, it throws.
        /// </remarks>
        /// <exception cref="InvalidCastException">
        /// Thrown if trying to get the value of JSON token that is not a boolean (i.e. JsonTokenType.True or JsonTokenType.False).
        /// </exception>
        public bool TryGetValueAsBoolean(out bool value)
        {
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
                throw ThrowHelper.GetInvalidCastException_ExpectedBoolean(TokenType);
            }
            return true;
        }

        /// <summary>
        /// Reads the next JSON token value from the source as a <see cref="int"/>.
        /// Returns true if the entire UTF-8 encoded token value can be successfully 
        /// parsed to a <see cref="int"/> value.
        /// Returns false otherwise.
        /// </summary>
        /// <exception cref="InvalidCastException">
        /// Thrown if trying to get the value of JSON token that is not a JsonTokenType.Number.
        /// </exception>
        public bool TryGetValueAsInt32(out int value)
        {
            if (TokenType != JsonTokenType.Number)
            {
                throw ThrowHelper.GetInvalidCastException_ExpectedNumber(TokenType);
            }

            return Utf8Parser.TryParse(ValueSpan, out value, out int bytesConsumed) && ValueSpan.Length == bytesConsumed;
        }

        /// <summary>
        /// Reads the next JSON token value from the source as a <see cref="long"/>.
        /// Returns true if the entire UTF-8 encoded token value can be successfully 
        /// parsed to a <see cref="long"/> value.
        /// Returns false otherwise.
        /// </summary>
        /// <exception cref="InvalidCastException">
        /// Thrown if trying to get the value of JSON token that is not a JsonTokenType.Number.
        /// </exception>
        public bool TryGetValueAsInt64(out long value)
        {
            if (TokenType != JsonTokenType.Number)
            {
                throw ThrowHelper.GetInvalidCastException_ExpectedNumber(TokenType);
            }

            return Utf8Parser.TryParse(ValueSpan, out value, out int bytesConsumed) && ValueSpan.Length == bytesConsumed;
        }

        /// <summary>
        /// Reads the next JSON token value from the source as a <see cref="float"/>.
        /// Returns true if the entire UTF-8 encoded token value can be successfully 
        /// parsed to a <see cref="float"/> value.
        /// Returns false otherwise.
        /// </summary>
        /// <exception cref="InvalidCastException">
        /// Thrown if trying to get the value of JSON token that is not a JsonTokenType.Number.
        /// </exception>
        public bool TryGetValueAsSingle(out float value)
        {
            if (TokenType != JsonTokenType.Number)
            {
                throw ThrowHelper.GetInvalidCastException_ExpectedNumber(TokenType);
            }

            char standardFormat = ValueSpan.IndexOfAny((byte)'e', (byte)'E') >= 0 ? 'e' : default;
            return Utf8Parser.TryParse(ValueSpan, out value, out int bytesConsumed, standardFormat) && ValueSpan.Length == bytesConsumed;
        }

        /// <summary>
        /// Reads the next JSON token value from the source as a <see cref="double"/>.
        /// Returns true if the entire UTF-8 encoded token value can be successfully 
        /// parsed to a <see cref="double"/> value.
        /// Returns false otherwise.
        /// </summary>
        /// <exception cref="InvalidCastException">
        /// Thrown if trying to get the value of JSON token that is not a JsonTokenType.Number.
        /// </exception>
        public bool TryGetValueAsDouble(out double value)
        {
            if (TokenType != JsonTokenType.Number)
            {
                throw ThrowHelper.GetInvalidCastException_ExpectedNumber(TokenType);
            }

            char standardFormat = ValueSpan.IndexOfAny((byte)'e', (byte)'E') >= 0 ? 'e' : default;
            return Utf8Parser.TryParse(ValueSpan, out value, out int bytesConsumed, standardFormat) && ValueSpan.Length == bytesConsumed;
        }

        /// <summary>
        /// Reads the next JSON token value from the source as a <see cref="decimal"/>.
        /// Returns true if the entire UTF-8 encoded token value can be successfully 
        /// parsed to a <see cref="decimal"/> value.
        /// Returns false otherwise.
        /// </summary>
        /// <exception cref="InvalidCastException">
        /// Thrown if trying to get the value of JSON token that is not a JsonTokenType.Number.
        /// </exception>
        public bool TryGetValueAsDecimal(out decimal value)
        {
            if (TokenType != JsonTokenType.Number)
            {
                throw ThrowHelper.GetInvalidCastException_ExpectedNumber(TokenType);
            }

            char standardFormat = ValueSpan.IndexOfAny((byte)'e', (byte)'E') >= 0 ? 'e' : default;
            return Utf8Parser.TryParse(ValueSpan, out value, out int bytesConsumed, standardFormat) && ValueSpan.Length == bytesConsumed;
        }
    }
}

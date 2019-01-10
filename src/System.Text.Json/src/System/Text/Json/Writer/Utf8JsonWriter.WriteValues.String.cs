// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;

namespace System.Text.Json
{
    public ref partial struct Utf8JsonWriter
    {
        /// <summary>
        /// Writes the UTF-16 text value (as a JSON string) as an element of a JSON array.
        /// </summary>
        /// <param name="value">The UTF-16 encoded value to be written as a UTF-8 transcoded JSON string element of a JSON array.</param>
        /// <param name="suppressEscaping">If this is set, the writer assumes the value is properly escaped and skips the escaping step.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified value is too large.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteStringValue(string value, bool suppressEscaping = false)
           => WriteStringValue(value.AsSpan(), suppressEscaping);

        /// <summary>
        /// Writes the UTF-16 text value (as a JSON string) as an element of a JSON array.
        /// </summary>
        /// <param name="value">The UTF-16 encoded value to be written as a UTF-8 transcoded JSON string element of a JSON array.</param>
        /// <param name="suppressEscaping">If this is set, the writer assumes the value is properly escaped and skips the escaping step.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified value is too large.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteStringValue(ReadOnlySpan<char> value, bool suppressEscaping = false)
        {
            JsonWriterHelper.ValidateValue(ref value);

            if (!suppressEscaping)
            {
                WriteStringSuppressFalse(ref value);
            }
            else
            {
                WriteStringByOptions(ref value);
            }

            _currentDepth |= 1 << 31;
            _tokenType = JsonTokenType.String;
        }

        private void WriteStringSuppressFalse(ref ReadOnlySpan<char> value)
        {
            int valueIdx = JsonWriterHelper.NeedsEscaping(value);

            Debug.Assert(valueIdx >= -1 && valueIdx < int.MaxValue / 2);

            if (valueIdx != -1)
            {
                WriteStringEscapeValue(ref value, valueIdx);
            }
            else
            {
                WriteStringByOptions(ref value);
            }
        }

        private void WriteStringByOptions(ref ReadOnlySpan<char> value)
        {
            if (_writerOptions.Indented)
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingValue();
                }
                WriteStringIndented(ref value);
            }
            else
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingValue();
                }
                WriteStringMinimized(ref value);
            }
        }

        private void WriteStringMinimized(ref ReadOnlySpan<char> escapedValue)
        {
            int idx = 0;
            if (_currentDepth < 0)
            {
                while (_buffer.Length <= idx)
                {
                    GrowAndEnsure();
                }
                _buffer[idx++] = JsonConstants.ListSeparator;
            }

            WriteStringValue(ref escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteStringIndented(ref ReadOnlySpan<char> escapedValue)
        {
            int idx = 0;
            if (_currentDepth < 0)
            {
                while (_buffer.Length <= idx)
                {
                    GrowAndEnsure();
                }
                _buffer[idx++] = JsonConstants.ListSeparator;
            }

            if (_tokenType != JsonTokenType.None)
                WriteNewLine(ref idx);

            int indent = Indentation;
            while (true)
            {
                bool result = JsonWriterHelper.TryWriteIndentation(_buffer.Slice(idx), indent, out int bytesWritten);
                idx += bytesWritten;
                if (result)
                {
                    break;
                }
                indent -= bytesWritten;
                AdvanceAndGrow(idx);
                idx = 0;
            }

            WriteStringValue(ref escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteStringEscapeValue(ref ReadOnlySpan<char> value, int firstEscapeIndexVal)
        {
            Debug.Assert(int.MaxValue / JsonConstants.MaxExpansionFactorWhileEscaping >= value.Length);

            char[] valueArray = null;

            if (firstEscapeIndexVal != -1)
            {
                int length = firstEscapeIndexVal + JsonConstants.MaxExpansionFactorWhileEscaping * (value.Length - firstEscapeIndexVal);

                Span<char> span;
                if (length > StackallocThreshold)
                {
                    valueArray = ArrayPool<char>.Shared.Rent(length);
                    span = valueArray;
                }
                else
                {
                    // Cannot create a span directly since the span gets exposed outside this method.
                    unsafe
                    {
                        char* ptr = stackalloc char[length];
                        span = new Span<char>(ptr, length);
                    }
                }
                JsonWriterHelper.EscapeString(ref value, ref span, firstEscapeIndexVal, out int written);
                value = span.Slice(0, written);
            }

            WriteStringByOptions(ref value);

            if (valueArray != null)
            {
                ArrayPool<char>.Shared.Return(valueArray);
            }
        }

        /// <summary>
        /// Writes the UTF-8 text value (as a JSON string) as an element of a JSON array.
        /// </summary>
        /// <param name="value">The UTF-8 encoded value to be written as a JSON string element of a JSON array.</param>
        /// <param name="suppressEscaping">If this is set, the writer assumes the value is properly escaped and skips the escaping step.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified value is too large.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteStringValue(ReadOnlySpan<byte> value, bool suppressEscaping = false)
        {
            JsonWriterHelper.ValidateValue(ref value);

            if (!suppressEscaping)
            {
                WriteStringSuppressFalse(ref value);
            }
            else
            {
                WriteStringByOptions(ref value);
            }

            _currentDepth |= 1 << 31;
            _tokenType = JsonTokenType.String;
        }

        private void WriteStringSuppressFalse(ref ReadOnlySpan<byte> value)
        {
            int valueIdx = JsonWriterHelper.NeedsEscaping(value);

            Debug.Assert(valueIdx >= -1 && valueIdx < int.MaxValue / 2);

            if (valueIdx != -1)
            {
                WriteStringEscapeValue(ref value, valueIdx);
            }
            else
            {
                WriteStringByOptions(ref value);
            }
        }

        private void WriteStringByOptions(ref ReadOnlySpan<byte> value)
        {
            if (_writerOptions.Indented)
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingValue();
                }
                WriteStringIndented(ref value);
            }
            else
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingValue();
                }
                WriteStringMinimized(ref value);
            }
        }

        private void WriteStringMinimized(ref ReadOnlySpan<byte> escapedValue)
        {
            int idx = 0;
            if (_currentDepth < 0)
            {
                while (_buffer.Length <= idx)
                {
                    GrowAndEnsure();
                }
                _buffer[idx++] = JsonConstants.ListSeparator;
            }

            WriteStringValue(ref escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteStringIndented(ref ReadOnlySpan<byte> escapedValue)
        {
            int idx = 0;
            if (_currentDepth < 0)
            {
                while (_buffer.Length <= idx)
                {
                    GrowAndEnsure();
                }
                _buffer[idx++] = JsonConstants.ListSeparator;
            }

            if (_tokenType != JsonTokenType.None)
                WriteNewLine(ref idx);

            int indent = Indentation;
            while (true)
            {
                bool result = JsonWriterHelper.TryWriteIndentation(_buffer.Slice(idx), indent, out int bytesWritten);
                idx += bytesWritten;
                if (result)
                {
                    break;
                }
                indent -= bytesWritten;
                AdvanceAndGrow(idx);
                idx = 0;
            }

            WriteStringValue(ref escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteStringEscapeValue(ref ReadOnlySpan<byte> value, int firstEscapeIndexVal)
        {
            Debug.Assert(int.MaxValue / JsonConstants.MaxExpansionFactorWhileEscaping >= value.Length);

            byte[] valueArray = null;

            if (firstEscapeIndexVal != -1)
            {
                int length = firstEscapeIndexVal + JsonConstants.MaxExpansionFactorWhileEscaping * (value.Length - firstEscapeIndexVal);

                Span<byte> span;
                if (length > StackallocThreshold)
                {
                    valueArray = ArrayPool<byte>.Shared.Rent(length);
                    span = valueArray;
                }
                else
                {
                    // Cannot create a span directly since the span gets exposed outside this method.
                    unsafe
                    {
                        byte* ptr = stackalloc byte[length];
                        span = new Span<byte>(ptr, length);
                    }
                }
                JsonWriterHelper.EscapeString(ref value, ref span, firstEscapeIndexVal, out int written);
                value = span.Slice(0, written);
            }

            WriteStringByOptions(ref value);

            if (valueArray != null)
            {
                ArrayPool<byte>.Shared.Return(valueArray);
            }
        }
    }
}

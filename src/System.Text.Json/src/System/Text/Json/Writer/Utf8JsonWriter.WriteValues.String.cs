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
        /// Writes the string text value (as a JSON string) as an element of a JSON array.
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
            JsonWriterHelper.ValidateValue(value);

            if (!suppressEscaping)
            {
                WriteStringSuppressFalse(value);
            }
            else
            {
                WriteStringByOptions(value);
            }

            SetFlagToAddListSeparatorBeforeNextItem();
            _tokenType = JsonTokenType.String;
        }

        private void WriteStringSuppressFalse(ReadOnlySpan<char> value)
        {
            int valueIdx = JsonWriterHelper.NeedsEscaping(value);

            Debug.Assert(valueIdx >= -1 && valueIdx < int.MaxValue / 2);

            if (valueIdx != -1)
            {
                WriteStringEscapeValue(value, valueIdx);
            }
            else
            {
                WriteStringByOptions(value);
            }
        }

        private void WriteStringByOptions(ReadOnlySpan<char> value)
        {
            if (_writerOptions.Indented)
            {
                ValidateWritingValue();
                WriteStringIndented(value);
            }
            else
            {
                ValidateWritingValue();
                WriteStringMinimized(value);
            }
        }

        private void WriteStringMinimized(ReadOnlySpan<char> escapedValue)
        {
            int idx = 0;
            if (_currentDepth < 0)
            {
                if (_buffer.Length <= idx)
                {
                    GrowAndEnsure();
                }
                _buffer[idx++] = JsonConstants.ListSeparator;
            }

            WriteStringValue(escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteStringIndented(ReadOnlySpan<char> escapedValue)
        {
            int idx = 0;
            if (_currentDepth < 0)
            {
                if (_buffer.Length <= idx)
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
                AdvanceAndGrow(ref idx);
            }

            WriteStringValue(escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteStringEscapeValue(ReadOnlySpan<char> value, int firstEscapeIndexVal)
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
                    // Cannot create a span directly since it gets passed to instance methods on a ref struct.
                    unsafe
                    {
                        char* ptr = stackalloc char[length];
                        span = new Span<char>(ptr, length);
                    }
                }
                JsonWriterHelper.EscapeString(value, span, firstEscapeIndexVal, out int written);
                value = span.Slice(0, written);
            }

            WriteStringByOptions(value);

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
            JsonWriterHelper.ValidateValue(value);

            if (!suppressEscaping)
            {
                WriteStringSuppressFalse(value);
            }
            else
            {
                WriteStringByOptions(value);
            }

            SetFlagToAddListSeparatorBeforeNextItem();
            _tokenType = JsonTokenType.String;
        }

        private void WriteStringSuppressFalse(ReadOnlySpan<byte> value)
        {
            int valueIdx = JsonWriterHelper.NeedsEscaping(value);

            Debug.Assert(valueIdx >= -1 && valueIdx < int.MaxValue / 2);

            if (valueIdx != -1)
            {
                WriteStringEscapeValue(value, valueIdx);
            }
            else
            {
                WriteStringByOptions(value);
            }
        }

        private void WriteStringByOptions(ReadOnlySpan<byte> value)
        {
            if (_writerOptions.Indented)
            {
                ValidateWritingValue();
                WriteStringIndented(value);
            }
            else
            {
                ValidateWritingValue();
                WriteStringMinimized(value);
            }
        }

        private void WriteStringMinimized(ReadOnlySpan<byte> escapedValue)
        {
            int idx = 0;
            if (_currentDepth < 0)
            {
                if (_buffer.Length <= idx)
                {
                    GrowAndEnsure();
                }
                _buffer[idx++] = JsonConstants.ListSeparator;
            }

            WriteStringValue(escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteStringIndented(ReadOnlySpan<byte> escapedValue)
        {
            int idx = 0;
            if (_currentDepth < 0)
            {
                if (_buffer.Length <= idx)
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
                AdvanceAndGrow(ref idx);
            }

            WriteStringValue(escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteStringEscapeValue(ReadOnlySpan<byte> value, int firstEscapeIndexVal)
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
                    // Cannot create a span directly since it gets passed to instance methods on a ref struct.
                    unsafe
                    {
                        byte* ptr = stackalloc byte[length];
                        span = new Span<byte>(ptr, length);
                    }
                }
                JsonWriterHelper.EscapeString(value, span, firstEscapeIndexVal, out int written);
                value = span.Slice(0, written);
            }

            WriteStringByOptions(value);

            if (valueArray != null)
            {
                ArrayPool<byte>.Shared.Return(valueArray);
            }
        }
    }
}

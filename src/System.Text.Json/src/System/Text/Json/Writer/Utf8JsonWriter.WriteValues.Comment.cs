// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Text.Json
{
    public ref partial struct Utf8JsonWriter
    {
        /// <summary>
        /// Writes the UTF-16 text value (as a JSON comment).
        /// </summary>
        /// <param name="utf16Text">The UTF-16 encoded value to be written as a UTF-8 transcoded JSON comment within /*..*/.</param>
        /// <param name="suppressEscaping">If this is set, the writer assumes the value is properly escaped and skips the escaping step.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified value is too large.
        /// </exception>
        public void WriteCommentValue(string utf16Text, bool suppressEscaping = false)
            => WriteCommentValue(utf16Text.AsSpan(), suppressEscaping);

        /// <summary>
        /// Writes the UTF-16 text value (as a JSON comment).
        /// </summary>
        /// <param name="utf16Text">The UTF-16 encoded value to be written as a UTF-8 transcoded JSON comment within /*..*/.</param>
        /// <param name="suppressEscaping">If this is set, the writer assumes the value is properly escaped and skips the escaping step.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified value is too large.
        /// </exception>
        public void WriteCommentValue(ReadOnlySpan<char> utf16Text, bool suppressEscaping = false)
        {
            JsonWriterHelper.ValidateValue(ref utf16Text);

            if (!suppressEscaping)
                WriteCommentSuppressFalse(ref utf16Text);
            else
                WriteCommentByOptions(ref utf16Text);
        }

        private void WriteCommentSuppressFalse(ref ReadOnlySpan<char> value)
        {
            int valueIdx = JsonWriterHelper.NeedsEscaping(value);

            Debug.Assert(valueIdx >= -1 && valueIdx < int.MaxValue / 2);

            if (valueIdx != -1)
            {
                WriteCommentEscapeValue(ref value, valueIdx);
            }
            else
            {
                WriteCommentByOptions(ref value);
            }
        }

        private void WriteCommentByOptions(ref ReadOnlySpan<char> value)
        {
            if (_writerOptions.Indented)
            {
                WriteCommentIndented(ref value);
            }
            else
            {
                WriteCommentMinimized(ref value);
            }
        }

        private void WriteCommentMinimized(ref ReadOnlySpan<char> escapedValue)
        {
            int idx = 0;

            WriteCommentValue(ref escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteCommentIndented(ref ReadOnlySpan<char> escapedValue)
        {
            int idx = 0;

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

            WriteCommentValue(ref escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteCommentEscapeValue(ref ReadOnlySpan<char> value, int firstEscapeIndexVal)
        {
            Debug.Assert(int.MaxValue / MaxExpansionFactorWhileEscaping >= value.Length);

            char[] valueArray = null;

            if (firstEscapeIndexVal != -1)
            {
                int length = firstEscapeIndexVal + MaxExpansionFactorWhileEscaping * (value.Length - firstEscapeIndexVal);

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

            WriteCommentByOptions(ref value);

            if (valueArray != null)
                ArrayPool<char>.Shared.Return(valueArray);
        }

        /// <summary>
        /// Writes the UTF-8 text value (as a JSON comment).
        /// </summary>
        /// <param name="utf8Text">The UTF-8 encoded value to be written as a JSON comment within /*..*/.</param>
        /// <param name="suppressEscaping">If this is set, the writer assumes the value is properly escaped and skips the escaping step.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified value is too large.
        /// </exception>
        public void WriteCommentValue(ReadOnlySpan<byte> utf8Text, bool suppressEscaping = false)
        {
            JsonWriterHelper.ValidateValue(ref utf8Text);

            if (!suppressEscaping)
                WriteCommentSuppressFalse(ref utf8Text);
            else
                WriteCommentByOptions(ref utf8Text);
        }

        private void WriteCommentSuppressFalse(ref ReadOnlySpan<byte> value)
        {
            int valueIdx = JsonWriterHelper.NeedsEscaping(value);

            Debug.Assert(valueIdx >= -1 && valueIdx < int.MaxValue / 2);

            if (valueIdx != -1)
            {
                WriteCommentEscapeValue(ref value, valueIdx);
            }
            else
            {
                WriteCommentByOptions(ref value);
            }
        }

        private void WriteCommentByOptions(ref ReadOnlySpan<byte> value)
        {
            if (_writerOptions.Indented)
            {
                WriteCommentIndented(ref value);
            }
            else
            {
                WriteCommentMinimized(ref value);
            }
        }

        private void WriteCommentMinimized(ref ReadOnlySpan<byte> escapedValue)
        {
            int idx = 0;

            WriteCommentValue(ref escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteCommentIndented(ref ReadOnlySpan<byte> escapedValue)
        {
            int idx = 0;

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

            WriteCommentValue(ref escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteCommentEscapeValue(ref ReadOnlySpan<byte> value, int firstEscapeIndexVal)
        {
            Debug.Assert(int.MaxValue / MaxExpansionFactorWhileEscaping >= value.Length);

            byte[] valueArray = null;

            if (firstEscapeIndexVal != -1)
            {
                int length = firstEscapeIndexVal + MaxExpansionFactorWhileEscaping * (value.Length - firstEscapeIndexVal);

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

            WriteCommentByOptions(ref value);

            if (valueArray != null)
                ArrayPool<byte>.Shared.Return(valueArray);
        }

        private void WriteCommentValue(ref ReadOnlySpan<char> escapedValue, ref int idx)
        {
            while (_buffer.Length <= idx)
            {
                AdvanceAndGrow(idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.Slash;

            while (_buffer.Length <= idx)
            {
                AdvanceAndGrow(idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.Asterisk;

            ReadOnlySpan<byte> byteSpan = MemoryMarshal.AsBytes(escapedValue);
            int partialConsumed = 0;
            while (true)
            {
                OperationStatus status = JsonWriterHelper.ToUtf8(byteSpan.Slice(partialConsumed), _buffer.Slice(idx), out int consumed, out int written);
                idx += written;
                if (status == OperationStatus.Done)
                {
                    break;
                }
                partialConsumed += consumed;
                AdvanceAndGrow(idx);
                idx = 0;
            }

            while (_buffer.Length <= idx)
            {
                AdvanceAndGrow(idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.Asterisk;

            while (_buffer.Length <= idx)
            {
                AdvanceAndGrow(idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.Slash;
        }

        private void WriteCommentValue(ref ReadOnlySpan<byte> escapedValue, ref int idx)
        {
            while (_buffer.Length <= idx)
            {
                AdvanceAndGrow(idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.Slash;

            while (_buffer.Length <= idx)
            {
                AdvanceAndGrow(idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.Asterisk;

            CopyLoop(ref escapedValue, ref idx);

            while (_buffer.Length <= idx)
            {
                AdvanceAndGrow(idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.Asterisk;

            while (_buffer.Length <= idx)
            {
                AdvanceAndGrow(idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.Slash;
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Text.Json
{
    public sealed partial class Utf8JsonWriter
    {
        /// <summary>
        /// Writes the string text value (as a JSON comment).
        /// </summary>
        /// <param name="value">The UTF-16 encoded value to be written as a UTF-8 transcoded JSON comment within /*..*/.</param>
        /// <remarks>
        /// The value is escaped before writing.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified value is too large.
        /// </exception>
        public void WriteCommentValue(string value)
            => WriteCommentValue(value.AsSpan());

        /// <summary>
        /// Writes the UTF-16 text value (as a JSON comment).
        /// </summary>
        /// <param name="value">The UTF-16 encoded value to be written as a UTF-8 transcoded JSON comment within /*..*/.</param>
        /// <remarks>
        /// The value is escaped before writing.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified value is too large.
        /// </exception>
        public void WriteCommentValue(ReadOnlySpan<char> value)
        {
            JsonWriterHelper.ValidateValue(value);

            WriteCommentEscape(value);
        }

        private void WriteCommentEscape(ReadOnlySpan<char> value)
        {
            int valueIdx = JsonWriterHelper.NeedsEscaping(value);

            Debug.Assert(valueIdx >= -1 && valueIdx < value.Length);

            if (valueIdx != -1)
            {
                WriteCommentEscapeValue(value, valueIdx);
            }
            else
            {
                WriteCommentByOptions(value);
            }
        }

        private void WriteCommentByOptions(ReadOnlySpan<char> value)
        {
            if (Options.Indented)
            {
                WriteCommentIndented(value);
            }
            else
            {
                WriteCommentMinimized(value);
            }
        }

        private void WriteCommentMinimized(ReadOnlySpan<char> escapedValue)
        {
            Debug.Assert(escapedValue.Length < (int.MaxValue / JsonConstants.MaxExpansionFactorWhileTranscoding) - 4);

            // All ASCII, /*...*/ => escapedValue.Length + 4
            // Optionally, up to 3x growth when transcoding
            int maxRequired = (escapedValue.Length * JsonConstants.MaxExpansionFactorWhileTranscoding) + 4;

            if (_memory.Length - BytesPending < maxRequired)
            {
                Grow(maxRequired);
            }

            Span<byte> output = _memory.Span;

            output[BytesPending++] = JsonConstants.Slash;
            output[BytesPending++] = JsonConstants.Asterisk;

            TranscodeAndWrite(escapedValue, output);

            output[BytesPending++] = JsonConstants.Asterisk;
            output[BytesPending++] = JsonConstants.Slash;
        }

        private void WriteCommentIndented(ReadOnlySpan<char> escapedValue)
        {
            int indent = Indentation;
            Debug.Assert(indent <= 2 * JsonConstants.MaxWriterDepth);

            Debug.Assert(escapedValue.Length < (int.MaxValue / JsonConstants.MaxExpansionFactorWhileTranscoding) - indent - 4 - s_newLineLength);

            // All ASCII, /*...*/ => escapedValue.Length + 4
            // Optionally, 1-2 bytes for new line, and up to 3x growth when transcoding
            int maxRequired = indent + (escapedValue.Length * JsonConstants.MaxExpansionFactorWhileTranscoding) + 4 + s_newLineLength;

            if (_memory.Length - BytesPending < maxRequired)
            {
                Grow(maxRequired);
            }

            Span<byte> output = _memory.Span;

            if (_tokenType != JsonTokenType.None)
            {
                WriteNewLine(output);
            }

            JsonWriterHelper.WriteIndentation(output.Slice(BytesPending), indent);
            BytesPending += indent;

            output[BytesPending++] = JsonConstants.Slash;
            output[BytesPending++] = JsonConstants.Asterisk;

            TranscodeAndWrite(escapedValue, output);

            output[BytesPending++] = JsonConstants.Asterisk;
            output[BytesPending++] = JsonConstants.Slash;
        }

        private void WriteCommentEscapeValue(ReadOnlySpan<char> value, int firstEscapeIndexVal)
        {
            Debug.Assert(int.MaxValue / JsonConstants.MaxExpansionFactorWhileEscaping >= value.Length);
            Debug.Assert(firstEscapeIndexVal >= 0 && firstEscapeIndexVal < value.Length);

            char[] valueArray = null;

            int length = JsonWriterHelper.GetMaxEscapedLength(value.Length, firstEscapeIndexVal);

            Span<char> escapedValue = length <= JsonConstants.StackallocThreshold ?
                stackalloc char[length] :
                (valueArray = ArrayPool<char>.Shared.Rent(length));

            JsonWriterHelper.EscapeString(value, escapedValue, firstEscapeIndexVal, out int written);

            WriteCommentByOptions(escapedValue.Slice(0, written));

            if (valueArray != null)
            {
                ArrayPool<char>.Shared.Return(valueArray);
            }
        }

        /// <summary>
        /// Writes the UTF-8 text value (as a JSON comment).
        /// </summary>
        /// <param name="utf8Value">The UTF-8 encoded value to be written as a JSON comment within /*..*/.</param>
        /// <remarks>
        /// The value is escaped before writing.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified value is too large.
        /// </exception>
        public void WriteCommentValue(ReadOnlySpan<byte> utf8Value)
        {
            JsonWriterHelper.ValidateValue(utf8Value);

            WriteCommentEscape(utf8Value);
        }

        private void WriteCommentEscape(ReadOnlySpan<byte> utf8Value)
        {
            int valueIdx = JsonWriterHelper.NeedsEscaping(utf8Value);

            Debug.Assert(valueIdx >= -1 && valueIdx < utf8Value.Length);

            if (valueIdx != -1)
            {
                WriteCommentEscapeValue(utf8Value, valueIdx);
            }
            else
            {
                WriteCommentByOptions(utf8Value);
            }
        }

        private void WriteCommentByOptions(ReadOnlySpan<byte> utf8Value)
        {
            if (Options.Indented)
            {
                WriteCommentIndented(utf8Value);
            }
            else
            {
                WriteCommentMinimized(utf8Value);
            }
        }

        private void WriteCommentMinimized(ReadOnlySpan<byte> escapedValue)
        {
            Debug.Assert(escapedValue.Length < int.MaxValue - 4);

            int maxRequired = escapedValue.Length + 4; // /*...*/

            if (_memory.Length - BytesPending < maxRequired)
            {
                Grow(maxRequired);
            }

            Span<byte> output = _memory.Span;

            output[BytesPending++] = JsonConstants.Slash;
            output[BytesPending++] = JsonConstants.Asterisk;

            escapedValue.CopyTo(output.Slice(BytesPending));
            BytesPending += escapedValue.Length;

            output[BytesPending++] = JsonConstants.Asterisk;
            output[BytesPending++] = JsonConstants.Slash;
        }

        private void WriteCommentIndented(ReadOnlySpan<byte> escapedValue)
        {
            int indent = Indentation;
            Debug.Assert(indent <= 2 * JsonConstants.MaxWriterDepth);

            Debug.Assert(escapedValue.Length < int.MaxValue - indent - 4 - s_newLineLength);

            int minRequired = indent + escapedValue.Length + 4; // /*...*/
            int maxRequired = minRequired + s_newLineLength; // Optionally, 1-2 bytes for new line

            if (_memory.Length - BytesPending < maxRequired)
            {
                Grow(maxRequired);
            }

            Span<byte> output = _memory.Span;

            if (_tokenType != JsonTokenType.None)
            {
                WriteNewLine(output);
            }

            JsonWriterHelper.WriteIndentation(output.Slice(BytesPending), indent);
            BytesPending += indent;

            output[BytesPending++] = JsonConstants.Slash;
            output[BytesPending++] = JsonConstants.Asterisk;

            escapedValue.CopyTo(output.Slice(BytesPending));
            BytesPending += escapedValue.Length;

            output[BytesPending++] = JsonConstants.Asterisk;
            output[BytesPending++] = JsonConstants.Slash;
        }

        private void WriteCommentEscapeValue(ReadOnlySpan<byte> utf8Value, int firstEscapeIndexVal)
        {
            Debug.Assert(int.MaxValue / JsonConstants.MaxExpansionFactorWhileEscaping >= utf8Value.Length);
            Debug.Assert(firstEscapeIndexVal >= 0 && firstEscapeIndexVal < utf8Value.Length);

            byte[] valueArray = null;

            int length = JsonWriterHelper.GetMaxEscapedLength(utf8Value.Length, firstEscapeIndexVal);

            Span<byte> escapedValue = length <= JsonConstants.StackallocThreshold ?
                stackalloc byte[length] :
                (valueArray = ArrayPool<byte>.Shared.Rent(length));

            JsonWriterHelper.EscapeString(utf8Value, escapedValue, firstEscapeIndexVal, out int written);

            WriteCommentByOptions(escapedValue.Slice(0, written));

            if (valueArray != null)
            {
                ArrayPool<byte>.Shared.Return(valueArray);
            }
        }
    }
}

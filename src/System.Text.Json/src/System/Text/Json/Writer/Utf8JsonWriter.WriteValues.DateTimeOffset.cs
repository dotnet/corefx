// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;


namespace System.Text.Json
{
    public sealed partial class Utf8JsonWriter
    {
        /// <summary>
        /// Writes the <see cref="DateTimeOffset"/> value (as a JSON string) as an element of a JSON array.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in invalid JSON being written (while validation is enabled).
        /// </exception>
        /// <remarks>
        /// Writes the <see cref="DateTimeOffset"/> using the round-trippable ('O') <see cref="StandardFormat"/> , for example: 2017-06-12T05:30:45.7680000-07:00.
        /// </remarks>
        public void WriteStringValue(DateTimeOffset value)
        {
            ValidateWritingValue();
            if (_options.Indented)
            {
                WriteStringValueIndented(value);
            }
            else
            {
                WriteStringValueMinimized(value);
            }

            SetFlagToAddListSeparatorBeforeNextItem();
            _tokenType = JsonTokenType.String;
        }

        private void WriteStringValueMinimized(DateTimeOffset value)
        {
            int maxRequired = JsonConstants.MaximumFormatDateTimeOffsetLength + 3; // 2 quotes, and optionally, 1 list separator

            if (_memory.Length - _currentIndex < maxRequired)
            {
                Grow(maxRequired);
            }

            Span<byte> output = _memory.Span;

            if (_currentDepth < 0)
            {
                output[_currentIndex++] = JsonConstants.ListSeparator;
            }

            output[_currentIndex++] = JsonConstants.Quote;

            Span<byte> tempSpan = stackalloc byte[JsonConstants.MaximumFormatDateTimeOffsetLength];
            bool result = Utf8Formatter.TryFormat(value, tempSpan, out int bytesWritten, s_dateTimeStandardFormat);
            Debug.Assert(result);
            JsonWriterHelper.TrimDateTimeOffset(tempSpan.Slice(0, bytesWritten), out bytesWritten);
            tempSpan.Slice(0, bytesWritten).CopyTo(output.Slice(_currentIndex));
            _currentIndex += bytesWritten;

            output[_currentIndex++] = JsonConstants.Quote;
        }

        private void WriteStringValueIndented(DateTimeOffset value)
        {
            int indent = Indentation;
            Debug.Assert(indent <= 2 * JsonConstants.MaxWriterDepth);

            // 2 quotes, and optionally, 1 list separator and 1-2 bytes for new line
            int maxRequired = indent + JsonConstants.MaximumFormatDateTimeOffsetLength + 3 + s_newLineLength;

            if (_memory.Length - _currentIndex < maxRequired)
            {
                Grow(maxRequired);
            }

            Span<byte> output = _memory.Span;

            if (_currentDepth < 0)
            {
                output[_currentIndex++] = JsonConstants.ListSeparator;
            }

            if (_tokenType != JsonTokenType.PropertyName)
            {
                if (_tokenType != JsonTokenType.None)
                {
                    WriteNewLine(output);
                }
                JsonWriterHelper.WriteIndentation(output.Slice(_currentIndex), indent);
                _currentIndex += indent;
            }

            output[_currentIndex++] = JsonConstants.Quote;

            Span<byte> tempSpan = stackalloc byte[JsonConstants.MaximumFormatDateTimeOffsetLength];
            bool result = Utf8Formatter.TryFormat(value, tempSpan, out int bytesWritten, s_dateTimeStandardFormat);
            Debug.Assert(result);
            JsonWriterHelper.TrimDateTimeOffset(tempSpan.Slice(0, bytesWritten), out bytesWritten);
            tempSpan.Slice(0, bytesWritten).CopyTo(output.Slice(_currentIndex));
            _currentIndex += bytesWritten;

            output[_currentIndex++] = JsonConstants.Quote;
        }
    }
}

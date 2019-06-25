// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;
using System.Globalization;

namespace System.Text.Json
{
    public sealed partial class Utf8JsonWriter
    {
        /// <summary>
        /// Writes the <see cref="double"/> value (as a JSON number) as an element of a JSON array.
        /// </summary>
        /// <param name="value">The value to be written as a JSON number as an element of a JSON array.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        /// <remarks>
        /// Writes the <see cref="double"/> using a <see cref="StandardFormat"/> of 'G' on .NET Core
        /// and 'G17' on .NET Framework.
        /// </remarks>
        public void WriteNumberValue(double value)
        {
            JsonWriterHelper.ValidateDouble(value);

            ValidateWritingValue();
            if (Options.Indented)
            {
                WriteNumberValueIndented(value);
            }
            else
            {
                WriteNumberValueMinimized(value);
            }

            SetFlagToAddListSeparatorBeforeNextItem();
            _tokenType = JsonTokenType.Number;
        }

        private void WriteNumberValueMinimized(double value)
        {
            int maxRequired = JsonConstants.MaximumFormatDoubleLength + 1; // Optionally, 1 list separator

            if (_memory.Length - BytesPending < maxRequired)
            {
                Grow(maxRequired);
            }

            Span<byte> output = _memory.Span;

            if (_currentDepth < 0)
            {
                output[BytesPending++] = JsonConstants.ListSeparator;
            }

            bool result = Utf8Formatter.TryFormat(value, output.Slice(BytesPending), out int bytesWritten);
            Debug.Assert(result);
            BytesPending += bytesWritten;
        }

        private void WriteNumberValueIndented(double value)
        {
            int indent = Indentation;
            Debug.Assert(indent <= 2 * JsonConstants.MaxWriterDepth);

            int maxRequired = indent + JsonConstants.MaximumFormatDoubleLength + 1 + s_newLineLength; // Optionally, 1 list separator and 1-2 bytes for new line

            if (_memory.Length - BytesPending < maxRequired)
            {
                Grow(maxRequired);
            }

            Span<byte> output = _memory.Span;

            if (_currentDepth < 0)
            {
                output[BytesPending++] = JsonConstants.ListSeparator;
            }

            if (_tokenType != JsonTokenType.PropertyName)
            {
                if (_tokenType != JsonTokenType.None)
                {
                    WriteNewLine(output);
                }
                JsonWriterHelper.WriteIndentation(output.Slice(BytesPending), indent);
                BytesPending += indent;
            }

            bool result = TryFormatDouble(value, output.Slice(BytesPending), out int bytesWritten);
            Debug.Assert(result);
            BytesPending += bytesWritten;
        }

        private static bool TryFormatDouble(double value, Span<byte> destination, out int bytesWritten)
        {
#if BUILDING_INBOX_LIBRARY
            return Utf8Formatter.TryFormat(value, destination, out bytesWritten);
#else
            const string FormatString = "G17";

            // We first try to format into a stack-allocated buffer, and if it succeeds, we can avoid
            // all allocation.  If that fails, we fall back to allocating strings.  If it proves impactful,
            // that allocation (as well as roundtripping from byte to char and back to byte) could be avoided by
            // calling into a refactored Number.FormatSingle/Double directly.

            const int StackBufferLength = 128; // large enough to handle the majority cases
            Span<char> stackBuffer = stackalloc char[StackBufferLength];
            ReadOnlySpan<char> utf16Text = stackalloc char[0];

            // Try to format into the stack buffer.  If we're successful, we can avoid all allocations.
            if (value.TryFormat(stackBuffer, out int formattedLength, FormatString, CultureInfo.InvariantCulture))
            {
                utf16Text = stackBuffer.Slice(0, formattedLength);
            }
            else
            {
                // The stack buffer wasn't large enough.  If the destination buffer isn't at least as
                // big as the stack buffer, we know the whole operation will eventually fail, so we
                // can just fail now.
                if (destination.Length <= StackBufferLength)
                {
                    bytesWritten = 0;
                    return false;
                }

                // Fall back to using a string format and allocating a string for the resulting formatted value.
                utf16Text = value.ToString(FormatString, CultureInfo.InvariantCulture);
            }

            // Copy the value to the destination, if it's large enough.

            if (utf16Text.Length > destination.Length)
            {
                bytesWritten = 0;
                return false;
            }

            try
            {
                bytesWritten = Encoding.UTF8.GetBytes(utf16Text, destination);
                return true;
            }
            catch
            {
                bytesWritten = 0;
                return false;
            }
#endif
        }
    }
}

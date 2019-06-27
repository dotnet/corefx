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
        /// Writes the <see cref="float"/> value (as a JSON number) as an element of a JSON array.
        /// </summary>
        /// <param name="value">The value to be written as a JSON number as an element of a JSON array.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        /// <remarks>
        /// Writes the <see cref="float"/> using the default <see cref="StandardFormat"/> on .NET Core 3 or higher
        /// and 'G9' on any other framework.
        /// </remarks>
        public void WriteNumberValue(float value)
        {
            JsonWriterHelper.ValidateSingle(value);

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

        private void WriteNumberValueMinimized(float value)
        {
            int maxRequired = JsonConstants.MaximumFormatSingleLength + 1; // Optionally, 1 list separator

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

        private void WriteNumberValueIndented(float value)
        {
            int indent = Indentation;
            Debug.Assert(indent <= 2 * JsonConstants.MaxWriterDepth);

            int maxRequired = indent + JsonConstants.MaximumFormatSingleLength + 1 + s_newLineLength; // Optionally, 1 list separator and 1-2 bytes for new line

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

            bool result = TryFormatSingle(value, output.Slice(BytesPending), out int bytesWritten);
            Debug.Assert(result);
            BytesPending += bytesWritten;
        }

        private static bool TryFormatSingle(float value, Span<byte> destination, out int bytesWritten)
        {
            // Frameworks that are not .NET Core 3.0 or higher do not produce roundtrippable strings by
            // default. Further, the Utf8Formatter on older frameworks does not support taking a precision
            // specifier for 'G' nor does it represent other formats such as 'R'. As such, we duplicate
            // the .NET Core 3.0 logic of forwarding to the UTF16 formatter and transcoding it back to UTF8.

#if BUILDING_INBOX_LIBRARY
            return Utf8Formatter.TryFormat(value, destination, out bytesWritten);
#else
            const string FormatString = "G9";

            // We first try to format into a stack-allocated buffer, and if it succeeds, we can avoid
            // all allocation.  If that fails, we fall back to allocating strings.  If it proves impactful,
            // that allocation (as well as roundtripping from byte to char and back to byte) could be avoided by
            // calling into a refactored Number.FormatSingle/Double directly.

#if netfx
            // However, the ISpanFormattable interface isn't available for netfx, so it needs to be #ifdef'd out.
            string utf16Text = string.Empty;
            {
#else
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
#endif

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
#if netfx
                byte[] bytes = Encoding.UTF8.GetBytes(utf16Text);

                if (bytes.Length > destination.Length)
                {
                    bytesWritten = 0;
                    return false;
                }

                bytes.CopyTo(destination);
                bytesWritten = bytes.Length;
#else
                bytesWritten = Encoding.UTF8.GetBytes(utf16Text, destination);
#endif
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

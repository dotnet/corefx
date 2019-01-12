// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;

namespace System.Text.Json
{
    public ref partial struct Utf8JsonWriter
    {
        /// <summary>
        /// Writes the property name and <see cref="Guid"/> value (as a JSON string) as part of a name/value pair of a JSON object.
        /// </summary>
        /// <param name="propertyName">The UTF-16 encoded property name of the JSON object to be transcoded and written as UTF-8.</param>
        /// <param name="value">The value to be written as a JSON string as part of the name/value pair.</param>
        /// <param name="suppressEscaping">If this is set, the writer assumes the property name is properly escaped and skips the escaping step.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified property name is too large.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        /// <remarks>
        /// Writes the <see cref="Guid"/> using the default <see cref="StandardFormat"/> (i.e. 'D'), as the form: nnnnnnnn-nnnn-nnnn-nnnn-nnnnnnnnnnnn.
        /// </remarks>
        public void WriteString(string propertyName, Guid value, bool suppressEscaping = false)
            => WriteString(propertyName.AsSpan(), value, suppressEscaping);

        /// <summary>
        /// Writes the property name and <see cref="Guid"/> value (as a JSON string) as part of a name/value pair of a JSON object.
        /// </summary>
        /// <param name="propertyName">The UTF-16 encoded property name of the JSON object to be transcoded and written as UTF-8.</param>
        /// <param name="value">The value to be written as a JSON string as part of the name/value pair.</param>
        /// <param name="suppressEscaping">If this is set, the writer assumes the property name is properly escaped and skips the escaping step.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified property name is too large.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        /// <remarks>
        /// Writes the <see cref="Guid"/> using the default <see cref="StandardFormat"/> (i.e. 'D'), as the form: nnnnnnnn-nnnn-nnnn-nnnn-nnnnnnnnnnnn.
        /// </remarks>
        public void WriteString(ReadOnlySpan<char> propertyName, Guid value, bool suppressEscaping = false)
        {
            JsonWriterHelper.ValidateProperty(propertyName);

            if (!suppressEscaping)
            {
                WriteStringSuppressFalse(propertyName, value);
            }
            else
            {
                WriteStringByOptions(propertyName, value);
            }

            SetFlagToAddListSeparatorBeforeNextItem();
            _tokenType = JsonTokenType.String;
        }

        /// <summary>
        /// Writes the property name and <see cref="Guid"/> value (as a JSON string) as part of a name/value pair of a JSON object.
        /// </summary>
        /// <param name="propertyName">The UTF-8 encoded property name of the JSON object to be written.</param>
        /// <param name="value">The value to be written as a JSON string as part of the name/value pair.</param>
        /// <param name="suppressEscaping">If this is set, the writer assumes the property name is properly escaped and skips the escaping step.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified property name is too large.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        /// <remarks>
        /// Writes the <see cref="Guid"/> using the default <see cref="StandardFormat"/> (i.e. 'D'), as the form: nnnnnnnn-nnnn-nnnn-nnnn-nnnnnnnnnnnn.
        /// </remarks>
        public void WriteString(ReadOnlySpan<byte> propertyName, Guid value, bool suppressEscaping = false)
        {
            JsonWriterHelper.ValidateProperty(propertyName);

            if (!suppressEscaping)
            {
                WriteStringSuppressFalse(propertyName, value);
            }
            else
            {
                WriteStringByOptions(propertyName, value);
            }

            SetFlagToAddListSeparatorBeforeNextItem();
            _tokenType = JsonTokenType.String;
        }

        private void WriteStringSuppressFalse(ReadOnlySpan<char> propertyName, Guid value)
        {
            int propertyIdx = JsonWriterHelper.NeedsEscaping(propertyName);

            Debug.Assert(propertyIdx >= -1 && propertyIdx < int.MaxValue / 2);

            if (propertyIdx != -1)
            {
                WriteStringEscapeProperty(propertyName, value, propertyIdx);
            }
            else
            {
                WriteStringByOptions(propertyName, value);
            }
        }

        private void WriteStringSuppressFalse(ReadOnlySpan<byte> propertyName, Guid value)
        {
            int propertyIdx = JsonWriterHelper.NeedsEscaping(propertyName);

            Debug.Assert(propertyIdx >= -1 && propertyIdx < int.MaxValue / 2);

            if (propertyIdx != -1)
            {
                WriteStringEscapeProperty(propertyName, value, propertyIdx);
            }
            else
            {
                WriteStringByOptions(propertyName, value);
            }
        }

        private void WriteStringEscapeProperty(ReadOnlySpan<char> propertyName, Guid value, int firstEscapeIndexProp)
        {
            Debug.Assert(int.MaxValue / JsonConstants.MaxExpansionFactorWhileEscaping >= propertyName.Length);

            char[] propertyArray = null;

            int length = JsonWriterHelper.GetMaxEscapedLength(propertyName.Length, firstEscapeIndexProp);
            Span<char> span;
            if (length > StackallocThreshold)
            {
                propertyArray = ArrayPool<char>.Shared.Rent(length);
                span = propertyArray;
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
            JsonWriterHelper.EscapeString(propertyName, span, firstEscapeIndexProp, out int written);

            WriteStringByOptions(span.Slice(0, written), value);

            if (propertyArray != null)
            {
                ArrayPool<char>.Shared.Return(propertyArray);
            }
        }

        private void WriteStringEscapeProperty(ReadOnlySpan<byte> propertyName, Guid value, int firstEscapeIndexProp)
        {
            Debug.Assert(int.MaxValue / JsonConstants.MaxExpansionFactorWhileEscaping >= propertyName.Length);

            byte[] propertyArray = null;

            int length = JsonWriterHelper.GetMaxEscapedLength(propertyName.Length, firstEscapeIndexProp);
            Span<byte> span;
            if (length > StackallocThreshold)
            {
                propertyArray = ArrayPool<byte>.Shared.Rent(length);
                span = propertyArray;
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
            JsonWriterHelper.EscapeString(propertyName, span, firstEscapeIndexProp, out int written);

            WriteStringByOptions(span.Slice(0, written), value);

            if (propertyArray != null)
            {
                ArrayPool<byte>.Shared.Return(propertyArray);
            }
        }

        private void WriteStringByOptions(ReadOnlySpan<char> propertyName, Guid value)
        {
            ValidateWritingProperty();
            if (_writerOptions.Indented)
            {
                WriteStringIndented(propertyName, value);
            }
            else
            {
                WriteStringMinimized(propertyName, value);
            }
        }

        private void WriteStringByOptions(ReadOnlySpan<byte> propertyName, Guid value)
        {
            ValidateWritingProperty();
            if (_writerOptions.Indented)
            {
                WriteStringIndented(propertyName, value);
            }
            else
            {
                WriteStringMinimized(propertyName, value);
            }
        }

        private void WriteStringMinimized(ReadOnlySpan<char> escapedPropertyName, Guid value)
        {
            int idx = WritePropertyNameMinimized(escapedPropertyName);

            WriteStringValue(value, ref idx);

            Advance(idx);
        }

        private void WriteStringMinimized(ReadOnlySpan<byte> escapedPropertyName, Guid value)
        {
            int idx = WritePropertyNameMinimized(escapedPropertyName);

            WriteStringValue(value, ref idx);

            Advance(idx);
        }

        private void WriteStringIndented(ReadOnlySpan<char> escapedPropertyName, Guid value)
        {
            int idx = WritePropertyNameIndented(escapedPropertyName);

            WriteStringValue(value, ref idx);

            Advance(idx);
        }

        private void WriteStringIndented(ReadOnlySpan<byte> escapedPropertyName, Guid value)
        {
            int idx = WritePropertyNameIndented(escapedPropertyName);

            WriteStringValue(value, ref idx);

            Advance(idx);
        }

        private void WriteStringValue(Guid value, ref int idx)
        {
            if (_buffer.Length <= idx)
            {
                AdvanceAndGrow(ref idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.Quote;

            FormatLoop(value, ref idx);

            if (_buffer.Length <= idx)
            {
                AdvanceAndGrow(ref idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.Quote;
        }

        private void FormatLoop(Guid value, ref int idx)
        {
            if (!Utf8Formatter.TryFormat(value, _buffer.Slice(idx), out int bytesWritten))
            {
                AdvanceAndGrow(ref idx, JsonConstants.MaximumFormatGuidLength);
                Utf8Formatter.TryFormat(value, _buffer, out bytesWritten);
            }
            idx += bytesWritten;
        }
    }
}

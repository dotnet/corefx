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
        /// Writes the property name and <see cref="double"/> value (as a JSON number) as part of a name/value pair of a JSON object.
        /// </summary>
        /// <param name="propertyName">The UTF-16 encoded property name of the JSON object to be transcoded and written as UTF-8.</param>
        /// <param name="value">The value to be written as a JSON number as part of the name/value pair.</param>
        /// <param name="suppressEscaping">If this is set, the writer assumes the property name is properly escaped and skips the escaping step.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified property name is too large.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteNumber(string propertyName, double value, bool suppressEscaping = false)
            => WriteNumber(propertyName.AsSpan(), value, suppressEscaping);

        /// <summary>
        /// Writes the property name and <see cref="double"/> value (as a JSON number) as part of a name/value pair of a JSON object.
        /// </summary>
        /// <param name="propertyName">The UTF-16 encoded property name of the JSON object to be transcoded and written as UTF-8.</param>
        /// <param name="value">The value to be written as a JSON number as part of the name/value pair.</param>
        /// <param name="suppressEscaping">If this is set, the writer assumes the property name is properly escaped and skips the escaping step.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified property name is too large.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteNumber(ReadOnlySpan<char> propertyName, double value, bool suppressEscaping = false)
        {
            JsonWriterHelper.ValidateProperty(propertyName);
            JsonWriterHelper.ValidateDouble(value);

            if (!suppressEscaping)
            {
                WriteNumberSuppressFalse(propertyName, value);
            }
            else
            {
                WriteNumberByOptions(propertyName, value);
            }

            _currentDepth |= 1 << 31;
            _tokenType = JsonTokenType.Number;
        }

        /// <summary>
        /// Writes the property name and <see cref="double"/> value (as a JSON number) as part of a name/value pair of a JSON object.
        /// </summary>
        /// <param name="propertyName">The UTF-8 encoded property name of the JSON object to be written.</param>
        /// <param name="value">The value to be written as a JSON number as part of the name/value pair.</param>
        /// <param name="suppressEscaping">If this is set, the writer assumes the property name is properly escaped and skips the escaping step.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified property name is too large.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteNumber(ReadOnlySpan<byte> propertyName, double value, bool suppressEscaping = false)
        {
            JsonWriterHelper.ValidateProperty(propertyName);
            JsonWriterHelper.ValidateDouble(value);

            if (!suppressEscaping)
            {
                WriteNumberSuppressFalse(propertyName, value);
            }
            else
            {
                WriteNumberByOptions(propertyName, value);
            }

            _currentDepth |= 1 << 31;
            _tokenType = JsonTokenType.Number;
        }

        private void WriteNumberSuppressFalse(in ReadOnlySpan<char> propertyName, double value)
        {
            int propertyIdx = JsonWriterHelper.NeedsEscaping(propertyName);

            Debug.Assert(propertyIdx >= -1 && propertyIdx < int.MaxValue / 2);

            if (propertyIdx != -1)
            {
                WriteNumberEscapeProperty(propertyName, value, propertyIdx);
            }
            else
            {
                WriteNumberByOptions(propertyName, value);
            }
        }

        private void WriteNumberSuppressFalse(in ReadOnlySpan<byte> propertyName, double value)
        {
            int propertyIdx = JsonWriterHelper.NeedsEscaping(propertyName);

            Debug.Assert(propertyIdx >= -1 && propertyIdx < int.MaxValue / 2);

            if (propertyIdx != -1)
            {
                WriteNumberEscapeProperty(propertyName, value, propertyIdx);
            }
            else
            {
                WriteNumberByOptions(propertyName, value);
            }
        }

        private void WriteNumberEscapeProperty(in ReadOnlySpan<char> propertyName, double value, int firstEscapeIndexProp)
        {
            Debug.Assert(int.MaxValue / JsonConstants.MaxExpansionFactorWhileEscaping >= propertyName.Length);

            char[] propertyArray = null;

            int length = firstEscapeIndexProp + JsonConstants.MaxExpansionFactorWhileEscaping * (propertyName.Length - firstEscapeIndexProp);
            Span<char> span;
            if (length > StackallocThreshold)
            {
                propertyArray = ArrayPool<char>.Shared.Rent(length);
                span = propertyArray;
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
            JsonWriterHelper.EscapeString(propertyName, span, firstEscapeIndexProp, out int written);

            WriteNumberByOptions(span.Slice(0, written), value);

            if (propertyArray != null)
            {
                ArrayPool<char>.Shared.Return(propertyArray);
            }
        }

        private void WriteNumberEscapeProperty(in ReadOnlySpan<byte> propertyName, double value, int firstEscapeIndexProp)
        {
            Debug.Assert(int.MaxValue / JsonConstants.MaxExpansionFactorWhileEscaping >= propertyName.Length);

            byte[] propertyArray = null;

            int length = firstEscapeIndexProp + JsonConstants.MaxExpansionFactorWhileEscaping * (propertyName.Length - firstEscapeIndexProp);
            Span<byte> span;
            if (length > StackallocThreshold)
            {
                propertyArray = ArrayPool<byte>.Shared.Rent(length);
                span = propertyArray;
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
            JsonWriterHelper.EscapeString(propertyName, span, firstEscapeIndexProp, out int written);

            WriteNumberByOptions(span.Slice(0, written), value);

            if (propertyArray != null)
            {
                ArrayPool<byte>.Shared.Return(propertyArray);
            }
        }

        private void WriteNumberByOptions(in ReadOnlySpan<char> propertyName, double value)
        {
            if (_writerOptions.Indented)
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingProperty();
                }
                WriteNumberIndented(propertyName, value);
            }
            else
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingProperty();
                }
                WriteNumberMinimized(propertyName, value);
            }
        }

        private void WriteNumberByOptions(in ReadOnlySpan<byte> propertyName, double value)
        {
            if (_writerOptions.Indented)
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingProperty();
                }
                WriteNumberIndented(propertyName, value);
            }
            else
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingProperty();
                }
                WriteNumberMinimized(propertyName, value);
            }
        }

        private void WriteNumberMinimized(in ReadOnlySpan<char> escapedPropertyName, double value)
        {
            int idx = WritePropertyNameMinimized(escapedPropertyName);

            WriteNumberValueFormatLoop(value, ref idx);

            Advance(idx);
        }

        private void WriteNumberMinimized(in ReadOnlySpan<byte> escapedPropertyName, double value)
        {
            int idx = WritePropertyNameMinimized(escapedPropertyName);

            WriteNumberValueFormatLoop(value, ref idx);

            Advance(idx);
        }

        private void WriteNumberIndented(in ReadOnlySpan<char> escapedPropertyName, double value)
        {
            int idx = WritePropertyNameIndented(escapedPropertyName);

            WriteNumberValueFormatLoop(value, ref idx);

            Advance(idx);
        }

        private void WriteNumberIndented(in ReadOnlySpan<byte> escapedPropertyName, double value)
        {
            int idx = WritePropertyNameIndented(escapedPropertyName);

            WriteNumberValueFormatLoop(value, ref idx);

            Advance(idx);
        }

        private void WriteNumberValueFormatLoop(double value, ref int idx)
        {
            int bytesWritten;
            while (!Utf8Formatter.TryFormat(value, _buffer.Slice(idx), out bytesWritten))
            {
                AdvanceAndGrow(idx, JsonConstants.MaximumFormatDoubleLength);
                idx = 0;
            }
            idx += bytesWritten;
        }
    }
}

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
        public void WriteString(string propertyName, DateTime value, bool suppressEscaping = false)
            => WriteString(propertyName.AsSpan(), value, suppressEscaping);

        public void WriteString(ReadOnlySpan<char> propertyName, DateTime value, bool suppressEscaping = false)
        {
            JsonWriterHelper.ValidateProperty(ref propertyName);

            if (!suppressEscaping)
                WriteStringSuppressFalse(ref propertyName, value);
            else
                WriteStringByOptions(ref propertyName, value);

            _currentDepth |= 1 << 31;
            _tokenType = JsonTokenType.String;
        }

        public void WriteString(ReadOnlySpan<byte> propertyName, DateTime value, bool suppressEscaping = false)
        {
            JsonWriterHelper.ValidateProperty(ref propertyName);

            if (!suppressEscaping)
                WriteStringSuppressFalse(ref propertyName, value);
            else
                WriteStringByOptions(ref propertyName, value);

            _currentDepth |= 1 << 31;
            _tokenType = JsonTokenType.String;
        }

        private void WriteStringSuppressFalse(ref ReadOnlySpan<char> propertyName, DateTime value)
        {
            int propertyIdx = JsonWriterHelper.NeedsEscaping(propertyName);

            Debug.Assert(propertyIdx >= -1 && propertyIdx < int.MaxValue / 2);

            if (propertyIdx != -1)
            {
                WriteStringEscapeProperty(ref propertyName, value, propertyIdx);
            }
            else
            {
                WriteStringByOptions(ref propertyName, value);
            }
        }

        private void WriteStringSuppressFalse(ref ReadOnlySpan<byte> propertyName, DateTime value)
        {
            int propertyIdx = JsonWriterHelper.NeedsEscaping(propertyName);

            Debug.Assert(propertyIdx >= -1 && propertyIdx < int.MaxValue / 2);

            if (propertyIdx != -1)
            {
                WriteStringEscapeProperty(ref propertyName, value, propertyIdx);
            }
            else
            {
                WriteStringByOptions(ref propertyName, value);
            }
        }

        private void WriteStringEscapeProperty(ref ReadOnlySpan<char> propertyName, DateTime value, int firstEscapeIndexProp)
        {
            Debug.Assert(int.MaxValue / 6 >= propertyName.Length);

            char[] propertyArray = null;

            int length = firstEscapeIndexProp + 6 * (propertyName.Length - firstEscapeIndexProp);
            Span<char> span;
            if (length > 256)
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
            JsonWriterHelper.EscapeString(ref propertyName, ref span, firstEscapeIndexProp, out int written);
            propertyName = span.Slice(0, written);

            WriteStringByOptions(ref propertyName, value);

            if (propertyArray != null)
                ArrayPool<char>.Shared.Return(propertyArray);
        }

        private void WriteStringEscapeProperty(ref ReadOnlySpan<byte> propertyName, DateTime value, int firstEscapeIndexProp)
        {
            Debug.Assert(int.MaxValue / 6 >= propertyName.Length);

            byte[] propertyArray = null;

            int length = firstEscapeIndexProp + 6 * (propertyName.Length - firstEscapeIndexProp);
            Span<byte> span;
            if (length > 256)
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
            JsonWriterHelper.EscapeString(ref propertyName, ref span, firstEscapeIndexProp, out int written);
            propertyName = span.Slice(0, written);

            WriteStringByOptions(ref propertyName, value);

            if (propertyArray != null)
                ArrayPool<byte>.Shared.Return(propertyArray);
        }

        private void WriteStringByOptions(ref ReadOnlySpan<char> propertyName, DateTime value)
        {
            if (_writerOptions.Indented)
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingProperty();
                }
                WriteStringIndented(ref propertyName, value);
            }
            else
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingProperty();
                }
                WriteStringMinimized(ref propertyName, value);
            }
        }

        private void WriteStringByOptions(ref ReadOnlySpan<byte> propertyName, DateTime value)
        {
            if (_writerOptions.Indented)
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingProperty();
                }
                WriteStringIndented(ref propertyName, value);
            }
            else
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingProperty();
                }
                WriteStringMinimized(ref propertyName, value);
            }
        }

        private void WriteStringMinimized(ref ReadOnlySpan<char> escapedPropertyName, DateTime value)
        {
            int idx = WritePropertyNameMinimized(ref escapedPropertyName);

            WriteStringValue(value, ref idx);

            Advance(idx);
        }

        private void WriteStringMinimized(ref ReadOnlySpan<byte> escapedPropertyName, DateTime value)
        {
            int idx = WritePropertyNameMinimized(ref escapedPropertyName);

            WriteStringValue(value, ref idx);

            Advance(idx);
        }

        private void WriteStringIndented(ref ReadOnlySpan<char> escapedPropertyName, DateTime value)
        {
            int idx = WritePropertyNameIndented(ref escapedPropertyName);

            WriteStringValue(value, ref idx);

            Advance(idx);
        }

        private void WriteStringIndented(ref ReadOnlySpan<byte> escapedPropertyName, DateTime value)
        {
            int idx = WritePropertyNameIndented(ref escapedPropertyName);

            WriteStringValue(value, ref idx);

            Advance(idx);
        }

        private void WriteStringValue(DateTime value, ref int idx)
        {
            while (_buffer.Length <= idx)
            {
                AdvanceAndGrow(idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.Quote;

            FormatLoop(value, ref idx);

            while (_buffer.Length <= idx)
            {
                AdvanceAndGrow(idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.Quote;
        }

        private void FormatLoop(DateTime value, ref int idx)
        {
            int bytesWritten;
            while (!Utf8Formatter.TryFormat(value, _buffer.Slice(idx), out bytesWritten))
            {
                AdvanceAndGrow(idx, JsonConstants.MaximumDateTimeLength);
                idx = 0;
            }
            idx += bytesWritten;
        }
    }
}

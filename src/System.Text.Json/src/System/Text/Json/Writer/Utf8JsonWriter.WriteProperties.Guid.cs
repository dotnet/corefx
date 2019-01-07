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
        public void WriteString(string propertyName, Guid value, bool suppressEscaping = false)
            => WriteString(propertyName.AsSpan(), value, suppressEscaping);

        public void WriteString(ReadOnlySpan<char> propertyName, Guid value, bool suppressEscaping = false)
        {
            JsonWriterHelper.ValidateProperty(ref propertyName);

            if (!suppressEscaping)
                WriteStringSuppressFalse(ref propertyName, value);
            else
                WriteStringByOptions(ref propertyName, value);

            _currentDepth |= 1 << 31;
            _tokenType = JsonTokenType.String;
        }

        public void WriteString(ReadOnlySpan<byte> propertyName, Guid value, bool suppressEscaping = false)
        {
            JsonWriterHelper.ValidateProperty(ref propertyName);

            if (!suppressEscaping)
                WriteStringSuppressFalse(ref propertyName, value);
            else
                WriteStringByOptions(ref propertyName, value);

            _currentDepth |= 1 << 31;
            _tokenType = JsonTokenType.String;
        }

        private void WriteStringSuppressFalse(ref ReadOnlySpan<char> propertyName, Guid value)
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

        private void WriteStringSuppressFalse(ref ReadOnlySpan<byte> propertyName, Guid value)
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

        private void WriteStringEscapeProperty(ref ReadOnlySpan<char> propertyName, Guid value, int firstEscapeIndexProp)
        {
            Debug.Assert(int.MaxValue / MaxExpansionFactorWhileEscaping >= propertyName.Length);

            char[] propertyArray = null;

            int length = firstEscapeIndexProp + MaxExpansionFactorWhileEscaping * (propertyName.Length - firstEscapeIndexProp);
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
            JsonWriterHelper.EscapeString(ref propertyName, ref span, firstEscapeIndexProp, out int written);
            propertyName = span.Slice(0, written);

            WriteStringByOptions(ref propertyName, value);

            if (propertyArray != null)
                ArrayPool<char>.Shared.Return(propertyArray);
        }

        private void WriteStringEscapeProperty(ref ReadOnlySpan<byte> propertyName, Guid value, int firstEscapeIndexProp)
        {
            Debug.Assert(int.MaxValue / MaxExpansionFactorWhileEscaping >= propertyName.Length);

            byte[] propertyArray = null;

            int length = firstEscapeIndexProp + MaxExpansionFactorWhileEscaping * (propertyName.Length - firstEscapeIndexProp);
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
            JsonWriterHelper.EscapeString(ref propertyName, ref span, firstEscapeIndexProp, out int written);
            propertyName = span.Slice(0, written);

            WriteStringByOptions(ref propertyName, value);

            if (propertyArray != null)
                ArrayPool<byte>.Shared.Return(propertyArray);
        }

        private void WriteStringByOptions(ref ReadOnlySpan<char> propertyName, Guid value)
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

        private void WriteStringByOptions(ref ReadOnlySpan<byte> propertyName, Guid value)
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

        private void WriteStringMinimized(ref ReadOnlySpan<char> escapedPropertyName, Guid value)
        {
            int idx = WritePropertyNameMinimized(ref escapedPropertyName);

            WriteStringValue(value, ref idx);

            Advance(idx);
        }

        private void WriteStringMinimized(ref ReadOnlySpan<byte> escapedPropertyName, Guid value)
        {
            int idx = WritePropertyNameMinimized(ref escapedPropertyName);

            WriteStringValue(value, ref idx);

            Advance(idx);
        }

        private void WriteStringIndented(ref ReadOnlySpan<char> escapedPropertyName, Guid value)
        {
            int idx = WritePropertyNameIndented(ref escapedPropertyName);

            WriteStringValue(value, ref idx);

            Advance(idx);
        }

        private void WriteStringIndented(ref ReadOnlySpan<byte> escapedPropertyName, Guid value)
        {
            int idx = WritePropertyNameIndented(ref escapedPropertyName);

            WriteStringValue(value, ref idx);

            Advance(idx);
        }

        private void WriteStringValue(Guid value, ref int idx)
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

        private void FormatLoop(Guid value, ref int idx)
        {
            int bytesWritten;
            while (!Utf8Formatter.TryFormat(value, _buffer.Slice(idx), out bytesWritten))
            {
                AdvanceAndGrow(idx, JsonConstants.MaximumGuidLength);
                idx = 0;
            }
            idx += bytesWritten;
        }
    }
}

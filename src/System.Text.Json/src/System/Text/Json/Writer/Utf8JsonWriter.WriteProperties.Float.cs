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
        public void WriteNumber(string propertyName, float value, bool suppressEscaping = false)
            => WriteNumber(propertyName.AsSpan(), value, suppressEscaping);

        public void WriteNumber(ReadOnlySpan<char> propertyName, float value, bool suppressEscaping = false)
        {
            JsonWriterHelper.ValidateProperty(ref propertyName);

            if (!suppressEscaping)
                WriteNumberSuppressFalse(ref propertyName, value);
            else
                WriteNumberByOptions(ref propertyName, value);

            _currentDepth |= 1 << 31;
            _tokenType = JsonTokenType.Number;
        }

        public void WriteNumber(ReadOnlySpan<byte> propertyName, float value, bool suppressEscaping = false)
        {
            JsonWriterHelper.ValidateProperty(ref propertyName);

            if (!suppressEscaping)
                WriteNumberSuppressFalse(ref propertyName, value);
            else
                WriteNumberByOptions(ref propertyName, value);

            _currentDepth |= 1 << 31;
            _tokenType = JsonTokenType.Number;
        }

        private void WriteNumberSuppressFalse(ref ReadOnlySpan<char> propertyName, float value)
        {
            int propertyIdx = JsonWriterHelper.NeedsEscaping(propertyName);

            Debug.Assert(propertyIdx >= -1 && propertyIdx < int.MaxValue / 2);

            if (propertyIdx != -1)
            {
                WriteNumberEscapeProperty(ref propertyName, value, propertyIdx);
            }
            else
            {
                WriteNumberByOptions(ref propertyName, value);
            }
        }

        private void WriteNumberSuppressFalse(ref ReadOnlySpan<byte> propertyName, float value)
        {
            int propertyIdx = JsonWriterHelper.NeedsEscaping(propertyName);

            Debug.Assert(propertyIdx >= -1 && propertyIdx < int.MaxValue / 2);

            if (propertyIdx != -1)
            {
                WriteNumberEscapeProperty(ref propertyName, value, propertyIdx);
            }
            else
            {
                WriteNumberByOptions(ref propertyName, value);
            }
        }

        private void WriteNumberEscapeProperty(ref ReadOnlySpan<char> propertyName, float value, int firstEscapeIndexProp)
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

            WriteNumberByOptions(ref propertyName, value);

            if (propertyArray != null)
                ArrayPool<char>.Shared.Return(propertyArray);
        }

        private void WriteNumberEscapeProperty(ref ReadOnlySpan<byte> propertyName, float value, int firstEscapeIndexProp)
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

            WriteNumberByOptions(ref propertyName, value);

            if (propertyArray != null)
                ArrayPool<byte>.Shared.Return(propertyArray);
        }

        private void WriteNumberByOptions(ref ReadOnlySpan<char> propertyName, float value)
        {
            if (_writerOptions.Indented)
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingProperty();
                }
                WriteNumberIndented(ref propertyName, value);
            }
            else
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingProperty();
                }
                WriteNumberMinimized(ref propertyName, value);
            }
        }

        private void WriteNumberByOptions(ref ReadOnlySpan<byte> propertyName, float value)
        {
            if (_writerOptions.Indented)
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingProperty();
                }
                WriteNumberIndented(ref propertyName, value);
            }
            else
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingProperty();
                }
                WriteNumberMinimized(ref propertyName, value);
            }
        }

        private void WriteNumberMinimized(ref ReadOnlySpan<char> escapedPropertyName, float value)
        {
            int idx = WritePropertyNameMinimized(ref escapedPropertyName);

            WriteNumberValueFormatLoop(value, ref idx);

            Advance(idx);
        }

        private void WriteNumberMinimized(ref ReadOnlySpan<byte> escapedPropertyName, float value)
        {
            int idx = WritePropertyNameMinimized(ref escapedPropertyName);

            WriteNumberValueFormatLoop(value, ref idx);

            Advance(idx);
        }

        private void WriteNumberIndented(ref ReadOnlySpan<char> escapedPropertyName, float value)
        {
            int idx = WritePropertyNameIndented(ref escapedPropertyName);

            WriteNumberValueFormatLoop(value, ref idx);

            Advance(idx);
        }

        private void WriteNumberIndented(ref ReadOnlySpan<byte> escapedPropertyName, float value)
        {
            int idx = WritePropertyNameIndented(ref escapedPropertyName);

            WriteNumberValueFormatLoop(value, ref idx);

            Advance(idx);
        }

        private void WriteNumberValueFormatLoop(float value, ref int idx)
        {
            int bytesWritten;
            while (!Utf8Formatter.TryFormat(value, _buffer.Slice(idx), out bytesWritten))
            {
                AdvanceAndGrow(idx, JsonConstants.MaximumSingleLength);
                idx = 0;
            }
            idx += bytesWritten;
        }
    }
}

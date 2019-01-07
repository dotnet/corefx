// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;

namespace System.Text.Json
{
    public ref partial struct Utf8JsonWriter
    {
        [CLSCompliant(false)]
        public void WriteNumber(string propertyName, ulong value, bool suppressEscaping = false)
            => WriteNumber(propertyName.AsSpan(), value, suppressEscaping);

        [CLSCompliant(false)]
        public void WriteNumber(ReadOnlySpan<char> propertyName, ulong value, bool suppressEscaping = false)
        {
            JsonWriterHelper.ValidateProperty(ref propertyName);

            if (!suppressEscaping)
                WriteNumberSuppressFalse(ref propertyName, value);
            else
                WriteNumberByOptions(ref propertyName, value);

            _currentDepth |= 1 << 31;
            _tokenType = JsonTokenType.Number;
        }

        [CLSCompliant(false)]
        public void WriteNumber(ReadOnlySpan<byte> propertyName, ulong value, bool suppressEscaping = false)
        {
            JsonWriterHelper.ValidateProperty(ref propertyName);

            if (!suppressEscaping)
                WriteNumberSuppressFalse(ref propertyName, value);
            else
                WriteNumberByOptions(ref propertyName, value);

            _currentDepth |= 1 << 31;
            _tokenType = JsonTokenType.Number;
        }

        [CLSCompliant(false)]
        public void WriteNumber(string propertyName, uint value, bool suppressEscaping = false)
            => WriteNumber(propertyName.AsSpan(), (ulong)value, suppressEscaping);

        [CLSCompliant(false)]
        public void WriteNumber(ReadOnlySpan<char> propertyName, uint value, bool suppressEscaping = false)
            => WriteNumber(propertyName, (ulong)value, suppressEscaping);

        [CLSCompliant(false)]
        public void WriteNumber(ReadOnlySpan<byte> propertyName, uint value, bool suppressEscaping = false)
            => WriteNumber(propertyName, (ulong)value, suppressEscaping);

        private void WriteNumberSuppressFalse(ref ReadOnlySpan<char> propertyName, ulong value)
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

        private void WriteNumberSuppressFalse(ref ReadOnlySpan<byte> propertyName, ulong value)
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

        private void WriteNumberEscapeProperty(ref ReadOnlySpan<char> propertyName, ulong value, int firstEscapeIndexProp)
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

        private void WriteNumberEscapeProperty(ref ReadOnlySpan<byte> propertyName, ulong value, int firstEscapeIndexProp)
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

        private void WriteNumberByOptions(ref ReadOnlySpan<char> propertyName, ulong value)
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

        private void WriteNumberByOptions(ref ReadOnlySpan<byte> propertyName, ulong value)
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

        private void WriteNumberMinimized(ref ReadOnlySpan<char> escapedPropertyName, ulong value)
        {
            int idx = WritePropertyNameMinimized(ref escapedPropertyName);

            WriteNumberValueFormatLoop(value, ref idx);

            Advance(idx);
        }

        private void WriteNumberMinimized(ref ReadOnlySpan<byte> escapedPropertyName, ulong value)
        {
            int idx = WritePropertyNameMinimized(ref escapedPropertyName);

            WriteNumberValueFormatLoop(value, ref idx);

            Advance(idx);
        }

        private void WriteNumberIndented(ref ReadOnlySpan<char> escapedPropertyName, ulong value)
        {
            int idx = WritePropertyNameIndented(ref escapedPropertyName);

            WriteNumberValueFormatLoop(value, ref idx);

            Advance(idx);
        }

        private void WriteNumberIndented(ref ReadOnlySpan<byte> escapedPropertyName, ulong value)
        {
            int idx = WritePropertyNameIndented(ref escapedPropertyName);

            WriteNumberValueFormatLoop(value, ref idx);

            Advance(idx);
        }

        private void WriteNumberValueFormatLoop(ulong value, ref int idx)
        {
            int bytesWritten;
            // Using Utf8Formatter with default StandardFormat is roughly 30% slower (17 ns versus 12 ns)
            // See: https://github.com/dotnet/corefx/issues/25425
            // Utf8Formatter.TryFormat(value, _buffer.Slice(idx), out bytesWritten);
            while (!JsonWriterHelper.TryFormatUInt64Default(value, _buffer.Slice(idx), out bytesWritten))
            {
                AdvanceAndGrow(idx, JsonConstants.MaximumUInt64Length);
                idx = 0;
            }
            idx += bytesWritten;
        }
    }
}

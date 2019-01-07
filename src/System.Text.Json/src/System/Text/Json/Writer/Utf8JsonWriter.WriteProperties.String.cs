// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Text.Json
{
    public ref partial struct Utf8JsonWriter
    {
        public void WriteString(string propertyName, string value, bool suppressEscaping = false)
            => WriteString(propertyName.AsSpan(), value.AsSpan(), suppressEscaping);

        public void WriteString(ReadOnlySpan<char> propertyName, ReadOnlySpan<char> value, bool suppressEscaping = false)
        {
            JsonWriterHelper.ValidatePropertyAndValue(ref propertyName, ref value);

            if (!suppressEscaping)
                WriteStringSuppressFalse(ref propertyName, ref value);
            else
                WriteStringSuppressTrue(ref propertyName, ref value);

            _currentDepth |= 1 << 31;
            _tokenType = JsonTokenType.String;
        }

        public void WriteString(ReadOnlySpan<byte> propertyName, ReadOnlySpan<byte> value, bool suppressEscaping = false)
        {
            JsonWriterHelper.ValidatePropertyAndValue(ref propertyName, ref value);

            if (!suppressEscaping)
                WriteStringSuppressFalse(ref propertyName, ref value);
            else
                WriteStringSuppressTrue(ref propertyName, ref value);

            _currentDepth |= 1 << 31;
            _tokenType = JsonTokenType.String;
        }

        public void WriteString(string propertyName, ReadOnlySpan<char> value, bool suppressEscaping = false)
            => WriteString(propertyName.AsSpan(), value, suppressEscaping);

        public void WriteString(ReadOnlySpan<byte> propertyName, ReadOnlySpan<char> value, bool suppressEscaping = false)
        {
            JsonWriterHelper.ValidatePropertyAndValue(ref propertyName, ref value);

            if (!suppressEscaping)
                WriteStringSuppressFalse(ref propertyName, ref value);
            else
                WriteStringSuppressTrue(ref propertyName, ref value);

            _currentDepth |= 1 << 31;
            _tokenType = JsonTokenType.String;
        }

        public void WriteString(string propertyName, ReadOnlySpan<byte> value, bool suppressEscaping = false)
            => WriteString(propertyName.AsSpan(), value, suppressEscaping);

        public void WriteString(ReadOnlySpan<char> propertyName, ReadOnlySpan<byte> value, bool suppressEscaping = false)
        {
            JsonWriterHelper.ValidatePropertyAndValue(ref propertyName, ref value);

            if (!suppressEscaping)
                WriteStringSuppressFalse(ref propertyName, ref value);
            else
                WriteStringSuppressTrue(ref propertyName, ref value);

            _currentDepth |= 1 << 31;
            _tokenType = JsonTokenType.String;
        }

        public void WriteString(ReadOnlySpan<char> propertyName, string value, bool suppressEscaping = false)
            => WriteString(propertyName, value.AsSpan(), suppressEscaping);

        public void WriteString(ReadOnlySpan<byte> propertyName, string value, bool suppressEscaping = false)
            => WriteString(propertyName, value.AsSpan(), suppressEscaping);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteStringSuppressTrue(ref ReadOnlySpan<char> escapedPropertyName, ref ReadOnlySpan<char> value)
        {
            int valueIdx = JsonWriterHelper.NeedsEscaping(value);
            if (valueIdx != -1)
            {
                WriteStringEscapeValueOnly(ref escapedPropertyName, ref value, valueIdx);
            }
            else
            {
                WriteStringByOptions(ref escapedPropertyName, ref value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteStringSuppressTrue(ref ReadOnlySpan<byte> escapedPropertyName, ref ReadOnlySpan<byte> value)
        {
            int valueIdx = JsonWriterHelper.NeedsEscaping(value);
            if (valueIdx != -1)
            {
                WriteStringEscapeValueOnly(ref escapedPropertyName, ref value, valueIdx);
            }
            else
            {
                WriteStringByOptions(ref escapedPropertyName, ref value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteStringSuppressTrue(ref ReadOnlySpan<char> escapedPropertyName, ref ReadOnlySpan<byte> value)
        {
            int valueIdx = JsonWriterHelper.NeedsEscaping(value);
            if (valueIdx != -1)
            {
                WriteStringEscapeValueOnly(ref escapedPropertyName, ref value, valueIdx);
            }
            else
            {
                WriteStringByOptions(ref escapedPropertyName, ref value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteStringSuppressTrue(ref ReadOnlySpan<byte> escapedPropertyName, ref ReadOnlySpan<char> value)
        {
            int valueIdx = JsonWriterHelper.NeedsEscaping(value);
            if (valueIdx != -1)
            {
                WriteStringEscapeValueOnly(ref escapedPropertyName, ref value, valueIdx);
            }
            else
            {
                WriteStringByOptions(ref escapedPropertyName, ref value);
            }
        }

        private void WriteStringEscapeValueOnly(ref ReadOnlySpan<char> escapedPropertyName, ref ReadOnlySpan<char> value, int firstEscapeIndex)
        {
            Debug.Assert(int.MaxValue / MaxExpansionFactorWhileEscaping >= value.Length);

            char[] valueArray = ArrayPool<char>.Shared.Rent(firstEscapeIndex + MaxExpansionFactorWhileEscaping * (value.Length - firstEscapeIndex));
            Span<char> span = valueArray;
            JsonWriterHelper.EscapeString(ref value, ref span, firstEscapeIndex, out int written);
            value = span.Slice(0, written);

            WriteStringByOptions(ref escapedPropertyName, ref value);

            ArrayPool<char>.Shared.Return(valueArray);
        }

        private void WriteStringEscapeValueOnly(ref ReadOnlySpan<byte> escapedPropertyName, ref ReadOnlySpan<byte> value, int firstEscapeIndex)
        {
            Debug.Assert(int.MaxValue / MaxExpansionFactorWhileEscaping >= value.Length);

            byte[] valueArray = ArrayPool<byte>.Shared.Rent(firstEscapeIndex + MaxExpansionFactorWhileEscaping * (value.Length - firstEscapeIndex));
            Span<byte> span = valueArray;
            JsonWriterHelper.EscapeString(ref value, ref span, firstEscapeIndex, out int written);
            value = span.Slice(0, written);

            WriteStringByOptions(ref escapedPropertyName, ref value);

            ArrayPool<byte>.Shared.Return(valueArray);
        }

        private void WriteStringEscapeValueOnly(ref ReadOnlySpan<char> escapedPropertyName, ref ReadOnlySpan<byte> value, int firstEscapeIndex)
        {
            Debug.Assert(int.MaxValue / MaxExpansionFactorWhileEscaping >= value.Length);

            byte[] valueArray = ArrayPool<byte>.Shared.Rent(firstEscapeIndex + MaxExpansionFactorWhileEscaping * (value.Length - firstEscapeIndex));
            Span<byte> span = valueArray;
            JsonWriterHelper.EscapeString(ref value, ref span, firstEscapeIndex, out int written);
            value = span.Slice(0, written);

            WriteStringByOptions(ref escapedPropertyName, ref value);

            ArrayPool<byte>.Shared.Return(valueArray);
        }

        private void WriteStringEscapeValueOnly(ref ReadOnlySpan<byte> escapedPropertyName, ref ReadOnlySpan<char> value, int firstEscapeIndex)
        {
            Debug.Assert(int.MaxValue / MaxExpansionFactorWhileEscaping >= value.Length);

            char[] valueArray = ArrayPool<char>.Shared.Rent(firstEscapeIndex + MaxExpansionFactorWhileEscaping * (value.Length - firstEscapeIndex));
            Span<char> span = valueArray;
            JsonWriterHelper.EscapeString(ref value, ref span, firstEscapeIndex, out int written);
            value = span.Slice(0, written);

            WriteStringByOptions(ref escapedPropertyName, ref value);

            ArrayPool<char>.Shared.Return(valueArray);
        }

        private void WriteStringSuppressFalse(ref ReadOnlySpan<char> propertyName, ref ReadOnlySpan<char> value)
        {
            int valueIdx = JsonWriterHelper.NeedsEscaping(value);
            int propertyIdx = JsonWriterHelper.NeedsEscaping(propertyName);

            Debug.Assert(valueIdx >= -1 && valueIdx < int.MaxValue / 2);
            Debug.Assert(propertyIdx >= -1 && propertyIdx < int.MaxValue / 2);

            // Equivalent to: valueIdx != -1 || propertyIdx != -1
            if (valueIdx + propertyIdx != -2)
            {
                WriteStringEscapePropertyOrValue(ref propertyName, ref value, propertyIdx, valueIdx);
            }
            else
            {
                WriteStringByOptions(ref propertyName, ref value);
            }
        }

        private void WriteStringSuppressFalse(ref ReadOnlySpan<byte> propertyName, ref ReadOnlySpan<byte> value)
        {
            int valueIdx = JsonWriterHelper.NeedsEscaping(value);
            int propertyIdx = JsonWriterHelper.NeedsEscaping(propertyName);

            Debug.Assert(valueIdx >= -1 && valueIdx < int.MaxValue / 2);
            Debug.Assert(propertyIdx >= -1 && propertyIdx < int.MaxValue / 2);

            // Equivalent to: valueIdx != -1 || propertyIdx != -1
            if (valueIdx + propertyIdx != -2)
            {
                WriteStringEscapePropertyOrValue(ref propertyName, ref value, propertyIdx, valueIdx);
            }
            else
            {
                WriteStringByOptions(ref propertyName, ref value);
            }
        }

        private void WriteStringSuppressFalse(ref ReadOnlySpan<char> propertyName, ref ReadOnlySpan<byte> value)
        {
            int valueIdx = JsonWriterHelper.NeedsEscaping(value);
            int propertyIdx = JsonWriterHelper.NeedsEscaping(propertyName);

            Debug.Assert(valueIdx >= -1 && valueIdx < int.MaxValue / 2);
            Debug.Assert(propertyIdx >= -1 && propertyIdx < int.MaxValue / 2);

            // Equivalent to: valueIdx != -1 || propertyIdx != -1
            if (valueIdx + propertyIdx != -2)
            {
                WriteStringEscapePropertyOrValue(ref propertyName, ref value, propertyIdx, valueIdx);
            }
            else
            {
                WriteStringByOptions(ref propertyName, ref value);
            }
        }

        private void WriteStringSuppressFalse(ref ReadOnlySpan<byte> propertyName, ref ReadOnlySpan<char> value)
        {
            int valueIdx = JsonWriterHelper.NeedsEscaping(value);
            int propertyIdx = JsonWriterHelper.NeedsEscaping(propertyName);

            Debug.Assert(valueIdx >= -1 && valueIdx < int.MaxValue / 2);
            Debug.Assert(propertyIdx >= -1 && propertyIdx < int.MaxValue / 2);

            // Equivalent to: valueIdx != -1 || propertyIdx != -1
            if (valueIdx + propertyIdx != -2)
            {
                WriteStringEscapePropertyOrValue(ref propertyName, ref value, propertyIdx, valueIdx);
            }
            else
            {
                WriteStringByOptions(ref propertyName, ref value);
            }
        }

        private void WriteStringEscapePropertyOrValue(ref ReadOnlySpan<char> propertyName, ref ReadOnlySpan<char> value, int firstEscapeIndexProp, int firstEscapeIndexVal)
        {
            Debug.Assert(int.MaxValue / MaxExpansionFactorWhileEscaping >= value.Length);
            Debug.Assert(int.MaxValue / MaxExpansionFactorWhileEscaping >= propertyName.Length);

            char[] valueArray = null;
            char[] propertyArray = null;

            if (firstEscapeIndexVal != -1)
            {
                int length = firstEscapeIndexVal + MaxExpansionFactorWhileEscaping * (value.Length - firstEscapeIndexVal);

                Span<char> span;
                if (length > StackallocThreshold)
                {
                    valueArray = ArrayPool<char>.Shared.Rent(length);
                    span = valueArray;
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
                JsonWriterHelper.EscapeString(ref value, ref span, firstEscapeIndexVal, out int written);
                value = span.Slice(0, written);
            }

            if (firstEscapeIndexProp != -1)
            {
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
            }

            WriteStringByOptions(ref propertyName, ref value);

            if (valueArray != null)
                ArrayPool<char>.Shared.Return(valueArray);

            if (propertyArray != null)
                ArrayPool<char>.Shared.Return(propertyArray);
        }

        private void WriteStringEscapePropertyOrValue(ref ReadOnlySpan<byte> propertyName, ref ReadOnlySpan<byte> value, int firstEscapeIndexProp, int firstEscapeIndexVal)
        {
            Debug.Assert(int.MaxValue / MaxExpansionFactorWhileEscaping >= value.Length);
            Debug.Assert(int.MaxValue / MaxExpansionFactorWhileEscaping >= propertyName.Length);

            byte[] valueArray = null;
            byte[] propertyArray = null;

            if (firstEscapeIndexVal != -1)
            {
                int length = firstEscapeIndexVal + MaxExpansionFactorWhileEscaping * (value.Length - firstEscapeIndexVal);

                Span<byte> span;
                if (length > StackallocThreshold)
                {
                    valueArray = ArrayPool<byte>.Shared.Rent(length);
                    span = valueArray;
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
                JsonWriterHelper.EscapeString(ref value, ref span, firstEscapeIndexVal, out int written);
                value = span.Slice(0, written);
            }

            if (firstEscapeIndexProp != -1)
            {
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
            }

            WriteStringByOptions(ref propertyName, ref value);

            if (valueArray != null)
                ArrayPool<byte>.Shared.Return(valueArray);

            if (propertyArray != null)
                ArrayPool<byte>.Shared.Return(propertyArray);
        }

        private void WriteStringEscapePropertyOrValue(ref ReadOnlySpan<char> propertyName, ref ReadOnlySpan<byte> value, int firstEscapeIndexProp, int firstEscapeIndexVal)
        {
            Debug.Assert(int.MaxValue / MaxExpansionFactorWhileEscaping >= value.Length);
            Debug.Assert(int.MaxValue / MaxExpansionFactorWhileEscaping >= propertyName.Length);

            byte[] valueArray = null;
            char[] propertyArray = null;

            if (firstEscapeIndexVal != -1)
            {
                int length = firstEscapeIndexVal + MaxExpansionFactorWhileEscaping * (value.Length - firstEscapeIndexVal);

                Span<byte> span;
                if (length > StackallocThreshold)
                {
                    valueArray = ArrayPool<byte>.Shared.Rent(length);
                    span = valueArray;
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
                JsonWriterHelper.EscapeString(ref value, ref span, firstEscapeIndexVal, out int written);
                value = span.Slice(0, written);
            }

            if (firstEscapeIndexProp != -1)
            {
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
            }

            WriteStringByOptions(ref propertyName, ref value);

            if (valueArray != null)
                ArrayPool<byte>.Shared.Return(valueArray);

            if (propertyArray != null)
                ArrayPool<char>.Shared.Return(propertyArray);
        }

        private void WriteStringEscapePropertyOrValue(ref ReadOnlySpan<byte> propertyName, ref ReadOnlySpan<char> value, int firstEscapeIndexProp, int firstEscapeIndexVal)
        {
            Debug.Assert(int.MaxValue / MaxExpansionFactorWhileEscaping >= value.Length);
            Debug.Assert(int.MaxValue / MaxExpansionFactorWhileEscaping >= propertyName.Length);

            char[] valueArray = null;
            byte[] propertyArray = null;

            if (firstEscapeIndexVal != -1)
            {
                int length = firstEscapeIndexVal + MaxExpansionFactorWhileEscaping * (value.Length - firstEscapeIndexVal);

                Span<char> span;
                if (length > StackallocThreshold)
                {
                    valueArray = ArrayPool<char>.Shared.Rent(length);
                    span = valueArray;
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
                JsonWriterHelper.EscapeString(ref value, ref span, firstEscapeIndexVal, out int written);
                value = span.Slice(0, written);
            }

            if (firstEscapeIndexProp != -1)
            {
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
            }

            WriteStringByOptions(ref propertyName, ref value);

            if (valueArray != null)
                ArrayPool<char>.Shared.Return(valueArray);

            if (propertyArray != null)
                ArrayPool<byte>.Shared.Return(propertyArray);
        }

        private void WriteStringByOptions(ref ReadOnlySpan<char> propertyName, ref ReadOnlySpan<char> value)
        {
            if (_writerOptions.Indented)
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingProperty();
                }
                WriteStringIndented(ref propertyName, ref value);
            }
            else
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingProperty();
                }
                WriteStringMinimized(ref propertyName, ref value);
            }
        }

        private void WriteStringByOptions(ref ReadOnlySpan<byte> propertyName, ref ReadOnlySpan<byte> value)
        {
            if (_writerOptions.Indented)
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingProperty();
                }
                WriteStringIndented(ref propertyName, ref value);
            }
            else
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingProperty();
                }
                WriteStringMinimized(ref propertyName, ref value);
            }
        }

        private void WriteStringByOptions(ref ReadOnlySpan<char> propertyName, ref ReadOnlySpan<byte> value)
        {
            if (_writerOptions.Indented)
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingProperty();
                }
                WriteStringIndented(ref propertyName, ref value);
            }
            else
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingProperty();
                }
                WriteStringMinimized(ref propertyName, ref value);
            }
        }

        private void WriteStringByOptions(ref ReadOnlySpan<byte> propertyName, ref ReadOnlySpan<char> value)
        {
            if (_writerOptions.Indented)
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingProperty();
                }
                WriteStringIndented(ref propertyName, ref value);
            }
            else
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingProperty();
                }
                WriteStringMinimized(ref propertyName, ref value);
            }
        }

        private void WriteStringMinimized(ref ReadOnlySpan<char> escapedPropertyName, ref ReadOnlySpan<char> escapedValue)
        {
            int idx = WritePropertyNameMinimized(ref escapedPropertyName);

            WriteStringValue(ref escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteStringMinimized(ref ReadOnlySpan<byte> escapedPropertyName, ref ReadOnlySpan<byte> escapedValue)
        {
            int idx = WritePropertyNameMinimized(ref escapedPropertyName);

            WriteStringValue(ref escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteStringMinimized(ref ReadOnlySpan<char> escapedPropertyName, ref ReadOnlySpan<byte> escapedValue)
        {
            int idx = WritePropertyNameMinimized(ref escapedPropertyName);

            WriteStringValue(ref escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteStringMinimized(ref ReadOnlySpan<byte> escapedPropertyName, ref ReadOnlySpan<char> escapedValue)
        {
            int idx = WritePropertyNameMinimized(ref escapedPropertyName);

            WriteStringValue(ref escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteStringIndented(ref ReadOnlySpan<char> escapedPropertyName, ref ReadOnlySpan<char> escapedValue)
        {
            int idx = WritePropertyNameIndented(ref escapedPropertyName);

            WriteStringValue(ref escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteStringIndented(ref ReadOnlySpan<byte> escapedPropertyName, ref ReadOnlySpan<byte> escapedValue)
        {
            int idx = WritePropertyNameIndented(ref escapedPropertyName);

            WriteStringValue(ref escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteStringIndented(ref ReadOnlySpan<char> escapedPropertyName, ref ReadOnlySpan<byte> escapedValue)
        {
            int idx = WritePropertyNameIndented(ref escapedPropertyName);

            WriteStringValue(ref escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteStringIndented(ref ReadOnlySpan<byte> escapedPropertyName, ref ReadOnlySpan<char> escapedValue)
        {
            int idx = WritePropertyNameIndented(ref escapedPropertyName);

            WriteStringValue(ref escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteStringValue(ref ReadOnlySpan<char> escapedValue, ref int idx)
        {
            while (_buffer.Length <= idx)
            {
                AdvanceAndGrow(idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.Quote;

            ReadOnlySpan<byte> byteSpan = MemoryMarshal.AsBytes(escapedValue);
            int partialConsumed = 0;
            while (true)
            {
                OperationStatus status = JsonWriterHelper.ToUtf8(byteSpan.Slice(partialConsumed), _buffer.Slice(idx), out int consumed, out int written);
                idx += written;
                if (status == OperationStatus.Done)
                {
                    break;
                }
                partialConsumed += consumed;
                AdvanceAndGrow(idx);
                idx = 0;
            }

            while (_buffer.Length <= idx)
            {
                AdvanceAndGrow(idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.Quote;
        }

        private void WriteStringValue(ref ReadOnlySpan<byte> escapedValue, ref int idx)
        {
            while (_buffer.Length <= idx)
            {
                AdvanceAndGrow(idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.Quote;

            CopyLoop(ref escapedValue, ref idx);

            while (_buffer.Length <= idx)
            {
                AdvanceAndGrow(idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.Quote;
        }
    }
}

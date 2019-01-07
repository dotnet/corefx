// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;

namespace System.Text.Json
{
    public ref partial struct Utf8JsonWriter
    {
        public void WriteStringValue(string utf16Text, bool suppressEscaping = false)
           => WriteStringValue(utf16Text.AsSpan(), suppressEscaping);

        public void WriteStringValue(ReadOnlySpan<char> utf16Text, bool suppressEscaping = false)
        {
            JsonWriterHelper.ValidateValue(ref utf16Text);

            if (!suppressEscaping)
                WriteStringSuppressFalse(ref utf16Text);
            else
                WriteStringByOptions(ref utf16Text);

            _currentDepth |= 1 << 31;
            _tokenType = JsonTokenType.String;
        }

        private void WriteStringSuppressFalse(ref ReadOnlySpan<char> value)
        {
            int valueIdx = JsonWriterHelper.NeedsEscaping(value);

            Debug.Assert(valueIdx >= -1 && valueIdx < int.MaxValue / 2);

            if (valueIdx != -1)
            {
                WriteStringEscapeValue(ref value, valueIdx);
            }
            else
            {
                WriteStringByOptions(ref value);
            }
        }

        private void WriteStringByOptions(ref ReadOnlySpan<char> value)
        {
            if (_writerOptions.Indented)
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingValue();
                }
                WriteStringIndented(ref value);
            }
            else
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingValue();
                }
                WriteStringMinimized(ref value);
            }
        }

        private void WriteStringMinimized(ref ReadOnlySpan<char> escapedValue)
        {
            int idx = 0;
            if (_currentDepth < 0)
            {
                while (_buffer.Length <= idx)
                {
                    GrowAndEnsure();
                }
                _buffer[idx++] = JsonConstants.ListSeperator;
            }

            WriteStringValue(ref escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteStringIndented(ref ReadOnlySpan<char> escapedValue)
        {
            int idx = 0;
            if (_currentDepth < 0)
            {
                while (_buffer.Length <= idx)
                {
                    GrowAndEnsure();
                }
                _buffer[idx++] = JsonConstants.ListSeperator;
            }

            if (_tokenType != JsonTokenType.None)
                WriteNewLine(ref idx);

            int indent = Indentation;
            while (true)
            {
                bool result = JsonWriterHelper.TryWriteIndentation(_buffer.Slice(idx), indent, out int bytesWritten);
                idx += bytesWritten;
                if (result)
                {
                    break;
                }
                indent -= bytesWritten;
                AdvanceAndGrow(idx);
                idx = 0;
            }

            WriteStringValue(ref escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteStringEscapeValue(ref ReadOnlySpan<char> value, int firstEscapeIndexVal)
        {
            Debug.Assert(int.MaxValue / MaxExpansionFactorWhileEscaping >= value.Length);

            char[] valueArray = null;

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

            WriteStringByOptions(ref value);

            if (valueArray != null)
                ArrayPool<char>.Shared.Return(valueArray);
        }

        public void WriteStringValue(ReadOnlySpan<byte> utf8Text, bool suppressEscaping = false)
        {
            JsonWriterHelper.ValidateValue(ref utf8Text);

            if (!suppressEscaping)
                WriteStringSuppressFalse(ref utf8Text);
            else
                WriteStringByOptions(ref utf8Text);

            _currentDepth |= 1 << 31;
            _tokenType = JsonTokenType.String;
        }

        private void WriteStringSuppressFalse(ref ReadOnlySpan<byte> value)
        {
            int valueIdx = JsonWriterHelper.NeedsEscaping(value);

            Debug.Assert(valueIdx >= -1 && valueIdx < int.MaxValue / 2);

            if (valueIdx != -1)
            {
                WriteStringEscapeValue(ref value, valueIdx);
            }
            else
            {
                WriteStringByOptions(ref value);
            }
        }

        private void WriteStringByOptions(ref ReadOnlySpan<byte> value)
        {
            if (_writerOptions.Indented)
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingValue();
                }
                WriteStringIndented(ref value);
            }
            else
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingValue();
                }
                WriteStringMinimized(ref value);
            }
        }

        private void WriteStringMinimized(ref ReadOnlySpan<byte> escapedValue)
        {
            int idx = 0;
            if (_currentDepth < 0)
            {
                while (_buffer.Length <= idx)
                {
                    GrowAndEnsure();
                }
                _buffer[idx++] = JsonConstants.ListSeperator;
            }

            WriteStringValue(ref escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteStringIndented(ref ReadOnlySpan<byte> escapedValue)
        {
            int idx = 0;
            if (_currentDepth < 0)
            {
                while (_buffer.Length <= idx)
                {
                    GrowAndEnsure();
                }
                _buffer[idx++] = JsonConstants.ListSeperator;
            }

            if (_tokenType != JsonTokenType.None)
                WriteNewLine(ref idx);

            int indent = Indentation;
            while (true)
            {
                bool result = JsonWriterHelper.TryWriteIndentation(_buffer.Slice(idx), indent, out int bytesWritten);
                idx += bytesWritten;
                if (result)
                {
                    break;
                }
                indent -= bytesWritten;
                AdvanceAndGrow(idx);
                idx = 0;
            }

            WriteStringValue(ref escapedValue, ref idx);

            Advance(idx);
        }

        private void WriteStringEscapeValue(ref ReadOnlySpan<byte> value, int firstEscapeIndexVal)
        {
            Debug.Assert(int.MaxValue / MaxExpansionFactorWhileEscaping >= value.Length);

            byte[] valueArray = null;

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

            WriteStringByOptions(ref value);

            if (valueArray != null)
                ArrayPool<byte>.Shared.Return(valueArray);
        }
    }
}

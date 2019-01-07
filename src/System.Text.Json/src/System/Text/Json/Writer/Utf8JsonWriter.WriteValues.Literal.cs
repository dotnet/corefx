// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json
{
    public ref partial struct Utf8JsonWriter
    {
        public void WriteNullValue()
        {
            WriteLiteralByOptions(JsonConstants.NullValue);

            _currentDepth |= 1 << 31;
            _tokenType = JsonTokenType.Null;
        }

        public void WriteBooleanValue(bool value)
        {
            if (value)
            {
                WriteLiteralByOptions(JsonConstants.TrueValue);
                _tokenType = JsonTokenType.True;
            }
            else
            {
                WriteLiteralByOptions(JsonConstants.FalseValue);
                _tokenType = JsonTokenType.False;
            }

            _currentDepth |= 1 << 31;
        }

        private void WriteLiteralByOptions(ReadOnlySpan<byte> value)
        {
            if (_writerOptions.Indented)
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingValue();
                }
                WriteLiteralIndented(ref value);
            }
            else
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingValue();
                }
                WriteLiteralMinimized(ref value);
            }
        }

        private void WriteLiteralMinimized(ref ReadOnlySpan<byte> value)
        {
            // Calculated based on the following: ',null' OR ',true' OR ',false'
            int bytesNeeded = value.Length + 1;
            if (_buffer.Length < bytesNeeded)
            {
                GrowAndEnsure(bytesNeeded);
            }

            int idx = 0;
            if (_currentDepth < 0)
            {
                _buffer[idx++] = JsonConstants.ListSeperator;
            }

            value.CopyTo(_buffer.Slice(idx));

            Advance(idx + value.Length);
        }

        private void WriteLiteralIndented(ref ReadOnlySpan<byte> value)
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

            CopyLoop(ref value, ref idx);

            Advance(idx);
        }
    }
}

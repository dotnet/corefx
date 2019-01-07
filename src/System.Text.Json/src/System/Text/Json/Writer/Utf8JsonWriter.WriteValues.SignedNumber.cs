// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Text.Json
{
    public ref partial struct Utf8JsonWriter
    {
        public void WriteNumberValue(int value)
            => WriteNumberValue((long)value);

        public void WriteNumberValue(long value)
        {
            if (_writerOptions.Indented)
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingValue();
                }
                WriteNumberValueIndented(value);
            }
            else
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingValue();
                }
                WriteNumberValueMinimized(value);
            }

            _currentDepth |= 1 << 31;
            _tokenType = JsonTokenType.Number;
        }

        private void WriteNumberValueMinimized(long value)
        {
            // Calculated based on the following: ',long.MaxValue'
            int bytesNeeded = JsonConstants.MaximumInt64Length + 1;
            if (_buffer.Length < bytesNeeded)
            {
                GrowAndEnsure(bytesNeeded);
            }

            int idx = 0;
            if (_currentDepth < 0)
            {
                _buffer[idx++] = JsonConstants.ListSeperator;
            }

            JsonWriterHelper.TryFormatInt64Default(value, _buffer.Slice(idx), out int bytesWritten);

            Advance(idx + bytesWritten);
        }

        private void WriteNumberValueIndented(long value)
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

            WriteNumberValueFormatLoop(value, ref idx);

            Advance(idx);
        }
    }
}

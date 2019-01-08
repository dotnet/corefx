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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ValidatePropertyNameAndDepth(ref ReadOnlySpan<char> propertyName)
        {
            if (propertyName.Length > JsonConstants.MaxCharacterTokenSize || CurrentDepth >= JsonConstants.MaxWriterDepth)
                ThrowHelper.ThrowJsonWriterOrArgumentException(propertyName, _currentDepth);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ValidatePropertyNameAndDepth(ref ReadOnlySpan<byte> propertyName)
        {
            if (propertyName.Length > JsonConstants.MaxTokenSize || CurrentDepth >= JsonConstants.MaxWriterDepth)
                ThrowHelper.ThrowJsonWriterOrArgumentException(propertyName, _currentDepth);
        }

        private void ValidateWritingProperty()
        {
            if (!_inObject)
            {
                Debug.Assert(_tokenType != JsonTokenType.StartObject);
                ThrowHelper.ThrowJsonWriterException(ExceptionResource.CannotWritePropertyWithinArray, tokenType: _tokenType);
            }
        }

        private int WritePropertyNameMinimized(ref ReadOnlySpan<byte> escapedPropertyName)
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

            while (_buffer.Length <= idx)
            {
                AdvanceAndGrow(idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.Quote;

            CopyLoop(ref escapedPropertyName, ref idx);

            while (_buffer.Length <= idx)
            {
                AdvanceAndGrow(idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.Quote;

            while (_buffer.Length <= idx)
            {
                AdvanceAndGrow(idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.KeyValueSeperator;

            return idx;
        }

        private int WritePropertyNameIndented(ref ReadOnlySpan<byte> escapedPropertyName)
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

            while (_buffer.Length <= idx)
            {
                AdvanceAndGrow(idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.Quote;

            CopyLoop(ref escapedPropertyName, ref idx);

            while (_buffer.Length <= idx)
            {
                AdvanceAndGrow(idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.Quote;

            while (_buffer.Length <= idx)
            {
                AdvanceAndGrow(idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.KeyValueSeperator;

            while (_buffer.Length <= idx)
            {
                AdvanceAndGrow(idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.Space;

            return idx;
        }

        private int WritePropertyNameMinimized(ref ReadOnlySpan<char> escapedPropertyName)
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

            while (_buffer.Length <= idx)
            {
                AdvanceAndGrow(idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.Quote;

            ReadOnlySpan<byte> byteSpan = MemoryMarshal.AsBytes(escapedPropertyName);
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

            while (_buffer.Length <= idx)
            {
                AdvanceAndGrow(idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.KeyValueSeperator;

            return idx;
        }

        private int WritePropertyNameIndented(ref ReadOnlySpan<char> escapedPropertyName)
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

            while (_buffer.Length <= idx)
            {
                AdvanceAndGrow(idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.Quote;

            ReadOnlySpan<byte> byteSpan = MemoryMarshal.AsBytes(escapedPropertyName);
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

            while (_buffer.Length <= idx)
            {
                AdvanceAndGrow(idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.KeyValueSeperator;

            while (_buffer.Length <= idx)
            {
                AdvanceAndGrow(idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.Space;

            return idx;
        }
    }
}

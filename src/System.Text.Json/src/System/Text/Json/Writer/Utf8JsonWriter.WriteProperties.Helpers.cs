// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Text.Json
{
    public sealed partial class Utf8JsonWriter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ValidatePropertyNameAndDepth(ReadOnlySpan<char> propertyName)
        {
            if (propertyName.Length > JsonConstants.MaxCharacterTokenSize || CurrentDepth >= JsonConstants.MaxWriterDepth)
                ThrowHelper.ThrowInvalidOperationOrArgumentException(propertyName, _currentDepth);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ValidatePropertyNameAndDepth(ReadOnlySpan<byte> utf8PropertyName)
        {
            if (utf8PropertyName.Length > JsonConstants.MaxUnescapedTokenSize || CurrentDepth >= JsonConstants.MaxWriterDepth)
                ThrowHelper.ThrowInvalidOperationOrArgumentException(utf8PropertyName, _currentDepth);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ValidateDepth()
        {
            if (CurrentDepth >= JsonConstants.MaxWriterDepth)
                ThrowHelper.ThrowInvalidOperationException(_currentDepth);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ValidateWritingProperty()
        {
            if (!_options.SkipValidation)
            {
                if (!_inObject || _tokenType == JsonTokenType.PropertyName)
                {
                    Debug.Assert(_tokenType != JsonTokenType.StartObject);
                    ThrowHelper.ThrowInvalidOperationException(ExceptionResource.CannotWritePropertyWithinArray, currentDepth: default, token: default, _tokenType);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ValidateWritingProperty(byte token)
        {
            if (!_options.SkipValidation)
            {
                if (!_inObject || _tokenType == JsonTokenType.PropertyName)
                {
                    Debug.Assert(_tokenType != JsonTokenType.StartObject);
                    ThrowHelper.ThrowInvalidOperationException(ExceptionResource.CannotWritePropertyWithinArray, currentDepth: default, token: default, _tokenType);
                }
                UpdateBitStackOnStart(token);
            }
        }

        private void WritePropertyNameMinimized(ReadOnlySpan<byte> escapedPropertyName, byte token)
        {
            Debug.Assert(escapedPropertyName.Length < int.MaxValue - 5);

            int minRequired = escapedPropertyName.Length + 4; // 2 quotes, 1 colon, and 1 start token
            int maxRequired = minRequired + 1; // Optionally, 1 list separator

            if (_memory.Length - _currentIndex < maxRequired)
            {
                Grow(maxRequired);
            }

            Span<byte> output = _memory.Span;

            if (_currentDepth < 0)
            {
                output[_currentIndex++] = JsonConstants.ListSeparator;
            }
            output[_currentIndex++] = JsonConstants.Quote;

            escapedPropertyName.CopyTo(output.Slice(_currentIndex));
            _currentIndex += escapedPropertyName.Length;

            output[_currentIndex++] = JsonConstants.Quote;
            output[_currentIndex++] = JsonConstants.KeyValueSeperator;
            output[_currentIndex++] = token;
        }

        private void WritePropertyNameIndented(ReadOnlySpan<byte> escapedPropertyName, byte token)
        {
            int indent = Indentation;
            Debug.Assert(indent <= 2 * JsonConstants.MaxWriterDepth);

            Debug.Assert(escapedPropertyName.Length < int.MaxValue - indent - 6 - s_newLineLength);

            int minRequired = indent + escapedPropertyName.Length + 5; // 2 quotes, 1 colon, 1 space, and 1 start token
            int maxRequired = minRequired + 1 + s_newLineLength; // Optionally, 1 list separator and 1-2 bytes for new line

            if (_memory.Length - _currentIndex < maxRequired)
            {
                Grow(maxRequired);
            }

            Span<byte> output = _memory.Span;

            if (_currentDepth < 0)
            {
                output[_currentIndex++] = JsonConstants.ListSeparator;
            }

            Debug.Assert(_options.SkipValidation || _tokenType != JsonTokenType.PropertyName);

            if (_tokenType != JsonTokenType.None)
            {
                WriteNewLine(output);
            }

            JsonWriterHelper.WriteIndentation(output.Slice(_currentIndex), indent);
            _currentIndex += indent;

            output[_currentIndex++] = JsonConstants.Quote;

            escapedPropertyName.CopyTo(output.Slice(_currentIndex));
            _currentIndex += escapedPropertyName.Length;

            output[_currentIndex++] = JsonConstants.Quote;

            output[_currentIndex++] = JsonConstants.KeyValueSeperator;
            output[_currentIndex++] = JsonConstants.Space;
            output[_currentIndex++] = token;
        }

        private void WritePropertyNameMinimized(ReadOnlySpan<char> escapedPropertyName, byte token)
        {
            Debug.Assert(escapedPropertyName.Length < (int.MaxValue / JsonConstants.MaxExpansionFactorWhileTranscoding) - 5);

            // All ASCII, 2 quotes, 1 colon, and 1 start token => escapedPropertyName.Length + 4
            // Optionally, 1 list separator, and up to 3x growth when transcoding
            int maxRequired = (escapedPropertyName.Length * JsonConstants.MaxExpansionFactorWhileTranscoding) + 5;

            if (_memory.Length - _currentIndex < maxRequired)
            {
                Grow(maxRequired);
            }

            Span<byte> output = _memory.Span;

            if (_currentDepth < 0)
            {
                output[_currentIndex++] = JsonConstants.ListSeparator;
            }
            output[_currentIndex++] = JsonConstants.Quote;

            TranscodeAndWrite(escapedPropertyName, output);

            output[_currentIndex++] = JsonConstants.Quote;
            output[_currentIndex++] = JsonConstants.KeyValueSeperator;
            output[_currentIndex++] = token;
        }

        private void WritePropertyNameIndented(ReadOnlySpan<char> escapedPropertyName, byte token)
        {
            int indent = Indentation;
            Debug.Assert(indent <= 2 * JsonConstants.MaxWriterDepth);

            Debug.Assert(escapedPropertyName.Length < (int.MaxValue / JsonConstants.MaxExpansionFactorWhileTranscoding) - indent - 6 - s_newLineLength);

            // All ASCII, 2 quotes, 1 colon, 1 space, and 1 start token => indent + escapedPropertyName.Length + 5 
            // Optionally, 1 list separator, 1-2 bytes for new line, and up to 3x growth when transcoding
            int maxRequired = indent + (escapedPropertyName.Length * JsonConstants.MaxExpansionFactorWhileTranscoding) + 6 + s_newLineLength;

            if (_memory.Length - _currentIndex < maxRequired)
            {
                Grow(maxRequired);
            }

            Span<byte> output = _memory.Span;

            if (_currentDepth < 0)
            {
                output[_currentIndex++] = JsonConstants.ListSeparator;
            }

            Debug.Assert(_options.SkipValidation || _tokenType != JsonTokenType.PropertyName);

            if (_tokenType != JsonTokenType.None)
            {
                WriteNewLine(output);
            }

            JsonWriterHelper.WriteIndentation(output.Slice(_currentIndex), indent);
            _currentIndex += indent;

            output[_currentIndex++] = JsonConstants.Quote;

            TranscodeAndWrite(escapedPropertyName, output);

            output[_currentIndex++] = JsonConstants.Quote;

            output[_currentIndex++] = JsonConstants.KeyValueSeperator;
            output[_currentIndex++] = JsonConstants.Space;
            output[_currentIndex++] = token;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TranscodeAndWrite(ReadOnlySpan<char> escapedPropertyName, Span<byte> output)
        {
            ReadOnlySpan<byte> byteSpan = MemoryMarshal.AsBytes(escapedPropertyName);
            OperationStatus status = JsonWriterHelper.ToUtf8(byteSpan, output.Slice(_currentIndex), out int consumed, out int written);
            Debug.Assert(status == OperationStatus.Done);
            Debug.Assert(consumed == byteSpan.Length);
            _currentIndex += written;
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Text.Json
{
    public ref partial struct Utf8JsonWriter
    {
        private const int MinimumSizeThreshold = 256;
        private const int StackallocThreshold = 256;
        private const int MaxExpansionFactorWhileEscaping = 6;
        private const int SpacesPerIndent = 2;
        private const int DefaultGrowthSize = 4096;

        private readonly IBufferWriter<byte> _output;
        private int _buffered;
        private Span<byte> _buffer;

        public long BytesWritten
        {
            get
            {
                Debug.Assert(BytesCommitted <= long.MaxValue - _buffered);
                return BytesCommitted + _buffered;
            }
        }

        public long BytesCommitted { get; private set; }

        private bool _inObject;
        private bool _isNotPrimitive;
        private JsonTokenType _tokenType;
        private readonly JsonWriterOptions _writerOptions;
        private BitStack _bitStack;

        // The highest order bit of _currentDepth is used to discern whether we are writing the first item in a list or not.
        // if (_currentDepth >> 31) == 1, add a list separator before writing the item
        // else, no list separator is needed since we are writing the first item.
        private int _currentDepth;

        private int Indentation => CurrentDepth * SpacesPerIndent;

        public int CurrentDepth => _currentDepth & JsonConstants.RemoveFlagsBitMask;

        public JsonWriterState CurrentState => new JsonWriterState
        {
            _bytesWritten = BytesWritten,
            _bytesCommitted = BytesCommitted,
            _inObject = _inObject,
            _isNotPrimitive = _isNotPrimitive,
            _tokenType = _tokenType,
            _writerOptions = _writerOptions,
            _bitStack = _bitStack,
        };

        /// <summary>
        /// Constructs a JSON writer with a specified <paramref name="bufferWriter"/>.
        /// </summary>
        /// <param name="bufferWriter">An instance of <see cref="IBufferWriter{T}" /> used for writing bytes to an output channel.</param>
        /// <param name="state">Specifies whether to add whitespace to the output text for user readability.</param>
        public Utf8JsonWriter(IBufferWriter<byte> bufferWriter, JsonWriterState state = default)
        {
            _output = bufferWriter ?? throw new ArgumentNullException(nameof(bufferWriter));
            _buffered = 0;
            BytesCommitted = 0;
            _buffer = _output.GetSpan();

            _inObject = state._inObject;
            _isNotPrimitive = state._isNotPrimitive;
            _tokenType = state._tokenType;
            _writerOptions = state._writerOptions;
            _bitStack = state._bitStack;

            _currentDepth = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Advance(int count)
        {
            Debug.Assert(count >= 0 && _buffered <= int.MaxValue - count);

            _buffered += count;
            _buffer = _buffer.Slice(count);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void GrowSpan(int count)
        {
            Flush();

            _buffer = _output.GetSpan(count);

            if (_buffer.Length < count && count < MinimumSizeThreshold)
            {
                throw new ArgumentException("The IBufferWriter could not provide a span that is large enough to continue.");
            }

            Debug.Assert(_buffer.Length >= Math.Min(count, MinimumSizeThreshold));
        }

        public void Flush(bool isFinalBlock = true)
        {
            //TODO: Fix exception message and check other potential conditions for invalid end.
            if (isFinalBlock && !_writerOptions.SkipValidation && CurrentDepth != 0)
                ThrowHelper.ThrowJsonWriterException("Invalid end of JSON.");

            Flush();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Flush()
        {
            BytesCommitted += _buffered;
            _output.Advance(_buffered);
            _buffered = 0;
        }

        public void WriteStartArray()
        {
            WriteStart(JsonConstants.OpenBracket);
            _tokenType = JsonTokenType.StartArray;
        }

        public void WriteStartObject()
        {
            WriteStart(JsonConstants.OpenBrace);
            _tokenType = JsonTokenType.StartObject;
        }

        private void WriteStart(byte token)
        {
            // TODO: Use throw helper with proper error messages
            if (CurrentDepth >= JsonConstants.MaxWriterDepth)
                ThrowHelper.ThrowJsonWriterException("Depth too large.");

            if (_writerOptions.SlowPath)
                WriteStartSlow(token);
            else
                WriteStartMinimized(token);

            _currentDepth &= JsonConstants.RemoveFlagsBitMask;
            _currentDepth++;
            _isNotPrimitive = true;
        }

        private void WriteStartMinimized(byte token)
        {
            // Calculated based on the following: ',[' OR ',{'
            int bytesNeeded = 2;
            while (_buffer.Length < bytesNeeded)
            {
                GrowAndEnsure();
            }

            if (_currentDepth < 0)
            {
                _buffer[0] = JsonConstants.ListSeperator;
                _buffer[1] = token;
            }
            else
            {
                bytesNeeded--;
                _buffer[0] = token;
            }
            Advance(bytesNeeded);
        }

        private void WriteStartSlow(byte token)
        {
            Debug.Assert(_writerOptions.Indented || !_writerOptions.SkipValidation);

            if (_writerOptions.Indented)
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateStart(token);
                    UpdateBitStackOnStart(token);
                }
                WriteStartIndented(token);
            }
            else
            {
                Debug.Assert(!_writerOptions.SkipValidation);
                ValidateStart(token);
                UpdateBitStackOnStart(token);
                WriteStartMinimized(token);
            }
        }

        private void ValidateStart(byte token)
        {
            if (_inObject)
            {
                Debug.Assert(_tokenType != JsonTokenType.None && _tokenType != JsonTokenType.StartArray);
                ThrowHelper.ThrowJsonWriterException(token, _tokenType);
            }
            else
            {
                Debug.Assert(_tokenType != JsonTokenType.StartObject);
                if (_tokenType != JsonTokenType.None && !_isNotPrimitive)
                {
                    ThrowHelper.ThrowJsonWriterException(token, _tokenType);
                }
            }
        }

        private void WriteStartIndented(byte token)
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
            _buffer[idx++] = token;

            Advance(idx);
        }

        public void WriteStartArray(ReadOnlySpan<byte> propertyName, bool suppressEscaping = false)
        {
            ValidatePropertyNameAndDepth(ref propertyName);

            if (!suppressEscaping)
                WriteStartSuppressFalse(ref propertyName, JsonConstants.OpenBracket);
            else
                WriteStartByOptions(ref propertyName, JsonConstants.OpenBracket);

            _currentDepth &= JsonConstants.RemoveFlagsBitMask;
            _currentDepth++;
            _isNotPrimitive = true;
            _tokenType = JsonTokenType.StartArray;
        }

        public void WriteStartObject(ReadOnlySpan<byte> propertyName, bool suppressEscaping = false)
        {
            ValidatePropertyNameAndDepth(ref propertyName);

            if (!suppressEscaping)
                WriteStartSuppressFalse(ref propertyName, JsonConstants.OpenBrace);
            else
                WriteStartByOptions(ref propertyName, JsonConstants.OpenBrace);

            _currentDepth &= JsonConstants.RemoveFlagsBitMask;
            _currentDepth++;
            _isNotPrimitive = true;
            _tokenType = JsonTokenType.StartObject;
        }

        private void WriteStartSuppressFalse(ref ReadOnlySpan<byte> propertyName, byte token)
        {
            int propertyIdx = JsonWriterHelper.NeedsEscaping(propertyName);

            Debug.Assert(propertyIdx >= -1 && propertyIdx < int.MaxValue / 2);

            if (propertyIdx != -1)
            {
                WriteStartEscapeProperty(ref propertyName, token, propertyIdx);
            }
            else
            {
                WriteStartByOptions(ref propertyName, token);
            }
        }

        private void WriteStartByOptions(ref ReadOnlySpan<byte> propertyName, byte token)
        {
            int idx;
            if (_writerOptions.Indented)
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingProperty();
                    UpdateBitStackOnStart(token);
                }
                idx = WritePropertyNameIndented(ref propertyName);
            }
            else
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingProperty();
                    UpdateBitStackOnStart(token);
                }
                idx = WritePropertyNameMinimized(ref propertyName);
            }

            if (1 > _buffer.Length - idx)
            {
                AdvanceAndGrow(idx, 1);
                idx = 0;
            }

            _buffer[idx++] = token;

            Advance(idx);
        }

        private void WriteStartEscapeProperty(ref ReadOnlySpan<byte> propertyName, byte token, int firstEscapeIndexProp)
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

            WriteStartByOptions(ref propertyName, token);

            if (propertyArray != null)
                ArrayPool<byte>.Shared.Return(propertyArray);
        }

        public void WriteStartArray(string propertyName, bool suppressEscaping = false)
            => WriteStartArray(propertyName.AsSpan(), suppressEscaping);

        public void WriteStartObject(string propertyName, bool suppressEscaping = false)
            => WriteStartObject(propertyName.AsSpan(), suppressEscaping);

        public void WriteStartArray(ReadOnlySpan<char> propertyName, bool suppressEscaping = false)
        {
            ValidatePropertyNameAndDepth(ref propertyName);

            if (!suppressEscaping)
                WriteStartSuppressFalse(ref propertyName, JsonConstants.OpenBracket);
            else
                WriteStartByOptions(ref propertyName, JsonConstants.OpenBracket);

            _currentDepth &= JsonConstants.RemoveFlagsBitMask;
            _currentDepth++;
            _isNotPrimitive = true;
            _tokenType = JsonTokenType.StartArray;
        }

        public void WriteStartObject(ReadOnlySpan<char> propertyName, bool suppressEscaping = false)
        {
            ValidatePropertyNameAndDepth(ref propertyName);

            if (!suppressEscaping)
                WriteStartSuppressFalse(ref propertyName, JsonConstants.OpenBrace);
            else
                WriteStartByOptions(ref propertyName, JsonConstants.OpenBrace);

            _currentDepth &= JsonConstants.RemoveFlagsBitMask;
            _currentDepth++;
            _isNotPrimitive = true;
            _tokenType = JsonTokenType.StartObject;
        }

        private void WriteStartSuppressFalse(ref ReadOnlySpan<char> propertyName, byte token)
        {
            int propertyIdx = JsonWriterHelper.NeedsEscaping(propertyName);

            Debug.Assert(propertyIdx >= -1 && propertyIdx < int.MaxValue / 2);

            if (propertyIdx != -1)
            {
                WriteStartEscapeProperty(ref propertyName, token, propertyIdx);
            }
            else
            {
                WriteStartByOptions(ref propertyName, token);
            }
        }

        private void WriteStartByOptions(ref ReadOnlySpan<char> propertyName, byte token)
        {
            int idx;
            if (_writerOptions.Indented)
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingProperty();
                    UpdateBitStackOnStart(token);
                }
                idx = WritePropertyNameIndented(ref propertyName);
            }
            else
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateWritingProperty();
                    UpdateBitStackOnStart(token);
                }
                idx = WritePropertyNameMinimized(ref propertyName);
            }

            if (1 > _buffer.Length - idx)
            {
                AdvanceAndGrow(idx, 1);
                idx = 0;
            }

            _buffer[idx++] = token;

            Advance(idx);
        }

        private void WriteStartEscapeProperty(ref ReadOnlySpan<char> propertyName, byte token, int firstEscapeIndexProp)
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

            WriteStartByOptions(ref propertyName, token);

            if (propertyArray != null)
                ArrayPool<char>.Shared.Return(propertyArray);
        }

        public void WriteEndArray()
        {
            WriteEnd(JsonConstants.CloseBracket);
            _tokenType = JsonTokenType.EndArray;
        }

        public void WriteEndObject()
        {
            WriteEnd(JsonConstants.CloseBrace);
            _tokenType = JsonTokenType.EndObject;
        }

        private void WriteEnd(byte token)
        {
            _currentDepth |= 1 << 31;
            _currentDepth--;

            // Necessary if WriteEndX is called without a corresponding WriteStartX first.
            // Checking for int.MaxValue because int.MinValue - 1 = int.MaxValue
            if (_currentDepth == int.MaxValue)
            {
                _currentDepth = 0;
            }

            if (_writerOptions.SlowPath)
                WriteEndSlow(token);
            else
                WriteEndMinimized(token);
        }

        private void WriteEndMinimized(byte token)
        {
            while (_buffer.Length < 1)
            {
                GrowAndEnsure();
            }

            _buffer[0] = token;
            Advance(1);
        }

        private void WriteEndSlow(byte token)
        {
            Debug.Assert(_writerOptions.Indented || !_writerOptions.SkipValidation);

            if (_writerOptions.Indented)
            {
                if (!_writerOptions.SkipValidation)
                {
                    ValidateEnd(token);
                }
                WriteEndIndented(token);
            }
            else
            {
                Debug.Assert(!_writerOptions.SkipValidation);
                ValidateEnd(token);
                WriteEndMinimized(token);
            }
        }

        private void ValidateEnd(byte token)
        {
            if (_bitStack.CurrentDepth <= 0)
                ThrowHelper.ThrowJsonWriterException(token);    //TODO: Add resource message

            if (token == JsonConstants.CloseBracket)
            {
                if (_inObject)
                {
                    Debug.Assert(_tokenType != JsonTokenType.None);
                    ThrowHelper.ThrowJsonWriterException(token);    //TODO: Add resource message
                }
            }
            else
            {
                Debug.Assert(token == JsonConstants.CloseBrace);

                if (!_inObject)
                {
                    ThrowHelper.ThrowJsonWriterException(token);    //TODO: Add resource message
                }
            }

            _inObject = _bitStack.Pop();
        }

        private void WriteEndIndented(byte token)
        {
            // Do not format/indent empty JSON object/array.
            if ((_tokenType == JsonTokenType.StartObject && token == JsonConstants.CloseBrace)
                || (_tokenType == JsonTokenType.StartArray && token == JsonConstants.CloseBracket))
            {
                WriteEndMinimized(token);
            }
            else
            {
                int idx = 0;
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
                _buffer[idx++] = token;

                Advance(idx);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteNewLine(ref int idx)
        {
            // Write '\r\n' OR '\n', depending on OS
            if (JsonWriterHelper.s_newLineUtf8.Length == 2)
            {
                while (_buffer.Length <= idx)
                {
                    AdvanceAndGrow(idx);
                    idx = 0;
                }
                _buffer[idx++] = JsonConstants.CarriageReturn;
            }

            while (_buffer.Length <= idx)
            {
                AdvanceAndGrow(idx);
                idx = 0;
            }
            _buffer[idx++] = JsonConstants.LineFeed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateBitStackOnStart(byte token)
        {
            if (token == JsonConstants.OpenBracket)
            {
                _bitStack.PushFalse();
                _inObject = false;
            }
            else
            {
                Debug.Assert(token == JsonConstants.OpenBrace);
                _bitStack.PushTrue();
                _inObject = true;
            }
        }

        private void GrowAndEnsure()
        {
            int previousSpanLength = _buffer.Length;
            GrowSpan(DefaultGrowthSize);
            if (_buffer.Length <= previousSpanLength)
            {
                GrowSpan(DefaultGrowthSize);
                if (_buffer.Length <= previousSpanLength)
                {
                    //TODO: Use Throwhelper and fix message.
                    throw new OutOfMemoryException("Failed to get a larger buffer when growing.");
                }
            }
        }

        private void GrowAndEnsure(int minimumSize)
        {
            GrowSpan(DefaultGrowthSize);
            if (_buffer.Length <= minimumSize)
            {
                GrowSpan(DefaultGrowthSize);
                if (_buffer.Length <= minimumSize)
                {
                    //TODO: Use Throwhelper and fix message.
                    throw new OutOfMemoryException("Failed to get buffer larger than minimumSize when growing.");
                }
            }
        }

        private void AdvanceAndGrow(int alreadyWritten)
        {
            Debug.Assert(alreadyWritten >= 0);
            Advance(alreadyWritten);
            GrowAndEnsure();
        }

        private void AdvanceAndGrow(int alreadyWritten, int minimumSize)
        {
            Debug.Assert(minimumSize >= 1 && minimumSize <= 128);
            Advance(alreadyWritten);
            GrowAndEnsure(minimumSize);
        }

        private void CopyLoop(ref ReadOnlySpan<byte> span, ref int idx)
        {
            while (true)
            {
                if (span.Length <= _buffer.Length - idx)
                {
                    span.CopyTo(_buffer.Slice(idx));
                    idx += span.Length;
                    break;
                }

                span.Slice(0, _buffer.Length - idx).CopyTo(_buffer.Slice(idx));
                span = span.Slice(_buffer.Length - idx);
                AdvanceAndGrow(_buffer.Length);
                idx = 0;
            }
        }
    }
}

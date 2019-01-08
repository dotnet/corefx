﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Text.Json
{
    /// <summary>
    /// Provides a high-performance API for forward-only, non-cached writing of UTF-8 encoded JSON text.
    /// It writes the text sequentially with no caching and adheres to the JSON RFC
    /// by default (https://tools.ietf.org/html/rfc8259), with the exception of writing comments.
    /// When the user attempts to write invalid JSON and validation is enabled, it throws
    /// a JsonWriterException with a context specific error message.
    /// Since this type is a ref struct, it does not directly support async. However, it does provide
    /// support for reentrancy to write partial data, and continue writing in chunks.
    /// To be able to format the output with indentation and whitespace OR to skip validation, create an instance of 
    /// <see cref="JsonWriterState"/> and pass that in to the writer.
    /// </summary>
    public ref partial struct Utf8JsonWriter
    {
        private const int StackallocThreshold = 256;
        private const int MaxExpansionFactorWhileEscaping = 6;
        private const int SpacesPerIndent = 2;
        private const int DefaultGrowthSize = 4096;

        private readonly IBufferWriter<byte> _output;
        private int _buffered;
        private Span<byte> _buffer;

        /// <summary>
        /// Returns the total amount of bytes written by the <see cref="Utf8JsonWriter"/> so far
        /// for the current instance of the <see cref="Utf8JsonWriter"/>.
        /// This includes data that has been written beyond what has already been committed.
        /// </summary>
        public long BytesWritten
        {
            get
            {
                Debug.Assert(BytesCommitted <= long.MaxValue - _buffered);
                return BytesCommitted + _buffered;
            }
        }

        /// <summary>
        /// Returns the total amount of bytes committed to the output by the <see cref="Utf8JsonWriter"/> so far
        /// for the current instance of the <see cref="Utf8JsonWriter"/>.
        /// This is how much the IBufferWriter has advanced.
        /// </summary>
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

        /// <summary>
        /// Tracks the recursive depth of the nested objects / arrays within the JSON text
        /// written so far. This provides the depth of the current token.
        /// </summary>
        public int CurrentDepth => _currentDepth & JsonConstants.RemoveFlagsBitMask;

        /// <summary>
        /// Returns the current snapshot of the <see cref="Utf8JsonWriter"/> state which must
        /// be captured by the caller and passed back in to the <see cref="Utf8JsonWriter"/> ctor with more data.
        /// Unlike the <see cref="Utf8JsonWriter"/>, which is a ref struct, the state can survive
        /// across async/await boundaries and hence this type is required to provide support for reading
        /// in more data asynchronously before continuing with a new instance of the <see cref="Utf8JsonWriter"/>.
        /// </summary>
        public JsonWriterState CurrentState
        {
            get
            {
                // Getting the state for creating a new Utf8JsonWriter without first committing the data that has been written
                // would result in an inconsistent state. Therefore, calling Flush before getting the current state.
                if (_buffered != 0)
                {
                    Flush();
                }
                return new JsonWriterState
                {
                    _bytesWritten = BytesWritten,
                    _bytesCommitted = BytesCommitted,
                    _inObject = _inObject,
                    _isNotPrimitive = _isNotPrimitive,
                    _tokenType = _tokenType,
                    _currentDepth = _currentDepth,
                    _writerOptions = _writerOptions,
                    _bitStack = _bitStack,
                };
            }
        }

        /// <summary>
        /// Constructs a new <see cref="Utf8JsonWriter"/> instance with a specified <paramref name="bufferWriter"/>.
        /// </summary>
        /// <param name="bufferWriter">An instance of <see cref="IBufferWriter{T}" /> used as a destination for writing JSON text into.</param>
        /// <param name="state">If this is the first call to the ctor, pass in a default state. Otherwise,
        /// capture the state from the previous instance of the <see cref="Utf8JsonWriter"/> and pass that back.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the instance of <see cref="IBufferWriter{T}" /> that is passed in is null.
        /// </exception>
        /// <remarks>
        /// Since this type is a ref struct, it is a stack-only type and all the limitations of ref structs apply to it.
        /// This is the reason why the ctor accepts a <see cref="JsonWriterState"/>.
        /// </remarks>
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

            _currentDepth = state._currentDepth;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Advance(int count)
        {
            Debug.Assert(count >= 0 && _buffered <= int.MaxValue - count);

            _buffered += count;
            _buffer = _buffer.Slice(count);
        }

        /// <summary>
        /// Advances the underlying <see cref="IBufferWriter{T}" /> based on what has been written so far.
        /// </summary>
        /// <param name="isFinalBlock">Let's the writer know whether more data will be written. This is used to validate
        /// that the JSON written sor far is structurally valid if no more data is to follow.</param>
        /// <exception cref="JsonWriterException">
        /// Thrown when incomplete JSON has been written and <paramref name="isFinalBlock"/> is true.
        /// (for example when an open object or array needs to be closed).
        /// </exception>
        public void Flush(bool isFinalBlock = true)
        {
            if (isFinalBlock && !_writerOptions.SkipValidation && CurrentDepth != 0)
                ThrowHelper.ThrowJsonWriterException(ExceptionResource.ZeroDepthAtEnd, _currentDepth);

            Flush();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Flush()
        {
            BytesCommitted += _buffered;
            _output.Advance(_buffered);
            _buffered = 0;
        }

        /// <summary>
        /// Writes the beginning of a JSON array.
        /// </summary>
        /// <exception cref="JsonWriterException">
        /// Thrown when the depth of the JSON has exceeded the maximum depth of 1000 
        /// OR if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteStartArray()
        {
            WriteStart(JsonConstants.OpenBracket);
            _tokenType = JsonTokenType.StartArray;
        }

        /// <summary>
        /// Writes the beginning of a JSON object.
        /// </summary>
        /// <exception cref="JsonWriterException">
        /// Thrown when the depth of the JSON has exceeded the maximum depth of 1000 
        /// OR if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteStartObject()
        {
            WriteStart(JsonConstants.OpenBrace);
            _tokenType = JsonTokenType.StartObject;
        }

        private void WriteStart(byte token)
        {
            if (CurrentDepth >= JsonConstants.MaxWriterDepth)
                ThrowHelper.ThrowJsonWriterException(ExceptionResource.DepthTooLarge, _currentDepth);

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
                    ValidateStart();
                    UpdateBitStackOnStart(token);
                }
                WriteStartIndented(token);
            }
            else
            {
                Debug.Assert(!_writerOptions.SkipValidation);
                ValidateStart();
                UpdateBitStackOnStart(token);
                WriteStartMinimized(token);
            }
        }

        private void ValidateStart()
        {
            if (_inObject)
            {
                Debug.Assert(_tokenType != JsonTokenType.None && _tokenType != JsonTokenType.StartArray);
                ThrowHelper.ThrowJsonWriterException(ExceptionResource.CannotStartObjectArrayWithoutProperty, tokenType: _tokenType);
            }
            else
            {
                Debug.Assert(_tokenType != JsonTokenType.StartObject);
                if (_tokenType != JsonTokenType.None && (!_isNotPrimitive || CurrentDepth == 0))
                {
                    ThrowHelper.ThrowJsonWriterException(ExceptionResource.CannotStartObjectArrayAfterPrimitiveOrClose, tokenType: _tokenType);
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

        /// <summary>
        /// Writes the beginning of a JSON array with a property name as the key.
        /// </summary>
        /// <param name="propertyName">The UTF-8 encoded property name of the JSON array to be written.</param>
        /// <param name="suppressEscaping">If this is set, the writer assumes the property name is properly escaped and skips the escaping step.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified property name is too large.
        /// </exception>
        /// <exception cref="JsonWriterException">
        /// Thrown when the depth of the JSON has exceeded the maximum depth of 1000 
        /// OR if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
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

        /// <summary>
        /// Writes the beginning of a JSON object with a property name as the key.
        /// </summary>
        /// <param name="propertyName">The UTF-8 encoded property name of the JSON object to be written.</param>
        /// <param name="suppressEscaping">If this is set, the writer assumes the property name is properly escaped and skips the escaping step.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified property name is too large.
        /// </exception>
        /// <exception cref="JsonWriterException">
        /// Thrown when the depth of the JSON has exceeded the maximum depth of 1000 
        /// OR if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
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

        /// <summary>
        /// Writes the beginning of a JSON array with a property name as the key.
        /// </summary>
        /// <param name="propertyName">The UTF-16 encoded property name of the JSON array to be transcoded and written as UTF-8.</param>
        /// <param name="suppressEscaping">If this is set, the writer assumes the property name is properly escaped and skips the escaping step.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified property name is too large.
        /// </exception>
        /// <exception cref="JsonWriterException">
        /// Thrown when the depth of the JSON has exceeded the maximum depth of 1000 
        /// OR if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteStartArray(string propertyName, bool suppressEscaping = false)
            => WriteStartArray(propertyName.AsSpan(), suppressEscaping);

        /// <summary>
        /// Writes the beginning of a JSON object with a property name as the key.
        /// </summary>
        /// <param name="propertyName">The UTF-16 encoded property name of the JSON object to be transcoded and written as UTF-8.</param>
        /// <param name="suppressEscaping">If this is set, the writer assumes the property name is properly escaped and skips the escaping step.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified property name is too large.
        /// </exception>
        /// <exception cref="JsonWriterException">
        /// Thrown when the depth of the JSON has exceeded the maximum depth of 1000 
        /// OR if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteStartObject(string propertyName, bool suppressEscaping = false)
            => WriteStartObject(propertyName.AsSpan(), suppressEscaping);

        /// <summary>
        /// Writes the beginning of a JSON array with a property name as the key.
        /// </summary>
        /// <param name="propertyName">The UTF-16 encoded property name of the JSON array to be transcoded and written as UTF-8.</param>
        /// <param name="suppressEscaping">If this is set, the writer assumes the property name is properly escaped and skips the escaping step.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified property name is too large.
        /// </exception>
        /// <exception cref="JsonWriterException">
        /// Thrown when the depth of the JSON has exceeded the maximum depth of 1000 
        /// OR if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
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

        /// <summary>
        /// Writes the beginning of a JSON object with a property name as the key.
        /// </summary>
        /// <param name="propertyName">The UTF-16 encoded property name of the JSON object to be transcoded and written as UTF-8.</param>
        /// <param name="suppressEscaping">If this is set, the writer assumes the property name is properly escaped and skips the escaping step.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when the specified property name is too large.
        /// </exception>
        /// <exception cref="JsonWriterException">
        /// Thrown when the depth of the JSON has exceeded the maximum depth of 1000 
        /// OR if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
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

        /// <summary>
        /// Writes the end of a JSON array.
        /// </summary>
        /// <exception cref="JsonWriterException">
        /// Thrown if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
        public void WriteEndArray()
        {
            WriteEnd(JsonConstants.CloseBracket);
            _tokenType = JsonTokenType.EndArray;
        }

        /// <summary>
        /// Writes the end of a JSON object.
        /// </summary>
        /// <exception cref="JsonWriterException">
        /// Thrown if this would result in an invalid JSON to be written (while validation is enabled).
        /// </exception>
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
                ThrowHelper.ThrowJsonWriterException(ExceptionResource.MismatchedObjectArray, token);

            if (token == JsonConstants.CloseBracket)
            {
                if (_inObject)
                {
                    Debug.Assert(_tokenType != JsonTokenType.None);
                    ThrowHelper.ThrowJsonWriterException(ExceptionResource.MismatchedObjectArray, token);
                }
            }
            else
            {
                Debug.Assert(token == JsonConstants.CloseBrace);

                if (!_inObject)
                {
                    ThrowHelper.ThrowJsonWriterException(ExceptionResource.MismatchedObjectArray, token);
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
            Flush();
            int previousSpanLength = _buffer.Length;
            _buffer = _output.GetSpan(DefaultGrowthSize);
            if (_buffer.Length <= previousSpanLength)
            {
                _buffer = _output.GetSpan(DefaultGrowthSize);
                if (_buffer.Length <= previousSpanLength)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.FailedToGetLargerSpan);
                }
            }
        }

        private void GrowAndEnsure(int minimumSize)
        {
            Flush();
            _buffer = _output.GetSpan(DefaultGrowthSize);
            if (_buffer.Length < minimumSize)
            {
                _buffer = _output.GetSpan(DefaultGrowthSize);
                if (_buffer.Length < minimumSize)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.FailedToGetMinimumSizeSpan, minimumSize);
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

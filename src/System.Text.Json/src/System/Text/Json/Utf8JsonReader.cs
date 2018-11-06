// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Text.Json
{
    /// <summary>
    /// Provides a high-performance API for forward-only, read-only access to the UTF-8 encoded JSON text.
    /// It processes the text sequentially with no caching and adheres strictly to the JSON RFC
    /// by default (https://tools.ietf.org/html/rfc8259). When it encounters invalid JSON, it throws
    /// a JsonReaderException with basic error information like line number and byte position on the line.
    /// Since this type is a ref struct, it does not directly support async. However, it does provide
    /// support for reentrancy to read incomplete data, and continue reading once more data is presented.
    /// To be able to set max depth while reading OR allow skipping comments, create an instance of 
    /// <see cref="JsonReaderState"/> and pass that in to the reader.
    /// </summary>
    public ref partial struct Utf8JsonReader
    {
        private ReadOnlySpan<byte> _buffer;

        private bool _isFinalBlock;

        private ulong _stackFreeContainer;
        private long _lineNumber;
        private long _lineBytePosition;
        private int _consumed;
        private int _currentDepth;
        private int _maxDepth;
        private bool _inObject;
        private bool _isNotPrimitive;
        private JsonTokenType _tokenType;
        private JsonReaderOptions _readerOptions;
        private Stack<JsonTokenType> _stack;

        private long _totalConsumed;
        private bool _isLastSegment;
        private readonly bool _isSingleSegment;

        private bool IsLastSpan => _isFinalBlock && (_isSingleSegment || _isLastSegment);

        /// <summary>
        /// Gets the value of the last processed token as a ReadOnlySpan&lt;byte&gt; slice
        /// of the input payload. If the JSON is provided within a ReadOnlySequence&lt;byte&gt;
        /// and the slice that represents the token value fits in a single segment, then
        /// ValueSpan will contain the sliced value since it can be represented as a span.
        /// </summary>
        public ReadOnlySpan<byte> ValueSpan { get; private set; }

        /// <summary>
        /// Returns the total amount of bytes consumed by the <see cref="Utf8JsonReader"/> so far
        /// for the current instance of the <see cref="Utf8JsonReader"/> with the given UTF-8 encoded input text.
        /// </summary>
        public long BytesConsumed => _totalConsumed + _consumed;

        /// <summary>
        /// Tracks the recursive depth of the nested objects / arrays within the JSON text
        /// processed so far. This provides the depth of the current token.
        /// </summary>
        public int CurrentDepth => _currentDepth;

        /// <summary>
        /// Gets the type of the last processed JSON token in the UTF-8 encoded JSON text.
        /// </summary>
        public JsonTokenType TokenType => _tokenType;

        /// <summary>
        /// Returns the current snapshot of the <see cref="Utf8JsonReader"/> state which must
        /// be captured by the caller and passed back in to the <see cref="Utf8JsonReader"/> ctor with more data.
        /// Unlike the <see cref="Utf8JsonReader"/>, which is a ref struct, the state can survive
        /// across async/await boundaries and hence this type is required to provide support for reading
        /// in more data asynchronously before continuing with a new instance of the <see cref="Utf8JsonReader"/>.
        /// </summary>
        public JsonReaderState CurrentState => new JsonReaderState
        {
            _stackFreeContainer = _stackFreeContainer,
            _lineNumber = _lineNumber,
            _lineBytePosition = _lineBytePosition,
            _bytesConsumed = BytesConsumed,
            _currentDepth = _currentDepth,
            _maxDepth = _maxDepth,
            _inObject = _inObject,
            _isNotPrimitive = _isNotPrimitive,
            _tokenType = _tokenType,
            _readerOptions = _readerOptions,
            _stack = _stack,
        };

        /// <summary>
        /// Constructs a new <see cref="Utf8JsonReader"/> instance.
        /// </summary>
        /// <param name="jsonData">The ReadOnlySpan&lt;byte&gt; containing the UTF-8 encoded JSON text to process.</param>
        /// <param name="isFinalBlock">True when the input span contains the entire data to process.
        /// Set to false only if it is known that the input span contains partial data with more data to follow.</param>
        /// <param name="state">If this is the first call to the ctor, pass in a default state. Otherwise,
        /// capture the state from the previous instance of the <see cref="Utf8JsonReader"/> and pass that back.</param>
        /// <remarks>
        /// Since this type is a ref struct, it is a stack-only type and all the limitations of ref structs apply to it.
        /// This is the reason why the ctor accepts a <see cref="JsonReaderState"/>.
        /// </remarks>
        public Utf8JsonReader(ReadOnlySpan<byte> jsonData, bool isFinalBlock, JsonReaderState state)
        {
            _buffer = jsonData;

            _isFinalBlock = isFinalBlock;

            // Note: We do not retain _bytesConsumed or _sequencePosition as they reset with the new input data
            _stackFreeContainer = state._stackFreeContainer;
            _lineNumber = state._lineNumber;
            _lineBytePosition = state._lineBytePosition;
            _currentDepth = state._currentDepth;
            _maxDepth = state._maxDepth == 0 ? JsonReaderState.StackFreeMaxDepth : state._maxDepth;
            _inObject = state._inObject;
            _isNotPrimitive = state._isNotPrimitive;
            _tokenType = state._tokenType;
            _readerOptions = state._readerOptions;
            _stack = state._stack;

            _consumed = 0;
            _totalConsumed = 0;
            _isLastSegment = _isFinalBlock;
            _isSingleSegment = true;

            ValueSpan = ReadOnlySpan<byte>.Empty;
        }

        /// <summary>
        /// Read the next JSON token from input source.
        /// </summary>
        /// <returns>True if the token was read successfully, else false.</returns>
        /// <exception cref="JsonReaderException">
        /// Thrown when an invalid JSON token is encountered according to the JSON RFC
        /// or if the current depth exceeds the recursive limit set by the max depth.
        /// </exception>
        public bool Read() => ReadSingleSegment();

        private void StartObject()
        {
            if (_currentDepth >= _maxDepth)
                ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ObjectDepthTooLarge);

            _currentDepth++;
            _consumed++;
            _lineBytePosition++;

            if (_readerOptions.CommentHandling != JsonCommentHandling.AllowComments &&
                _currentDepth <= JsonReaderState.StackFreeMaxDepth)
            {
                _stackFreeContainer = (_stackFreeContainer << 1) | 1;
            }
            else
            {
                _stack.Push(JsonTokenType.StartObject);
            }

            _tokenType = JsonTokenType.StartObject;
            _inObject = true;
        }

        private void EndObject()
        {
            if (!_inObject || _currentDepth <= 0)
                ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ObjectEndWithinArray);

            _consumed++;
            _lineBytePosition++;

            if (_readerOptions.CommentHandling != JsonCommentHandling.AllowComments)
            {
                if (_currentDepth <= JsonReaderState.StackFreeMaxDepth)
                {
                    _stackFreeContainer >>= 1;
                    _inObject = (_stackFreeContainer & 1) != 0;
                }
                else
                {
                    Debug.Assert(_stack.Count > 0);
                    _stack.Pop();
                    if (_stack.Count < JsonReaderState.StackFreeMaxDepth)
                    {
                        _inObject = (_stackFreeContainer & 1) != 0;
                    }
                    else
                    {
                        _inObject = _stack.Peek() != JsonTokenType.StartArray;
                    }
                }
            }
            else
            {
                Debug.Assert(_stack.Count > 0);
                _stack.Pop();
                _inObject = _stack.Count != 0 && _stack.Peek() != JsonTokenType.StartArray;
            }

            _currentDepth--;
            _tokenType = JsonTokenType.EndObject;
        }

        private void StartArray()
        {
            if (_currentDepth >= _maxDepth)
                ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ArrayDepthTooLarge);

            _currentDepth++;
            _consumed++;
            _lineBytePosition++;

            if (_readerOptions.CommentHandling != JsonCommentHandling.AllowComments &&
                _currentDepth <= JsonReaderState.StackFreeMaxDepth)
            {
                _stackFreeContainer = _stackFreeContainer << 1;
            }
            else
            {
                _stack.Push(JsonTokenType.StartArray);
            }

            _tokenType = JsonTokenType.StartArray;
            _inObject = false;
        }

        private void EndArray()
        {
            if (_inObject || _currentDepth <= 0)
                ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ArrayEndWithinObject);

            _consumed++;
            _lineBytePosition++;

            if (_readerOptions.CommentHandling != JsonCommentHandling.AllowComments)
            {
                if (_currentDepth <= JsonReaderState.StackFreeMaxDepth)
                {
                    _stackFreeContainer >>= 1;
                    _inObject = (_stackFreeContainer & 1) != 0;
                }
                else
                {
                    Debug.Assert(_stack.Count > 0);
                    _stack.Pop();
                    if (_stack.Count < JsonReaderState.StackFreeMaxDepth)
                    {
                        _inObject = (_stackFreeContainer & 1) != 0;
                    }
                    else
                    {
                        _inObject = _stack.Peek() != JsonTokenType.StartArray;
                    }
                }
            }
            else
            {
                Debug.Assert(_stack.Count > 0);
                _stack.Pop();
                _inObject = _stack.Count != 0 && _stack.Peek() != JsonTokenType.StartArray;
            }

            _currentDepth--;
            _tokenType = JsonTokenType.EndArray;
        }

        private bool ReadSingleSegment()
        {
            bool retVal = false;

            if (!HasMoreData())
            {
                goto Done;
            }

            byte first = _buffer[_consumed];

            if (first <= JsonConstants.Space)
            {
                SkipWhiteSpace();
                if (!HasMoreData())
                {
                    goto Done;
                }
                first = _buffer[_consumed];
            }

            if (_tokenType == JsonTokenType.None)
            {
                goto ReadFirstToken;
            }

            if (first == JsonConstants.Slash)
            {
                retVal = ConsumeNextTokenOrRollback(first);
                goto Done;
            }

            if (_tokenType == JsonTokenType.StartObject)
            {
                if (first == JsonConstants.CloseBrace)
                {
                    EndObject();
                }
                else
                {
                    if (first != JsonConstants.Quote)
                    {
                        ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfPropertyNotFound, first);
                    }

                    int prevConsumed = _consumed;
                    long prevPosition = _lineBytePosition;
                    long prevLineNumber = _lineNumber;
                    retVal = ConsumePropertyName();
                    if (!retVal)
                    {
                        // roll back potential changes
                        _consumed = prevConsumed;
                        _tokenType = JsonTokenType.StartObject;
                        _lineBytePosition = prevPosition;
                        _lineNumber = prevLineNumber;
                    }
                    goto Done;
                }
            }
            else if (_tokenType == JsonTokenType.StartArray)
            {
                if (first == JsonConstants.CloseBracket)
                {
                    EndArray();
                }
                else
                {
                    retVal = ConsumeValue(first);
                    goto Done;
                }
            }
            else if (_tokenType == JsonTokenType.PropertyName)
            {
                retVal = ConsumeValue(first);
                goto Done;
            }
            else
            {
                retVal = ConsumeNextTokenOrRollback(first);
                goto Done;
            }

            retVal = true;

        Done:
            return retVal;

        ReadFirstToken:
            retVal = ReadFirstToken(first);
            goto Done;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasMoreData()
        {
            if (_consumed >= (uint)_buffer.Length)
            {
                if (_isNotPrimitive && IsLastSpan)
                {
                    if (_currentDepth != 0)
                    {
                        ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.InvalidEndOfJson);
                    }

                    if (_readerOptions.CommentHandling == JsonCommentHandling.AllowComments && _tokenType == JsonTokenType.Comment)
                    {
                        return false;
                    }

                    if (_tokenType != JsonTokenType.EndArray && _tokenType != JsonTokenType.EndObject)
                    {
                        ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.InvalidEndOfJson);
                    }
                }
                return false;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasMoreData(ExceptionResource resource)
        {
            if (_consumed >= (uint)_buffer.Length)
            {
                if (IsLastSpan)
                {
                    ThrowHelper.ThrowJsonReaderException(ref this, resource);
                }
                return false;
            }
            return true;
        }

        private bool ReadFirstToken(byte first)
        {
            if (first == JsonConstants.OpenBrace)
            {
                _currentDepth++;
                if (_readerOptions.CommentHandling != JsonCommentHandling.AllowComments)
                {
                    _stackFreeContainer = 1;
                }
                else
                {
                    _stack.Push(JsonTokenType.StartObject);
                }
                _tokenType = JsonTokenType.StartObject;
                _consumed++;
                _lineBytePosition++;
                _inObject = true;
                _isNotPrimitive = true;
            }
            else if (first == JsonConstants.OpenBracket)
            {
                _currentDepth++;
                if (_readerOptions.CommentHandling == JsonCommentHandling.AllowComments)
                {
                    _stack.Push(JsonTokenType.StartArray);
                }
                _tokenType = JsonTokenType.StartArray;
                _consumed++;
                _lineBytePosition++;
                _isNotPrimitive = true;
            }
            else
            {
                //Create local copy to avoid bounds checks.
                ReadOnlySpan<byte> localBuffer = _buffer;

                if ((uint)(first - '0') <= '9' - '0' || first == '-')
                {
                    if (!TryGetNumber(localBuffer.Slice(_consumed), out int numberOfBytes))
                    {
                        return false;
                    }
                    _tokenType = JsonTokenType.Number;
                    _consumed += numberOfBytes;
                    _lineBytePosition += numberOfBytes;
                    goto Done;
                }
                else if (ConsumeValue(first))
                {
                    goto Done;
                }

                return false;

            Done:
                if (_consumed >= (uint)localBuffer.Length)
                {
                    return true;
                }

                if (localBuffer[_consumed] <= JsonConstants.Space)
                {
                    SkipWhiteSpace();
                    if (_consumed >= (uint)localBuffer.Length)
                    {
                        return true;
                    }
                }

                if (_readerOptions.CommentHandling != JsonCommentHandling.Default)
                {
                    if (_readerOptions.CommentHandling == JsonCommentHandling.AllowComments)
                    {
                        if (_tokenType == JsonTokenType.Comment || localBuffer[_consumed] == JsonConstants.Slash)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        // JsonCommentHandling.SkipComments
                        if (_tokenType == JsonTokenType.StartObject || _tokenType == JsonTokenType.StartArray)
                        {
                            _isNotPrimitive = true;
                        }
                        return true;
                    }
                }
                ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedEndAfterSingleJson, localBuffer[_consumed]);
            }
            return true;
        }

        private void SkipWhiteSpace()
        {
            //Create local copy to avoid bounds checks.
            ReadOnlySpan<byte> localBuffer = _buffer;
            for (; _consumed < localBuffer.Length; _consumed++)
            {
                byte val = localBuffer[_consumed];
                if (val != JsonConstants.Space &&
                    val != JsonConstants.CarriageReturn &&
                    val != JsonConstants.LineFeed &&
                    val != JsonConstants.Tab)
                {
                    break;
                }

                if (val == JsonConstants.LineFeed)
                {
                    _lineNumber++;
                    _lineBytePosition = 0;
                }
                else
                {
                    _lineBytePosition++;
                }
            }
        }

        /// <summary>
        /// This method contains the logic for processing the next value token and determining
        /// what type of data it is.
        /// </summary>
        private bool ConsumeValue(byte marker)
        {
            if (marker == JsonConstants.Quote)
            {
                return ConsumeString();
            }
            else if (marker == JsonConstants.OpenBrace)
            {
                StartObject();
            }
            else if (marker == JsonConstants.OpenBracket)
            {
                StartArray();
            }
            else if ((uint)(marker - '0') <= '9' - '0' || marker == '-')
            {
                return ConsumeNumber();
            }
            else if (marker == 'f')
            {
                return ConsumeLiteral(JsonConstants.FalseValue, JsonTokenType.False);
            }
            else if (marker == 't')
            {
                return ConsumeLiteral(JsonConstants.TrueValue, JsonTokenType.True);
            }
            else if (marker == 'n')
            {
                return ConsumeLiteral(JsonConstants.NullValue, JsonTokenType.Null);
            }
            else
            {
                switch (_readerOptions.CommentHandling)
                {
                    case JsonCommentHandling.Default:
                        break;
                    case JsonCommentHandling.AllowComments:
                        if (marker == JsonConstants.Slash)
                        {
                            return ConsumeComment();
                        }
                        break;
                    case JsonCommentHandling.SkipComments:
                        if (marker == JsonConstants.Slash)
                        {
                            if (SkipComment())
                            {
                                if (_consumed >= (uint)_buffer.Length)
                                {
                                    if (_isNotPrimitive && IsLastSpan && _tokenType != JsonTokenType.EndArray && _tokenType != JsonTokenType.EndObject)
                                    {
                                        ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.InvalidEndOfJson);
                                    }
                                    return false;
                                }

                                byte first = _buffer[_consumed];

                                if (first <= JsonConstants.Space)
                                {
                                    SkipWhiteSpace();
                                    if (!HasMoreData())
                                    {
                                        return false;
                                    }
                                    first = _buffer[_consumed];
                                }

                                return ConsumeValue(first);
                            }
                            return false;
                        }
                        break;
                }
                ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfValueNotFound, marker);
            }
            return true;
        }

        // Consumes 'null', or 'true', or 'false'
        private bool ConsumeLiteral(ReadOnlySpan<byte> literal, JsonTokenType tokenType)
        {
            ReadOnlySpan<byte> span = _buffer.Slice(_consumed);
            Debug.Assert(span.Length > 0);
            Debug.Assert(span[0] == 'n' || span[0] == 't' || span[0] == 'f');

            if (!span.StartsWith(literal))
            {
                return CheckLiteral(span, literal);
            }

            ValueSpan = span.Slice(0, literal.Length);
            _tokenType = tokenType;
            _consumed += literal.Length;
            _lineBytePosition += literal.Length;
            return true;
        }

        private bool CheckLiteral(ReadOnlySpan<byte> span, ReadOnlySpan<byte> literal)
        {
            Debug.Assert(span.Length > 0 && span[0] == literal[0]);

            int indexOfFirstMismatch = 0;

            for (int i = 1; i < literal.Length; i++)
            {
                if (span.Length > i)
                {
                    if (span[i] != literal[i])
                    {
                        _lineBytePosition += i;
                        ThrowInvalidLiteral(span);
                    }
                }
                else
                {
                    indexOfFirstMismatch = i;
                    break;
                }
            }

            Debug.Assert(indexOfFirstMismatch > 0 && indexOfFirstMismatch < literal.Length);

            if (IsLastSpan)
            {
                _lineBytePosition += indexOfFirstMismatch;
                ThrowInvalidLiteral(span);
            }
            return false;
        }

        private void ThrowInvalidLiteral(ReadOnlySpan<byte> span)
        {
            byte firstByte = span[0];

            ExceptionResource resource;
            switch (firstByte)
            {
                case (byte)'t':
                    resource = ExceptionResource.ExpectedTrue;
                    break;
                case (byte)'f':
                    resource = ExceptionResource.ExpectedFalse;
                    break;
                default:
                    Debug.Assert(firstByte == 'n');
                    resource = ExceptionResource.ExpectedNull;
                    break;
            }
            ThrowHelper.ThrowJsonReaderException(ref this, resource, bytes: span);
        }

        private bool ConsumeNumber()
        {
            if (!TryGetNumber(_buffer.Slice(_consumed), out int consumed))
            {
                return false;
            }

            _tokenType = JsonTokenType.Number;
            _consumed += consumed;
            _lineBytePosition += consumed;

            if (_consumed >= (uint)_buffer.Length)
            {
                if (IsLastSpan)
                {
                    ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedEndOfDigitNotFound, _buffer[_consumed - 1]);
                }
                else
                {
                    return false;
                }
            }

            // TODO: Is this check necessary or is it redundant and can be removed?
            if (JsonConstants.Delimiters.IndexOf(_buffer[_consumed]) < 0)
            {
                ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedEndOfDigitNotFound, _buffer[_consumed]);
            }
            return true;
        }

        private bool ConsumePropertyName()
        {
            if (!ConsumeString())
            {
                return false;
            }

            if (!HasMoreData(ExceptionResource.ExpectedValueAfterPropertyNameNotFound))
            {
                return false;
            }

            byte first = _buffer[_consumed];

            if (first <= JsonConstants.Space)
            {
                SkipWhiteSpace();
                if (!HasMoreData(ExceptionResource.ExpectedValueAfterPropertyNameNotFound))
                {
                    return false;
                }
                first = _buffer[_consumed];
            }

            // The next character must be a key / value seperator. Validate and skip.
            if (first != JsonConstants.KeyValueSeperator)
            {
                ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedSeparaterAfterPropertyNameNotFound, first);
            }

            _consumed++;
            _lineBytePosition++;
            _tokenType = JsonTokenType.PropertyName;
            return true;
        }

        private bool ConsumeString()
        {
            Debug.Assert(_buffer.Length >= _consumed + 1);
            Debug.Assert(_buffer[_consumed] == JsonConstants.Quote);

            //Create local copy to avoid bounds checks.
            ReadOnlySpan<byte> localBuffer = _buffer;

            int idx = localBuffer.Slice(_consumed + 1).IndexOf(JsonConstants.Quote);
            if (idx < 0)
            {
                if (IsLastSpan)
                {
                    ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.EndOfStringNotFound);
                }
                else
                {
                    return false;
                }
            }

            if (localBuffer[idx + _consumed] != JsonConstants.BackSlash)
            {
                localBuffer = localBuffer.Slice(_consumed + 1, idx);

                if (localBuffer.IndexOfAnyControlOrEscape() != -1)
                {
                    _lineBytePosition++;
                    ValidateEscapingAndHex(localBuffer);
                    goto Done;
                }

                _lineBytePosition += idx + 1;

            Done:
                _lineBytePosition++;
                ValueSpan = localBuffer;
                _tokenType = JsonTokenType.String;
                _consumed += idx + 2;
                return true;
            }
            else
            {
                return ConsumeStringWithNestedQuotes();
            }
        }

        private bool ConsumeStringWithNestedQuotes()
        {
            //TODO: Optimize looking for nested quotes
            //TODO: Avoid redoing first IndexOf search
            int i = _consumed + 1;

            Debug.Assert(_buffer.Length >= i);

            while (true)
            {
                int counter = 0;
                int foundIdx = _buffer.Slice(i).IndexOf(JsonConstants.Quote);
                if (foundIdx == -1)
                {
                    if (IsLastSpan)
                    {
                        ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.EndOfStringNotFound);
                    }
                    else
                    {
                        return false;
                    }
                }
                if (foundIdx == 0)
                {
                    break;
                }
                for (int j = i + foundIdx - 1; j >= i; j--)
                {
                    if (_buffer[j] != JsonConstants.BackSlash)
                    {
                        if ((counter & 1) == 0)
                        {
                            i += foundIdx;
                            goto FoundEndOfString;
                        }
                        break;
                    }
                    else
                    {
                        counter++;
                    }
                }
                i += foundIdx + 1;
            }

        FoundEndOfString:
            int startIndex = _consumed + 1;
            ReadOnlySpan<byte> foundStringToken = _buffer.Slice(startIndex, i - startIndex);

            if (foundStringToken.IndexOfAnyControlOrEscape() != -1)
            {
                _lineBytePosition++;
                ValidateEscapingAndHex(foundStringToken);
                goto Done;
            }

            _lineBytePosition = i;

        Done:
            _lineBytePosition++;
            ValueSpan = foundStringToken;
            _tokenType = JsonTokenType.String;
            _consumed = i + 1;
            return true;
        }

        // https://tools.ietf.org/html/rfc8259#section-7
        private void ValidateEscapingAndHex(ReadOnlySpan<byte> data)
        {
            // TODO: Optimize validating string contents
            bool nextCharEscaped = false;
            for (int i = 0; i < data.Length; i++)
            {
                byte currentByte = data[i];
                if (currentByte == JsonConstants.BackSlash)
                {
                    nextCharEscaped = !nextCharEscaped;
                }
                else if (nextCharEscaped)
                {
                    int index = JsonConstants.EscapableChars.IndexOf(currentByte);
                    if (index == -1)
                    {
                        ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.InvalidCharacterWithinString, currentByte);
                    }

                    if (currentByte == 'n')
                    {
                        // Escaped new line character
                        _lineBytePosition = -1; // Should be 0, but we increment _lineBytePosition below already
                        _lineNumber++;
                    }
                    else if (currentByte == 'u')
                    {
                        // Expecting 4 hex digits to follow the escaped 'u'
                        _lineBytePosition++;
                        int startIndex = i + 1;
                        for (int j = startIndex; j < data.Length; j++)
                        {
                            byte nextByte = data[j];
                            if (!JsonReaderHelper.IsHexDigit(nextByte))
                            {
                                ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.InvalidCharacterWithinString, nextByte);
                            }
                            if (j - startIndex >= 4)
                            {
                                break;
                            }
                            _lineBytePosition++;
                        }
                        i += 4;
                    }
                    nextCharEscaped = false;
                }
                else if (currentByte < JsonConstants.Space)
                {
                    ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.InvalidCharacterWithinString, currentByte);
                }

                _lineBytePosition++;
            }
        }

        // https://tools.ietf.org/html/rfc7159#section-6
        private bool TryGetNumber(ReadOnlySpan<byte> data, out int consumed)
        {
            // TODO: Optimize number processing and validation
            // TODO: Cache the type of number into a private field (int, long, double, etc.)
            Debug.Assert(data.Length > 0);

            consumed = 0;
            int i = 0;

            ConsumeNumberResult signResult = ConsumeNegativeSign(ref data, ref i);
            if (signResult == ConsumeNumberResult.NeedMoreData)
            {
                return false;
            }

            Debug.Assert(signResult == ConsumeNumberResult.OperationIncomplete);

            byte nextByte = data[i];
            Debug.Assert(nextByte >= '0' && nextByte <= '9');

            if (nextByte == '0')
            {
                ConsumeNumberResult result = ConsumeZero(ref data, ref i);
                if (result == ConsumeNumberResult.NeedMoreData)
                {
                    return false;
                }
                if (result == ConsumeNumberResult.Success)
                {
                    goto Done;
                }

                nextByte = data[i];
            }
            else
            {
                i++;
                ConsumeNumberResult result = ConsumeIntegerDigits(ref data, ref i);
                if (result == ConsumeNumberResult.NeedMoreData)
                {
                    return false;
                }
                if (result == ConsumeNumberResult.Success)
                {
                    goto Done;
                }
                nextByte = data[i];
                if (nextByte != '.' && nextByte != 'E' && nextByte != 'e')
                {
                    _lineBytePosition += i;
                    ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedNextDigitComponentNotFound, nextByte);
                }
            }

            Debug.Assert(nextByte == '.' || nextByte == 'E' || nextByte == 'e');

            if (nextByte == '.')
            {
                i++;
                ConsumeNumberResult result = ConsumeDecimalDigits(ref data, ref i, nextByte);
                if (result == ConsumeNumberResult.NeedMoreData)
                {
                    return false;
                }
                if (result == ConsumeNumberResult.Success)
                {
                    goto Done;
                }

                nextByte = data[i];
                if (nextByte != 'E' && nextByte != 'e')
                {
                    _lineBytePosition += i;
                    ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedNextDigitEValueNotFound, nextByte);
                }
            }

            Debug.Assert(nextByte == 'E' || nextByte == 'e');
            i++;

            signResult = ConsumeSign(ref data, ref i, nextByte);
            if (signResult == ConsumeNumberResult.NeedMoreData)
            {
                return false;
            }

            Debug.Assert(signResult == ConsumeNumberResult.OperationIncomplete);

            i++;
            ConsumeNumberResult resultExponent = ConsumeIntegerDigits(ref data, ref i);
            if (resultExponent == ConsumeNumberResult.NeedMoreData)
            {
                return false;
            }
            if (resultExponent == ConsumeNumberResult.Success)
            {
                goto Done;
            }
            if (resultExponent == ConsumeNumberResult.OperationIncomplete)
            {
                _lineBytePosition += i;
                ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedEndOfDigitNotFound, nextByte);
            }

        Done:
            ValueSpan = data.Slice(0, i);
            consumed = i;
            return true;
        }

        private ConsumeNumberResult ConsumeNegativeSign(ref ReadOnlySpan<byte> data, ref int i)
        {
            byte nextByte = data[i];

            if (nextByte == '-')
            {
                i++;
                if (i >= data.Length)
                {
                    if (IsLastSpan)
                    {
                        _lineBytePosition += i;
                        ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedDigitNotFoundEndOfData, nextByte);
                    }
                    else
                    {
                        return ConsumeNumberResult.NeedMoreData;
                    }
                }

                nextByte = data[i];
                if (!JsonReaderHelper.IsDigit(nextByte))
                {
                    _lineBytePosition += i;
                    ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedDigitNotFound, nextByte);
                }
            }
            return ConsumeNumberResult.OperationIncomplete;
        }

        private ConsumeNumberResult ConsumeZero(ref ReadOnlySpan<byte> data, ref int i)
        {
            i++;
            byte nextByte = default;
            if (i < data.Length)
            {
                nextByte = data[i];
                if (JsonConstants.Delimiters.IndexOf(nextByte) >= 0)
                {
                    return ConsumeNumberResult.Success;
                }
            }
            else
            {
                if (IsLastSpan)
                {
                    return ConsumeNumberResult.Success;
                }
                else
                {
                    return ConsumeNumberResult.NeedMoreData;
                }
            }
            nextByte = data[i];
            if (nextByte != '.' && nextByte != 'E' && nextByte != 'e')
            {
                _lineBytePosition += i;
                ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedNextDigitComponentNotFound, nextByte);
            }

            return ConsumeNumberResult.OperationIncomplete;
        }

        private ConsumeNumberResult ConsumeIntegerDigits(ref ReadOnlySpan<byte> data, ref int i)
        {
            byte nextByte = default;
            for (; i < data.Length; i++)
            {
                nextByte = data[i];
                if (!JsonReaderHelper.IsDigit(nextByte))
                {
                    break;
                }
            }
            if (i >= data.Length)
            {
                if (IsLastSpan)
                {
                    return ConsumeNumberResult.Success;
                }
                else
                {
                    return ConsumeNumberResult.NeedMoreData;
                }
            }
            if (JsonConstants.Delimiters.IndexOf(nextByte) >= 0)
            {
                return ConsumeNumberResult.Success;
            }

            return ConsumeNumberResult.OperationIncomplete;
        }

        private ConsumeNumberResult ConsumeDecimalDigits(ref ReadOnlySpan<byte> data, ref int i, byte nextByte)
        {
            if (i >= data.Length)
            {
                if (IsLastSpan)
                {
                    _lineBytePosition += i;
                    ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedDigitNotFoundEndOfData, nextByte);
                }
                else
                {
                    return ConsumeNumberResult.NeedMoreData;
                }
            }
            nextByte = data[i];
            if (!JsonReaderHelper.IsDigit(nextByte))
            {
                _lineBytePosition += i;
                ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedDigitNotFound, nextByte);
            }
            i++;

            return ConsumeIntegerDigits(ref data, ref i);
        }

        private ConsumeNumberResult ConsumeSign(ref ReadOnlySpan<byte> data, ref int i, byte nextByte)
        {
            if (i >= data.Length)
            {
                if (IsLastSpan)
                {
                    _lineBytePosition += i;
                    ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedDigitNotFoundEndOfData, nextByte);
                }
                else
                {
                    return ConsumeNumberResult.NeedMoreData;
                }
            }

            nextByte = data[i];
            if (nextByte == '+' || nextByte == '-')
            {
                i++;
                if (i >= data.Length)
                {
                    if (IsLastSpan)
                    {
                        _lineBytePosition += i;
                        ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedDigitNotFoundEndOfData, nextByte);
                    }
                    else
                    {
                        return ConsumeNumberResult.NeedMoreData;
                    }
                }
                nextByte = data[i];
            }

            if (!JsonReaderHelper.IsDigit(nextByte))
            {
                _lineBytePosition += i;
                ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedDigitNotFound, nextByte);
            }

            return ConsumeNumberResult.OperationIncomplete;
        }

        private bool ConsumeNextTokenOrRollback(byte marker)
        {
            int prevConsumed = _consumed;
            long prevPosition = _lineBytePosition;
            long prevLineNumber = _lineNumber;
            JsonTokenType prevTokenType = _tokenType;
            ConsumeTokenResult result = ConsumeNextToken(marker);
            if (result == ConsumeTokenResult.Success)
            {
                return true;
            }
            if (result == ConsumeTokenResult.IncompleteRollback)
            {
                _consumed = prevConsumed;
                _tokenType = prevTokenType;
                _lineBytePosition = prevPosition;
                _lineNumber = prevLineNumber;
            }
            return false;
        }

        /// <summary>
        /// This method consumes the next token regardless of whether we are inside an object or an array.
        /// For an object, it reads the next property name token. For an array, it just reads the next value.
        /// </summary>
        private ConsumeTokenResult ConsumeNextToken(byte marker)
        {
            // TODO: Avoid unconstrained recursive calls
            if (_readerOptions.CommentHandling != JsonCommentHandling.Default)
            {
                //TODO: Re-evaluate use of ConsumeTokenResult enum for the common case
                if (_readerOptions.CommentHandling == JsonCommentHandling.AllowComments)
                {
                    if (marker == JsonConstants.Slash)
                    {
                        return ConsumeComment() ? ConsumeTokenResult.Success : ConsumeTokenResult.IncompleteRollback;
                    }
                    if (_tokenType == JsonTokenType.Comment)
                    {
                        JsonTokenType lastTokenType = _stack.Pop();
                        if (lastTokenType >= JsonTokenType.String && lastTokenType <= JsonTokenType.Null)
                        {
                            _tokenType = _inObject ? JsonTokenType.StartObject : JsonTokenType.StartArray;
                        }
                        else
                        {
                            _tokenType = lastTokenType;
                        }

                        if (!HasMoreData())
                        {
                            _stack.Push(_tokenType);
                            return ConsumeTokenResult.IncompleteRollback;
                        }

                        byte first = _buffer[_consumed];

                        if (first <= JsonConstants.Space)
                        {
                            SkipWhiteSpace();
                            if (!HasMoreData())
                            {
                                _stack.Push(_tokenType);
                                return ConsumeTokenResult.IncompleteRollback;
                            }
                            first = _buffer[_consumed];
                        }

                        if (!_isNotPrimitive)
                        {
                            if (first != JsonConstants.Slash && _tokenType != JsonTokenType.None && _tokenType != JsonTokenType.Comment)
                            {
                                ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedEndAfterSingleJson, first);
                            }
                        }

                        if (first == JsonConstants.ListSeperator || first == JsonConstants.CloseBrace || first == JsonConstants.CloseBracket)
                        {
                            if (ConsumeNextToken(first) == ConsumeTokenResult.Success)
                            {
                                return ConsumeTokenResult.Success;
                            }
                            else
                            {
                                _stack.Push(_tokenType);
                                return ConsumeTokenResult.IncompleteRollback;
                            }
                        }
                        else
                        {
                            if (ReadSingleSegment())
                            {
                                return ConsumeTokenResult.Success;
                            }
                            else
                            {
                                _stack.Push(_tokenType);
                                return ConsumeTokenResult.IncompleteRollback;
                            }
                        }
                    }
                }
                else
                {
                    // JsonCommentHandling.SkipComments
                    if (marker == JsonConstants.Slash)
                    {
                        if (SkipComment())
                        {
                            return ReadSingleSegment() ? ConsumeTokenResult.Success : ConsumeTokenResult.IncompleteNoRollback;
                        }
                        return ConsumeTokenResult.IncompleteRollback;
                    }
                }
            }

            if (!_isNotPrimitive)
            {
                ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedEndAfterSingleJson, marker);
            }

            if (marker == JsonConstants.ListSeperator)
            {
                _consumed++;
                _lineBytePosition++;

                if (_consumed >= (uint)_buffer.Length)
                {
                    if (IsLastSpan)
                    {
                        _consumed--;
                        _lineBytePosition--;
                        ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfPropertyOrValueNotFound);
                    }
                    else
                    {
                        return ConsumeTokenResult.IncompleteRollback;
                    }
                }
                byte first = _buffer[_consumed];

                if (first <= JsonConstants.Space)
                {
                    SkipWhiteSpace();
                    // The next character must be a start of a property name or value.
                    if (!HasMoreData(ExceptionResource.ExpectedStartOfPropertyOrValueNotFound))
                    {
                        return ConsumeTokenResult.IncompleteRollback;
                    }
                    first = _buffer[_consumed];
                }

                if (_inObject)
                {
                    if (first != JsonConstants.Quote)
                    {
                        ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfPropertyNotFound, first);
                    }
                    return ConsumePropertyName() ? ConsumeTokenResult.Success : ConsumeTokenResult.IncompleteRollback;
                }
                else
                {
                    return ConsumeValue(first) ? ConsumeTokenResult.Success : ConsumeTokenResult.IncompleteRollback;
                }
            }
            else if (marker == JsonConstants.CloseBrace)
            {
                EndObject();
            }
            else if (marker == JsonConstants.CloseBracket)
            {
                EndArray();
            }
            else
            {
                ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.FoundInvalidCharacter, marker);
            }
            return ConsumeTokenResult.Success;
        }

        private bool SkipComment()
        {
            //Create local copy to avoid bounds checks.
            ReadOnlySpan<byte> localBuffer = _buffer.Slice(_consumed + 1);

            if (localBuffer.Length > 0)
            {
                byte marker = localBuffer[0];
                if (marker == JsonConstants.Slash)
                {
                    return SkipSingleLineComment(localBuffer.Slice(1), out _);
                }
                else if (marker == JsonConstants.Asterisk)
                {
                    return SkipMultiLineComment(localBuffer.Slice(1), out _);
                }
                else
                {
                    ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfValueNotFound, marker);
                }
            }

            if (IsLastSpan)
            {
                ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfValueNotFound, JsonConstants.Slash);
            }
            return false;
        }

        private bool SkipSingleLineComment(ReadOnlySpan<byte> localBuffer, out int idx)
        {
            //TODO: Match Json.NET's end of comment semantics
            idx = localBuffer.IndexOf(JsonConstants.LineFeed);
            if (idx == -1)
            {
                if (IsLastSpan)
                {
                    idx = localBuffer.Length;
                    // Assume everything on this line is a comment and there is no more data.
                    _lineBytePosition += 2 + localBuffer.Length;
                    goto Done;
                }
                else
                {
                    return false;
                }
            }

            _consumed++;
            _lineBytePosition = 0;
            _lineNumber++;
        Done:
            _consumed += 2 + idx;
            return true;
        }

        private bool SkipMultiLineComment(ReadOnlySpan<byte> localBuffer, out int idx)
        {
            idx = 0;
            while (true)
            {
                int foundIdx = localBuffer.Slice(idx).IndexOf(JsonConstants.Slash);
                if (foundIdx == -1)
                {
                    if (IsLastSpan)
                        ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.EndOfCommentNotFound);
                    else
                    {
                        return false;
                    }
                }
                if (foundIdx != 0 && localBuffer[foundIdx + idx - 1] == JsonConstants.Asterisk)
                {
                    idx += foundIdx;
                    break;
                }
                idx += foundIdx + 1;
            }

            Debug.Assert(idx >= 1);
            _consumed += 3 + idx;

            (int newLines, int newLineIndex) = JsonReaderHelper.CountNewLines(localBuffer.Slice(0, idx - 1));
            _lineNumber += newLines;
            if (newLineIndex != -1)
            {
                _lineBytePosition = idx - newLineIndex;
            }
            else
            {
                _lineBytePosition += 4 + idx - 1;
            }
            return true;
        }

        private bool ConsumeComment()
        {
            //Create local copy to avoid bounds checks.
            ReadOnlySpan<byte> localBuffer = _buffer.Slice(_consumed + 1);

            if (localBuffer.Length > 0)
            {
                byte marker = localBuffer[0];
                if (marker == JsonConstants.Slash)
                {
                    return ConsumeSingleLineComment(localBuffer.Slice(1));
                }
                else if (marker == JsonConstants.Asterisk)
                {
                    return ConsumeMultiLineComment(localBuffer.Slice(1));
                }
                else
                {
                    ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfValueNotFound, marker);
                }
            }

            if (IsLastSpan)
            {
                ThrowHelper.ThrowJsonReaderException(ref this, ExceptionResource.ExpectedStartOfValueNotFound, JsonConstants.Slash);
            }
            return false;
        }

        private bool ConsumeSingleLineComment(ReadOnlySpan<byte> localBuffer)
        {
            if (!SkipSingleLineComment(localBuffer, out int idx))
            {
                return false;
            }

            ValueSpan = localBuffer.Slice(0, idx);
            _stack.Push(_tokenType);
            _tokenType = JsonTokenType.Comment;
            return true;
        }

        private bool ConsumeMultiLineComment(ReadOnlySpan<byte> localBuffer)
        {
            if (!SkipMultiLineComment(localBuffer, out int idx))
            {
                return false;
            }

            ValueSpan = localBuffer.Slice(0, idx - 1);
            _stack.Push(_tokenType);
            _tokenType = JsonTokenType.Comment;
            return true;
        }
    }
}

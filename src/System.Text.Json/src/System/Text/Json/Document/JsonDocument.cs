// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace System.Text.Json
{
    /// <summary>
    ///   Provides a mechanism for examining the structural content of a JSON value without
    ///   automatically instantiating data values.
    /// </summary>
    /// <remarks>
    ///   This class utilizes resources from pooled memory to minimize the garbage collector (GC)
    ///   impact in high-usage scenarios. Failure to properly Dispose this object will result in
    ///   the memory not being returned to the pool, which will cause an increase in GC impact across
    ///   various parts of the framework.
    /// </remarks>
    public sealed partial class JsonDocument : IDisposable
    {
        private ReadOnlyMemory<byte> _utf8Json;
        private MetadataDb _parsedData;
        private byte[] _extraRentedBytes;
        private (int, string) _lastIndexAndString = (-1, null);

        /// <summary>
        ///   Whether or not this JsonDocument is detached from memory provided during a call to Parse.
        /// </summary>
        public bool IsDetached { get; }

        /// <summary>
        ///   Whether or not this JsonDocument should be disposed when no longer needed.
        /// </summary>
        /// <remarks>
        ///   The default behavior for JsonDocument is to utilize pooled arrays for its data, and
        ///   <see cref="Dispose"/> returns those arrays to their respective pools. A call to
        ///   <see cref="Detach"/> can indicate whether the utilized memory comes from the array
        ///   pools, or new arrays held by this type. When new arrays are used, Dispose has no effect
        ///   and therefore no longer needs to be called.
        /// </remarks>
        public bool IsDisposable { get; }

        /// <summary>
        ///   The <see cref="JsonElement"/> representing the value of the document.
        /// </summary>
        public JsonElement RootElement => new JsonElement(this, 0);

        private JsonDocument(
            ReadOnlyMemory<byte> utf8Json,
            MetadataDb parsedData,
            byte[] extraRentedBytes,
            bool isDisposable=true)
        {
            Debug.Assert(!utf8Json.IsEmpty);

            _utf8Json = utf8Json;
            _parsedData = parsedData;
            _extraRentedBytes = extraRentedBytes;

            IsDetached = (extraRentedBytes != null);
            IsDisposable = isDisposable;

            if (!isDisposable)
            {
                Debug.Assert(extraRentedBytes == null);
                IsDetached = true;
            }
        }

        private JsonDocument(JsonDocument source, bool useArrayPools)
        {
            if (useArrayPools)
            {
                byte[] newJson = ArrayPool<byte>.Shared.Rent(source._utf8Json.Length);
                source._utf8Json.Span.CopyTo(newJson);
                _utf8Json = newJson;
                _extraRentedBytes = newJson;
            }
            else
            {
                _utf8Json = source._utf8Json.ToArray();
            }

            _parsedData = new MetadataDb(source._parsedData, useArrayPools);
            IsDetached = true;
            IsDisposable = useArrayPools;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_utf8Json.IsEmpty || !IsDisposable)
            {
                return;
            }

            int length = _utf8Json.Length;
            _utf8Json = ReadOnlyMemory<byte>.Empty;
            _parsedData.Dispose();

            // When "extra rented bytes exist" they contain the document,
            // and thus need to be cleared before being returned.
            if (_extraRentedBytes != null)
            {
                _extraRentedBytes.AsSpan(0, length).Clear();
                ArrayPool<byte>.Shared.Return(_extraRentedBytes);
                _extraRentedBytes = null;
            }
        }

        /// <summary>
        ///   Get a JsonDocument representing the same contents as the current object,
        ///   but which does not depend on the data provided to
        ///   <see cref="Parse(ReadOnlyMemory{byte},JsonReaderOptions)"/> (or another overload)
        ///   remaining unchanged.
        /// </summary>
        /// <param name="useArrayPools">
        ///   <see langword="true"/> to use pooled arrays where possible, <see langword="false"/> to
        ///   use newly created arrays for simpler lifetime management.
        ///   (Defaults to <see langword="false"/>.)
        /// </param>
        /// <remarks>
        ///   <para>
        ///     This method returns <see langword="this"/> when doing so is only detectable via
        ///     a reference equality test. If the current instance is already detached (
        ///     <see cref="IsDetached"/> == <see langword="true"/>) and has GC lifetime semantics
        ///     instead of IDisposable lifetime (<see cref="IsDisposable"/> == <see langword="false"/>)
        ///     then no functional side effects can be observed by multiple callers acting on the
        ///     same instance.
        ///   </para>
        ///   <para>
        ///     When invoking this method with <paramref name="useArrayPools"/> == <see langword="true"/>,
        ///     the caller is responsible for managing the lifetime of the returned object (as an
        ///     <see cref="IDisposable"/>). When <paramref name="useArrayPools"/> == <see langword="false"/>,
        ///     <see cref="Dispose"/> has no effect and therefore the caller has no lifetime management
        ///     responsibilities.
        ///   </para>
        /// </remarks>
        /// <returns>
        ///   A JsonDocument instance representing the same contents, but with memory independent
        ///   of what was provided to <see cref="Parse(ReadOnlyMemory{byte},JsonReaderOptions)"/> (or
        ///   another overload).
        /// </returns>
        public JsonDocument Detach(bool useArrayPools=false)
        {
            if (IsDetached && !IsDisposable)
            {
                return this;
            }

            return new JsonDocument(this, useArrayPools);
        }

        /// <summary>
        ///   This is an implementation detail and MUST NOT be called by source-package consumers.
        /// </summary>
        internal JsonTokenType GetJsonTokenType(int index)
        {
            CheckNotDisposed();

            return _parsedData.GetJsonTokenType(index);
        }

        /// <summary>
        ///   This is an implementation detail and MUST NOT be called by source-package consumers.
        /// </summary>
        internal int GetArrayLength(int index)
        {
            CheckNotDisposed();

            DbRow row = _parsedData.Get(index);

            CheckExpectedType(JsonTokenType.StartArray, row.TokenType);

            return row.SizeOrLength;
        }

        /// <summary>
        ///   This is an implementation detail and MUST NOT be called by source-package consumers.
        /// </summary>
        internal JsonElement GetArrayIndexElement(int currentIndex, int arrayIndex)
        {
            CheckNotDisposed();

            DbRow row = _parsedData.Get(currentIndex);

            CheckExpectedType(JsonTokenType.StartArray, row.TokenType);

            int arrayLength = row.SizeOrLength;

            if ((uint)arrayIndex >= (uint)arrayLength)
            {
                throw new IndexOutOfRangeException();
            }

            if (!row.HasComplexChildren)
            {
                // Since we wouldn't be here without having completed the document parse, and we
                // already vetted the index against the length, this new index will always be
                // within the table.
                return new JsonElement(this, currentIndex + ((arrayIndex + 1) * DbRow.Size));
            }

            int elementCount = 0;
            int objectOffset = currentIndex + DbRow.Size;

            for (; objectOffset < _parsedData.Length; objectOffset += DbRow.Size)
            {
                if (arrayIndex == elementCount)
                {
                    return new JsonElement(this, objectOffset);
                }

                row = _parsedData.Get(objectOffset);

                if (!row.IsSimpleValue)
                {
                    objectOffset += DbRow.Size * row.NumberOfRows;
                }

                elementCount++;
            }

            Debug.Fail(
                $"Ran out of database searching for array index {arrayIndex} from {currentIndex} when length was {arrayLength}");
            throw new IndexOutOfRangeException();
        }

        /// <summary>
        ///   This is an implementation detail and MUST NOT be called by source-package consumers.
        /// </summary>
        internal int GetEndIndex(int index, bool includeEndElement)
        {
            CheckNotDisposed();

            DbRow row = _parsedData.Get(index);

            if (row.IsSimpleValue)
            {
                return index + DbRow.Size;
            }

            int endIndex = index + DbRow.Size * row.NumberOfRows;

            if (includeEndElement)
            {
                endIndex += DbRow.Size;
            }

            return endIndex;
        }

        private ReadOnlyMemory<byte> GetRawValue(int index, bool includeQuotes)
        {
            CheckNotDisposed();

            DbRow row = _parsedData.Get(index);

            if (row.IsSimpleValue)
            {
                if (includeQuotes && row.TokenType == JsonTokenType.String)
                {
                    // Start one character earlier than the value (the open quote)
                    // End one character after the value (the close quote)
                    return _utf8Json.Slice(row.Location - 1, row.SizeOrLength + 2);
                }

                return _utf8Json.Slice(row.Location, row.SizeOrLength);
            }

            int endElementIdx = GetEndIndex(index, includeEndElement: false);
            int start = row.Location;
            row = _parsedData.Get(endElementIdx);
            return _utf8Json.Slice(start, row.Location - start + row.SizeOrLength);
        }

        private ReadOnlyMemory<byte> GetPropertyRawValue(int valueIndex)
        {
            CheckNotDisposed();

            // The property name is stored one row before the value
            DbRow row = _parsedData.Get(valueIndex - DbRow.Size);
            Debug.Assert(row.TokenType == JsonTokenType.PropertyName);

            // Subtract one for the open quote.
            int start = row.Location - 1;
            int end;

            row = _parsedData.Get(valueIndex);

            if (row.IsSimpleValue)
            {
                end = row.Location + row.SizeOrLength;

                // If the value was a string, pick up the terminating quote.
                if (row.TokenType == JsonTokenType.String)
                {
                    end++;
                }

                return _utf8Json.Slice(start, end - start);
            }

            int endElementIdx = GetEndIndex(valueIndex, includeEndElement: false);
            row = _parsedData.Get(endElementIdx);
            end = row.Location + row.SizeOrLength;
            return _utf8Json.Slice(start, end - start);
        }

        /// <summary>
        ///   This is an implementation detail and MUST NOT be called by source-package consumers.
        /// </summary>
        internal string GetString(int index, JsonTokenType expectedType)
        {
            CheckNotDisposed();

            (int lastIdx, string lastString) = _lastIndexAndString;

            if (lastIdx == index)
            {
                return lastString;
            }

            DbRow row = _parsedData.Get(index);

            JsonTokenType tokenType = row.TokenType;

            if (tokenType == JsonTokenType.Null)
            {
                return null;
            }

            CheckExpectedType(expectedType, tokenType);

            ReadOnlySpan<byte> data = _utf8Json.Span;
            ReadOnlySpan<byte> segment = data.Slice(row.Location, row.SizeOrLength);

            if (row.HasComplexChildren)
            {
                int backslash = segment.IndexOf(JsonConstants.BackSlash);
                lastString = JsonReaderHelper.GetUnescapedString(segment, backslash);
            }
            else
            {
                lastString = JsonReaderHelper.TranscodeHelper(segment);
            }

            _lastIndexAndString = (index, lastString);
            return lastString;
        }

        /// <summary>
        ///   This is an implementation detail and MUST NOT be called by source-package consumers.
        /// </summary>
        internal string GetNameOfPropertyValue(int index)
        {
            // The property name is one row before the property value
            return GetString(index - DbRow.Size, JsonTokenType.PropertyName);
        }

        /// <summary>
        ///   This is an implementation detail and MUST NOT be called by source-package consumers.
        /// </summary>
        internal bool TryGetValue(int index, out int value)
        {
            CheckNotDisposed();

            DbRow row = _parsedData.Get(index);

            CheckExpectedType(JsonTokenType.Number, row.TokenType);

            ReadOnlySpan<byte> data = _utf8Json.Span;
            ReadOnlySpan<byte> segment = data.Slice(row.Location, row.SizeOrLength);

            if (Utf8Parser.TryParse(segment, out int tmp, out int consumed) &&
                consumed == segment.Length)
            {
                value = tmp;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        ///   This is an implementation detail and MUST NOT be called by source-package consumers.
        /// </summary>
        internal bool TryGetValue(int index, out uint value)
        {
            CheckNotDisposed();

            DbRow row = _parsedData.Get(index);

            CheckExpectedType(JsonTokenType.Number, row.TokenType);

            ReadOnlySpan<byte> data = _utf8Json.Span;
            ReadOnlySpan<byte> segment = data.Slice(row.Location, row.SizeOrLength);

            if (Utf8Parser.TryParse(segment, out uint tmp, out int consumed) &&
                consumed == segment.Length)
            {
                value = tmp;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        ///   This is an implementation detail and MUST NOT be called by source-package consumers.
        /// </summary>
        internal bool TryGetValue(int index, out long value)
        {
            CheckNotDisposed();

            DbRow row = _parsedData.Get(index);

            CheckExpectedType(JsonTokenType.Number, row.TokenType);

            ReadOnlySpan<byte> data = _utf8Json.Span;
            ReadOnlySpan<byte> segment = data.Slice(row.Location, row.SizeOrLength);

            if (Utf8Parser.TryParse(segment, out long tmp, out int consumed) &&
                consumed == segment.Length)
            {
                value = tmp;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        ///   This is an implementation detail and MUST NOT be called by source-package consumers.
        /// </summary>
        internal bool TryGetValue(int index, out ulong value)
        {
            CheckNotDisposed();

            DbRow row = _parsedData.Get(index);

            CheckExpectedType(JsonTokenType.Number, row.TokenType);

            ReadOnlySpan<byte> data = _utf8Json.Span;
            ReadOnlySpan<byte> segment = data.Slice(row.Location, row.SizeOrLength);

            if (Utf8Parser.TryParse(segment, out ulong tmp, out int consumed) &&
                consumed == segment.Length)
            {
                value = tmp;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        ///   This is an implementation detail and MUST NOT be called by source-package consumers.
        /// </summary>
        internal bool TryGetValue(int index, out double value)
        {
            CheckNotDisposed();

            DbRow row = _parsedData.Get(index);

            CheckExpectedType(JsonTokenType.Number, row.TokenType);

            ReadOnlySpan<byte> data = _utf8Json.Span;
            ReadOnlySpan<byte> segment = data.Slice(row.Location, row.SizeOrLength);

            char standardFormat = row.HasComplexChildren ? JsonConstants.ScientificNotationFormat : default;

            if (Utf8Parser.TryParse(segment, out double tmp, out int bytesConsumed, standardFormat) &&
                segment.Length == bytesConsumed)
            {
                value = tmp;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        ///   This is an implementation detail and MUST NOT be called by source-package consumers.
        /// </summary>
        internal bool TryGetValue(int index, out float value)
        {
            CheckNotDisposed();

            DbRow row = _parsedData.Get(index);

            CheckExpectedType(JsonTokenType.Number, row.TokenType);

            ReadOnlySpan<byte> data = _utf8Json.Span;
            ReadOnlySpan<byte> segment = data.Slice(row.Location, row.SizeOrLength);

            char standardFormat = row.HasComplexChildren ? JsonConstants.ScientificNotationFormat : default;

            if (Utf8Parser.TryParse(segment, out float tmp, out int bytesConsumed, standardFormat) &&
                segment.Length == bytesConsumed)
            {
                value = tmp;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        ///   This is an implementation detail and MUST NOT be called by source-package consumers.
        /// </summary>
        internal bool TryGetValue(int index, out decimal value)
        {
            CheckNotDisposed();

            DbRow row = _parsedData.Get(index);

            CheckExpectedType(JsonTokenType.Number, row.TokenType);

            ReadOnlySpan<byte> data = _utf8Json.Span;
            ReadOnlySpan<byte> segment = data.Slice(row.Location, row.SizeOrLength);

            char standardFormat = row.HasComplexChildren ? JsonConstants.ScientificNotationFormat : default;

            if (Utf8Parser.TryParse(segment, out decimal tmp, out int bytesConsumed, standardFormat) &&
                segment.Length == bytesConsumed)
            {
                value = tmp;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        ///   This is an implementation detail and MUST NOT be called by source-package consumers.
        /// </summary>
        internal bool TryGetValue(int index, out DateTime value)
        {
            CheckNotDisposed();

            DbRow row = _parsedData.Get(index);

            CheckExpectedType(JsonTokenType.String, row.TokenType);

            ReadOnlySpan<byte> data = _utf8Json.Span;
            ReadOnlySpan<byte> segment = data.Slice(row.Location, row.SizeOrLength);

            if (JsonHelpers.TryParseAsISO(segment, out DateTime tmp, out int bytesConsumed) &&
                segment.Length == bytesConsumed)
            {
                value = tmp;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        ///   This is an implementation detail and MUST NOT be called by source-package consumers.
        /// </summary>
        internal bool TryGetValue(int index, out DateTimeOffset value)
        {
            CheckNotDisposed();

            DbRow row = _parsedData.Get(index);

            CheckExpectedType(JsonTokenType.String, row.TokenType);

            ReadOnlySpan<byte> data = _utf8Json.Span;
            ReadOnlySpan<byte> segment = data.Slice(row.Location, row.SizeOrLength);

            if (JsonHelpers.TryParseAsISO(segment, out DateTimeOffset tmp, out int bytesConsumed) &&
                segment.Length == bytesConsumed)
            {
                value = tmp;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        ///   This is an implementation detail and MUST NOT be called by source-package consumers.
        /// </summary>
        internal string GetRawValueAsString(int index)
        {
            ReadOnlyMemory<byte> segment = GetRawValue(index, includeQuotes: true);
            return JsonReaderHelper.TranscodeHelper(segment.Span);
        }

        /// <summary>
        ///   This is an implementation detail and MUST NOT be called by source-package consumers.
        /// </summary>
        internal string GetPropertyRawValueAsString(int valueIndex)
        {
            ReadOnlyMemory<byte> segment = GetPropertyRawValue(valueIndex);
            return JsonReaderHelper.TranscodeHelper(segment.Span);
        }

        /// <summary>
        ///   This is an implementation detail and MUST NOT be called by source-package consumers.
        /// </summary>
        internal JsonElement DetachElement(int index)
        {
            int endIndex = GetEndIndex(index, true);
            MetadataDb newDb = _parsedData.CopySegment(index, endIndex);
            ReadOnlyMemory<byte> segmentCopy = GetRawValue(index, includeQuotes: true).ToArray();

            JsonDocument newDocument =
                new JsonDocument(segmentCopy, newDb, extraRentedBytes: null, isDisposable: false);

            return newDocument.RootElement;
        }

        private static void Parse(
            ReadOnlySpan<byte> utf8JsonSpan,
            Utf8JsonReader reader,
            ref MetadataDb database,
            ref StackRowStack stack)
        {
            bool inArray = false;
            int arrayItemsCount = 0;
            int numberOfRowsForMembers = 0;
            int numberOfRowsForValues = 0;

            ref byte jsonStart = ref MemoryMarshal.GetReference(utf8JsonSpan);

            while (reader.Read())
            {
                JsonTokenType tokenType = reader.TokenType;

                int tokenStart = Unsafe.ByteOffset(
                    ref jsonStart,
                    ref MemoryMarshal.GetReference(reader.ValueSpan)).ToInt32();

                if (tokenType == JsonTokenType.StartObject)
                {
                    if (inArray)
                    {
                        arrayItemsCount++;
                    }

                    numberOfRowsForValues++;
                    database.Append(tokenType, tokenStart, DbRow.UnknownSize);
                    var row = new StackRow(numberOfRowsForMembers + 1);
                    stack.Push(row);
                    numberOfRowsForMembers = 0;
                }
                else if (tokenType == JsonTokenType.EndObject)
                {
                    int rowIndex = database.FindIndexOfFirstUnsetSizeOrLength(JsonTokenType.StartObject);

                    numberOfRowsForValues++;
                    numberOfRowsForMembers++;
                    database.SetLength(rowIndex, numberOfRowsForMembers);

                    int newRowIndex = database.Length;
                    database.Append(tokenType, tokenStart, reader.ValueSpan.Length);
                    database.SetNumberOfRows(rowIndex, numberOfRowsForMembers);
                    database.SetNumberOfRows(newRowIndex, numberOfRowsForMembers);

                    StackRow row = stack.Pop();
                    numberOfRowsForMembers += row.SizeOrLength;
                }
                else if (tokenType == JsonTokenType.StartArray)
                {
                    if (inArray)
                    {
                        arrayItemsCount++;
                    }

                    numberOfRowsForMembers++;
                    database.Append(tokenType, tokenStart, DbRow.UnknownSize);
                    var row = new StackRow(arrayItemsCount, numberOfRowsForValues + 1);
                    stack.Push(row);
                    arrayItemsCount = 0;
                    numberOfRowsForValues = 0;
                }
                else if (tokenType == JsonTokenType.EndArray)
                {
                    int rowIndex = database.FindIndexOfFirstUnsetSizeOrLength(JsonTokenType.StartArray);

                    numberOfRowsForValues++;
                    numberOfRowsForMembers++;
                    database.SetLength(rowIndex, arrayItemsCount);
                    database.SetNumberOfRows(rowIndex, numberOfRowsForValues);

                    // If the array item count is (e.g.) 12 and the number of rows is (e.g.) 13
                    // then the extra row is just this EndArray item, so the array was made up
                    // of simple values.
                    //
                    // If the off-by-one relationship does not hold, then one of the values was
                    // more than one row, making it a complex object.
                    //
                    // This check is similar to tracking the start array and painting it when
                    // StartObject or StartArray is encountered, but avoids the mixed state
                    // where "UnknownSize" implies "has complex children".
                    if (arrayItemsCount + 1 != numberOfRowsForValues)
                    {
                        database.SetHasComplexChildren(rowIndex);
                    }

                    int newRowIndex = database.Length;
                    database.Append(tokenType, tokenStart, reader.ValueSpan.Length);
                    database.SetNumberOfRows(newRowIndex, numberOfRowsForValues);

                    StackRow row = stack.Pop();
                    arrayItemsCount = row.SizeOrLength;
                    numberOfRowsForValues += row.NumberOfRows;
                }
                else if (tokenType == JsonTokenType.PropertyName)
                {
                    numberOfRowsForValues++;
                    numberOfRowsForMembers++;
                    database.Append(tokenType, tokenStart, reader.ValueSpan.Length);

                    if (reader._stringHasEscaping)
                    {
                        database.SetHasComplexChildren(database.Length - DbRow.Size);
                    }

                    Debug.Assert(!inArray);
                }
                else
                {
                    Debug.Assert(tokenType >= JsonTokenType.String && tokenType <= JsonTokenType.Null);
                    numberOfRowsForValues++;
                    numberOfRowsForMembers++;
                    database.Append(tokenType, tokenStart, reader.ValueSpan.Length);

                    if (inArray)
                    {
                        arrayItemsCount++;
                    }

                    if (tokenType == JsonTokenType.Number)
                    {
                        switch (reader._numberFormat)
                        {
                            case JsonConstants.ScientificNotationFormat:
                                database.SetHasComplexChildren(database.Length - DbRow.Size);
                                break;
                            default:
                                Debug.Assert(
                                    reader._numberFormat == default,
                                    $"Unhandled numeric format {reader._numberFormat}");
                                break;
                        }
                    }
                    else if (tokenType == JsonTokenType.String)
                    {
                        if (reader._stringHasEscaping)
                        {
                            database.SetHasComplexChildren(database.Length - DbRow.Size);
                        }
                    }
                }

                inArray = reader.IsInArray;
            }

            Debug.Assert(reader.BytesConsumed == utf8JsonSpan.Length);
            database.TrimExcess();
        }

        private void CheckNotDisposed()
        {
            if (_utf8Json.IsEmpty)
            {
                throw new ObjectDisposedException(nameof(JsonDocument));
            }
        }

        private void CheckExpectedType(JsonTokenType expected, JsonTokenType actual)
        {
            if (expected != actual)
            {
                throw ThrowHelper.GetJsonElementWrongTypeException(expected, actual);
            }
        }

        private static void CheckSupportedOptions(JsonReaderOptions readerOptions)
        {
            if (readerOptions.CommentHandling == JsonCommentHandling.Allow)
            {
                throw new ArgumentException(
                    SR.JsonDocumentDoesNotSupportComments,
                    nameof(readerOptions));
            }
        }
    }
}

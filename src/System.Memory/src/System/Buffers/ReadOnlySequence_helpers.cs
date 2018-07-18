// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Internal.Runtime.CompilerServices;

namespace System.Buffers
{
    public readonly partial struct ReadOnlySequence<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool TryGetBuffer(in SequencePosition position, out ReadOnlyMemory<T> memory, out SequencePosition next)
        {
            object positionObject = position.GetObject();
            next = default;

            if (positionObject == null)
            {
                memory = default;
                return false;
            }

            SequenceType type = GetSequenceType();
            object endObject = _sequenceEnd.GetObject();
            int startIndex = GetIndex(position);
            int endIndex = GetIndex(_sequenceEnd);

            if (type == SequenceType.MultiSegment)
            {
                Debug.Assert(positionObject is ReadOnlySequenceSegment<T>);

                ReadOnlySequenceSegment<T> startSegment = (ReadOnlySequenceSegment<T>)positionObject;

                if (startSegment != endObject)
                {
                    ReadOnlySequenceSegment<T> nextSegment = startSegment.Next;

                    if (nextSegment == null)
                        ThrowHelper.ThrowInvalidOperationException_EndPositionNotReached();

                    next = new SequencePosition(nextSegment, 0);
                    memory = startSegment.Memory.Slice(startIndex);
                }
                else
                {
                    memory = startSegment.Memory.Slice(startIndex, endIndex - startIndex);
                }
            }
            else
            {
                if (positionObject != endObject)
                    ThrowHelper.ThrowInvalidOperationException_EndPositionNotReached();

                if (type == SequenceType.Array)
                {
                    Debug.Assert(positionObject is T[]);

                    memory = new ReadOnlyMemory<T>((T[])positionObject, startIndex, endIndex - startIndex);
                }
                else if (typeof(T) == typeof(char) && type == SequenceType.String)
                {
                    Debug.Assert(positionObject is string);

                    memory = (ReadOnlyMemory<T>)(object)((string)positionObject).AsMemory(startIndex, endIndex - startIndex);
                }
                else // type == SequenceType.MemoryManager
                {
                    Debug.Assert(type == SequenceType.MemoryManager);
                    Debug.Assert(positionObject is MemoryManager<T>);

                    memory = ((MemoryManager<T>)positionObject).Memory.Slice(startIndex, endIndex - startIndex);
                }
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ReadOnlyMemory<T> GetFirstBuffer()
        {
            object startObject = _sequenceStart.GetObject();

            if (startObject == null)
                return default;

            int startIndex = _sequenceStart.GetInteger();
            int endIndex = _sequenceEnd.GetInteger();

            bool isMultiSegment = startObject != _sequenceEnd.GetObject();

            // The highest bit of startIndex and endIndex are used to infer the sequence type
            // The code below is structured this way for performance reasons and is equivalent to the following:
            // SequenceType type = GetSequenceType();
            // if (type == SequenceType.MultiSegment) { ... }
            // else if (type == SequenceType.Array) { ... }
            // else if (type == SequenceType.String){ ... }
            // else if (type == SequenceType.MemoryManager) { ... }

            // Highest bit of startIndex: A = startIndex >> 31
            // Highest bit of endIndex: B = endIndex >> 31

            if (startIndex >= 0)
            {
                // A == 0 && B == 0 means SequenceType.MultiSegment
                // A == 0 && B == 1 means SequenceType.Array

                if (endIndex >= 0)  // SequenceType.MultiSegment
                {
                    ReadOnlyMemory<T> memory = ((ReadOnlySequenceSegment<T>)startObject).Memory;
                    if (isMultiSegment)
                    {
                        return memory.Slice(startIndex);
                    }
                    return memory.Slice(startIndex, endIndex - startIndex);
                }
                else // endIndex < 0, SequenceType.Array
                {
                    if (isMultiSegment)
                        ThrowHelper.ThrowInvalidOperationException_EndPositionNotReached();

                    return new ReadOnlyMemory<T>((T[])startObject, startIndex, (endIndex & ReadOnlySequence.IndexBitMask) - startIndex);
                }
            }
            else // startIndex < 0
            {
                if (isMultiSegment)
                    ThrowHelper.ThrowInvalidOperationException_EndPositionNotReached();

                // A == 1 && B == 1 means SequenceType.String
                // A == 1 && B == 0 means SequenceType.MemoryManager

                // The type == char check here is redundant. However, we still have it to allow
                // the JIT to see when that the code is unreachable and eliminate it.
                if (typeof(T) == typeof(char) && endIndex < 0)  // SequenceType.String
                {
                    // No need to remove the FlagBitMask since (endIndex - startIndex) == (endIndex & ReadOnlySequence.IndexBitMask) - (startIndex & ReadOnlySequence.IndexBitMask)
                    return (ReadOnlyMemory<T>)(object)((string)startObject).AsMemory((startIndex & ReadOnlySequence.IndexBitMask), endIndex - startIndex);
                }
                else // endIndex >= 0, SequenceType.MemoryManager
                {
                    startIndex &= ReadOnlySequence.IndexBitMask;
                    return ((MemoryManager<T>)startObject).Memory.Slice(startIndex, endIndex - startIndex);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SequencePosition Seek(in SequencePosition start, in SequencePosition end, long offset, ExceptionArgument argument)
        {
            int startIndex = GetIndex(start);
            int endIndex = GetIndex(end);

            object startObject = start.GetObject();
            object endObject = end.GetObject();

            if (startObject != endObject)
            {
                Debug.Assert(startObject != null);
                var startSegment = (ReadOnlySequenceSegment<T>)startObject;

                int currentLength = startSegment.Memory.Length - startIndex;

                // Position in start segment, defer to single segment seek
                if (currentLength > offset)
                    goto IsSingleSegment;

                if (currentLength < 0)
                    ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();

                // End of segment. Move to start of next.
                return SeekMultiSegment(startSegment.Next, endObject, endIndex, offset - currentLength, argument);
            }

            Debug.Assert(startObject == endObject);

            if (endIndex - startIndex < offset)
                ThrowHelper.ThrowArgumentOutOfRangeException(argument);

        // Single segment Seek
        IsSingleSegment:
            return new SequencePosition(startObject, startIndex + (int)offset);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static SequencePosition SeekMultiSegment(ReadOnlySequenceSegment<T> currentSegment, object endObject, int endIndex, long offset, ExceptionArgument argument)
        {
            Debug.Assert(currentSegment != null);
            Debug.Assert(offset >= 0);

            while (currentSegment != null && currentSegment != endObject)
            {
                int memoryLength = currentSegment.Memory.Length;

                // Fully contained in this segment
                if (memoryLength > offset)
                    goto FoundSegment;

                // Move to next
                offset -= memoryLength;
                currentSegment = currentSegment.Next;
            }

            // Hit the end of the segments but didn't reach the count
            if (currentSegment == null || endIndex < offset)
                ThrowHelper.ThrowArgumentOutOfRangeException(argument);

        FoundSegment:
            return new SequencePosition(currentSegment, (int)offset);
        }

        private void BoundsCheck(in SequencePosition position)
        {
            uint sliceStartIndex = (uint)GetIndex(position);
            uint startIndex = (uint)GetIndex(_sequenceStart);
            uint endIndex = (uint)GetIndex(_sequenceEnd);

            object startObject = _sequenceStart.GetObject();
            object endObject = _sequenceEnd.GetObject();

            // Single-Segment Sequence
            if (startObject == endObject)
            {
                if (!InRange(sliceStartIndex, startIndex, endIndex))
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
                }
            }
            else
            {
                // Multi-Segment Sequence
                // Storing this in a local since it is used twice within InRange()
                ulong startRange = (ulong)(((ReadOnlySequenceSegment<T>)startObject).RunningIndex + startIndex);
                if (!InRange(
                    (ulong)(((ReadOnlySequenceSegment<T>)position.GetObject()).RunningIndex + sliceStartIndex),
                    startRange,
                    (ulong)(((ReadOnlySequenceSegment<T>)endObject).RunningIndex + endIndex)))
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
                }
            }
        }

        private void BoundsCheck(uint sliceStartIndex, object sliceStartObject, uint sliceEndIndex, object sliceEndObject)
        {
            uint startIndex = (uint)GetIndex(_sequenceStart);
            uint endIndex = (uint)GetIndex(_sequenceEnd);

            object startObject = _sequenceStart.GetObject();
            object endObject = _sequenceEnd.GetObject();

            // Single-Segment Sequence
            if (startObject == endObject)
            {
                if (sliceStartObject != sliceEndObject ||
                    sliceStartObject != startObject ||
                    sliceStartIndex > sliceEndIndex ||
                    sliceStartIndex < startIndex ||
                    sliceEndIndex > endIndex)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
                }
            }
            else
            {
                // Multi-Segment Sequence
                // This optimization works because we know sliceStartIndex, sliceEndIndex, startIndex, and endIndex are all >= 0
                Debug.Assert(sliceStartIndex >= 0 && startIndex >= 0 && endIndex >= 0);

                ulong sliceStartRange = (ulong)(((ReadOnlySequenceSegment<T>)sliceStartObject).RunningIndex + sliceStartIndex);
                ulong sliceEndRange = (ulong)(((ReadOnlySequenceSegment<T>)sliceEndObject).RunningIndex + sliceEndIndex);

                if (sliceStartRange > sliceEndRange)
                    ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();

                if (sliceStartRange < (ulong)(((ReadOnlySequenceSegment<T>)startObject).RunningIndex + startIndex)
                    || sliceEndRange > (ulong)(((ReadOnlySequenceSegment<T>)endObject).RunningIndex + endIndex))
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
                }
            }
        }

        private static SequencePosition GetEndPosition(ReadOnlySequenceSegment<T> startSegment, object startObject, int startIndex, object endObject, int endIndex, long length)
        {
            int currentLength = startSegment.Memory.Length - startIndex;

            if (currentLength > length)
            {
                return new SequencePosition(startObject, startIndex + (int)length);
            }

            if (currentLength < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();

            return SeekMultiSegment(startSegment.Next, endObject, endIndex, length - currentLength, ExceptionArgument.length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SequenceType GetSequenceType()
        {
            // We take high order bits of two indexes and move them
            // to a first and second position to convert to SequenceType

            // if (start < 0  and end < 0)
            // start >> 31 = -1, end >> 31 = -1
            // 2 * (-1) + (-1) = -3, result = (SequenceType)3

            // if (start < 0  and end >= 0)
            // start >> 31 = -1, end >> 31 = 0
            // 2 * (-1) + 0 = -2, result = (SequenceType)2

            // if (start >= 0  and end >= 0)
            // start >> 31 = 0, end >> 31 = 0
            // 2 * 0 + 0 = 0, result = (SequenceType)0

            // if (start >= 0  and end < 0)
            // start >> 31 = 0, end >> 31 = -1
            // 2 * 0 + (-1) = -1, result = (SequenceType)1

            return (SequenceType)(-(2 * (_sequenceStart.GetInteger() >> 31) + (_sequenceEnd.GetInteger() >> 31)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetIndex(in SequencePosition position) => position.GetInteger() & ReadOnlySequence.IndexBitMask;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ReadOnlySequence<T> SliceImpl(in SequencePosition start, in SequencePosition end)
        {
            // In this method we reset high order bits from indices
            // of positions that were passed in
            // and apply type bits specific for current ReadOnlySequence type

            return new ReadOnlySequence<T>(
                start.GetObject(),
                GetIndex(start) | (_sequenceStart.GetInteger() & ReadOnlySequence.FlagBitMask),
                end.GetObject(),
                GetIndex(end) | (_sequenceEnd.GetInteger() & ReadOnlySequence.FlagBitMask)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private long GetLength()
        {
            int startIndex = GetIndex(_sequenceStart);
            int endIndex = GetIndex(_sequenceEnd);
            object startObject = _sequenceStart.GetObject();
            object endObject = _sequenceEnd.GetObject();

            if (startObject != endObject)
            {
                var startSegment = (ReadOnlySequenceSegment<T>)startObject;
                var endSegment = (ReadOnlySequenceSegment<T>)endObject;
                // (End offset) - (start offset)
                return (endSegment.RunningIndex + endIndex) - (startSegment.RunningIndex + startIndex);
            }

            // Single segment length
            return endIndex - startIndex;
        }

        internal bool TryGetReadOnlySequenceSegment(out ReadOnlySequenceSegment<T> startSegment, out int startIndex, out ReadOnlySequenceSegment<T> endSegment, out int endIndex)
        {
            object startObject = _sequenceStart.GetObject();

            // Default or not MultiSegment
            if (startObject == null || GetSequenceType() != SequenceType.MultiSegment)
            {
                startSegment = null;
                startIndex = 0;
                endSegment = null;
                endIndex = 0;
                return false;
            }
            
            Debug.Assert(_sequenceEnd.GetObject() != null);

            startSegment = (ReadOnlySequenceSegment<T>)startObject;
            startIndex = GetIndex(_sequenceStart);
            endSegment = (ReadOnlySequenceSegment<T>)_sequenceEnd.GetObject();
            endIndex = GetIndex(_sequenceEnd);
            return true;
        }

        internal bool TryGetArray(out ArraySegment<T> segment)
        {
            if (GetSequenceType() != SequenceType.Array)
            {
                segment = default;
                return false;
            }

            Debug.Assert(_sequenceStart.GetObject() != null);

            int startIndex = GetIndex(_sequenceStart);
            segment = new ArraySegment<T>((T[])_sequenceStart.GetObject(), startIndex, GetIndex(_sequenceEnd) - startIndex);
            return true;
        }

        internal bool TryGetString(out string text, out int start, out int length)
        {
            if (typeof(T) != typeof(char) || GetSequenceType() != SequenceType.String)
            {
                start = 0;
                length = 0;
                text = null;
                return false;
            }

            Debug.Assert(_sequenceStart.GetObject() != null);

            start = GetIndex(_sequenceStart);
            length = GetIndex(_sequenceEnd) - start;
            text = (string)_sequenceStart.GetObject();
            return true;
        }

        private static bool InRange(uint value, uint start, uint end)
        {
            // _sequenceStart and _sequenceEnd must be well-formed
            Debug.Assert(start <= int.MaxValue);
            Debug.Assert(end <= int.MaxValue);
            Debug.Assert(start <= end);

            // The case, value > int.MaxValue, is invalid, and hence it shouldn't be in the range.
            // If value > int.MaxValue, it is invariably greater than both 'start' and 'end'.
            // In that case, the experession simplifies to value <= end, which will return false.

            // The case, value < start, is invalid.
            // In that case, (value - start) would underflow becoming larger than int.MaxValue.
            // (end - start) can never underflow and hence must be within 0 and int.MaxValue. 
            // So, we will correctly return false.

            // The case, value > end, is invalid.
            // In that case, the expression simplifies to value <= end, which will return false.
            // This is because end > start & value > end implies value > start as well.

            // In all other cases, value is valid, and we return true.

            // Equivalent to: return (start <= value && value <= start)
            return (value - start) <= (end - start);
        }

        private static bool InRange(ulong value, ulong start, ulong end)
        {
            // _sequenceStart and _sequenceEnd must be well-formed
            Debug.Assert(start <= long.MaxValue);
            Debug.Assert(end <= long.MaxValue);
            Debug.Assert(start <= end);

            // The case, value > long.MaxValue, is invalid, and hence it shouldn't be in the range.
            // If value > long.MaxValue, it is invariably greater than both 'start' and 'end'.
            // In that case, the experession simplifies to value <= end, which will return false.

            // The case, value < start, is invalid.
            // In that case, (value - start) would underflow becoming larger than long.MaxValue.
            // (end - start) can never underflow and hence must be within 0 and long.MaxValue. 
            // So, we will correctly return false.

            // The case, value > end, is invalid.
            // In that case, the expression simplifies to value <= end, which will return false.
            // This is because end > start & value > end implies value > start as well.

            // In all other cases, value is valid, and we return true.

            // Equivalent to: return (start <= value && value <= start)
            return (value - start) <= (end - start);
        }
    }
}

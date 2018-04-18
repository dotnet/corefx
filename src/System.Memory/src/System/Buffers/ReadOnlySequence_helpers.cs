// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

#if !netstandard
using Internal.Runtime.CompilerServices;
#endif

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

            if (startIndex >= 0)
            {
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
        internal SequencePosition Seek(in SequencePosition start, in SequencePosition end, int offset) => Seek(start, end, (long)offset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal SequencePosition Seek(in SequencePosition start, in SequencePosition end, long offset)
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

                // End of segment. Move to start of next.
                return SeekMultiSegment(startSegment.Next, endObject, endIndex, offset - currentLength);
            }

            Debug.Assert(startObject == endObject);

            if (endIndex - startIndex < offset)
                ThrowHelper.ThrowArgumentOutOfRangeException_OffsetOutOfRange();

        // Single segment Seek
        IsSingleSegment:
            return new SequencePosition(startObject, startIndex + (int)offset);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static SequencePosition SeekMultiSegment(ReadOnlySequenceSegment<T> currentSegment, object endObject, int endIndex, long offset)
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
                ThrowHelper.ThrowArgumentOutOfRangeException_OffsetOutOfRange();

        FoundSegment:
            return new SequencePosition(currentSegment, (int)offset);
        }

        private void BoundsCheck(uint sliceStartIndex, object sliceStartObject)
        {
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
                    (ulong)(((ReadOnlySequenceSegment<T>)sliceStartObject).RunningIndex + sliceStartIndex),
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

                if (sliceStartRange > sliceEndRange ||
                    sliceStartRange < (ulong)(((ReadOnlySequenceSegment<T>)startObject).RunningIndex + startIndex) ||
                    sliceEndRange > (ulong)(((ReadOnlySequenceSegment<T>)endObject).RunningIndex + endIndex))
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
                }
            }
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
            GetTypeAndIndices(Start.GetInteger(), End.GetInteger(), out SequenceType type, out startIndex, out endIndex);

            if (type != SequenceType.MultiSegment)
            {
                startSegment = null;
                endSegment = null;
                return false;
            }

            Debug.Assert(Start.GetObject() != null);
            Debug.Assert(Start.GetObject() is ReadOnlySequenceSegment<T>);
            Debug.Assert(End.GetObject() != null);
            Debug.Assert(End.GetObject() is ReadOnlySequenceSegment<T>);

            startSegment = Unsafe.As<ReadOnlySequenceSegment<T>>(Start.GetObject());
            endSegment = Unsafe.As<ReadOnlySequenceSegment<T>>(End.GetObject());
            return true;
        }

        internal bool TryGetArray(out ArraySegment<T> segment)
        {
            GetTypeAndIndices(Start.GetInteger(), End.GetInteger(), out SequenceType type, out int startIndex, out int endIndex);

            if (type != SequenceType.Array)
            {
                segment = default;
                return false;
            }

            Debug.Assert(Start.GetObject() != null);
            Debug.Assert(Start.GetObject() is T[]);

            segment = new ArraySegment<T>(Unsafe.As<T[]>(Start.GetObject()), startIndex, endIndex - startIndex);
            return true;
        }

        internal bool TryGetReadOnlyMemory(out ReadOnlyMemory<T> memory)
        {
            GetTypeAndIndices(Start.GetInteger(), End.GetInteger(), out SequenceType type, out int startIndex, out int endIndex);

            object startObject = Start.GetObject();
            object endObject = End.GetObject();

            Debug.Assert(startObject != null);
            Debug.Assert(endObject != null);

            int length = endIndex - startIndex;
            if (startObject != endObject)
            {
                // Can't get ReadOnlyMemory from multi-block segments
                memory = default;
                return false;
            }
            else if (type == SequenceType.Array)
            {
                Debug.Assert(startObject is T[]);

                memory = new ReadOnlyMemory<T>(Unsafe.As<T[]>(startObject));
            }
            else if (type == SequenceType.MemoryManager)
            {
                Debug.Assert(startObject is MemoryManager<T>);

                memory = Unsafe.As<MemoryManager<T>>(startObject).Memory;
            }
            else if (typeof(T) == typeof(char) && type == SequenceType.String)
            {
                Debug.Assert(startObject is string);

                var text = Unsafe.As<string>(startObject);
                memory = (ReadOnlyMemory<T>)(object)text.AsMemory();
            }
            else // ReadOnlySequenceSegment
            {
                Debug.Assert(startObject is ReadOnlySequenceSegment<T>);

                memory = Unsafe.As<ReadOnlySequenceSegment<T>>(startObject).Memory;
            }

            memory = memory.Slice(startIndex, length);
            return true;
        }

        internal bool TryGetString(out string text, out int start, out int length)
        {
            if (typeof(T) != typeof(char))
            {
                start = 0;
                length = 0;
                text = null;
                return false;
            }

            GetTypeAndIndices(Start.GetInteger(), End.GetInteger(), out SequenceType type, out int startIndex, out int endIndex);

            if (type != SequenceType.String)
            {
                start = 0;
                length = 0;
                text = null;
                return false;
            }

            start = startIndex;
            length = endIndex - startIndex;
            text = (string)Start.GetObject();

            return true;
        }
    }
}

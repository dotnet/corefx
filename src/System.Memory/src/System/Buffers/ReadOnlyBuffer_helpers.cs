// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System.Buffers
{
    public readonly partial struct ReadOnlyBuffer<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool TryGetBuffer(SequencePosition start, SequencePosition end, out ReadOnlyMemory<T> data, out SequencePosition next)
        {
            if (start.Segment == null)
            {
                data = default;
                next = default;
                return false;
            }

            var startIndex = start.Index;
            var endIndex = end.Index;
            var type = GetBufferType();

            startIndex = GetIndex(startIndex);
            endIndex = GetIndex(endIndex);

            switch (type)
            {
                case BufferType.MemoryList:
                    var bufferSegment = (IMemoryList<T>) start.Segment;
                    var currentEndIndex = bufferSegment.Memory.Length;

                    if (bufferSegment == end.Segment)
                    {
                        currentEndIndex = endIndex;
                        next = default;
                    }
                    else
                    {
                        var nextSegment = bufferSegment.Next;
                        if (nextSegment == null)
                        {
                            if (end.Segment != null)
                            {
                                ThrowHelper.ThrowInvalidOperationException_EndPositionNotReached();
                            }

                            next = default;
                        }
                        else
                        {
                            next = new SequencePosition(nextSegment, 0);
                        }
                    }

                    data = bufferSegment.Memory.Slice(startIndex, currentEndIndex - startIndex);

                    return true;


                case BufferType.OwnedMemory:
                    var ownedMemory = (OwnedMemory<T>) start.Segment;
                    data = ownedMemory.Memory.Slice(startIndex, endIndex - startIndex);

                    if (ownedMemory != end.Segment)
                    {
                        ThrowHelper.ThrowInvalidOperationException_EndPositionNotReached();
                    }

                    next = default;
                    return true;

                case BufferType.Array:
                    var array = (T[]) start.Segment;
                    data = new Memory<T>(array, startIndex, endIndex - startIndex);

                    if (array != end.Segment)
                    {
                        ThrowHelper.ThrowInvalidOperationException_EndPositionNotReached();
                    }

                    next = default;
                    return true;
                default:
                    ThrowHelper.ThrowInvalidOperationException_UnexpectedSegmentType();
                    next = default;
                    data = default;
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal SequencePosition Seek(SequencePosition start, SequencePosition end, int count, bool checkEndReachable = true)
        {
            var startIndex = start.Index;
            var endIndex = end.Index;
            var type = GetBufferType();

            startIndex = GetIndex(startIndex);
            endIndex = GetIndex(endIndex);

            switch (type)
            {
                case BufferType.MemoryList:
                    if (start.Segment == end.Segment && endIndex - startIndex >= count)
                    {
                        return new SequencePosition(start.Segment, startIndex + count);
                    }
                    return SeekMultiSegment((IMemoryList<byte>) start.Segment, startIndex, (IMemoryList<byte>) end.Segment, endIndex, count, checkEndReachable);


                case BufferType.OwnedMemory:
                case BufferType.Array:
                    if (start.Segment != end.Segment)
                    {
                        ThrowHelper.ThrowInvalidOperationException_EndPositionNotReached();
                    }

                    if (endIndex - startIndex >= count)
                    {
                        return new SequencePosition(start.Segment, startIndex + count);
                    }

                    ThrowHelper.ThrowArgumentOutOfRangeException_CountOutOfRange();
                    return default;

                default:
                    ThrowHelper.ThrowInvalidOperationException_UnexpectedSegmentType();
                    return default;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal SequencePosition Seek(SequencePosition start, SequencePosition end, long count, bool checkEndReachable = true)
        {
            var startIndex = start.Index;
            var endIndex = end.Index;
            var type = GetBufferType();

            startIndex = GetIndex(startIndex);
            endIndex = GetIndex(endIndex);

            switch (type)
            {
                case BufferType.MemoryList:
                    if (start.Segment == end.Segment && endIndex - startIndex >= count)
                    {
                        // end.Index >= bytes + Index and end.Index is int
                        return new SequencePosition(start.Segment, startIndex + (int)count);
                    }
                    return SeekMultiSegment((IMemoryList<byte>) start.Segment, startIndex, (IMemoryList<byte>) end.Segment, endIndex, count, checkEndReachable);


                case BufferType.OwnedMemory:
                case BufferType.Array:
                    if (endIndex - startIndex >= count)
                    {
                        // end.Index >= bytes + Index and end.Index is int
                        return new SequencePosition(start.Segment, startIndex + (int)count);
                    }

                    ThrowHelper.ThrowArgumentOutOfRangeException_CountOutOfRange();
                    return default;

                default:
                    ThrowHelper.ThrowInvalidOperationException_UnexpectedSegmentType();
                    return default;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static SequencePosition SeekMultiSegment(IMemoryList<byte> start, int startIndex, IMemoryList<byte> end, int endPosition, long count, bool checkEndReachable)
        {
            SequencePosition result = default;
            var foundResult = false;
            var current = start;
            var currentIndex = startIndex;

            while (current != null)
            {
                // We need to loop up until the end to make sure start and end are connected
                // if end is not trusted
                if (!foundResult)
                {
                    var memory = current.Memory;
                    var currentEnd = current == end ? endPosition : memory.Length;

                    memory = memory.Slice(0, currentEnd - currentIndex);
                    // We would prefer to put position in the beginning of next segment
                    // then past the end of previous one, but only if we are not leaving current buffer
                    if (memory.Length > count ||
                       (memory.Length == count && current == end))
                    {
                        result = new SequencePosition(current, currentIndex + (int)count);
                        foundResult = true;
                        if (!checkEndReachable)
                        {
                            break;
                        }
                    }

                    count -= memory.Length;
                }

                if (current.Next == null && current != end)
                {
                    ThrowHelper.ThrowInvalidOperationException_EndPositionNotReached();
                }

                current = current.Next;
                currentIndex = 0;
            }

            if (!foundResult)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException_CountOutOfRange();
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private long GetLength(SequencePosition start, SequencePosition end)
        {
            var startIndex = start.Index;
            var endIndex = end.Index;
            var type = GetBufferType();

            startIndex = GetIndex(startIndex);
            endIndex = GetIndex(endIndex);

            switch (type)
            {
                case BufferType.MemoryList:
                    return GetLength((IMemoryList<T>) start.Segment, startIndex, (IMemoryList<T>)end.Segment, endIndex);

                case BufferType.OwnedMemory:
                case BufferType.Array:
                    return endIndex - startIndex;
                default:
                    ThrowHelper.ThrowInvalidOperationException_UnexpectedSegmentType();
                    return default;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long GetLength(
            IMemoryList<T> start,
            int startIndex,
            IMemoryList<T> endSegment,
            int endIndex)
        {
            if (start == endSegment)
            {
                return endIndex - startIndex;
            }

            return (endSegment.RunningIndex - start.Next.RunningIndex)
                   + (start.Memory.Length - startIndex)
                   + endIndex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BoundsCheck(SequencePosition start, SequencePosition position)
        {
            var startIndex = start.Index;
            var endIndex = position.Index;
            var type = GetBufferType();

            startIndex = GetIndex(startIndex);
            endIndex = GetIndex(endIndex);

            switch (type)
            {
                case BufferType.OwnedMemory:
                case BufferType.Array:
                    if (endIndex > startIndex)
                    {
                        ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
                    }
                    return;
                case BufferType.MemoryList:
                    var segment = (IMemoryList<T>)position.Segment;
                    var memoryList = (IMemoryList<T>) start.Segment;

                    if (segment.RunningIndex - startIndex > memoryList.RunningIndex - endIndex)
                    {
                        ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
                    }
                    return;
                default:
                    ThrowHelper.ThrowInvalidOperationException_UnexpectedSegmentType();
                    return;
            }
        }

        private class ReadOnlyBufferSegment : IMemoryList<T>
        {
            public Memory<T> Memory { get; set; }
            public IMemoryList<T> Next { get; set; }
            public long RunningIndex { get; set; }
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System.Buffers
{
    public readonly partial struct ReadOnlySequence<T>
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

            int startIndex = start.Index;
            int endIndex = end.Index;
            SequenceType type = GetSequenceType();

            startIndex = GetIndex(startIndex);
            endIndex = GetIndex(endIndex);

            switch (type)
            {
                case SequenceType.MemoryList:
                    var segment = (IMemoryList<T>)start.Segment;
                    Memory<T> bufferSegmentMemory = segment.Memory;
                    int currentEndIndex = bufferSegmentMemory.Length;

                    if (segment == end.Segment)
                    {
                        currentEndIndex = endIndex;
                        next = default;
                    }
                    else
                    {
                        IMemoryList<T> nextSegment = segment.Next;
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

                    data = bufferSegmentMemory.Slice(startIndex, currentEndIndex - startIndex);
                    return true;

                case SequenceType.OwnedMemory:
                    var ownedMemory = (OwnedMemory<T>)start.Segment;
                    if (ownedMemory != end.Segment)
                    {
                        ThrowHelper.ThrowInvalidOperationException_EndPositionNotReached();
                    }

                    data = ownedMemory.Memory.Slice(startIndex, endIndex - startIndex);
                    next = default;
                    return true;

                case SequenceType.Array:
                    var array = (T[])start.Segment;

                    if (array != end.Segment)
                    {
                        ThrowHelper.ThrowInvalidOperationException_EndPositionNotReached();
                    }

                    data = new Memory<T>(array, startIndex, endIndex - startIndex);
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
        internal SequencePosition Seek(SequencePosition start, SequencePosition end, int count)
        {
            int startIndex = start.Index;
            int endIndex = end.Index;
            SequenceType type = GetSequenceType();

            startIndex = GetIndex(startIndex);
            endIndex = GetIndex(endIndex);

            switch (type)
            {
                case SequenceType.MemoryList:
                    if (start.Segment == end.Segment && endIndex - startIndex >= count)
                    {
                        return new SequencePosition(start.Segment, startIndex + count);
                    }
                    return SeekMultiSegment((IMemoryList<byte>)start.Segment, startIndex, (IMemoryList<byte>)end.Segment, endIndex, count);

                case SequenceType.OwnedMemory:
                case SequenceType.Array:
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
        internal SequencePosition Seek(SequencePosition start, SequencePosition end, long count)
        {
            int startIndex = start.Index;
            int endIndex = end.Index;
            SequenceType type = GetSequenceType();

            startIndex = GetIndex(startIndex);
            endIndex = GetIndex(endIndex);

            switch (type)
            {
                case SequenceType.MemoryList:
                    if (start.Segment == end.Segment && endIndex - startIndex >= count)
                    {
                        // end.Index >= count + Index and end.Index is int
                        return new SequencePosition(start.Segment, startIndex + (int)count);
                    }
                    return SeekMultiSegment((IMemoryList<byte>)start.Segment, startIndex, (IMemoryList<byte>)end.Segment, endIndex, count);

                case SequenceType.OwnedMemory:
                case SequenceType.Array:
                    if (endIndex - startIndex >= count)
                    {
                        // end.Index >= count + Index and end.Index is int
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
        private static SequencePosition SeekMultiSegment(IMemoryList<byte> start, int startIndex, IMemoryList<byte> end, int endPosition, long count)
        {
            var memoryList = start.GetNext(startIndex + count, out int localOffset);
            if (memoryList == null || memoryList.GetLength(end) < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException_CountOutOfRange();
            }

            if (localOffset == memoryList.Memory.Length && memoryList != end)
            {
                memoryList = memoryList.Next;
                localOffset = 0;
            }
            return new SequencePosition(memoryList, localOffset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private long GetLength(SequencePosition start, SequencePosition end)
        {
            int startIndex = start.Index;
            int endIndex = end.Index;
            SequenceType type = GetSequenceType();

            startIndex = GetIndex(startIndex);
            endIndex = GetIndex(endIndex);

            switch (type)
            {
                case SequenceType.MemoryList:
                    return GetLength((IMemoryList<T>)start.Segment, startIndex, (IMemoryList<T>)end.Segment, endIndex);

                case SequenceType.OwnedMemory:
                case SequenceType.Array:
                    return endIndex - startIndex;

                default:
                    ThrowHelper.ThrowInvalidOperationException_UnexpectedSegmentType();
                    return default;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long GetLength(IMemoryList<T> start, int startIndex, IMemoryList<T> endSegment, int endIndex)
        {
            if (start == endSegment)
            {
                return endIndex - startIndex;
            }

            return start.Next.GetLength(endSegment) // Length of data in between first and last segment
                   + (start.Memory.Length - startIndex) // Length of data in first segment
                   + endIndex; // Length of data in last segment
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BoundsCheck(SequencePosition start, SequencePosition position)
        {
            int startIndex = start.Index;
            int endIndex = position.Index;
            SequenceType type = GetSequenceType();

            startIndex = GetIndex(startIndex);
            endIndex = GetIndex(endIndex);

            switch (type)
            {
                case SequenceType.OwnedMemory:
                case SequenceType.Array:
                    if (endIndex > startIndex)
                    {
                        ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
                    }
                    return;

                case SequenceType.MemoryList:
                    IMemoryList<T> segment = (IMemoryList<T>)position.Segment;
                    IMemoryList<T> memoryList = (IMemoryList<T>)start.Segment;

                    var segmentDistance = segment.GetLength(memoryList);
                    if (segmentDistance < 0 || (segmentDistance == 0 && startIndex < endIndex))
                    {
                        ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
                    }
                    return;

                default:
                    ThrowHelper.ThrowInvalidOperationException_UnexpectedSegmentType();
                    return;
            }
        }

        private sealed class ReadOnlySequenceSegment : IMemoryList<T>
        {
            public Memory<T> Memory { get; set; }
            public IMemoryList<T> Next { get; } = null;

            public IMemoryList<T> GetNext(long offset, out int localOffset)
            {
                if (offset < Memory.Length)
                {
                    localOffset = (int)offset;
                    return this;
                }

                localOffset = 0;
                return null;
            }

            public long GetLength(IMemoryList<T> memoryList)
            {
                if (memoryList != this)
                {
                    ThrowHelper.ThrowInvalidOperationException();
                }

                return 0;
            }
        }
    }
}

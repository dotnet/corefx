// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Buffers
{
    public readonly partial struct ReadOnlySequence<T>
    {
        internal bool TryGetMemoryList(out IMemoryList<T> startSegment, out int startIndex, out IMemoryList<T> endSegment, out int endIndex)
        {

            if (Start.Segment == null  || GetSequenceType() != SequenceType.MemoryList)
            {
                startSegment = null;
                endSegment = null;
                startIndex = 0;
                endIndex = 0;
                return false;
            }

            startIndex = GetIndex(Start.Index);
            endIndex = GetIndex(End.Index);
            startSegment = (IMemoryList<T>)Start.Segment;
            endSegment = (IMemoryList<T>)End.Segment;
            return true;
        }

        internal bool TryGetArray(out ArraySegment<T> array)
        {
            if (Start.Segment == null  || GetSequenceType() != SequenceType.Array)
            {
                array = default;
                return false;
            }

            int startIndex = GetIndex(Start.Index);
            array = new ArraySegment<T>((T[])Start.Segment, startIndex, GetIndex(End.Index) - startIndex);
            return true;
        }

        internal bool TryGetOwnedMemory(out OwnedMemory<T> ownedMemory, out int start, out int length)
        {
            if (Start.Segment == null  || GetSequenceType() != SequenceType.OwnedMemory)
            {
                ownedMemory = default;
                start = 0;
                length = 0;
                return false;
            }

            ownedMemory = (OwnedMemory<T>)Start.Segment;
            start = GetIndex(Start.Index);
            length = GetIndex(End.Index) - start;
            return true;
        }

        internal bool TryGetReadOnlyMemory(out ReadOnlyMemory<T> memory)
        {
            // Currently ReadOnlyMemory is stored inside single segment of IMemoryList
            if (TryGetMemoryList(out IMemoryList<T> startSegment, out int startIndex, out IMemoryList<T> endSegment, out int endIndex) &&
                startSegment == endSegment)
            {
                memory = startSegment.Memory.Slice(startIndex, endIndex - startIndex);
                return true;
            }

            memory = default;
            return false;
        }
    }
}

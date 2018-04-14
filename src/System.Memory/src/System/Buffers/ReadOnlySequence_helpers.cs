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
        internal ReadOnlyMemory<T> GetFirstBuffer(in SequencePosition start, in SequencePosition end)
        {
            SequenceType type;
            int startIndex = 0;
            int endIndex = 0;
            object startObject = start.GetObject();
            if (startObject != null)
            {
                GetTypeAndIndices(start.GetInteger(), end.GetInteger(), out type, out startIndex, out endIndex);
            }
            else
            {
                type = SequenceType.Empty;
            }

            int length = endIndex - startIndex;
            object endObject = end.GetObject();

            if (type != SequenceType.MultiSegment && type != SequenceType.Empty && startObject != endObject)
                ThrowHelper.ThrowInvalidOperationException_EndPositionNotReached();

            ReadOnlyMemory<T> memory;
            if (type == SequenceType.MultiSegment)
            {
                Debug.Assert(startObject is ReadOnlySequenceSegment<T>);
                ReadOnlySequenceSegment<T> startSegment = Unsafe.As<ReadOnlySequenceSegment<T>>(startObject);
                memory = startSegment.Memory;
                if (startObject != endObject)
                {
                    length = memory.Length - startIndex;
                }
            }
            else if (type == SequenceType.Array)
            {
                Debug.Assert(startObject is T[]);

                memory = new ReadOnlyMemory<T>(Unsafe.As<T[]>(startObject));
            }
            else if (type == SequenceType.MemoryManager)
            {
                Debug.Assert(startObject is MemoryManager<T>);

                memory = (Unsafe.As<MemoryManager<T>>(startObject)).Memory;
            }
            else if (typeof(T) == typeof(char) && type == SequenceType.String)
            {
                Debug.Assert(startObject is string);

                memory = (ReadOnlyMemory<T>)(object)(Unsafe.As<string>(startObject)).AsMemory();
            }
            else
            {
                return default;
            }

            return memory.Slice(startIndex, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal SequencePosition Seek(in SequencePosition start, in SequencePosition end, int offset) => Seek(start, end, (long)offset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal SequencePosition Seek(in SequencePosition start, in SequencePosition end, long offset)
        {
            GetTypeAndIndices(start.GetInteger(), end.GetInteger(), out SequenceType type, out int startIndex, out int endIndex);

            object startObject = start.GetObject();
            object endObject = end.GetObject();

            if (type == SequenceType.MultiSegment && startObject != endObject)
            {
                Debug.Assert(startObject is ReadOnlySequenceSegment<T>);
                var startSegment = Unsafe.As<ReadOnlySequenceSegment<T>>(startObject);

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
        private static SequencePosition SeekMultiSegment(ReadOnlySequenceSegment<T> currentSegment, object endObject, int endPosition, long offset)
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
            if (currentSegment == null || (currentSegment == endObject && endPosition < offset))
                ThrowHelper.ThrowArgumentOutOfRangeException_OffsetOutOfRange();

        FoundSegment:
            return new SequencePosition(currentSegment, (int)offset);
        }

        private static void CheckEndReachable(object startSegment, object endSegment)
        {
            Debug.Assert(startSegment != null);
            Debug.Assert(startSegment is ReadOnlySequenceSegment<T>);
            Debug.Assert(endSegment != null);

            var current = Unsafe.As<ReadOnlySequenceSegment<T>>(startSegment);

            while (current.Next != null)
            {
                current = current.Next;
                if (current == endSegment)
                {
                    // Found end
                    return;
                }
            }

            ThrowHelper.ThrowInvalidOperationException_EndPositionNotReached();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private long GetLength(in SequencePosition start, in SequencePosition end)
        {
            GetTypeAndIndices(start.GetInteger(), end.GetInteger(), out SequenceType type, out int startIndex, out int endIndex);

            object startObject = start.GetObject();
            object endObject = end.GetObject();
            if (type == SequenceType.MultiSegment && startObject != endObject)
            {
                Debug.Assert(startObject != null);
                Debug.Assert(startObject is ReadOnlySequenceSegment<T>);
                Debug.Assert(endObject != null);
                Debug.Assert(endObject is ReadOnlySequenceSegment<T>);

                var startSegment = Unsafe.As<ReadOnlySequenceSegment<T>>(startObject);
                var endSegment = Unsafe.As<ReadOnlySequenceSegment<T>>(endObject);
                // (end.RunningIndex + endIndex) - (start.RunningIndex + startIndex) // (End offset) - (start offset)
                return endSegment.RunningIndex - startSegment.RunningIndex - startIndex + endIndex; // Rearranged to avoid overflow
            }

            Debug.Assert(startObject == endObject);
            // Single segment length
            return endIndex - startIndex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BoundsCheck(in SequencePosition start, in SequencePosition end)
        {
            GetTypeAndIndices(start.GetInteger(), end.GetInteger(), out SequenceType type, out int startIndex, out int endIndex);

            object startObject = start.GetObject();
            object endObject = end.GetObject();
            if (type == SequenceType.MultiSegment && startObject != endObject)
            {
                Debug.Assert(startObject != null);
                Debug.Assert(startObject is ReadOnlySequenceSegment<T>);
                Debug.Assert(endObject != null);
                Debug.Assert(endObject is ReadOnlySequenceSegment<T>);

                var startSegment = Unsafe.As<ReadOnlySequenceSegment<T>>(startObject);
                var endSegment = Unsafe.As<ReadOnlySequenceSegment<T>>(endObject);

                // start.RunningIndex + startIndex <= end.RunningIndex + endIndex
                if (startSegment.RunningIndex - endIndex <= endSegment.RunningIndex - startIndex) // Rearranged to avoid overflow
                {
                    // Mult-segment in bounds
                    return;
                }
            }
            else if (startIndex <= endIndex)
            {
                Debug.Assert(startObject == endObject);
                // Single segment in bounds
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
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

        internal bool TryGetReadOnlyMemory(out ReadOnlyMemory<T> memory)
        {
            memory = GetFirstBuffer(_sequenceStart, _sequenceEnd);
            return _sequenceStart.GetObject() == _sequenceEnd.GetObject(); // Can't get ReadOnlyMemory from multi-block segments
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
    }
}

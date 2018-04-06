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
        private bool TryGetBuffer(in SequencePosition position, out ReadOnlyMemory<T> memory, out SequencePosition next)
        {
            next = default;
            object positionObject = position.GetObject();
            if (positionObject == null)
            {
                memory = default;
                return false;
            }

            int endIndex = GetIndex(_sequenceEnd);

            SequenceType type = GetSequenceType();
            if (type == SequenceType.MultiSegment)
            {
                Debug.Assert(positionObject is ReadOnlySequenceSegment<T>);
                ReadOnlySequenceSegment<T> positionSegment = Unsafe.As<ReadOnlySequenceSegment<T>>(positionObject);

                // Bounds check
                #if DEBUG
                Debug.Assert(positionSegment == _sequenceEnd.GetObject() || _sequenceStart.GetObject() != _sequenceEnd.GetObject());

                ReadOnlySequenceSegment<T> startSegment = Unsafe.As<ReadOnlySequenceSegment<T>>(_sequenceStart.GetObject());
                ReadOnlySequenceSegment<T> endSegment = Unsafe.As<ReadOnlySequenceSegment<T>>(_sequenceEnd.GetObject());

                // startSegment.RunningIndex + startIndex <= positionSegment.RunningIndex + positionIndex &&
                // positionSegment.RunningIndex + positionIndex <= endSegment.RunningIndex + endIndex
                // Rearranged to avoid overflow
                Debug.Assert(startSegment.RunningIndex - positionSegment.RunningIndex <= GetIndex(position) - GetIndex(_sequenceStart) ||
                             GetIndex(position) - GetIndex(_sequenceEnd) <= endSegment.RunningIndex - positionSegment.RunningIndex);
                #endif

                memory = positionSegment.Memory;
                if (positionSegment != _sequenceEnd.GetObject())
                {

                    ReadOnlySequenceSegment<T> nextSegment = positionSegment.Next;
                    Debug.Assert(nextSegment != null);

                    next = new SequencePosition(nextSegment, 0);
                    endIndex = memory.Length;
                }
            }
            else
            {
                // Bounds check
                Debug.Assert(positionObject == _sequenceEnd.GetObject());
                Debug.Assert(GetIndex(_sequenceStart) <= GetIndex(position) && GetIndex(position) <= GetIndex(_sequenceEnd));

                if (type == SequenceType.Array)
                {
                    Debug.Assert(positionObject is T[]);

                    memory = new ReadOnlyMemory<T>(Unsafe.As<T[]>(positionObject));
                }
                else if (typeof(T) == typeof(char) && type == SequenceType.String)
                {
                    Debug.Assert(positionObject is string);

                    memory = (ReadOnlyMemory<T>)(object)(Unsafe.As<string>(positionObject).AsMemory());
                }
                else // if (type == SequenceType.MemoryManager)
                {
                    Debug.Assert(positionObject is MemoryManager<T>);

                    memory = Unsafe.As<MemoryManager<T>>(positionObject).Memory;
                }
            }

            int positionIndex = GetIndex(position);
            memory = memory.Slice(positionIndex, endIndex - positionIndex);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ReadOnlyMemory<T> GetFirstBuffer()
        {
            int endIndex = GetIndex(_sequenceEnd);
            SequenceType type = GetSequenceType();
            object startObject = _sequenceStart.GetObject();
            ReadOnlyMemory<T> memory;
            if (type == SequenceType.MultiSegment)
            {
                Debug.Assert(startObject is ReadOnlySequenceSegment<T>);
                ReadOnlySequenceSegment<T> startSegment = Unsafe.As<ReadOnlySequenceSegment<T>>(startObject);
                memory = startSegment.Memory;
                if (startObject != _sequenceEnd.GetObject())
                {
                    endIndex = memory.Length;
                }
            }
            else if (type == SequenceType.Array)
            {
                Debug.Assert(startObject is T[]);

                memory = new ReadOnlyMemory<T>(Unsafe.As<T[]>(startObject));
            }
            else if (typeof(T) == typeof(char) && type == SequenceType.String)
            {
                Debug.Assert(startObject is string);

                memory = (ReadOnlyMemory<T>)(object)(Unsafe.As<string>(startObject).AsMemory());
            }
            else if (type == SequenceType.MemoryManager)
            {
                Debug.Assert(startObject is MemoryManager<T>);

                memory = Unsafe.As<MemoryManager<T>>(startObject).Memory;
            }
            else
            {
                return default;
            }

            int startIndex = GetIndex(_sequenceStart);
            return memory.Slice(startIndex, endIndex - startIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal SequencePosition Seek(in SequencePosition start, in SequencePosition end, int count) => Seek(start, end, (long)count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal SequencePosition Seek(in SequencePosition start, in SequencePosition end, long count)
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
                if (currentLength > count)
                    goto IsSingleSegment;

                // End of segment. Move to start of next.
                return SeekMultiSegment(startSegment.Next, startIndex, endObject, endIndex, count - currentLength);
            }

            Debug.Assert(startObject == endObject);

            if (endIndex - startIndex < count)
                ThrowHelper.ThrowArgumentOutOfRangeException_CountOutOfRange();

            // Single segment Seek
        IsSingleSegment:
            return new SequencePosition(startObject, startIndex + (int)count);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static SequencePosition SeekMultiSegment(ReadOnlySequenceSegment<T> currentSegment, int startIndex, object endObject, int endPosition, long count)
        {
            Debug.Assert(currentSegment != null);
            Debug.Assert(count >= 0);

            while (currentSegment != null && currentSegment != endObject)
            {
                int memoryLength = currentSegment.Memory.Length;

                // Fully contained in this segment
                if (memoryLength > count)
                    goto FoundSegment;

                // Move to next
                count -= memoryLength;
                currentSegment = currentSegment.Next;
            }

            // Hit the end of the segments but didn't reach the count
            if (currentSegment == null || (currentSegment == endObject && endPosition < count))
                ThrowHelper.ThrowArgumentOutOfRangeException_CountOutOfRange();

        FoundSegment:
            return new SequencePosition(currentSegment, (int)count);
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

                // startSegment.RunningIndex + startIndex <= endSegment.RunningIndex + endIndex
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

        internal bool TryGetMemoryManager(out MemoryManager<T> manager, out int start, out int length)
        {
            GetTypeAndIndices(Start.GetInteger(), End.GetInteger(), out SequenceType type, out start, out int endIndex);

            if (type != SequenceType.MemoryManager)
            {
                manager = default;
                length = 0;
                return false;
            }

            Debug.Assert(Start.GetObject() != null);
            Debug.Assert(Start.GetObject() is MemoryManager<T>);

            manager = Unsafe.As<MemoryManager<T>>(Start.GetObject());
            length = endIndex - start;
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

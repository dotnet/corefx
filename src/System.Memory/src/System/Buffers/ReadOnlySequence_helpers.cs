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
        private bool TryGetBuffer(in SequencePosition start, in SequencePosition end, out ReadOnlyMemory<T> data, out SequencePosition next)
        {
            next = default;
            var startObject = start.GetObject();
            if (startObject == null)
            {
                data = default;
                return false;
            }
            GetTypeAndIndices(start.GetInteger(), end.GetInteger(), out var type, out var startIndex, out var endIndex);

            if (type == SequenceType.MultiSegment)
            {
                Debug.Assert(startObject is ReadOnlySequenceSegment<T>);
                ReadOnlySequenceSegment<T> startSegment = Unsafe.As<ReadOnlySequenceSegment<T>>(startObject);

                data = startSegment.Memory;

                if (startSegment != end.GetObject())
                {
                    ReadOnlySequenceSegment<T> nextSegment = startSegment.Next;

                    //if (nextSegment == null)
                    //    ThrowHelper.ThrowInvalidOperationException_EndPositionNotReached();
                    Debug.Assert(nextSegment != null);

                    next = new SequencePosition(nextSegment, 0);
                    endIndex = data.Length;
                }
            }
            else
            {
                //if (startObject != end.GetObject())
                //    ThrowHelper.ThrowInvalidOperationException_EndPositionNotReached();
                Debug.Assert(startObject == end.GetObject());

                if (type == SequenceType.Array)
                {
                    Debug.Assert(startObject is T[]);

                    data = new ReadOnlyMemory<T>(Unsafe.As<T[]>(startObject));
                }
                else if (type == SequenceType.OwnedMemory)
                {
                    Debug.Assert(startObject is OwnedMemory<T>);

                    data = (Unsafe.As<OwnedMemory<T>>(startObject)).Memory;
                }
                else
                {
                    Debug.Assert(startObject is string);

                    data = (ReadOnlyMemory<T>)(object)(Unsafe.As<string>(startObject)).AsMemory();
                }
            }

            //Debug.Assert(startIndex >= (Start.GetInteger() & ReadOnlySequence.IndexBitMask));
            data = data.Slice(startIndex, endIndex - startIndex);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SequencePosition Seek(in SequencePosition start, in SequencePosition end, int offset) => Seek(start, end, (long)offset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SequencePosition Seek(in SequencePosition start, in SequencePosition end, long offset)
        {
            GetTypeAndIndices(start.GetInteger(), end.GetInteger(), out SequenceType type, out int startIndex, out int endIndex);

            object startObject = start.GetObject();
            object endObject;

            if (type == SequenceType.MultiSegment && startObject != (endObject = end.GetObject()))
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

            Debug.Assert(startObject == end.GetObject());

            if (endIndex - startIndex < offset)
                ThrowHelper.ThrowArgumentOutOfRangeException_OffsetOutOfRange();

            // Single segment Seek
        IsSingleSegment:
            return new SequencePosition(startObject, startIndex + (int)offset);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static SequencePosition SeekMultiSegment(ReadOnlySequenceSegment<T> currentSegment, object endObject, int endPosition, long offset)
        {
            Debug.Assert(offset >= 0);

            while (currentSegment != endObject)
            {
                Debug.Assert(currentSegment != null);

                int memoryLength = currentSegment.Memory.Length;

                // Fully contained in this segment
                if (memoryLength > offset)
                    goto FoundSegment;

                // Move to next
                offset -= memoryLength;
                currentSegment = currentSegment.Next;
            }

            // Hit the end of the segments but didn't reach the count
            if (endPosition < offset)
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

            if (type == SequenceType.MultiSegment)
            {
                object startObject = start.GetObject();
                object endObject = end.GetObject();
                if (startObject != endObject)
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
            }

            Debug.Assert(start.GetObject() == end.GetObject());
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
                if (startIndex - endIndex <= endSegment.RunningIndex - startSegment.RunningIndex) // Rearranged to avoid overflow
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

        internal bool TryGetOwnedMemory(out OwnedMemory<T> ownedMemory, out int start, out int length)
        {
            GetTypeAndIndices(Start.GetInteger(), End.GetInteger(), out SequenceType type, out start, out int endIndex);

            if (type != SequenceType.OwnedMemory)
            {
                ownedMemory = default;
                length = 0;
                return false;
            }

            Debug.Assert(Start.GetObject() != null);
            Debug.Assert(Start.GetObject() is OwnedMemory<T>);

            ownedMemory = Unsafe.As<OwnedMemory<T>>(Start.GetObject());
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
            else if (type == SequenceType.OwnedMemory)
            {
                Debug.Assert(startObject is OwnedMemory<T>);

                memory = Unsafe.As<OwnedMemory<T>>(startObject).Memory;
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

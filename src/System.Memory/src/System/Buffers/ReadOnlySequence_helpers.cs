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
        internal bool TryGetBuffer(in SequencePosition start, in SequencePosition end, out ReadOnlyMemory<T> data, out SequencePosition next)
        {
            next = default;

            GetTypeAndIndices(start.GetInteger(), end.GetInteger(), out SequenceType type, out int startIndex, out int endIndex);

            int length = endIndex - startIndex;
            object startObject = start.GetObject();
            object endObject = end.GetObject();

            Debug.Assert(startObject != null);
            Debug.Assert(endObject != null);

            if (type != SequenceType.MultiSegment && startObject != endObject)
                ThrowHelper.ThrowInvalidOperationException_EndPositionNotReached();

            if (type == SequenceType.MultiSegment)
            {
                Debug.Assert(startObject is ReadOnlySequenceSegment<T>);

                ReadOnlySequenceSegment<T> startSegment = Unsafe.As<ReadOnlySequenceSegment<T>>(startObject);
                if (startSegment != endObject)
                {
                    next = GetBufferCrossSegment(startIndex, end, ref startSegment, ref length);
                }

                data = startSegment.Memory.Slice(startIndex, length);
            }
            else if (type == SequenceType.Array)
            {
                Debug.Assert(startObject is T[]);

                data = new ReadOnlyMemory<T>(Unsafe.As<T[]>(startObject), startIndex, length);
            }
            else if (type == SequenceType.OwnedMemory)
            {
                Debug.Assert(startObject is OwnedMemory<T>);

                data = (Unsafe.As<OwnedMemory<T>>(startObject)).Memory.Slice(startIndex, length);
            }
            else if (typeof(T) == typeof(char) && type == SequenceType.String)
            {
                Debug.Assert(startObject is string);

                data = (ReadOnlyMemory<T>)(object)(Unsafe.As<string>(startObject)).AsMemory(startIndex, length);
            }
            else
            {
                data = default;
            }

            return type != SequenceType.Empty;
        }

        private static SequencePosition GetBufferCrossSegment(int startIndex, in SequencePosition end, ref ReadOnlySequenceSegment<T> startSegment, ref int length)
        {
            Debug.Assert(startSegment != null);

            ReadOnlySequenceSegment<T> nextSegment = startSegment.Next;
            int currentLength = startSegment.Memory.Length - startIndex;

            while (currentLength == 0 && nextSegment != null)
            {
                // Skip empty Segments
                startSegment = nextSegment;
                nextSegment = nextSegment.Next;
                currentLength = startSegment.Memory.Length;
            }

            length = currentLength;
            SequencePosition next;
            if (nextSegment == null)
            {
                if (end.GetObject() != null)
                    ThrowHelper.ThrowInvalidOperationException_EndPositionNotReached();

                next = default;
            }
            else
            {
                next = new SequencePosition(nextSegment, 0);
            }

            return next;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal SequencePosition Seek(in SequencePosition start, in SequencePosition end, int count)
        {
            GetTypeAndIndices(start.GetInteger(), end.GetInteger(), out SequenceType type, out int startIndex, out int endIndex);

            object startSegment = start.GetObject();
            bool notInRange = endIndex - startIndex < count;
            if (type == SequenceType.MultiSegment)
            {
                object endSegment = end.GetObject();
                if (notInRange || startSegment != endSegment)
                {
                    return SeekMultiSegment(startSegment, startIndex, endSegment, endIndex, count);
                }
            }
            else if (notInRange)
                ThrowHelper.ThrowArgumentOutOfRangeException_CountOutOfRange();

            return new SequencePosition(startSegment, startIndex + count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal SequencePosition Seek(in SequencePosition start, in SequencePosition end, long count)
        {
            GetTypeAndIndices(start.GetInteger(), end.GetInteger(), out SequenceType type, out int startIndex, out int endIndex);

            object startSegment = start.GetObject();
            bool notInRange = endIndex - startIndex < count;
            if (type == SequenceType.MultiSegment)
            {
                object endSegment = end.GetObject();
                if (notInRange || startSegment != endSegment)
                {
                    return SeekMultiSegment(startSegment, startIndex, endSegment, endIndex, count);
                }
            }
            else if (notInRange)
                ThrowHelper.ThrowArgumentOutOfRangeException_CountOutOfRange();

            return new SequencePosition(startSegment, startIndex + (int)count);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static SequencePosition SeekMultiSegment(object startSegment, int startIndex, object endSegment, int endPosition, long count)
        {
            Debug.Assert(startSegment != null);
            Debug.Assert(startSegment is ReadOnlySequenceSegment<T>);

            var current = Unsafe.As<ReadOnlySequenceSegment<T>>(startSegment);
            int currentLength = current.Memory.Length - startIndex;

            if (currentLength == count)
            {
                // End of segment. Move to start of next, if one exists
                if (current.Next != null)
                {
                    count = 0;
                    current = current.Next;
                }
            }
            else if (currentLength > count)
            {
                // Fully contained in this segment
                count += startIndex;
            }
            else
            {
                // Not in this segment. Move to next
                count -= currentLength;
                current = current.Next;

                while (current != null)
                {
                    bool isCurrentAtEnd = current == endSegment;
                    int memoryLength = isCurrentAtEnd ? endPosition : current.Memory.Length;

                    if (memoryLength > count || (memoryLength == count && isCurrentAtEnd))
                    {
                        // Fully contained in this segment; or at end of segment but isn't a next one to move to
                        break;
                    }

                    // Move to next
                    count -= memoryLength;
                    current = current.Next;
                }
            }

            // Hit the end of the segments but didn't reach the count
            if (current == null)
                ThrowHelper.ThrowArgumentOutOfRangeException_CountOutOfRange();

            return new SequencePosition(current, (int)count);
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
            }

            if (current != endSegment)
                ThrowHelper.ThrowInvalidOperationException_EndPositionNotReached();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private long GetLength(in SequencePosition start, in SequencePosition end)
        {
            GetTypeAndIndices(start.GetInteger(), end.GetInteger(), out SequenceType type, out int startIndex, out int endIndex);

            if (type == SequenceType.MultiSegment)
            {
                object startSegment = start.GetObject();
                object endSegment = end.GetObject();
                if (startSegment != endSegment)
                {
                    return GetLengthMultiSegment(startSegment, startIndex, endSegment, endIndex);
                }
            }

            return endIndex - startIndex;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static long GetLengthMultiSegment(object startSegment, int startIndex, object endSegment, int endIndex)
        {
            Debug.Assert(startSegment != null);
            Debug.Assert(startSegment is ReadOnlySequenceSegment<T>);
            Debug.Assert(endSegment != null);
            Debug.Assert(endSegment is ReadOnlySequenceSegment<T>);

            var start = Unsafe.As <ReadOnlySequenceSegment<T>>(startSegment);
            var end = Unsafe.As<ReadOnlySequenceSegment<T>>(endSegment);
            // (end.RunningIndex + endIndex) - (start.RunningIndex + startIndex) // (End offset) - (start offset)
            return end.RunningIndex - start.RunningIndex - startIndex + endIndex; // Rearranged to avoid overflow
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BoundsCheck(in SequencePosition start, in SequencePosition end)
        {
            GetTypeAndIndices(start.GetInteger(), end.GetInteger(), out SequenceType type, out int startIndex, out int endIndex);

            if (type == SequenceType.MultiSegment)
            {
                object startSegment = start.GetObject();
                object endSegment = end.GetObject();

                if (startSegment != endSegment)
                {
                    BoundsCheckMultiSegment(startSegment, startIndex, endSegment, endIndex);
                    return;
                }
            }

            if (startIndex <= endIndex)
            {
                return;
            }

            ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void BoundsCheckMultiSegment(object startSegment, int startIndex, object endSegment, int endIndex)
        {
            Debug.Assert(startSegment != null);
            Debug.Assert(startSegment is ReadOnlySequenceSegment<T>);
            Debug.Assert(endSegment != null);
            Debug.Assert(endSegment is ReadOnlySequenceSegment<T>);

            var start = Unsafe.As<ReadOnlySequenceSegment<T>>(startSegment);
            var end = Unsafe.As<ReadOnlySequenceSegment<T>>(endSegment);

            // start.RunningIndex + startIndex <= end.RunningIndex + endIndex
            if (start.RunningIndex - endIndex <= end.RunningIndex - startIndex) // Rearranged to avoid overflow
            {
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

        internal bool TryGetArray(out ArraySegment<T> array)
        {
            GetTypeAndIndices(Start.GetInteger(), End.GetInteger(), out SequenceType type, out int startIndex, out int endIndex);

            if (type != SequenceType.Array)
            {
                array = default;
                return false;
            }

            Debug.Assert(Start.GetObject() != null);
            Debug.Assert(Start.GetObject() is T[]);

            array = new ArraySegment<T>(Unsafe.As<T[]>(Start.GetObject()), startIndex, endIndex - startIndex);
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

            object startSegment = Start.GetObject();
            object endSegment = End.GetObject();

            Debug.Assert(startSegment != null);
            Debug.Assert(endSegment != null);

            int length = endIndex - startIndex;
            if (startSegment != endSegment)
            {
                // Can't get ReadOnlyMemory from multi-block segments
                memory = default;
                return false;
            }
            else if (type == SequenceType.Array)
            {
                Debug.Assert(startSegment is T[]);

                memory = new ReadOnlyMemory<T>(Unsafe.As<T[]>(startSegment), startIndex, length);
            }
            else if (type == SequenceType.OwnedMemory)
            {
                Debug.Assert(startSegment is OwnedMemory<T>);

                memory = Unsafe.As<OwnedMemory<T>>(startSegment).Memory.Slice(startIndex, length);
            }
            else if (typeof(T) == typeof(char) && type == SequenceType.String)
            {
                Debug.Assert(startSegment is string);

                memory = (ReadOnlyMemory<T>)(object)Unsafe.As<string>(startSegment).AsMemory(startIndex, length);
            }
            else // ReadOnlySequenceSegment
            {
                Debug.Assert(startSegment is ReadOnlySequenceSegment<T>);

                memory = Unsafe.As<ReadOnlySequenceSegment<T>>(startSegment).Memory.Slice(startIndex, length);
            }

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

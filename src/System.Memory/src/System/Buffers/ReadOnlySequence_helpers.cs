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
        internal bool TryGetBuffer(in SequencePosition position, out ReadOnlyMemory<T> memory, out SequencePosition next, bool trusted = false)
        {
            object positionObject = position.GetObject();
            if (positionObject == null)
                goto ReturnFalse;

            int positionIndex = GetIndex(position);
            SequenceType type = GetSequenceType();

            if (type == SequenceType.MultiSegment)
            {
                object endObject = _sequenceEnd.GetObject();
                if (!trusted && endObject == null) // Empty Segment
                    goto EndPositionNotReached;

                // End segment
                if (positionObject == endObject)
                {
                    ReadOnlySequenceSegment<T> positionSegment = Unsafe.As<ReadOnlySequenceSegment<T>>(positionObject);

                    // Bounds check
                    if (!trusted && positionSegment == _sequenceStart.GetObject() && positionIndex < GetIndex(_sequenceStart))
                        goto PositionOutOfRange;

                    int length = GetIndex(_sequenceEnd) - positionIndex;
                    if (!trusted && length < 0)
                        goto PositionOutOfRange;

                    memory = positionSegment.Memory.Slice(positionIndex, length);
                    next = default;
                    return true;
                }

                // Start or Middle segment
                {
                    ReadOnlySequenceSegment<T> positionSegment;

                    if (!trusted) // Bounds check
                    {
                        object startObject = _sequenceStart.GetObject();
                        if (positionObject == startObject) // Start segment
                        {
                            positionSegment = Unsafe.As<ReadOnlySequenceSegment<T>>(positionObject);
                            if (positionIndex < GetIndex(_sequenceStart))
                                goto PositionOutOfRange;
                        }
                        else // Middle Segment
                        {
                            positionSegment = positionObject as ReadOnlySequenceSegment<T>;
                            if (positionSegment == null)
                                goto EndPositionNotReached;

                            ReadOnlySequenceSegment<T> startSegment = Unsafe.As<ReadOnlySequenceSegment<T>>(startObject);
                            ReadOnlySequenceSegment<T> endSegment = Unsafe.As<ReadOnlySequenceSegment<T>>(endObject);

                            if (positionSegment.RunningIndex - startSegment.RunningIndex < 0 || endSegment.RunningIndex - positionSegment.RunningIndex < 0)
                                goto PositionOutOfRange;
                        }
                    }
                    else
                    {
                        positionSegment = Unsafe.As<ReadOnlySequenceSegment<T>>(positionObject);
                    }

                    ReadOnlySequenceSegment<T> nextSegment = positionSegment.Next;
                    if (!trusted && nextSegment == null)
                        goto EndPositionNotReached;
                    next = new SequencePosition(nextSegment, 0);

                    ReadOnlyMemory<T> positionMemory = positionSegment.Memory;
                    int length = positionMemory.Length - positionIndex;
                    if (!trusted && length < 0)
                        goto PositionOutOfRange;

                    memory = positionMemory.Slice(positionIndex, length);
                    return true;
                }
            }

            // Array or String or MemoryManager
            {
                // Bounds check
                if (!trusted && positionObject != _sequenceStart.GetObject())
                    goto EndPositionNotReached;

                int length = GetIndex(_sequenceEnd) - positionIndex;

                // Bounds check
                if (!trusted && (length < 0 || positionIndex < GetIndex(_sequenceStart)))
                    goto PositionOutOfRange;

                next = default;
                if (type == SequenceType.Array)
                {
                    Debug.Assert(positionObject is T[]);

                    memory = new ReadOnlyMemory<T>(Unsafe.As<T[]>(positionObject), positionIndex, length);
                    return true;
                }

                if (typeof(T) == typeof(char) && type == SequenceType.String)
                {
                    Debug.Assert(positionObject is string);

                    memory = (ReadOnlyMemory<T>)(object)(Unsafe.As<string>(positionObject).AsMemory().Slice(positionIndex, length));
                    return true;
                }

                // if (type == SequenceType.MemoryManager)
                {
                    Debug.Assert(positionObject is MemoryManager<T>);

                    memory = Unsafe.As<MemoryManager<T>>(positionObject).Memory.Slice(positionIndex, length);
                    return true;
                }
            }

        EndPositionNotReached:
            ThrowHelper.ThrowInvalidOperationException_EndPositionNotReached();
        PositionOutOfRange:
            ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
        ReturnFalse:
            next = default;
            memory = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ReadOnlyMemory<T> GetFirstBuffer()
        {
            SequenceType type = GetSequenceType();
            if (type == SequenceType.MultiSegment)
            {
                object startObject = _sequenceStart.GetObject();
                if (startObject == null) // Empty sequence
                    return default;

                Debug.Assert(startObject is ReadOnlySequenceSegment<T>);
                ReadOnlySequenceSegment<T> startSegment = Unsafe.As<ReadOnlySequenceSegment<T>>(startObject);

                if (startSegment != _sequenceEnd.GetObject())
                {
                    return startSegment.Memory.Slice(GetIndex(_sequenceStart));
                }

                int startIndex = GetIndex(_sequenceStart);
                return startSegment.Memory.Slice(startIndex, GetIndex(_sequenceEnd) - startIndex);
            }

            if (type == SequenceType.Array)
            {
                object startObject = _sequenceStart.GetObject();
                Debug.Assert(startObject is T[]);
                
                int startIndex = GetIndex(_sequenceStart);
                return new ReadOnlyMemory<T>(Unsafe.As<T[]>(startObject), startIndex, GetIndex(_sequenceEnd) - startIndex);
            }

            if (typeof(T) == typeof(char) && type == SequenceType.String)
            {
                object startObject = _sequenceStart.GetObject();
                Debug.Assert(startObject is string);

                int startIndex = GetIndex(_sequenceStart);
                return (ReadOnlyMemory<T>)(object)(Unsafe.As<string>(startObject).AsMemory(startIndex, GetIndex(_sequenceEnd) - startIndex));
            }

            // if (type == SequenceType.MemoryManager)
            {
                object startObject = _sequenceStart.GetObject();
                Debug.Assert(startObject is MemoryManager<T>);

                int startIndex = GetIndex(_sequenceStart);
                return Unsafe.As<MemoryManager<T>>(startObject).Memory.Slice(startIndex, GetIndex(_sequenceEnd) - startIndex);
            }
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
            GetTypeAndIndices(Start.GetInteger(), End.GetInteger(), out SequenceType type, out startIndex, out endIndex);

            if (type != SequenceType.MultiSegment || Start.GetObject() == null)
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
            if (startObject != endObject || startObject == null)
            {
                // Can't get ReadOnlyMemory from multi-block segments and default sequence
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
            if (typeof(T) != typeof(char) || GetSequenceType() != SequenceType.String)
            {
                start = 0;
                length = 0;
                text = null;
                return false;
            }

            Debug.Assert(_sequenceStart.GetObject() is string);

            text = Unsafe.As<string>(_sequenceStart.GetObject());
            start = GetIndex(_sequenceStart);
            length = GetIndex(_sequenceEnd) - start;
            return true;
        }
    }
}

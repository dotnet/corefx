// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Internal.Runtime.CompilerServices;

namespace System.Buffers
{
    /// <summary>
    /// Represents a sequence that can read a sequential series of <typeparam name="T" />.
    /// </summary>
    public readonly partial struct ReadOnlySequence<T>
    {
        private readonly SequencePosition _sequenceStart;
        private readonly SequencePosition _sequenceEnd;

        /// <summary>
        /// Returns empty <see cref="ReadOnlySequence{T}"/>
        /// </summary>
#if FEATURE_PORTABLE_SPAN
        public static readonly ReadOnlySequence<T> Empty = new ReadOnlySequence<T>(SpanHelpers.PerTypeValues<T>.EmptyArray);
#else
        public static readonly ReadOnlySequence<T> Empty = new ReadOnlySequence<T>(Array.Empty<T>());
#endif // FEATURE_PORTABLE_SPAN 

        /// <summary>
        /// Length of the <see cref="ReadOnlySequence{T}"/>.
        /// </summary>
        public long Length => GetLength(_sequenceStart, _sequenceEnd);

        /// <summary>
        /// Determines if the <see cref="ReadOnlySequence{T}"/> is empty.
        /// </summary>
        public bool IsEmpty => Length == 0;

        /// <summary>
        /// Determines if the <see cref="ReadOnlySequence{T}"/> contains a single <see cref="ReadOnlyMemory{T}"/> segment.
        /// </summary>
        public bool IsSingleSegment
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _sequenceStart.GetObject() == _sequenceEnd.GetObject();
        }

        /// <summary>
        /// Gets <see cref="ReadOnlyMemory{T}"/> from the first segment.
        /// </summary>
        public ReadOnlyMemory<T> First
        {
            get
            {
                TryGetBuffer(_sequenceStart, _sequenceEnd, out ReadOnlyMemory<T> data, out _);
                return data;
            }
        }

        /// <summary>
        /// A position to the start of the <see cref="ReadOnlySequence{T}"/>.
        /// </summary>
        public SequencePosition Start => _sequenceStart;

        /// <summary>
        /// A position to the end of the <see cref="ReadOnlySequence{T}"/>
        /// </summary>
        public SequencePosition End => _sequenceEnd;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ReadOnlySequence(object startSegment, int startIndexAndFlags, object endSegment, int endIndexAndFlags)
        {
            // Used by SliceImpl to create new ReadOnlySequence
            Debug.Assert(startSegment != null);
            Debug.Assert(endSegment != null);

            _sequenceStart = new SequencePosition(startSegment, startIndexAndFlags);
            _sequenceEnd = new SequencePosition(endSegment, endIndexAndFlags);
        }

        /// <summary>
        /// Creates an instance of <see cref="ReadOnlySequence{T}"/> from linked memory list represented by start and end segments
        /// and corresponding indexes in them.
        /// </summary>
        public ReadOnlySequence(ReadOnlySequenceSegment<T> startSegment, int startIndex, ReadOnlySequenceSegment<T> endSegment, int endIndex)
        {
            if (startSegment == null ||
                endSegment == null ||
                (uint)startSegment.Memory.Length < (uint)startIndex ||
                (uint)endSegment.Memory.Length < (uint)endIndex)
            {
                ThrowHelper.ThrowArgumentValidationException(startSegment, startIndex, endSegment);
            }

            _sequenceStart = new SequencePosition(startSegment, ReadOnlySequence.SegmentToSequenceStart(startIndex));
            _sequenceEnd = new SequencePosition(endSegment, ReadOnlySequence.SegmentToSequenceEnd(endIndex));

            if (startSegment == endSegment)
            {
                if (endIndex < startIndex)
                    ThrowHelper.ThrowArgumentValidationException(startSegment, startIndex, endSegment);
                return;
            }

            long longLength = endSegment.RunningIndex - startSegment.RunningIndex;
            if (longLength < 0)
                ThrowHelper.ThrowArgumentValidationException(startSegment, startIndex, endSegment);

            if (startIndex == startSegment.Memory.Length)
            {
                _sequenceStart = SeekStart(_sequenceStart, _sequenceEnd, 0);
                Debug.Assert(_sequenceStart.GetObject() != startSegment);
                Debug.Assert(_sequenceStart.GetInteger() == 0);

                startSegment = Unsafe.As<ReadOnlySequenceSegment<T>>(_sequenceStart.GetObject());
#pragma warning disable 162
                if (ReadOnlySequence.SegmentStartMask != 0)
                    _sequenceStart = new SequencePosition(startSegment, ReadOnlySequence.SegmentToSequenceStart(0));
#pragma warning restore 162

                if (startSegment == endSegment)
                    return;
            }

            if (endIndex == 0)
            {
                long longLength2 = longLength - startIndex + endIndex;
                if (longLength < 0)
                    ThrowHelper.ThrowArgumentValidationException(startSegment, startIndex, endSegment);

                _sequenceEnd = SeekEnd(_sequenceStart, _sequenceEnd, longLength2);
                Debug.Assert(_sequenceEnd.GetObject() != endSegment);
                Debug.Assert(_sequenceEnd.GetInteger() > 0);

#pragma warning disable 162
                if (ReadOnlySequence.SegmentStartMask != 0)
                {
                    endSegment = Unsafe.As<ReadOnlySequenceSegment<T>>(_sequenceEnd.GetObject());
                    _sequenceEnd = new SequencePosition(endSegment, ReadOnlySequence.SegmentToSequenceEnd(_sequenceEnd.GetInteger()));
                }
#pragma warning restore 162
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="ReadOnlySequence{T}"/> from the <see cref="T:T[]"/>.
        /// </summary>
        public ReadOnlySequence(T[] array)
        {
            if (array == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);

            _sequenceStart = new SequencePosition(array, ReadOnlySequence.ArrayToSequenceStart(0));
            _sequenceEnd = new SequencePosition(array, ReadOnlySequence.ArrayToSequenceEnd(array.Length));
        }

        /// <summary>
        /// Creates an instance of <see cref="ReadOnlySequence{T}"/> from the <see cref="T:T[]"/>, start and index.
        /// </summary>
        public ReadOnlySequence(T[] array, int start, int length)
        {
            if (array == null ||
                (uint)start > (uint)array.Length ||
                (uint)length > (uint)(array.Length - start))
                ThrowHelper.ThrowArgumentValidationException(array, start);

            if (length == 0)
            {
                array = Array.Empty<T>();
                start = 0;
            }

            _sequenceStart = new SequencePosition(array, ReadOnlySequence.ArrayToSequenceStart(start));
            _sequenceEnd = new SequencePosition(array, ReadOnlySequence.ArrayToSequenceEnd(start + length));
        }

        /// <summary>
        /// Creates an instance of <see cref="ReadOnlySequence{T}"/> from the <see cref="ReadOnlyMemory{T}"/>.
        /// Consumer is expected to manage lifetime of memory until <see cref="ReadOnlySequence{T}"/> is not used anymore.
        /// </summary>
        public ReadOnlySequence(ReadOnlyMemory<T> memory)
        {
            if (MemoryMarshal.TryGetOwnedMemory(memory, out OwnedMemory<T> ownedMemory, out int index, out int length))
            {
                if (length == 0)
                    goto Empty;

                _sequenceStart = new SequencePosition(ownedMemory, ReadOnlySequence.OwnedMemoryToSequenceStart(index));
                _sequenceEnd = new SequencePosition(ownedMemory, ReadOnlySequence.OwnedMemoryToSequenceEnd(length));
            }
            else if (MemoryMarshal.TryGetArray(memory, out ArraySegment<T> segment))
            {
                if (segment.Count == 0)
                    goto Empty;

                T[] array = segment.Array;
                int start = segment.Offset;
                _sequenceStart = new SequencePosition(array, ReadOnlySequence.ArrayToSequenceStart(start));
                _sequenceEnd = new SequencePosition(array, ReadOnlySequence.ArrayToSequenceEnd(start + segment.Count));
            }
            else if (typeof(T) == typeof(char))
            {
                if (!MemoryMarshal.TryGetString(((ReadOnlyMemory<char>)(object)memory), out string text, out int start, out length))
                    ThrowHelper.ThrowInvalidOperationException();

                if (length == 0)
                    goto Empty;

                _sequenceStart = new SequencePosition(text, ReadOnlySequence.StringToSequenceStart(start));
                _sequenceEnd = new SequencePosition(text, ReadOnlySequence.StringToSequenceEnd(start + length));
            }
            else
            {
                // Should never be reached
                ThrowHelper.ThrowInvalidOperationException();
                goto Empty;
            }
            return;

         Empty:
            T[] emptyArray = Array.Empty<T>();
            _sequenceStart = new SequencePosition(emptyArray, ReadOnlySequence.ArrayToSequenceStart(0));
            _sequenceEnd = new SequencePosition(emptyArray, ReadOnlySequence.ArrayToSequenceEnd(0));
        }

        /// <summary>
        /// Creates an instance of <see cref="ReadOnlySequence{T}"/> from the <see cref="OwnedMemory{T}"/>.
        /// Consumer is expected to manage lifetime of memory until <see cref="ReadOnlySequence{T}"/> is not used anymore.
        /// </summary>
        public ReadOnlySequence(OwnedMemory<T> ownedMemory)
        {
            if (ownedMemory == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.ownedMemory);

            if (ownedMemory.Length > 0)
            {
                _sequenceStart = new SequencePosition(ownedMemory, ReadOnlySequence.OwnedMemoryToSequenceStart(0));
                _sequenceEnd = new SequencePosition(ownedMemory, ReadOnlySequence.OwnedMemoryToSequenceEnd(ownedMemory.Length));
            }
            else
            {
                T[] emptyArray = Array.Empty<T>();
                _sequenceStart = new SequencePosition(emptyArray, ReadOnlySequence.ArrayToSequenceStart(0));
                _sequenceEnd = new SequencePosition(emptyArray, ReadOnlySequence.ArrayToSequenceEnd(0));
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="ReadOnlySequence{T}"/> from the <see cref="OwnedMemory{T}"/>, start and length.
        /// Consumer is expected to manage lifetime of memory until <see cref="ReadOnlySequence{T}"/> is not used anymore.
        /// </summary>
        public ReadOnlySequence(OwnedMemory<T> ownedMemory, int start, int length)
        {
            if (ownedMemory == null ||
                (uint)start > (uint)ownedMemory.Length ||
                (uint)length > (uint)(ownedMemory.Length - start))
                ThrowHelper.ThrowArgumentValidationException(ownedMemory, start);

            if (length > 0)
            {
                _sequenceStart = new SequencePosition(ownedMemory, ReadOnlySequence.OwnedMemoryToSequenceStart(start));
                _sequenceEnd = new SequencePosition(ownedMemory, ReadOnlySequence.OwnedMemoryToSequenceEnd(start + length));
            }
            else
            {
                T[] emptyArray = Array.Empty<T>();
                _sequenceStart = new SequencePosition(emptyArray, ReadOnlySequence.ArrayToSequenceStart(0));
                _sequenceEnd = new SequencePosition(emptyArray, ReadOnlySequence.ArrayToSequenceEnd(0));
            }
        }

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlySequence{T}"/>, beginning at <paramref name="start"/>, with <paramref name="length"/> items
        /// </summary>
        /// <param name="start">The index at which to begin this slice.</param>
        /// <param name="length">The length of the slice</param>
        public ReadOnlySequence<T> Slice(long start, long length)
        {
            if (start < 0 || length < 0)
                ThrowHelper.ThrowArgumentValidationException(start);

            SequencePosition begin = SeekStart(_sequenceStart, _sequenceEnd, start);
            SequencePosition end = SeekEnd(begin, _sequenceEnd, length);
            return SliceImpl(begin, end);
        }

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlySequence{T}"/>, beginning at <paramref name="start"/>, ending at <paramref name="end"/> (inclusive).
        /// </summary>
        /// <param name="start">The index at which to begin this slice.</param>
        /// <param name="end">The end (inclusive) of the slice</param>
        public ReadOnlySequence<T> Slice(long start, SequencePosition end)
        {
            if (start < 0)
                ThrowHelper.ThrowArgumentValidationException(start);
            BoundsCheck(_sequenceStart, end);
            BoundsCheck(end, _sequenceEnd);

            SequencePosition begin = SeekStart(_sequenceStart, end, start);
            return SliceImpl(begin, end);
        }

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlySequence{T}"/>, beginning at <paramref name="start"/>, with <paramref name="length"/> items
        /// </summary>
        /// <param name="start">The starting (inclusive) <see cref="SequencePosition"/> at which to begin this slice.</param>
        /// <param name="length">The length of the slice</param>
        public ReadOnlySequence<T> Slice(SequencePosition start, long length)
        {
            if (length < 0)
                ThrowHelper.ThrowArgumentValidationException(0);
            BoundsCheck(_sequenceStart, start);
            BoundsCheck(start, _sequenceEnd);

            SequencePosition end = SeekEnd(start, _sequenceEnd, length);
            return SliceImpl(start, end);
        }

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlySequence{T}"/>, beginning at <paramref name="start"/>, with <paramref name="length"/> items
        /// </summary>
        /// <param name="start">The index at which to begin this slice.</param>
        /// <param name="length">The length of the slice</param>
        public ReadOnlySequence<T> Slice(int start, int length)
        {
            if (start < 0 || length < 0)
                ThrowHelper.ThrowArgumentValidationException(start);

            SequencePosition begin = SeekStart(_sequenceStart, _sequenceEnd, start);
            SequencePosition end = SeekEnd(begin, _sequenceEnd, length);
            return SliceImpl(begin, end);
        }

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlySequence{T}"/>, beginning at <paramref name="start"/>, ending at <paramref name="end"/> (inclusive).
        /// </summary>
        /// <param name="start">The index at which to begin this slice.</param>
        /// <param name="end">The end (inclusive) of the slice</param>
        public ReadOnlySequence<T> Slice(int start, SequencePosition end)
        {
            if (start < 0)
                ThrowHelper.ThrowArgumentValidationException(start);
            BoundsCheck(_sequenceStart, end);
            BoundsCheck(end, _sequenceEnd);

            SequencePosition begin = SeekStart(_sequenceStart, end, start);
            return SliceImpl(begin, end);
        }

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlySequence{T}"/>, beginning at '<paramref name="start"/>, with <paramref name="length"/> items
        /// </summary>
        /// <param name="start">The starting (inclusive) <see cref="SequencePosition"/> at which to begin this slice.</param>
        /// <param name="length">The length of the slice</param>
        public ReadOnlySequence<T> Slice(SequencePosition start, int length)
        {
            if (length < 0)
                ThrowHelper.ThrowArgumentValidationException(0);
            BoundsCheck(_sequenceStart, start);
            BoundsCheck(start, _sequenceEnd);

            SequencePosition end = SeekEnd(start, _sequenceEnd, length);
            return SliceImpl(start, end);
        }

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlySequence{T}"/>, beginning at <paramref name="start"/>, ending at <paramref name="end"/> (inclusive).
        /// </summary>
        /// <param name="start">The starting (inclusive) <see cref="SequencePosition"/> at which to begin this slice.</param>
        /// <param name="end">The ending (inclusive) <see cref="SequencePosition"/> of the slice</param>
        public ReadOnlySequence<T> Slice(SequencePosition start, SequencePosition end)
        {
            BoundsCheck(_sequenceStart, start);
            BoundsCheck(start, end);
            BoundsCheck(end, _sequenceEnd);

            return SliceImpl(start, end);
        }

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlySequence{T}"/>, beginning at <paramref name="start"/>, ending at the existing <see cref="ReadOnlySequence{T}"/>'s end.
        /// </summary>
        /// <param name="start">The starting (inclusive) <see cref="SequencePosition"/> at which to begin this slice.</param>
        public ReadOnlySequence<T> Slice(SequencePosition start)
        {
            BoundsCheck(_sequenceStart, start);
            BoundsCheck(start, _sequenceEnd);

            return SliceImpl(start, _sequenceEnd);
        }

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlySequence{T}"/>, beginning at <paramref name="start"/>, ending at the existing <see cref="ReadOnlySequence{T}"/>'s end.
        /// </summary>
        /// <param name="start">The start index at which to begin this slice.</param>
        public ReadOnlySequence<T> Slice(long start)
        {
            if (start < 0)
                ThrowHelper.ThrowArgumentValidationException(start);
            if (start == 0)
                return this;

            SequencePosition begin = SeekStart(_sequenceStart, _sequenceEnd, start);
            return SliceImpl(begin, _sequenceEnd);
        }

        /// <inheritdoc />
        public override string ToString() => string.Format("System.Buffers.ReadOnlySequence<{0}>[{1}]", typeof(T).Name, Length);

        /// <summary>
        /// Returns an enumerator over the <see cref="ReadOnlySequence{T}"/>
        /// </summary>
        public Enumerator GetEnumerator() => new Enumerator(this);

        /// <summary>
        /// Returns a new <see cref="SequencePosition"/> at an <paramref name="offset"/> from the start of the sequence.
        /// </summary>
        public SequencePosition GetPosition(long offset) => GetPosition(offset, _sequenceStart);

        /// <summary>
        /// Returns a new <see cref="SequencePosition"/> at an <paramref name="offset"/> from the <paramref name="origin"/>
        /// </summary>
        public SequencePosition GetPosition(long offset, SequencePosition origin)
        {
            if (offset < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException_OffsetOutOfRange();

            return SeekEnd(origin, _sequenceEnd, offset);
        }

        /// <summary>
        /// Tries to retrieve next segment after <paramref name="position"/> and return its contents in <paramref name="memory"/>.
        /// Returns <code>false</code> if end of <see cref="ReadOnlySequence{T}"/> was reached otherwise <code>true</code>.
        /// Sets <paramref name="position"/> to the beginning of next segment if <paramref name="advance"/> is set to <code>true</code>.
        /// </summary>
        public bool TryGet(ref SequencePosition position, out ReadOnlyMemory<T> memory, bool advance = true)
        {
            bool result = TryGetBuffer(position, End, out memory, out SequencePosition next);
            if (advance)
            {
                position = next;
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ReadOnlySequence<T> SliceImpl(in SequencePosition start, in SequencePosition end)
        {
            // In this method we reset high order bits from indices
            // of positions that were passed in
            // and apply type bits specific for current ReadOnlySequence type

            object startObject = start.GetObject();
            int startIndex = start.GetInteger() & ReadOnlySequence.IndexBitMask;
            object endObject = end.GetObject();
            int endIndex = end.GetInteger() & ReadOnlySequence.IndexBitMask;

            if (startObject != endObject)
            {
                var startSegment = Unsafe.As<ReadOnlySequenceSegment<T>>(startObject);
                if (startIndex == startSegment.Memory.Length)
                {
                    SequencePosition begin = SeekStart(start, end, 0);
                    Debug.Assert(begin.GetObject() != startSegment);
                    Debug.Assert(begin.GetInteger() == 0);

                    startObject = begin.GetObject();
                    startIndex = 0;
                    if (startObject == endObject)
                        goto Return;
                }

                if (endIndex == 0)
                {
                    startSegment = Unsafe.As<ReadOnlySequenceSegment<T>>(startObject);
                    var endSegment = Unsafe.As<ReadOnlySequenceSegment<T>>(endObject);
                    long longLength = endSegment.RunningIndex - startSegment.RunningIndex - startIndex + endIndex;

                    SequencePosition finish = SeekEnd(start, end, longLength);
                    Debug.Assert(finish.GetObject() != endSegment);
                    Debug.Assert(finish.GetInteger() > 0);

                    endObject = finish.GetObject();
                    endIndex = finish.GetInteger();
                }
            }

        Return:
            return new ReadOnlySequence<T>(
                startObject, startIndex | (Start.GetInteger() & ReadOnlySequence.FlagBitMask),
                endObject, endIndex | (End.GetInteger() & ReadOnlySequence.FlagBitMask)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void GetTypeAndIndices(int start, int end, out SequenceType sequenceType, out int startIndex, out int endIndex)
        {
            startIndex = start & ReadOnlySequence.IndexBitMask;
            endIndex = end & ReadOnlySequence.IndexBitMask;
            // We take high order bits of two indexes index and move them
            // to a first and second position to convert to BufferType
            // Masking with 2 is required to only keep the second bit of Start.GetInteger()
            sequenceType = (SequenceType)((((uint)Start.GetInteger() >> 30) & 2) | (uint)End.GetInteger() >> 31);
        }

        /// <summary>
        /// An enumerator over the <see cref="ReadOnlySequence{T}"/>
        /// </summary>
        public struct Enumerator
        {
            private readonly ReadOnlySequence<T> _sequence;
            private SequencePosition _next;
            private ReadOnlyMemory<T> _currentMemory;

            /// <summary>Initialize the enumerator.</summary>
            /// <param name="sequence">The <see cref="ReadOnlySequence{T}"/> to enumerate.</param>
            public Enumerator(in ReadOnlySequence<T> sequence)
            {
                _currentMemory = default;
                _next = sequence.Start;
                _sequence = sequence;
            }

            /// <summary>
            /// The current <see cref="ReadOnlyMemory{T}"/>
            /// </summary>
            public ReadOnlyMemory<T> Current => _currentMemory;

            /// <summary>
            /// Moves to the next <see cref="ReadOnlyMemory{T}"/> in the <see cref="ReadOnlySequence{T}"/>
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                if (_next.GetObject() == null)
                {
                    return false;
                }

                return _sequence.TryGet(ref _next, out _currentMemory);
            }
        }

        private enum SequenceType
        {
            MultiSegment = 0x00,
            Array = 0x1,
            OwnedMemory = 0x2,
            String = 0x3,
            Empty = 0x4
        }
    }

    internal static class ReadOnlySequence
    {
        public const int FlagBitMask = 1 << 31;
        public const int IndexBitMask = ~FlagBitMask;

        public const int SegmentStartMask = 0;
        public const int SegmentEndMask = 0;

        public const int ArrayStartMask = 0;
        public const int ArrayEndMask = FlagBitMask;

        public const int OwnedMemoryStartMask = FlagBitMask;
        public const int OwnedMemoryEndMask = 0;

        public const int StringStartMask = FlagBitMask;
        public const int StringEndMask = FlagBitMask;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SegmentToSequenceStart(int startIndex) => startIndex | SegmentStartMask;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int SegmentToSequenceEnd(int endIndex) => endIndex | SegmentEndMask;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ArrayToSequenceStart(int startIndex) => startIndex | ArrayStartMask;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ArrayToSequenceEnd(int endIndex) => endIndex | ArrayEndMask;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int OwnedMemoryToSequenceStart(int startIndex) => startIndex | OwnedMemoryStartMask;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int OwnedMemoryToSequenceEnd(int endIndex) => endIndex | OwnedMemoryEndMask;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int StringToSequenceStart(int startIndex) => startIndex | StringStartMask;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int StringToSequenceEnd(int endIndex) => endIndex | StringEndMask;
    }
}

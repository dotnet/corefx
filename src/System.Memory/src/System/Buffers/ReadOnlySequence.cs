﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if !FEATURE_PORTABLE_SPAN
using Internal.Runtime.CompilerServices;
#endif // FEATURE_PORTABLE_SPAN

namespace System.Buffers
{
    /// <summary>
    /// Represents a sequence that can read a sequential series of <typeparam name="T" />.
    /// </summary>
    [DebuggerTypeProxy(typeof(ReadOnlySequenceDebugView<>))]
    [DebuggerDisplay("{ToString(),raw}")]
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
        public ReadOnlyMemory<T> First => GetFirstBuffer();

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

            // startSegment and endSegment can be null for default ReadOnlySequence only
            Debug.Assert((startSegment != null && endSegment != null) ||
                (startSegment == null && endSegment == null && startIndexAndFlags == 0 && endIndexAndFlags == 0));

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
                (startSegment != endSegment && startSegment.RunningIndex > endSegment.RunningIndex) ||
                (uint)startSegment.Memory.Length < (uint)startIndex ||
                (uint)endSegment.Memory.Length < (uint)endIndex ||
                (startSegment == endSegment && endIndex < startIndex))
                ThrowHelper.ThrowArgumentValidationException(startSegment, startIndex, endSegment);

            _sequenceStart = new SequencePosition(startSegment, ReadOnlySequence.SegmentToSequenceStart(startIndex));
            _sequenceEnd = new SequencePosition(endSegment, ReadOnlySequence.SegmentToSequenceEnd(endIndex));
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

            _sequenceStart = new SequencePosition(array, ReadOnlySequence.ArrayToSequenceStart(start));
            _sequenceEnd = new SequencePosition(array, ReadOnlySequence.ArrayToSequenceEnd(start + length));
        }

        /// <summary>
        /// Creates an instance of <see cref="ReadOnlySequence{T}"/> from the <see cref="ReadOnlyMemory{T}"/>.
        /// Consumer is expected to manage lifetime of memory until <see cref="ReadOnlySequence{T}"/> is not used anymore.
        /// </summary>
        public ReadOnlySequence(ReadOnlyMemory<T> memory)
        {
            if (MemoryMarshal.TryGetMemoryManager(memory, out MemoryManager<T> manager, out int index, out int length))
            {
                _sequenceStart = new SequencePosition(manager, ReadOnlySequence.MemoryManagerToSequenceStart(index));
                _sequenceEnd = new SequencePosition(manager, ReadOnlySequence.MemoryManagerToSequenceEnd(length));
            }
            else if (MemoryMarshal.TryGetArray(memory, out ArraySegment<T> segment))
            {
                T[] array = segment.Array;
                int start = segment.Offset;
                _sequenceStart = new SequencePosition(array, ReadOnlySequence.ArrayToSequenceStart(start));
                _sequenceEnd = new SequencePosition(array, ReadOnlySequence.ArrayToSequenceEnd(start + segment.Count));
            }
            else if (typeof(T) == typeof(char))
            {
                if (!MemoryMarshal.TryGetString(((ReadOnlyMemory<char>)(object)memory), out string text, out int start, out length))
                    ThrowHelper.ThrowInvalidOperationException();

                _sequenceStart = new SequencePosition(text, ReadOnlySequence.StringToSequenceStart(start));
                _sequenceEnd = new SequencePosition(text, ReadOnlySequence.StringToSequenceEnd(start + length));
            }
            else
            {
                // Should never be reached
                ThrowHelper.ThrowInvalidOperationException();
                _sequenceStart = default;
                _sequenceEnd = default;
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
                ThrowHelper.ThrowStartOrEndArgumentValidationException(start);

            SequencePosition begin = Seek(_sequenceStart, _sequenceEnd, start);
            SequencePosition end = Seek(begin, _sequenceEnd, length);
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
                ThrowHelper.ThrowStartOrEndArgumentValidationException(start);
            BoundsCheck(end, _sequenceEnd);

            SequencePosition begin = Seek(_sequenceStart, end, start);
            object beginObject = begin.GetObject();
            object endObject = end.GetObject();
            if (beginObject != endObject)
            {
                CheckEndReachable(beginObject, endObject);
            }
            return SliceImpl(begin, end);
        }

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlySequence{T}"/>, beginning at <paramref name="start"/>, with <paramref name="length"/> items
        /// </summary>
        /// <param name="start">The starting (inclusive) <see cref="SequencePosition"/> at which to begin this slice.</param>
        /// <param name="length">The length of the slice</param>
        public ReadOnlySequence<T> Slice(SequencePosition start, long length)
        {
            BoundsCheck(start, _sequenceEnd); // check start before length
            if (length < 0)
                // Passing value >= 0 means throw exception on length argument
                ThrowHelper.ThrowStartOrEndArgumentValidationException(0);

            SequencePosition end = Seek(start, _sequenceEnd, length);
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
                ThrowHelper.ThrowStartOrEndArgumentValidationException(start);

            SequencePosition begin = Seek(_sequenceStart, _sequenceEnd, start);
            SequencePosition end = Seek(begin, _sequenceEnd, length);
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
                ThrowHelper.ThrowStartOrEndArgumentValidationException(start);
            BoundsCheck(end, _sequenceEnd);

            SequencePosition begin = Seek(_sequenceStart, end, start);
            object beginObject = begin.GetObject();
            object endObject = end.GetObject();
            if (beginObject != endObject)
            {
                CheckEndReachable(beginObject, endObject);
            }
            return SliceImpl(begin, end);
        }

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlySequence{T}"/>, beginning at '<paramref name="start"/>, with <paramref name="length"/> items
        /// </summary>
        /// <param name="start">The starting (inclusive) <see cref="SequencePosition"/> at which to begin this slice.</param>
        /// <param name="length">The length of the slice</param>
        public ReadOnlySequence<T> Slice(SequencePosition start, int length)
        {
            BoundsCheck(start, _sequenceEnd); // check start before length
            if (length < 0)
                // Passing value >= 0 means throw exception on length argument
                ThrowHelper.ThrowStartOrEndArgumentValidationException(0);

            SequencePosition end = Seek(start, _sequenceEnd, length);
            return SliceImpl(start, end);
        }

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlySequence{T}"/>, beginning at <paramref name="start"/>, ending at <paramref name="end"/> (inclusive).
        /// </summary>
        /// <param name="start">The starting (inclusive) <see cref="SequencePosition"/> at which to begin this slice.</param>
        /// <param name="end">The ending (inclusive) <see cref="SequencePosition"/> of the slice</param>
        public ReadOnlySequence<T> Slice(SequencePosition start, SequencePosition end)
        {
            BoundsCheck(end, _sequenceEnd);
            BoundsCheck(start, end);

            return SliceImpl(start, end);
        }

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlySequence{T}"/>, beginning at <paramref name="start"/>, ending at the existing <see cref="ReadOnlySequence{T}"/>'s end.
        /// </summary>
        /// <param name="start">The starting (inclusive) <see cref="SequencePosition"/> at which to begin this slice.</param>
        public ReadOnlySequence<T> Slice(SequencePosition start)
        {
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
                ThrowHelper.ThrowStartOrEndArgumentValidationException(start);

            if (start == 0)
                return this;

            SequencePosition begin = Seek(_sequenceStart, _sequenceEnd, start);
            return SliceImpl(begin, _sequenceEnd);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (typeof(T) == typeof(char))
            {
                ReadOnlySequence<T> localThis = this;
                ReadOnlySequence<char> charSequence = Unsafe.As<ReadOnlySequence<T>, ReadOnlySequence<char>>(ref localThis);

                if (SequenceMarshal.TryGetString(charSequence, out string text, out int start, out int length))
                {
                    return text.Substring(start, length);
                }

                if (Length < int.MaxValue)
                {
#if !FEATURE_PORTABLE_SPAN
                    return string.Create((int)Length, charSequence, (span, sequence) =>
                    {
                        foreach (ReadOnlyMemory<char> readOnlyMemory in sequence)
                        {
                            ReadOnlySpan<char> sourceSpan = readOnlyMemory.Span;
                            sourceSpan.CopyTo(span);
                            span = span.Slice(sourceSpan.Length);
                        }
                    });
#else
                    return new string(charSequence.ToArray());
#endif
                }
            }

            return string.Format("System.Buffers.ReadOnlySequence<{0}>[{1}]", typeof(T).Name, Length);
        }

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

            return Seek(origin, _sequenceEnd, offset);
        }

        /// <summary>
        /// Tries to retrieve next segment after <paramref name="position"/> and return its contents in <paramref name="memory"/>.
        /// Returns <code>false</code> if end of <see cref="ReadOnlySequence{T}"/> was reached otherwise <code>true</code>.
        /// Sets <paramref name="position"/> to the beginning of next segment if <paramref name="advance"/> is set to <code>true</code>.
        /// </summary>
        public bool TryGet(ref SequencePosition position, out ReadOnlyMemory<T> memory, bool advance = true)
        {
            bool result = TryGetBuffer(position, out memory, out SequencePosition next);
            if (advance)
            {
                position = next;
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ReadOnlySequence<T> SliceImpl(in SequencePosition begin, in SequencePosition end)
        {
            // In this method we reset high order bits from indices
            // of positions that were passed in
            // and apply type bits specific for current ReadOnlySequence type

            return new ReadOnlySequence<T>(
                begin.GetObject(),
                begin.GetInteger() & ReadOnlySequence.IndexBitMask | (Start.GetInteger() & ReadOnlySequence.FlagBitMask),
                end.GetObject(),
                end.GetInteger() & ReadOnlySequence.IndexBitMask | (End.GetInteger() & ReadOnlySequence.FlagBitMask)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private SequenceType GetSequenceType()
        {
            // We take high order bits of two indexes and move them
            // to a first and second position to convert to SequenceType

            // if (start < 0  and end < 0)
            // start >> 31 = -1, end >> 31 = -1
            // 2 * (-1) + (-1) = -3, result = (SequenceType)3

            // if (start < 0  and end >= 0)
            // start >> 31 = -1, end >> 31 = 0
            // 2 * (-1) + 0 = -2, result = (SequenceType)2

            // if (start >= 0  and end >= 0)
            // start >> 31 = 0, end >> 31 = 0
            // 2 * 0 + 0 = 0, result = (SequenceType)0

            // if (start >= 0  and end < 0)
            // start >> 31 = 0, end >> 31 = -1
            // 2 * 0 + (-1) = -1, result = (SequenceType)1

            return (SequenceType)(-(2 * (_sequenceStart.GetInteger() >> 31) + (_sequenceEnd.GetInteger() >> 31)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetIndex(in SequencePosition position) => position.GetInteger() & ReadOnlySequence.IndexBitMask;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void GetTypeAndIndices(int start, int end, out SequenceType sequenceType, out int startIndex, out int endIndex)
        {
            startIndex = start & ReadOnlySequence.IndexBitMask;
            endIndex = end & ReadOnlySequence.IndexBitMask;
            // We take high order bits of two indexes index and move them
            // to a first and second position to convert to BufferType
            // Masking with 2 is required to only keep the second bit of Start.GetInteger()
            sequenceType = Start.GetObject() == null ? SequenceType.Empty : (SequenceType)((((uint)Start.GetInteger() >> 30) & 2) | (uint)End.GetInteger() >> 31);
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
            MemoryManager = 0x2,
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

        public const int MemoryManagerStartMask = FlagBitMask;
        public const int MemoryManagerEndMask = 0;

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
        public static int MemoryManagerToSequenceStart(int startIndex) => startIndex | MemoryManagerStartMask;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MemoryManagerToSequenceEnd(int endIndex) => endIndex | MemoryManagerEndMask;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int StringToSequenceStart(int startIndex) => startIndex | StringStartMask;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int StringToSequenceEnd(int endIndex) => endIndex | StringEndMask;
    }
}

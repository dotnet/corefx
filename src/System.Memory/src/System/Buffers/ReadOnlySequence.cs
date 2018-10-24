﻿// Licensed to the .NET Foundation under one or more agreements.
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
    [DebuggerTypeProxy(typeof(ReadOnlySequenceDebugView<>))]
    [DebuggerDisplay("{ToString(),raw}")]
    public readonly partial struct ReadOnlySequence<T>
    {
        private readonly SequencePosition _sequenceStart;
        private readonly SequencePosition _sequenceEnd;

        /// <summary>
        /// Returns empty <see cref="ReadOnlySequence{T}"/>
        /// </summary>
        public static readonly ReadOnlySequence<T> Empty = new ReadOnlySequence<T>(Array.Empty<T>());

        /// <summary>
        /// Length of the <see cref="ReadOnlySequence{T}"/>.
        /// </summary>
        public long Length => GetLength();

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

            _sequenceStart = new SequencePosition(startSegment, startIndex);
            _sequenceEnd = new SequencePosition(endSegment, endIndex);
        }

        /// <summary>
        /// Creates an instance of <see cref="ReadOnlySequence{T}"/> from the <see cref="T:T[]"/>.
        /// </summary>
        public ReadOnlySequence(T[] array)
        {
            if (array == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);

            _sequenceStart = new SequencePosition(array, 0);
            _sequenceEnd = new SequencePosition(array, array.Length);
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

            _sequenceStart = new SequencePosition(array, start);
            _sequenceEnd = new SequencePosition(array, start + length);
        }

        /// <summary>
        /// Creates an instance of <see cref="ReadOnlySequence{T}"/> from the <see cref="ReadOnlyMemory{T}"/>.
        /// Consumer is expected to manage lifetime of memory until <see cref="ReadOnlySequence{T}"/> is not used anymore.
        /// </summary>
        public ReadOnlySequence(ReadOnlyMemory<T> memory)
        {
            if (MemoryMarshal.TryGetMemoryManager(memory, out MemoryManager<T> manager, out int index, out int length))
            {
                var holder = new MemoryManagerHolder<T>(manager);
                _sequenceStart = new SequencePosition(holder, index);
                _sequenceEnd = new SequencePosition(holder, length);
            }
            else if (MemoryMarshal.TryGetArray(memory, out ArraySegment<T> segment))
            {
                T[] array = segment.Array;
                int start = segment.Offset;
                _sequenceStart = new SequencePosition(array, start);
                _sequenceEnd = new SequencePosition(array, start + segment.Count);
            }
            else if (typeof(T) == typeof(char))
            {
                if (!MemoryMarshal.TryGetString((ReadOnlyMemory<char>)(object)memory, out string text, out int start, out length))
                    ThrowHelper.ThrowInvalidOperationException();

                _sequenceStart = new SequencePosition(text, start);
                _sequenceEnd = new SequencePosition(text, start + length);
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

            SequencePosition begin;
            SequencePosition end;

            int startIndex = GetIndex(_sequenceStart);
            int endIndex = GetIndex(_sequenceEnd);

            object startObject = _sequenceStart.GetObject();
            object endObject = _sequenceEnd.GetObject();

            if (startObject != endObject)
            {
                Debug.Assert(startObject != null);
                var startSegment = (ReadOnlySequenceSegment<T>)startObject;

                int currentLength = startSegment.Memory.Length - startIndex;

                // Position in start segment
                if (currentLength > start)
                {
                    startIndex += (int)start;
                    begin = new SequencePosition(startObject, startIndex);

                    end = GetEndPosition(startSegment, startObject, startIndex, endObject, endIndex, length);
                }
                else
                {
                    if (currentLength < 0)
                        ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();

                    begin = SeekMultiSegment(startSegment.Next, endObject, endIndex, start - currentLength, ExceptionArgument.start);

                    int beginIndex = GetIndex(begin);
                    object beginObject = begin.GetObject();

                    if (beginObject != endObject)
                    {
                        Debug.Assert(beginObject != null);
                        end = GetEndPosition((ReadOnlySequenceSegment<T>)beginObject, beginObject, beginIndex, endObject, endIndex, length);
                    }
                    else
                    {
                        if (endIndex - beginIndex < length)
                            ThrowHelper.ThrowStartOrEndArgumentValidationException(0);  // Passing value >= 0 means throw exception on length argument

                        end = new SequencePosition(beginObject, beginIndex + (int)length);
                    }
                }
            }
            else
            {
                if (endIndex - startIndex < start)
                    ThrowHelper.ThrowStartOrEndArgumentValidationException(-1); // Passing value < 0 means throw exception on start argument

                startIndex += (int)start;
                begin = new SequencePosition(startObject, startIndex);

                if (endIndex - startIndex < length)
                    ThrowHelper.ThrowStartOrEndArgumentValidationException(0);  // Passing value >= 0 means throw exception on length argument

                end = new SequencePosition(startObject, startIndex + (int)length);
            }

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

            uint sliceEndIndex = (uint)GetIndex(end);
            object sliceEndObject = end.GetObject();

            uint startIndex = (uint)GetIndex(_sequenceStart);
            object startObject = _sequenceStart.GetObject();

            uint endIndex = (uint)GetIndex(_sequenceEnd);
            object endObject = _sequenceEnd.GetObject();

            // Single-Segment Sequence
            if (startObject == endObject)
            {
                if (!InRange(sliceEndIndex, startIndex, endIndex))
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
                }

                if (sliceEndIndex - startIndex < start)
                    ThrowHelper.ThrowStartOrEndArgumentValidationException(-1); // Passing value < 0 means throw exception on start argument

                goto FoundInFirstSegment;
            }

            // Multi-Segment Sequence
            var startSegment = (ReadOnlySequenceSegment<T>)startObject;
            ulong startRange = (ulong)(startSegment.RunningIndex + startIndex);
            ulong sliceRange = (ulong)(((ReadOnlySequenceSegment<T>)sliceEndObject).RunningIndex + sliceEndIndex);

            // This optimization works because we know sliceEndIndex, startIndex, and endIndex are all >= 0
            Debug.Assert(sliceEndIndex >= 0 && startIndex >= 0 && endIndex >= 0);
            if (!InRange(
                sliceRange,
                startRange,
                (ulong)(((ReadOnlySequenceSegment<T>)endObject).RunningIndex + endIndex)))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
            }

            if (startRange + (ulong)start > sliceRange)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.start);
            }

            int currentLength = startSegment.Memory.Length - (int)startIndex;

            // Position not in startSegment
            if (currentLength <= start)
            {
                if (currentLength < 0)
                    ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();

                // End of segment. Move to start of next.
                SequencePosition begin = SeekMultiSegment(startSegment.Next, sliceEndObject, (int)sliceEndIndex, start - currentLength, ExceptionArgument.start);
                return SliceImpl(begin, end);
            }

        FoundInFirstSegment:
            // startIndex + start <= int.MaxValue
            Debug.Assert(start <= int.MaxValue - startIndex);
            return SliceImpl(new SequencePosition(startObject, (int)startIndex + (int)start), end);
        }

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlySequence{T}"/>, beginning at <paramref name="start"/>, with <paramref name="length"/> items
        /// </summary>
        /// <param name="start">The starting (inclusive) <see cref="SequencePosition"/> at which to begin this slice.</param>
        /// <param name="length">The length of the slice</param>
        public ReadOnlySequence<T> Slice(SequencePosition start, long length)
        {
            // Check start before length
            uint sliceStartIndex = (uint)GetIndex(start);
            object sliceStartObject = start.GetObject();

            uint startIndex = (uint)GetIndex(_sequenceStart);
            object startObject = _sequenceStart.GetObject();

            uint endIndex = (uint)GetIndex(_sequenceEnd);
            object endObject = _sequenceEnd.GetObject();

            // Single-Segment Sequence
            if (startObject == endObject)
            {
                if (!InRange(sliceStartIndex, startIndex, endIndex))
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
                }

                if (length < 0)
                    // Passing value >= 0 means throw exception on length argument
                    ThrowHelper.ThrowStartOrEndArgumentValidationException(0);

                if (endIndex - sliceStartIndex < length)
                    ThrowHelper.ThrowStartOrEndArgumentValidationException(0);

                goto FoundInFirstSegment;
            }

            // Multi-Segment Sequence
            var sliceStartSegment = (ReadOnlySequenceSegment<T>)sliceStartObject;
            ulong sliceRange = (ulong)((sliceStartSegment.RunningIndex + sliceStartIndex));
            ulong startRange = (ulong)(((ReadOnlySequenceSegment<T>)startObject).RunningIndex + startIndex);
            ulong endRange = (ulong)(((ReadOnlySequenceSegment<T>)endObject).RunningIndex + endIndex);

            // This optimization works because we know sliceStartIndex, startIndex, and endIndex are all >= 0
            Debug.Assert(sliceStartIndex >= 0 && startIndex >= 0 && endIndex >= 0);
            if (!InRange(sliceRange, startRange, endRange))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();
            }

            if (length < 0)
                // Passing value >= 0 means throw exception on length argument
                ThrowHelper.ThrowStartOrEndArgumentValidationException(0);

            if (sliceRange + (ulong)length > endRange)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.length);
            }

            int currentLength = sliceStartSegment.Memory.Length - (int)sliceStartIndex;

            // Position not in startSegment
            if (currentLength < length)
            {
                if (currentLength < 0)
                    ThrowHelper.ThrowArgumentOutOfRangeException_PositionOutOfRange();

                // End of segment. Move to start of next.
                SequencePosition end = SeekMultiSegment(sliceStartSegment.Next, endObject, (int)endIndex, length - currentLength, ExceptionArgument.length);
                return SliceImpl(start, end);
            }

        FoundInFirstSegment:
            // sliceStartIndex + length <= int.MaxValue
            Debug.Assert(length <= int.MaxValue - sliceStartIndex);
            return SliceImpl(start, new SequencePosition(sliceStartObject, (int)sliceStartIndex + (int)length));
        }

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlySequence{T}"/>, beginning at <paramref name="start"/>, with <paramref name="length"/> items
        /// </summary>
        /// <param name="start">The index at which to begin this slice.</param>
        /// <param name="length">The length of the slice</param>
        public ReadOnlySequence<T> Slice(int start, int length) => Slice((long)start, length);

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlySequence{T}"/>, beginning at <paramref name="start"/>, ending at <paramref name="end"/> (inclusive).
        /// </summary>
        /// <param name="start">The index at which to begin this slice.</param>
        /// <param name="end">The end (inclusive) of the slice</param>
        public ReadOnlySequence<T> Slice(int start, SequencePosition end) => Slice((long)start, end);

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlySequence{T}"/>, beginning at '<paramref name="start"/>, with <paramref name="length"/> items
        /// </summary>
        /// <param name="start">The starting (inclusive) <see cref="SequencePosition"/> at which to begin this slice.</param>
        /// <param name="length">The length of the slice</param>
        public ReadOnlySequence<T> Slice(SequencePosition start, int length) => Slice(start, (long)length);

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlySequence{T}"/>, beginning at <paramref name="start"/>, ending at <paramref name="end"/> (inclusive).
        /// </summary>
        /// <param name="start">The starting (inclusive) <see cref="SequencePosition"/> at which to begin this slice.</param>
        /// <param name="end">The ending (inclusive) <see cref="SequencePosition"/> of the slice</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySequence<T> Slice(SequencePosition start, SequencePosition end)
        {
            BoundsCheck((uint)GetIndex(start), start.GetObject(), (uint)GetIndex(end), end.GetObject());
            return SliceImpl(start, end);
        }

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlySequence{T}"/>, beginning at <paramref name="start"/>, ending at the existing <see cref="ReadOnlySequence{T}"/>'s end.
        /// </summary>
        /// <param name="start">The starting (inclusive) <see cref="SequencePosition"/> at which to begin this slice.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySequence<T> Slice(SequencePosition start)
        {
            BoundsCheck(start);
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

            SequencePosition begin = Seek(_sequenceStart, _sequenceEnd, start, ExceptionArgument.start);
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
                    return string.Create((int)Length, charSequence, (span, sequence) => sequence.CopyTo(span)); 
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

            return Seek(origin, _sequenceEnd, offset, ExceptionArgument.offset);
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
        // Returns true iff the object has a component size;
        // i.e., is variable length like string, array, Utf8String.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool ObjectHasComponentSize(object obj)
        {
            // CLR objects are laid out in memory as follows.
            // [ pMethodTable || .. object data .. ]
            //   ^-- the object reference points here
            //
            // The first DWORD of the method table class will have its high bit set if the
            // method table has component size info stored somewhere. See member
            // MethodTable:IsStringOrArray in src\vm\methodtable.h for full details.
            //
            // So in effect this method is the equivalent of
            // return ((MethodTable*)(*obj))->IsStringOrArray();
            Debug.Assert(obj != null);
            return *(int*)GetObjectMethodTablePointer(obj) < 0;
        }

        // Given an object reference, returns its MethodTable* as an IntPtr.
        //[Intrinsic]
        //[NonVersionable]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IntPtr GetObjectMethodTablePointer(object obj)
        {
            Debug.Assert(obj != null);
            // We know that the first data field in any managed object is immediately after the
            // method table pointer, so just back up one pointer and immediately deref.
            // This is not ideal in terms of minimizing instruction count but is the best we can do at the moment.
            return Unsafe.Add(ref Unsafe.As<byte, IntPtr>(ref GetPinningHelper(obj).m_data), -1);
            // Ideally this method would be replaced by the VM with:
            // ldarg.0
            // ldind.i
            // ret
        }

        // Used for unsafe pinning of arbitrary objects.
        internal static PinningHelper GetPinningHelper(object o)
        {
            return Unsafe.As<PinningHelper>(o);
        }
    }

    internal sealed class MemoryManagerHolder<T>
    {
        public MemoryManagerHolder(MemoryManager<T> memoryManager)
        {
            MemoryManager = memoryManager;
        }

        public MemoryManager<T> MemoryManager { get; }
    }

    // Helper class to assist with unsafe pinning of arbitrary objects. The typical usage pattern is:
    // fixed (byte * pData = &JitHelpers.GetPinningHelper(value).m_data)
    // {
    //    ... pData is what Object::GetData() returns in VM ...
    // }
    internal class PinningHelper
    {
        public byte m_data;
    }
}

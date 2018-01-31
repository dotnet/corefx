// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable 1591

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Buffers
{
    /// <summary>
    /// Represents a buffer that can read a sequential series of bytes.
    /// </summary>
    public readonly partial struct ReadOnlyBuffer<T>
    {
        private const int IndexBitMask = 0x7FFFFFFF;

        internal const int MemoryListStartMask = 0;
        internal const int MemoryListEndMask = 0;

        internal const int ArrayStartMask = 0;
        internal const int ArrayEndMask = 1 << 31;

        internal const int OwnedMemoryStartMask = 1 << 31;
        internal const int OwnedMemoryEndMask = 0;

        internal readonly SequencePosition BufferStart;
        internal readonly SequencePosition BufferEnd;

        public static readonly ReadOnlyBuffer<T> Empty = new ReadOnlyBuffer<T>(new T[0]);

        /// <summary>
        /// Length of the <see cref="ReadOnlyBuffer{T}"/> in bytes.
        /// </summary>
        public long Length => GetLength(BufferStart, BufferEnd);

        /// <summary>
        /// Determines if the <see cref="ReadOnlyBuffer{T}"/> is empty.
        /// </summary>
        public bool IsEmpty => Length == 0;

        /// <summary>
        /// Determins if the <see cref="ReadOnlyBuffer{T}"/> is a single <see cref="Memory{Byte}"/>.
        /// </summary>
        public bool IsSingleSegment => BufferStart.Segment == BufferEnd.Segment;

        public ReadOnlyMemory<T> First
        {
            get
            {
                TryGetBuffer(BufferStart, BufferEnd, out var first, out _);
                return first;
            }
        }

        /// <summary>
        /// A position to the start of the <see cref="ReadOnlyBuffer{T}"/>.
        /// </summary>
        public SequencePosition Start => BufferStart;

        /// <summary>
        /// A position to the end of the <see cref="ReadOnlyBuffer{T}"/>
        /// </summary>
        public SequencePosition End => BufferEnd;

        private ReadOnlyBuffer(object startSegment, int startIndex, object endSegment, int endIndex)
        {
            Debug.Assert(startSegment != null);
            Debug.Assert(endSegment != null);

            BufferStart = new SequencePosition(startSegment, startIndex);
            BufferEnd = new SequencePosition(endSegment, endIndex);
        }

        public ReadOnlyBuffer(IMemoryList<T> startSegment, int startIndex, IMemoryList<T> endSegment, int endIndex)
        {
            Debug.Assert(startSegment != null);
            Debug.Assert(endSegment != null);
            Debug.Assert(startSegment.Memory.Length >= startIndex);
            Debug.Assert(endSegment.Memory.Length >= endIndex);

            BufferStart = new SequencePosition(startSegment, startIndex | MemoryListStartMask);
            BufferEnd = new SequencePosition(endSegment, endIndex | MemoryListEndMask);
        }

        public ReadOnlyBuffer(T[] array) : this(array, 0, array.Length)
        {
        }

        public ReadOnlyBuffer(T[] array, int offset, int length)
        {
            if (array == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            }

            BufferStart = new SequencePosition(array, offset | ArrayStartMask);
            BufferEnd = new SequencePosition(array, offset + length | ArrayEndMask);
        }

        // Consumer is expected to manage lifetime of memory until
        // ReadOnlyBuffer is not used anymore
        public ReadOnlyBuffer(Memory<T> memory)
        {
            var segment = new ReadOnlyBufferSegment
            {
                Memory = memory
            };
            BufferStart = new SequencePosition(segment, 0 | MemoryListStartMask);
            BufferEnd = new SequencePosition(segment, memory.Length | MemoryListEndMask);
        }

        public ReadOnlyBuffer(IEnumerable<Memory<T>> buffers)
        {
            ReadOnlyBufferSegment segment = null;
            ReadOnlyBufferSegment first = null;
            foreach (var buffer in buffers)
            {
                var newSegment = new ReadOnlyBufferSegment()
                {
                    Memory = buffer,
                    RunningIndex = segment?.RunningIndex ?? 0
                };

                if (segment != null)
                {
                    segment.Next = newSegment;
                }
                else
                {
                    first = newSegment;
                }

                segment = newSegment;
            }

            if (first == null)
            {
                first = segment = new ReadOnlyBufferSegment();
            }

            BufferStart = new SequencePosition(first, 0 | MemoryListStartMask);
            BufferEnd = new SequencePosition(segment, segment.Memory.Length | MemoryListEndMask);
        }

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlyBuffer{T}"/>, beginning at <paramref name="offset"/>, and is at most <paramref name="length"/> bytes
        /// </summary>
        /// <param name="offset">The index at which to begin this slice.</param>
        /// <param name="length">The length of the slice</param>
        public ReadOnlyBuffer<T> Slice(long offset, long length)
        {
            var begin = Seek(BufferStart, BufferEnd, offset, false);
            var end = Seek(begin, BufferEnd, length, false);
            return SliceImpl(begin, end);
        }

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlyBuffer{T}"/>, beginning at <paramref name="offset"/>, ending at <paramref name="end"/> (inclusive).
        /// </summary>
        /// <param name="offset">The index at which to begin this slice.</param>
        /// <param name="end">The end (inclusive) of the slice</param>
        public ReadOnlyBuffer<T> Slice(long offset, SequencePosition end)
        {
            BoundsCheck(BufferEnd, end);

            var begin = Seek(BufferStart, end, offset);
            return SliceImpl(begin, end);
        }

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlyBuffer{T}"/>, beginning at <paramref name="start"/>, and is at most <paramref name="length"/> bytes
        /// </summary>
        /// <param name="start">The starting (inclusive) <see cref="SequencePosition"/> at which to begin this slice.</param>
        /// <param name="length">The length of the slice</param>
        public ReadOnlyBuffer<T> Slice(SequencePosition start, long length)
        {
            BoundsCheck(BufferEnd, start);

            var end = Seek(start, BufferEnd, length, false);
            return SliceImpl(start, end);
        }

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlyBuffer{T}"/>, beginning at <paramref name="offset"/>, and is at most <paramref name="length"/> bytes
        /// </summary>
        /// <param name="offset">The index at which to begin this slice.</param>
        /// <param name="length">The length of the slice</param>
        public ReadOnlyBuffer<T> Slice(int offset, int length)
        {
            var begin = Seek(BufferStart, BufferEnd, offset, false);
            var end = Seek(begin, BufferEnd, length, false);
            return SliceImpl(begin, end);
        }

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlyBuffer{T}"/>, beginning at <paramref name="offset"/>, ending at <paramref name="end"/> (inclusive).
        /// </summary>
        /// <param name="offset">The index at which to begin this slice.</param>
        /// <param name="end">The end (inclusive) of the slice</param>
        public ReadOnlyBuffer<T> Slice(int offset, SequencePosition end)
        {
            BoundsCheck(BufferEnd, end);

            var begin = Seek(BufferStart, end, offset);
            return SliceImpl(begin, end);
        }

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlyBuffer{T}"/>, beginning at '<paramref name="start"/>, and is at most <paramref name="length"/> bytes
        /// </summary>
        /// <param name="start">The starting (inclusive) <see cref="SequencePosition"/> at which to begin this slice.</param>
        /// <param name="length">The length of the slice</param>
        public ReadOnlyBuffer<T> Slice(SequencePosition start, int length)
        {
            BoundsCheck(BufferEnd, start);

            var end = Seek(start, BufferEnd, length, false);
            return SliceImpl(start, end);
        }

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlyBuffer{T}"/>, beginning at <paramref name="start"/>, ending at <paramref name="end"/> (inclusive).
        /// </summary>
        /// <param name="start">The starting (inclusive) <see cref="SequencePosition"/> at which to begin this slice.</param>
        /// <param name="end">The ending (inclusive) <see cref="SequencePosition"/> of the slice</param>
        public ReadOnlyBuffer<T> Slice(SequencePosition start, SequencePosition end)
        {
            BoundsCheck(BufferEnd, end);
            BoundsCheck(end, start);

            return SliceImpl(start, end);
        }

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlyBuffer{T}"/>, beginning at <paramref name="start"/>, ending at the existing <see cref="ReadOnlyBuffer{T}"/>'s end.
        /// </summary>
        /// <param name="start">The starting (inclusive) <see cref="SequencePosition"/> at which to begin this slice.</param>
        public ReadOnlyBuffer<T> Slice(SequencePosition start)
        {
            BoundsCheck(BufferEnd, start);

            return SliceImpl(start, BufferEnd);
        }

        /// <summary>
        /// Forms a slice out of the given <see cref="ReadOnlyBuffer{T}"/>, beginning at <paramref name="offset"/>, ending at the existing <see cref="ReadOnlyBuffer{T}"/>'s end.
        /// </summary>
        /// <param name="offset">The start index at which to begin this slice.</param>
        public ReadOnlyBuffer<T> Slice(long offset)
        {
            if (offset == 0) return this;

            var begin = Seek(BufferStart, BufferEnd, offset, false);
            return SliceImpl(begin, BufferEnd);
        }

        /// <summary>
        /// Copy the <see cref="ReadOnlyBuffer{T}"/> to the specified <see cref="Span{Byte}"/>.
        /// </summary>
        /// <param name="destination">The destination <see cref="Span{Byte}"/>.</param>
        public void CopyTo(Span<T> destination)
        {
            if (Length > destination.Length)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.destination);
            }

            foreach (var segment in this)
            {
                segment.Span.CopyTo(destination);
                destination = destination.Slice(segment.Length);
            }
        }

        /// <summary>
        /// Converts the <see cref="ReadOnlyBuffer{T}"/> to an array/>
        /// </summary>
        public T[] ToArray()
        {
            var buffer = new T[Length];
            CopyTo(buffer);
            return buffer;
        }

        public override string ToString()
        {
            if (typeof(T) == typeof(byte))
            {
                if (this is ReadOnlyBuffer<byte> bytes)
                {
                    var sb = new StringBuilder();
                    foreach (var buffer in bytes)
                    {
                        SpanLiteralExtensions.AppendAsLiteral(buffer.Span, sb);
                    }
                    return sb.ToString();
                }
            }
            return base.ToString();
        }

        /// <summary>
        /// Returns an enumerator over the <see cref="ReadOnlyBuffer{T}"/>
        /// </summary>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public SequencePosition GetPosition(SequencePosition origin, long offset)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }
            return Seek(origin, BufferEnd, offset, false);
        }

        public bool TryGet(ref SequencePosition position, out ReadOnlyMemory<T> data, bool advance = true)
        {
            var result = TryGetBuffer(position, End, out data, out var next);
            if (advance)
            {
                position = next;
            }

            return result;
        }

        /// <summary>
        /// An enumerator over the <see cref="ReadOnlyBuffer{T}"/>
        /// </summary>
        public struct Enumerator
        {
            private readonly ReadOnlyBuffer<T> _readOnlyBuffer;
            private SequencePosition _next;
            private ReadOnlyMemory<T> _currentMemory;

            /// <summary>
            ///
            /// </summary>
            public Enumerator(ReadOnlyBuffer<T> readOnlyBuffer)
            {
                _readOnlyBuffer = readOnlyBuffer;
                _currentMemory = default;
                _next = readOnlyBuffer.Start;
            }

            /// <summary>
            /// The current <see cref="Memory{Byte}"/>
            /// </summary>
            public ReadOnlyMemory<T> Current => _currentMemory;

            /// <summary>
            /// Moves to the next <see cref="Memory{Byte}"/> in the <see cref="ReadOnlyBuffer{T}"/>
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                if (_next.Segment == null)
                {
                    return false;
                }

                return _readOnlyBuffer.TryGet(ref _next, out _currentMemory);
            }

            public Enumerator GetEnumerator()
            {
                return this;
            }

            public void Reset()
            {
                ThrowHelper.ThrowNotSupportedException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ReadOnlyBuffer<T> SliceImpl(SequencePosition begin, SequencePosition end)
        {
            return new ReadOnlyBuffer<T>(
                begin.Segment,
                begin.Index & IndexBitMask | (Start.Index & ~IndexBitMask),
                end.Segment,
                end.Index & IndexBitMask | (End.Index & ~IndexBitMask)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private BufferType GetBufferType()
        {
            // We take high order bits of two indexes index and move them
            // to a first and second position to convert to BufferType
            return (BufferType)((((uint)Start.Index >> 30) & 2) | (uint)End.Index >> 31);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetIndex(int index)
        {
            return index & IndexBitMask;
        }

        private enum BufferType
        {
            MemoryList = 0b00,
            Array = 0b01,
            OwnedMemory = 0b10
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Memory.Tests
{
    public abstract class ReadOnlySequenceFactory<T>
    {
        public static ReadOnlySequenceFactory<T> ArrayFactory { get; } = new ArrayTestSequenceFactory();
        public static ReadOnlySequenceFactory<T> MemoryFactory { get; } = new MemoryTestSequenceFactory();
        public static ReadOnlySequenceFactory<T> MemoryManagerFactory { get; } = new MemoryManagerTestSequenceFactory();
        public static ReadOnlySequenceFactory<T> SingleSegmentFactory { get; } = new SingleSegmentTestSequenceFactory();
        public static ReadOnlySequenceFactory<T> SegmentPerItemFactory { get; } = new BytePerSegmentTestSequenceFactory();
        public static ReadOnlySequenceFactory<T> SplitInThree { get; } = new SplitInThreeSegmentsTestSequenceFactory();

        public abstract ReadOnlySequence<T> CreateOfSize(int size);
        public abstract ReadOnlySequence<T> CreateWithContent(T[] data);

        internal class ArrayTestSequenceFactory : ReadOnlySequenceFactory<T>
        {
            public override ReadOnlySequence<T> CreateOfSize(int size)
            {
                return new ReadOnlySequence<T>(new T[size + 20], 10, size);
            }

            public override ReadOnlySequence<T> CreateWithContent(T[] data)
            {
                var startSegment = new T[data.Length + 20];
                Array.Copy(data, 0, startSegment, 10, data.Length);
                return new ReadOnlySequence<T>(startSegment, 10, data.Length);
            }
        }

        internal class MemoryTestSequenceFactory : ReadOnlySequenceFactory<T>
        {
            public override ReadOnlySequence<T> CreateOfSize(int size)
            {
                return CreateWithContent(new T[size]);
            }

            public override ReadOnlySequence<T> CreateWithContent(T[] data)
            {
                var startSegment = new T[data.Length + 20];
                Array.Copy(data, 0, startSegment, 10, data.Length);
                return new ReadOnlySequence<T>(new Memory<T>(startSegment, 10, data.Length));
            }
        }

        internal class SingleSegmentTestSequenceFactory : ReadOnlySequenceFactory<T>
        {
            public override ReadOnlySequence<T> CreateOfSize(int size)
            {
                return CreateWithContent(new T[size]);
            }

            public override ReadOnlySequence<T> CreateWithContent(T[] data)
            {
                return CreateSegments(data);
            }
        }

        internal class BytePerSegmentTestSequenceFactory : ReadOnlySequenceFactory<T>
        {
            public override ReadOnlySequence<T> CreateOfSize(int size)
            {
                return CreateWithContent(new T[size]);
            }

            public override ReadOnlySequence<T> CreateWithContent(T[] data)
            {
                var segments = new List<T[]>();

                segments.Add(Array.Empty<T>());
                foreach (var b in data)
                {
                    segments.Add(new[] { b });
                    segments.Add(Array.Empty<T>());
                }

                return CreateSegments(segments.ToArray());
            }
        }

        internal class SplitInThreeSegmentsTestSequenceFactory : ReadOnlySequenceFactory<T>
        {
            public override ReadOnlySequence<T> CreateOfSize(int size)
            {
                return CreateWithContent(new T[size]);
            }

            public override ReadOnlySequence<T> CreateWithContent(T[] data)
            {
                var third = data.Length / 3;

                return CreateSegments(
                    data.AsSpan(0, third).ToArray(),
                    data.AsSpan(third, third).ToArray(),
                    data.AsSpan(2 * third, data.Length - 2 * third).ToArray());
            }
        }

        internal class MemoryManagerTestSequenceFactory : ReadOnlySequenceFactory<T>
        {
            public override ReadOnlySequence<T> CreateOfSize(int size)
            {
#if DEBUG
                return new ReadOnlySequence<T>(new CustomMemoryManager(size + 1).Memory.Slice(1));
#else
                return new ReadOnlySequence<T>(new CustomMemoryManager(size).Memory);
#endif
            }

            public override ReadOnlySequence<T> CreateWithContent(T[] data)
            {
                return new ReadOnlySequence<T>(new CustomMemoryManager(data).Memory);
            }

            private unsafe class CustomMemoryManager : MemoryManager<T>
            {
                private readonly T[] _buffer;

                public CustomMemoryManager(int size) => _buffer = new T[size];

                public CustomMemoryManager(T[] content) => _buffer = content;

                public unsafe override Span<T> GetSpan() => _buffer;

                public override unsafe MemoryHandle Pin(int elementIndex = 0)
                {
                    if ((uint)elementIndex > (uint)_buffer.Length)
                    {
                        throw new ArgumentOutOfRangeException(nameof(elementIndex));
                    }

                    var handle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
                    return new MemoryHandle(Unsafe.Add<T>((void*)handle.AddrOfPinnedObject(), elementIndex), handle, this);
                }

                public override void Unpin() { }

                protected override void Dispose(bool disposing) { }
            }
        }

        public static ReadOnlySequence<T> CreateSegments(params T[][] inputs) => CreateSegments((IEnumerable<T[]>)inputs);

        public static ReadOnlySequence<T> CreateSegments(IEnumerable<T[]> inputs)
        {
            if (inputs == null || inputs.Count() == 0)
            {
                throw new InvalidOperationException();
            }

            BufferSegment<T> last = null;
            BufferSegment<T> first = null;
            foreach(T[] input in inputs)
            {
                int length = input.Length;
                int dataOffset = length / 2;
                var items = new T[length * 2];
                input.CopyTo(items, dataOffset);
                Memory<T> memory = new Memory<T>(items, dataOffset, length);

                if (first == null)
                {
                    first = new BufferSegment<T>(memory);
                    last = first;
                }
                else
                {
                    last = last.Append(memory);
                }
            }

            return new ReadOnlySequence<T>(first, 0, last, last.Memory.Length);
        }
    }
}

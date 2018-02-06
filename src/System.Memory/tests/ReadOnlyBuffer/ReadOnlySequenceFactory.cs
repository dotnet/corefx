// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Collections.Generic;
using System.MemoryTests;
using System.Text;

namespace System.Memory.Tests
{
    public abstract class ReadOnlySequenceFactory
    {
        public static ReadOnlySequenceFactory ArrayFactory { get; } = new ArrayTestSequenceFactory();
        public static ReadOnlySequenceFactory MemoryFactory { get; } = new MemoryTestSequenceFactory();
        public static ReadOnlySequenceFactory OwnedMemoryFactory { get; } = new OwnedMemoryTestSequenceFactory();
        public static ReadOnlySequenceFactory SingleSegmentFactory { get; } = new SingleSegmentTestSequenceFactory();
        public static ReadOnlySequenceFactory SegmentPerByteFactory { get; } = new BytePerSegmentTestSequenceFactory();

        public abstract ReadOnlySequence<byte> CreateOfSize(int size);
        public abstract ReadOnlySequence<byte> CreateWithContent(byte[] data);

        public ReadOnlySequence<byte> CreateWithContent(string data)
        {
            return CreateWithContent(Encoding.ASCII.GetBytes(data));
        }

        internal class ArrayTestSequenceFactory : ReadOnlySequenceFactory
        {
            public override ReadOnlySequence<byte> CreateOfSize(int size)
            {
                return new ReadOnlySequence<byte>(new byte[size + 20], 10, size);
            }

            public override ReadOnlySequence<byte> CreateWithContent(byte[] data)
            {
                var startSegment = new byte[data.Length + 20];
                Array.Copy(data, 0, startSegment, 10, data.Length);
                return new ReadOnlySequence<byte>(startSegment, 10, data.Length);
            }
        }

        internal class MemoryTestSequenceFactory : ReadOnlySequenceFactory
        {
            public override ReadOnlySequence<byte> CreateOfSize(int size)
            {
                return CreateWithContent(new byte[size]);
            }

            public override ReadOnlySequence<byte> CreateWithContent(byte[] data)
            {
                var startSegment = new byte[data.Length + 20];
                Array.Copy(data, 0, startSegment, 10, data.Length);
                return new ReadOnlySequence<byte>(new Memory<byte>(startSegment, 10, data.Length));
            }
        }

        internal class OwnedMemoryTestSequenceFactory : ReadOnlySequenceFactory
        {
            public override ReadOnlySequence<byte> CreateOfSize(int size)
            {
                return CreateWithContent(new byte[size]);
            }

            public override ReadOnlySequence<byte> CreateWithContent(byte[] data)
            {
                var startSegment = new byte[data.Length + 20];
                Array.Copy(data, 0, startSegment, 10, data.Length);
                return new ReadOnlySequence<byte>(new CustomMemoryForTest<byte>(startSegment, 10, data.Length));
            }
        }

        internal class SingleSegmentTestSequenceFactory : ReadOnlySequenceFactory
        {
            public override ReadOnlySequence<byte> CreateOfSize(int size)
            {
                return CreateWithContent(new byte[size]);
            }

            public override ReadOnlySequence<byte> CreateWithContent(byte[] data)
            {
                return CreateSegments(data);
            }
        }

        internal class BytePerSegmentTestSequenceFactory : ReadOnlySequenceFactory
        {
            public override ReadOnlySequence<byte> CreateOfSize(int size)
            {
                return CreateWithContent(new byte[size]);
            }

            public override ReadOnlySequence<byte> CreateWithContent(byte[] data)
            {
                var segments = new List<byte[]>();

                segments.Add(Array.Empty<byte>());
                foreach (var b in data)
                {
                    segments.Add(new[] { b });
                    segments.Add(Array.Empty<byte>());
                }

                return CreateSegments(segments.ToArray());
            }
        }

        public static ReadOnlySequence<byte> CreateSegments(params byte[][] inputs)
        {
            if (inputs == null || inputs.Length == 0)
            {
                throw new InvalidOperationException();
            }

            int i = 0;

            BufferSegment last = null;
            BufferSegment first = null;

            do
            {
                byte[] s = inputs[i];
                int length = s.Length;
                int dataOffset = length;
                var chars = new byte[length * 2];

                for (int j = 0; j < length; j++)
                {
                    chars[dataOffset + j] = s[j];
                }

                // Create a segment that has offset relative to the OwnedMemory and OwnedMemory itself has offset relative to array
                var memory = new Memory<byte>(chars).Slice(length, length);

                if (first == null)
                {
                    first = new BufferSegment(memory);
                    last = first;
                }
                else
                {
                    last = last.Append(memory);
                }
                i++;
            } while (i < inputs.Length);

            return new ReadOnlySequence<byte>(first, 0, last, last.Memory.Length);
        }
    }
}

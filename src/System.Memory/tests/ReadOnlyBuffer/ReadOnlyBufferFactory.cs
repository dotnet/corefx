// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace System.Memory.Tests
{
    public abstract class ReadOnlyBufferFactory
    {
        public static ReadOnlyBufferFactory Array { get; } = new ArrayTestBufferFactory();
        public static ReadOnlyBufferFactory OwnedMemory { get; } = new MemoryTestBufferFactory();
        public static ReadOnlyBufferFactory SingleSegment { get; } = new SingleSegmentTestBufferFactory();
        public static ReadOnlyBufferFactory SegmentPerByte { get; } = new BytePerSegmentTestBufferFactory();

        public abstract ReadOnlyBuffer<byte> CreateOfSize(int size);
        public abstract ReadOnlyBuffer<byte> CreateWithContent(byte[] data);

        public ReadOnlyBuffer<byte> CreateWithContent(string data)
        {
            return CreateWithContent(Encoding.ASCII.GetBytes(data));
        }

        internal class ArrayTestBufferFactory : ReadOnlyBufferFactory
        {
            public override ReadOnlyBuffer<byte> CreateOfSize(int size)
            {
                return new ReadOnlyBuffer<byte>(new byte[size + 20], 10, size);
            }

            public override ReadOnlyBuffer<byte> CreateWithContent(byte[] data)
            {
                var startSegment = new byte[data.Length + 20];
                System.Array.Copy(data, 0, startSegment, 10, data.Length);
                return new ReadOnlyBuffer<byte>(startSegment, 10, data.Length);
            }
        }

        internal class MemoryTestBufferFactory : ReadOnlyBufferFactory
        {
            public override ReadOnlyBuffer<byte> CreateOfSize(int size)
            {
                return CreateWithContent(new byte[size]);
            }

            public override ReadOnlyBuffer<byte> CreateWithContent(byte[] data)
            {
                var startSegment = new byte[data.Length + 20];
                System.Array.Copy(data, 0, startSegment, 10, data.Length);
                return new ReadOnlyBuffer<byte>(new Memory<byte>(startSegment, 10, data.Length));
            }
        }

        internal class SingleSegmentTestBufferFactory: ReadOnlyBufferFactory
        {
            public override ReadOnlyBuffer<byte> CreateOfSize(int size)
            {
                return CreateWithContent(new byte[size]);
            }

            public override ReadOnlyBuffer<byte> CreateWithContent(byte[] data)
            {
                return CreateSegments(data);
            }
        }

        internal class BytePerSegmentTestBufferFactory: ReadOnlyBufferFactory
        {
            public override ReadOnlyBuffer<byte> CreateOfSize(int size)
            {
                return CreateWithContent(new byte[size]);
            }

            public override ReadOnlyBuffer<byte> CreateWithContent(byte[] data)
            {
                var segments = new List<byte[]>();

                segments.Add(System.Array.Empty<byte>());
                foreach (var b in data)
                {
                    segments.Add(new [] { b });
                    segments.Add(System.Array.Empty<byte>());
                }

                return CreateSegments(segments.ToArray());
            }
        }

        public static ReadOnlyBuffer<byte> CreateSegments(params byte[][] inputs)
        {
            if (inputs == null || inputs.Length == 0)
            {
                throw new InvalidOperationException();
            }

            var i = 0;

            BufferSegment last = null;
            BufferSegment first = null;

            do
            {
                var s = inputs[i];
                var length = s.Length;
                var dataOffset = length;
                var chars = new byte[length * 2];

                for (int j = 0; j < length; j++)
                {
                    chars[dataOffset + j] = s[j];
                }

                // Create a segment that has offset relative to the OwnedMemory and OwnedMemory itself has offset relative to array
                var current = new BufferSegment
                {
                    Memory = new Memory<byte>(chars).Slice(length, length)
                };

                if (first == null)
                {
                    first = current;
                    last = current;
                }
                else
                {
                    last.SetNext(current);
                    last = current;
                }
                i++;
            } while (i < inputs.Length);

            return new ReadOnlyBuffer<byte>(first, 0, last, last.Memory.Length);
        }
    }
}

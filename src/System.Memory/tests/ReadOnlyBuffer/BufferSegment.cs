// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;

namespace System.Memory.Tests
{
    internal class BufferSegment : IMemoryList<byte>
    {
        public BufferSegment(Memory<byte> memory)
        {
            Memory = memory;
        }

        public long GetLength(IMemoryList<byte> memoryList)
        {
            var current = this;
            long length = 0;
            while (current != memoryList || current == null)
            {
                length += current.Memory.Length;
                current = current.Next;
            }
            return length;
        }

        public IMemoryList<byte> GetNext(long offset, out int localOffset)
        {
            var current = this;
            while (current != null)
            {
                if (offset < current.Memory.Length)
                {
                    localOffset = (int)offset;
                    return this;
                }

                current = (BufferSegment)current.Next;
            }

            localOffset = 0;
            return null;
        }

        public Memory<byte> Memory { get; set; }

        public IMemoryList<byte> Next { get; private set; }

        public BufferSegment Append(Memory<byte> memory)
        {
            var segment = new BufferSegment(memory);
            Next = segment;
            return segment;
        }
    }
}

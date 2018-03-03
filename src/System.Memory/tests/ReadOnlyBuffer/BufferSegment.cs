// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;

namespace System.Memory.Tests
{
    internal class BufferSegment<T> : ReadOnlySequenceSegment<T>
    {
        public BufferSegment(Memory<T> memory)
        {
            Memory = memory;
        }

        public BufferSegment<T> Append(Memory<T> memory)
        {
            var segment = new BufferSegment<T>(memory)
            {
                RunningIndex = RunningIndex + Memory.Length
            };
            Next = segment;
            return segment;
        }
    }
}

using System.Buffers;
using System.Diagnostics;

namespace System.Memory.Tests
{
    internal class BufferSegment : IMemoryList<byte>
    {
        /// <summary>
        /// Combined length of all segments before this
        /// </summary>
        public long RunningIndex { get; private set; }

        public Memory<byte> Memory { get; set; }

        public IMemoryList<byte> Next { get; private set; }

        public void SetNext(BufferSegment segment)
        {
            Debug.Assert(segment != null);
            Debug.Assert(Next == null);

            Next = segment;

            segment = this;

            while (segment.Next != null)
            {
                var next = (BufferSegment)segment.Next;
                next.RunningIndex = segment.RunningIndex + segment.Memory.Length;
                segment = next;
            }
        }
    }
}

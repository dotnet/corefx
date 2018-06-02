// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Text;
using Xunit;

namespace System.Buffers.Tests
{
    public class BuffersExtensionsTests
    {
        [Fact]
        public void WritingToSingleSegmentBuffer()
        {
            IBufferWriter<byte> bufferWriter = new TestBufferWriterSingleSegment();
            bufferWriter.Write(Encoding.UTF8.GetBytes("Hello"));
            bufferWriter.Write(Encoding.UTF8.GetBytes(" World!"));
            Assert.Equal("Hello World!", bufferWriter.ToString());
        }

        [Fact]
        public void WritingToMultiSegmentBuffer()
        {
            IBufferWriter<byte> bufferWriter = new TestBufferWriterMultiSegment();
            bufferWriter.Write(Encoding.UTF8.GetBytes("Hello"));
            bufferWriter.Write(Encoding.UTF8.GetBytes(" World!"));
            Assert.Equal("Hello World!", bufferWriter.ToString());
        }

        private class TestBufferWriterSingleSegment : IBufferWriter<byte>
        {
            byte[] _buffer = new byte[1000];
            int _written = 0;

            public Span<byte> GetBuffer => _buffer;

            public void Advance(int bytes)
            {
                _written += bytes;
            }

            public Memory<byte> GetMemory(int minimumLength = 0)
            {
                return ((Memory<byte>)_buffer).Slice(_written);
            }

            public Span<byte> GetSpan(int minimumLength)
            {
                return ((Span<byte>)_buffer).Slice(_written);
            }

            public int MaxBufferSize { get; } = Int32.MaxValue;

            public override string ToString()
            {
                return Encoding.UTF8.GetString(_buffer.AsSpan(0, _written));
            }
        }

        private class TestBufferWriterMultiSegment : IBufferWriter<byte>
        {
            private byte[] _current = new byte[0];
            private List<byte[]> _commited = new List<byte[]>();

            public void Advance(int bytes)
            {
                if (bytes == 0)
                    return;
                _commited.Add(_current.AsSpan(0, bytes).ToArray());
                _current = new byte[0];
            }

            public Memory<byte> GetMemory(int minimumLength = 0)
            {
                if (minimumLength == 0)
                    minimumLength = _current.Length + 1;
                if (minimumLength < _current.Length)
                    throw new InvalidOperationException();
                var newBuffer = new byte[minimumLength];
                _current.CopyTo(newBuffer.AsSpan());
                _current = newBuffer;
                return _current;
            }

            public Span<byte> GetSpan(int minimumLength)
            {
                if (minimumLength == 0)
                    minimumLength = _current.Length + 1;
                if (minimumLength < _current.Length)
                    throw new InvalidOperationException();
                var newBuffer = new byte[minimumLength];
                _current.CopyTo(newBuffer.AsSpan());
                _current = newBuffer;
                return _current;
            }

            public int MaxBufferSize { get; } = Int32.MaxValue;

            public override string ToString()
            {
                var builder = new StringBuilder();
                foreach (var buffer in _commited)
                {
                    builder.Append(Encoding.UTF8.GetString(buffer));
                }
                return builder.ToString();
            }
        }
    }

  
}

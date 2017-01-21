// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Reflection.PortableExecutable
{
    /// <summary>
    /// Simple BinaryReader wrapper to:
    ///
    ///  1) throw BadImageFormat instead of EndOfStream or ArgumentOutOfRange.
    ///  2) limit reads to a subset of the base stream.
    ///
    /// Only methods that are needed to read PE headers are implemented.
    /// </summary>
    internal struct PEBinaryReader
    {
        private readonly long _startOffset;
        private readonly long _maxOffset;
        private long _offset;
        private readonly Stream _stream;
        private byte[] _buffer;

        public PEBinaryReader(Stream stream, int size)
        {
            Debug.Assert(size >= 0 && size <= (stream.Length - stream.Position));

            _offset = _startOffset = stream.Position;
            _maxOffset = _startOffset + size;
            _buffer = new byte[8];
            _stream = stream;
        }

        private void FillBuffer(int numBytes)
        {
            int bytesRead = 0;
            int n = 0;

            do
            {
                n = _stream.Read(_buffer, bytesRead, numBytes - bytesRead);
                if (n == 0)
                {
                    Throw.ImageTooSmall();
                }
                bytesRead += n;
            } while (bytesRead < numBytes);
        }

        public int CurrentOffset
        {
            get { return (int)(_stream.Position - _startOffset); }
        }

        public void Seek(int offset)
        {
            CheckBounds(_startOffset, offset);
            _stream.Seek(_startOffset + offset, SeekOrigin.Begin);
        }

        public byte[] ReadBytes(int count)
        {
            CheckBounds(_stream.Position, count);

            if (count < 0)
            {
                Throw.ImageTooSmallOrContainsInvalidOffsetOrCount();
            }

            if (count == 0)
            {
                return Array.Empty<byte>();
            }

            byte[] result = new byte[count];
            int numRead = 0;
            do
            {
                int n = _stream.Read(result, numRead, count);
                if (n == 0)
                {
                    break;
                }

                numRead += n;
                count -= n;
            } while (count > 0);

            if (numRead != result.Length)
            {
                Throw.ImageTooSmallOrContainsInvalidOffsetOrCount();
            }

            return result;
        }

        public Byte ReadByte()
        {
            CheckBounds(sizeof(Byte));
            int b = _stream.ReadByte();
            return checked((byte)b);
        }

        public Int16 ReadInt16()
        {
            CheckBounds(sizeof(Int16));
            FillBuffer(2);
            return (short)(_buffer[0] | _buffer[1] << 8);
        }

        public ushort ReadUInt16()
        {
            CheckBounds(sizeof(ushort));
            FillBuffer(2);
            return (ushort)(_buffer[0] | _buffer[1] << 8);
        }

        public Int32 ReadInt32()
        {
            CheckBounds(sizeof(Int32));
            FillBuffer(4);
            return (int)(_buffer[0] | _buffer[1] << 8 | _buffer[2] << 16 | _buffer[3] << 24);
        }

        public UInt32 ReadUInt32()
        {
            CheckBounds(sizeof(UInt32));
            FillBuffer(4);
            return (uint)(_buffer[0] | _buffer[1] << 8 | _buffer[2] << 16 | _buffer[3] << 24);
        }

        public ulong ReadUInt64()
        {
            CheckBounds(sizeof(UInt64));
            FillBuffer(8);
            uint lo = (uint)(_buffer[0] | _buffer[1] << 8 |
                             _buffer[2] << 16 | _buffer[3] << 24);
            uint hi = (uint)(_buffer[4] | _buffer[5] << 8 |
                             _buffer[6] << 16 | _buffer[7] << 24);
            return ((ulong)hi) << 32 | lo;
        }

        /// <summary>
        /// Reads a fixed-length byte block as a null-padded UTF8-encoded string.
        /// The padding is not included in the returned string.
        /// 
        /// Note that it is legal for UTF8 strings to contain NUL; if NUL occurs
        /// between non-NUL codepoints, it is not considered to be padding and
        /// is included in the result.
        /// </summary>
        public string ReadNullPaddedUTF8(int byteCount)
        {
            byte[] bytes = ReadBytes(byteCount);
            int nonPaddedLength = 0;
            for (int i = bytes.Length; i > 0; --i)
            {
                if (bytes[i - 1] != 0)
                {
                    nonPaddedLength = i;
                    break;
                }
            }
            return Encoding.UTF8.GetString(bytes, 0, nonPaddedLength);
        }

        private void CheckBounds(uint count)
        {
            Debug.Assert(count <= sizeof(Int64));  // Error message assumes we're trying to read constant small number of bytes.
            Debug.Assert(_stream.Position >= 0 && _maxOffset >= 0);

            // Add cannot overflow because the worst case is (ulong)long.MaxValue + uint.MaxValue < ulong.MaxValue.
            if ((ulong)_stream.Position + count > (ulong)_maxOffset)
            {
                Throw.ImageTooSmall();
            }
        }

        private void CheckBounds(long startPosition, int count)
        {
            Debug.Assert(startPosition >= 0 && _maxOffset >= 0);

            // Add cannot overflow because the worst case is (ulong)long.MaxValue + uint.MaxValue < ulong.MaxValue.
            // Negative count is handled by overflow to greater than maximum size = int.MaxValue.
            if ((ulong)startPosition + unchecked((uint)count) > (ulong)_maxOffset)
            {
                Throw.ImageTooSmallOrContainsInvalidOffsetOrCount();
            }
        }
    }
}

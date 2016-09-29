// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.IO.Compression
{
    internal class OutputBuffer
    {
        private byte[] _byteBuffer;  // buffer for storing bytes
        private int _pos;            // position
        private uint _bitBuf;        // store uncomplete bits 
        private int _bitCount;       // number of bits in bitBuffer 

        // set the output buffer we will be using
        internal void UpdateBuffer(byte[] output)
        {
            _byteBuffer = output;
            _pos = 0;
        }

        internal int BytesWritten
        {
            get
            {
                return _pos;
            }
        }

        internal int FreeBytes
        {
            get
            {
                return _byteBuffer.Length - _pos;
            }
        }

        internal void WriteUInt16(ushort value)
        {
            Debug.Assert(FreeBytes >= 2, "No enough space in output buffer!");

            _byteBuffer[_pos++] = (byte)value;
            _byteBuffer[_pos++] = (byte)(value >> 8);
        }

        internal void WriteBits(int n, uint bits)
        {
            Debug.Assert(n <= 16, "length must be larger than 16!");
            _bitBuf |= bits << _bitCount;
            _bitCount += n;
            if (_bitCount >= 16)
            {
                Debug.Assert(_byteBuffer.Length - _pos >= 2, "No enough space in output buffer!");
                _byteBuffer[_pos++] = unchecked((byte)_bitBuf);
                _byteBuffer[_pos++] = unchecked((byte)(_bitBuf >> 8));
                _bitCount -= 16;
                _bitBuf >>= 16;
            }
        }

        // write the bits left in the output as bytes. 
        internal void FlushBits()
        {
            // flush bits from bit buffer to output buffer
            while (_bitCount >= 8)
            {
                _byteBuffer[_pos++] = unchecked((byte)_bitBuf);
                _bitCount -= 8;
                _bitBuf >>= 8;
            }

            if (_bitCount > 0)
            {
                _byteBuffer[_pos++] = unchecked((byte)_bitBuf);
                _bitBuf = 0;
                _bitCount = 0;
            }
        }

        internal void WriteBytes(byte[] byteArray, int offset, int count)
        {
            Debug.Assert(FreeBytes >= count, "Not enough space in output buffer!");
            // faster 
            if (_bitCount == 0)
            {
                Array.Copy(byteArray, offset, _byteBuffer, _pos, count);
                _pos += count;
            }
            else
            {
                WriteBytesUnaligned(byteArray, offset, count);
            }
        }

        private void WriteBytesUnaligned(byte[] byteArray, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                byte b = byteArray[offset + i];
                WriteByteUnaligned(b);
            }
        }

        private void WriteByteUnaligned(byte b)
        {
            WriteBits(8, b);
        }

        internal int BitsInBuffer
        {
            get
            {
                return (_bitCount / 8) + 1;
            }
        }

        internal OutputBuffer.BufferState DumpState()
        {
            OutputBuffer.BufferState savedState;
            savedState.pos = _pos;
            savedState.bitBuf = _bitBuf;
            savedState.bitCount = _bitCount;
            return savedState;
        }

        internal void RestoreState(OutputBuffer.BufferState state)
        {
            _pos = state.pos;
            _bitBuf = state.bitBuf;
            _bitCount = state.bitCount;
        }

        internal struct BufferState
        {
            internal int pos;            // position
            internal uint bitBuf;        // store uncomplete bits 
            internal int bitCount;       // number of bits in bitBuffer 
        }
    }
}

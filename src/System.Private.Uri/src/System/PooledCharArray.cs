// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;

namespace System
{
    internal struct PooledCharArray
    {
        private char[] _buffer;

        public PooledCharArray(int size)
        {
            _buffer = ArrayPool<char>.Shared.Rent(size);
        }

        public PooledCharArray(char[] fromArray)
        {
            _buffer = ArrayPool<char>.Shared.Rent(fromArray.Length);
            for (int i = 0; i < fromArray.Length; i++)
            {
                _buffer[i] = fromArray[i];
            }
        }

        public void Release()
        {
            if (_buffer != null)
            {
                ArrayPool<char>.Shared.Return(_buffer);
#nullable disable warnings
                _buffer = null;
#nullable restore warnings
            }
        }

        public int Length => _buffer.Length;

        public char[] Array => _buffer;

        public string GetStringAndRelease(int stringLength)
        {
            string ret = new string(_buffer, 0, stringLength);
            Release();
            return ret;
        }

        public void CopyAndRelease(char[] dest, int offset, int count)
        {
            while (count > 0)
            {
                dest[offset] = _buffer[offset++];
                count--;
            }
            Release();
        }

        public char this[int index]
        {
            get
            {
                return _buffer[index];
            }
            set
            {
                _buffer[index] = value;
            }
        }

        public unsafe bool IsSameString(char* ptrStr, int start, int end)
        {
            while (start <= end)
            {
                if (ptrStr[start] != _buffer[start])
                {
                    return false;
                }
                start++;
            }
            return true;
        }

        public void GrowAndCopy(int extraSpace)
        {
            char[] oldBuffer = _buffer;
            char[] newBuffer = ArrayPool<char>.Shared.Rent(_buffer.Length + extraSpace);
            Buffer.BlockCopy(oldBuffer, 0, newBuffer, 0, _buffer.Length);
            _buffer = newBuffer;
            ArrayPool<char>.Shared.Return(oldBuffer);
        }
    }
}

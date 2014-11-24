// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Xml
{
    internal abstract partial class Base64Encoder
    {
        private byte[] _leftOverBytes;
        private int _leftOverBytesCount;
        private char[] _charsLine;

        internal const int Base64LineSize = 76;
        internal const int LineSizeInBytes = Base64LineSize / 4 * 3;

        internal Base64Encoder()
        {
            _charsLine = new char[Base64LineSize];
        }

        internal abstract void WriteChars(char[] chars, int index, int count);

        internal void Encode(byte[] buffer, int index, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            if (count > buffer.Length - index)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            // encode left-over buffer
            if (_leftOverBytesCount > 0)
            {
                int i = _leftOverBytesCount;
                while (i < 3 && count > 0)
                {
                    _leftOverBytes[i++] = buffer[index++];
                    count--;
                }

                // the total number of buffer we have is less than 3 -> return
                if (count == 0 && i < 3)
                {
                    _leftOverBytesCount = i;
                    return;
                }

                // encode the left-over buffer and write out
                int leftOverChars = Convert.ToBase64CharArray(_leftOverBytes, 0, 3, _charsLine, 0);
                WriteChars(_charsLine, 0, leftOverChars);
            }

            // store new left-over buffer
            _leftOverBytesCount = count % 3;
            if (_leftOverBytesCount > 0)
            {
                count -= _leftOverBytesCount;
                if (_leftOverBytes == null)
                {
                    _leftOverBytes = new byte[3];
                }
                for (int i = 0; i < _leftOverBytesCount; i++)
                {
                    _leftOverBytes[i] = buffer[index + count + i];
                }
            }

            // encode buffer in 76 character long chunks
            int endIndex = index + count;
            int chunkSize = LineSizeInBytes;
            while (index < endIndex)
            {
                if (index + chunkSize > endIndex)
                {
                    chunkSize = endIndex - index;
                }
                int charCount = Convert.ToBase64CharArray(buffer, index, chunkSize, _charsLine, 0);
                WriteChars(_charsLine, 0, charCount);

                index += chunkSize;
            }
        }

        internal void Flush()
        {
            if (_leftOverBytesCount > 0)
            {
                int leftOverChars = Convert.ToBase64CharArray(_leftOverBytes, 0, _leftOverBytesCount, _charsLine, 0);
                WriteChars(_charsLine, 0, leftOverChars);
                _leftOverBytesCount = 0;
            }
        }
    }
}

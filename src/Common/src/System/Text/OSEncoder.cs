// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Diagnostics.Contracts;

namespace System.Text
{
    internal class OSEncoder : Encoder
    {
        private char _charLeftOver;
        private Encoding _encoding;

        private const char NULL_CHAR = (char)0;

        internal OSEncoder(Encoding encoding)
        {
            _encoding = encoding;
            Reset();
        }

        public override void Reset()
        {
            _charLeftOver = NULL_CHAR;
        }

        public override unsafe int GetByteCount(char[] chars, int index, int count, bool flush)
        {
            if (chars == null)
                throw new ArgumentNullException(nameof(chars), SR.ArgumentNull_Array);

            if (index < 0 || count < 0)
                throw new ArgumentOutOfRangeException(index < 0 ? nameof(index) : nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);

            if (chars.Length - index < count)
                throw new ArgumentOutOfRangeException(nameof(chars), SR.ArgumentOutOfRange_IndexCountBuffer);
            
            if (chars.Length == 0 && (_charLeftOver == NULL_CHAR || !flush))
                return 0;
            
            fixed (char *pChar = chars)
            {
                char *pSingleElementArray = stackalloc char[1]; // to avoid passing null to GetByteCount
                char *pBuffer = pChar == null ?  pSingleElementArray : pChar + index;
                return GetByteCount(pBuffer, count, flush);
            }
        }

        public unsafe override int GetByteCount(char* chars, int count, bool flush)
        {
            if (chars == null)
                throw new ArgumentNullException(nameof(chars), SR.ArgumentNull_Array);

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);
            
            bool excludeLastChar = count > 0 && !flush && Char.IsHighSurrogate(chars[count - 1]);

            if (excludeLastChar)
                count--;

            if (_charLeftOver == NULL_CHAR)
            {
                if (count <= 0)
                    return 0;

                return OSEncoding.WideCharToMultiByte(_encoding.CodePage, chars, count, null, 0);
            }

            // we have left over character
            if (count == 0 && !excludeLastChar && !flush)
                return 0;

            char [] bufferToProcess = new char[count+1];
            bufferToProcess[0] = _charLeftOver;
            fixed (char *pBuffer = bufferToProcess)
            {
                Buffer.MemoryCopy(chars, pBuffer+1, count * sizeof(char), count * sizeof(char));
                return OSEncoding.WideCharToMultiByte(_encoding.CodePage, pBuffer, bufferToProcess.Length, null, 0);
            }
        }

        public override unsafe int GetBytes(char[] chars, int charIndex, int charCount,
                                              byte[] bytes, int byteIndex, bool flush)
        {
            if (chars == null || bytes == null)
                throw new ArgumentNullException(chars == null ? nameof(chars) : nameof(bytes), SR.ArgumentNull_Array);

            if (charIndex < 0 || charCount < 0)
                throw new ArgumentOutOfRangeException(charIndex < 0 ? nameof(charIndex) : nameof(charCount), SR.ArgumentOutOfRange_NeedNonNegNum);

            if (chars.Length - charIndex < charCount)
                throw new ArgumentOutOfRangeException(nameof(chars), SR.ArgumentOutOfRange_IndexCountBuffer);

            if (byteIndex < 0 || byteIndex > bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(byteIndex), SR.ArgumentOutOfRange_Index);
            
            if (bytes.Length == 0)
                return 0;

            if (charCount == 0 && (_charLeftOver == NULL_CHAR || !flush))
                return 0;
            
            fixed (char* pChars = chars)
            fixed (byte* pBytes = bytes)
            {
                char *pSingleElementArray = stackalloc char[1]; // to avoid passing null to GetByteCount
                char *pBuffer = pChars == null ?  pSingleElementArray : pChars + charIndex;

                return GetBytes(pBuffer, charCount, pBytes + byteIndex, bytes.Length - byteIndex, flush);
            }
        }

        public unsafe override int GetBytes(char* chars, int charCount, byte* bytes, int byteCount, bool flush)
        {
            if (chars == null || bytes == null)
                throw new ArgumentNullException(chars == null ? nameof(chars) : nameof(bytes), SR.ArgumentNull_Array);

            if (byteCount < 0 || charCount < 0)
                throw new ArgumentOutOfRangeException(byteCount < 0 ? nameof(byteCount) : nameof(charCount), SR.ArgumentOutOfRange_NeedNonNegNum);

            if (byteCount == 0)
                return 0;

            char lastChar = charCount > 0 && !flush && Char.IsHighSurrogate(chars[charCount - 1]) ? chars[charCount - 1] : NULL_CHAR;

            if (lastChar != NULL_CHAR)
                charCount--;

            if (_charLeftOver == NULL_CHAR)
            {
                if (charCount <= 0)
                    return 0;

                int result =  OSEncoding.WideCharToMultiByte(_encoding.CodePage, chars, charCount, bytes, byteCount);
                _charLeftOver = lastChar;
                return result;
            }

            // we have left over character
            if (charCount == 0 && lastChar == NULL_CHAR && !flush)
                return 0;

            char [] bufferToProcess = new char[charCount+1];
            bufferToProcess[0] = _charLeftOver;
            fixed (char *pBuffer = bufferToProcess)
            {
                Buffer.MemoryCopy(chars, pBuffer+1, charCount * sizeof(char), charCount * sizeof(char));
                int result =  OSEncoding.WideCharToMultiByte(_encoding.CodePage, pBuffer, bufferToProcess.Length, bytes, byteCount);
                _charLeftOver = lastChar;
                return result;
            }
        }

        public override unsafe void Convert(char[] chars, int charIndex, int charCount,
                                              byte[] bytes, int byteIndex, int byteCount, bool flush,
                                              out int charsUsed, out int bytesUsed, out bool completed)
        {
            if (chars == null || bytes == null)
                throw new ArgumentNullException(chars == null ? nameof(chars) : nameof(bytes), SR.ArgumentNull_Array);

            if (charIndex < 0 || charCount < 0)
                throw new ArgumentOutOfRangeException(charIndex < 0 ? nameof(charIndex) : nameof(charCount), SR.ArgumentOutOfRange_NeedNonNegNum);

            if (byteIndex < 0 || byteCount < 0)
                throw new ArgumentOutOfRangeException(byteIndex < 0 ? nameof(byteIndex) : nameof(byteCount), SR.ArgumentOutOfRange_NeedNonNegNum);

            if (chars.Length - charIndex < charCount)
                throw new ArgumentOutOfRangeException(nameof(chars), SR.ArgumentOutOfRange_IndexCountBuffer);

            if (bytes.Length - byteIndex < byteCount)
                throw new ArgumentOutOfRangeException(nameof(bytes), SR.ArgumentOutOfRange_IndexCountBuffer);
            
            if (bytes.Length == 0 || (chars.Length == 0 && (_charLeftOver == NULL_CHAR || !flush)))
            {
                bytesUsed = 0;
                charsUsed = 0;
                completed = false;
                return;
            }

            fixed (char* pChars = chars)
            fixed (byte* pBytes = bytes)
            {
                char *pSingleElementArray = stackalloc char[1]; // to avoid passing null to Convert
                char *pBuffer = pChars == null ?  pSingleElementArray : pChars + charIndex;
                
                Convert(pBuffer, charCount, pBytes + byteIndex, byteCount, flush, out charsUsed, out bytesUsed, out completed);
            }
        }

        public override unsafe void Convert(char* chars, int charCount,
                                              byte* bytes, int byteCount, bool flush,
                                              out int charsUsed, out int bytesUsed, out bool completed)
        {
            if (bytes == null || chars == null)
                throw new ArgumentNullException(bytes == null ? nameof(bytes) : nameof(chars), SR.ArgumentNull_Array);
            if (charCount < 0 || byteCount < 0)
                throw new ArgumentOutOfRangeException(charCount < 0 ? nameof(charCount) : nameof(byteCount), SR.ArgumentOutOfRange_NeedNonNegNum);
            
            if (_charLeftOver == NULL_CHAR)
            {
                ConvertWorker(chars, charCount, bytes, byteCount, flush, out charsUsed, out bytesUsed, out completed);
                return;
            }

            // we have left over character
            char [] bufferToProcess = new char[charCount+1];
            bufferToProcess[0] = _charLeftOver;

            fixed (char *pBuffer = bufferToProcess)
            {
                Buffer.MemoryCopy(chars, pBuffer+1, charCount * sizeof(char), charCount * sizeof(char));
                ConvertWorker(pBuffer, bufferToProcess.Length, bytes, byteCount, flush, out charsUsed, out bytesUsed, out completed);
            }
        }

        private unsafe void ConvertWorker(char* chars, int charCount,
                                    byte* bytes, int byteCount, bool flush,
                                    out int charsUsed, out int bytesUsed, out bool completed)
        {
            char originalCharLeftOver = _charLeftOver;
            
            // clear the state to avoid allocating any more buffers
            _charLeftOver = NULL_CHAR;

            int count = charCount;
            while (count > 0)
            {
                int returnedByteCount = GetByteCount(chars, count, flush);
                if (returnedByteCount <= byteCount)
                    break;
                
                count /= 2;
            }

            if (count > 0)
            {
                // note GetBytes can change the _charLeftOver state
                bytesUsed = GetBytes(chars, count, bytes, byteCount, flush);
                charsUsed = originalCharLeftOver == NULL_CHAR ? count : count - 1;
                completed = _charLeftOver == NULL_CHAR && charCount == count;
                return;
            }

            _charLeftOver = originalCharLeftOver; // reset the state in case of failures
            bytesUsed = 0;
            charsUsed = 0;
            completed = false;
        }

        public Encoding Encoding
        {
            get
            {
                return _encoding;
            }
        }
    }
}

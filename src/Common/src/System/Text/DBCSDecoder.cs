// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Diagnostics;

namespace System.Text
{
    internal sealed class DecoderDBCS : Decoder
    {
        private readonly Encoding _encoding;
        private readonly byte[] _leadByteRanges = new byte[10]; // Max 5 ranges
        private int _rangesCount;
        private byte _leftOverLeadByte;
        
        internal DecoderDBCS(Encoding encoding)
        {
            _encoding = encoding;
            _rangesCount = Interop.Kernel32.GetLeadByteRanges(_encoding.CodePage, _leadByteRanges);
            Reset();
        }

        private bool IsLeadByte(byte b)
        {
            if (b < _leadByteRanges[0])
                return false;
            int i = 0;
            while (i < _rangesCount)
            {
                if (b >= _leadByteRanges[i] && b <= _leadByteRanges[i + 1])
                    return true;
                i += 2;
            }
            return false;
        }

        public override void Reset()
        {
            _leftOverLeadByte = 0;
        }

        public override unsafe int GetCharCount(byte[] bytes, int index, int count)
        {
            return GetCharCount(bytes, index, count, false);
        }

        public override unsafe int GetCharCount(byte[] bytes, int index, int count, bool flush)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes), SR.ArgumentNull_Array);

            if (index < 0 || count < 0)
                throw new ArgumentOutOfRangeException(index < 0 ? nameof(index) : nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);

            if (bytes.Length - index < count)
                throw new ArgumentOutOfRangeException(nameof(bytes), SR.ArgumentOutOfRange_IndexCountBuffer);
            
            if (count == 0 && (_leftOverLeadByte == 0 || !flush))
                return 0;

            fixed (byte* pBytes = bytes)
            {
                byte dummyByte;
                byte* pBuffer = pBytes == null ? &dummyByte : pBytes + index;
                
                return GetCharCount(pBuffer, count, flush);
            }
        }

        private unsafe int ConvertWithLeftOverByte(byte* bytes, int count, char* chars, int charCount)
        {
            Debug.Assert(_leftOverLeadByte != 0);
            byte* pTempBuffer = stackalloc byte[2];
            pTempBuffer[0] = _leftOverLeadByte;

            int index = 0;

            if (count > 0)
            {
                pTempBuffer[1] = bytes[0];
                index++;
            }

            int result = OSEncoding.MultiByteToWideChar(_encoding.CodePage, pTempBuffer, index+1, chars, charCount);

            if (count - index > 0)
                result += OSEncoding.MultiByteToWideChar(
                                        _encoding.CodePage, bytes + index, 
                                        count - index,
                                        chars == null ? null : chars + result, 
                                        chars == null ? 0 : charCount - result);

            return result;
        }

        public unsafe override int GetCharCount(byte* bytes, int count, bool flush)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes), SR.ArgumentNull_Array);

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);

            bool excludeLastByte = count > 0 && !flush && IsLastByteALeadByte(bytes, count);

            if (excludeLastByte)
                count--;

            if (_leftOverLeadByte == 0)
            {
                if (count <= 0)
                    return 0;

                return OSEncoding.MultiByteToWideChar(_encoding.CodePage, bytes, count, null, 0);
            }

            if (count == 0 && !excludeLastByte && !flush)
                return 0;

            return ConvertWithLeftOverByte(bytes, count, null, 0);
        }

        public override unsafe int GetChars(byte[] bytes, int byteIndex, int byteCount,
                                             char[] chars, int charIndex)
        {
            return GetChars(bytes, byteIndex, byteCount, chars, charIndex, false);
        }

        public override unsafe int GetChars(byte[] bytes, int byteIndex, int byteCount,
                                             char[] chars, int charIndex, bool flush)
        {
            if (bytes == null || chars == null)
                throw new ArgumentNullException(bytes == null ? nameof(bytes) : nameof(chars), SR.ArgumentNull_Array);

            if (byteIndex < 0 || byteCount < 0)
                throw new ArgumentOutOfRangeException(byteIndex < 0 ? nameof(byteIndex) : nameof(byteCount), SR.ArgumentOutOfRange_NeedNonNegNum);

            if (bytes.Length - byteIndex < byteCount)
                throw new ArgumentOutOfRangeException(nameof(bytes), SR.ArgumentOutOfRange_IndexCountBuffer);

            if (charIndex < 0 || charIndex > chars.Length)
                throw new ArgumentOutOfRangeException(nameof(charIndex), SR.ArgumentOutOfRange_Index);

            if (chars.Length == 0)
                return 0;

            if (byteCount == 0 && (_leftOverLeadByte == 0 || !flush))
                return 0;
            
            fixed (char* pChars = &chars[0])
            fixed (byte* pBytes = bytes)
            {
                byte dummyByte;
                byte* pBuffer = pBytes == null ? &dummyByte : pBytes + byteIndex;

                return GetChars(pBuffer, byteCount, pChars + charIndex, chars.Length - charIndex, flush);
            }
        }

        public unsafe override int GetChars(byte* bytes, int byteCount, char* chars, int charCount, bool flush)
        {
            if (chars == null || bytes == null)
                throw new ArgumentNullException(chars == null ? nameof(chars) : nameof(bytes), SR.ArgumentNull_Array);

            if (byteCount < 0 || charCount < 0)
                throw new ArgumentOutOfRangeException(byteCount < 0 ? nameof(byteCount) : nameof(charCount), SR.ArgumentOutOfRange_NeedNonNegNum);

            if (charCount == 0)
                return 0;

            byte lastByte = byteCount > 0 && !flush && IsLastByteALeadByte(bytes, byteCount) ? bytes[byteCount - 1] : (byte) 0;

            if (lastByte != 0)
                byteCount--;

            if (_leftOverLeadByte == 0)
            {
                if (byteCount <= 0)
                {
                    _leftOverLeadByte = lastByte;
                    return 0;
                }

                int result =  OSEncoding.MultiByteToWideChar(_encoding.CodePage, bytes, byteCount, chars, charCount);
                _leftOverLeadByte = lastByte;
                return result;
            }

            // we have left over lead byte
            if (byteCount == 0 && lastByte == 0 && !flush)
                return 0;

            int res = ConvertWithLeftOverByte(bytes, byteCount, chars, charCount);
            _leftOverLeadByte = lastByte;
            return res;
        }

        public override unsafe void Convert(byte[] bytes, int byteIndex, int byteCount,
                                              char[] chars, int charIndex, int charCount, bool flush,
                                              out int bytesUsed, out int charsUsed, out bool completed)
        {
            if (bytes == null || chars == null)
                throw new ArgumentNullException(bytes == null ? nameof(bytes) : nameof(chars), SR.ArgumentNull_Array);

            if (byteIndex < 0 || byteCount < 0)
                throw new ArgumentOutOfRangeException(byteIndex < 0 ? nameof(byteIndex) : nameof(byteCount), SR.ArgumentOutOfRange_NeedNonNegNum);

            if (charIndex < 0 || charCount < 0)
                throw new ArgumentOutOfRangeException(charIndex < 0 ? nameof(charIndex) : nameof(charCount), SR.ArgumentOutOfRange_NeedNonNegNum);

            if (bytes.Length - byteIndex < byteCount)
                throw new ArgumentOutOfRangeException(nameof(bytes), SR.ArgumentOutOfRange_IndexCountBuffer);

            if (chars.Length - charIndex < charCount)
                throw new ArgumentOutOfRangeException(nameof(chars), SR.ArgumentOutOfRange_IndexCountBuffer);

            if (charCount == 0 || (bytes.Length == 0 && (_leftOverLeadByte == 0 || !flush)))
            {
                bytesUsed = 0;
                charsUsed = 0;
                completed = false;
                return;
            }

            fixed (char* pChars = &chars[0])
            fixed (byte* pBytes = bytes)
            {
                byte dummyByte;
                byte* pBuffer = pBytes == null ? &dummyByte : pBytes + byteIndex;
                
                Convert(pBuffer, byteCount, pChars + charIndex, charCount, flush, out bytesUsed, out charsUsed, out completed);
            }
        }

        public unsafe override void Convert(byte* bytes, int byteCount,
                                              char* chars, int charCount, bool flush,
                                              out int bytesUsed, out int charsUsed, out bool completed)
        {
            if (chars == null || bytes == null)
                throw new ArgumentNullException(chars == null ? nameof(chars) : nameof(bytes), SR.ArgumentNull_Array);
            if (byteCount < 0 || charCount < 0)
                throw new ArgumentOutOfRangeException(byteCount < 0 ? nameof(byteCount) : nameof(charCount), SR.ArgumentOutOfRange_NeedNonNegNum);

            int count = byteCount;
            while (count > 0)
            {
                int returnedCharCount = GetCharCount(bytes, count, flush);
                if (returnedCharCount <= charCount)
                    break;
                
                count /= 2;
            }

            if (count > 0)
            {
                // note GetChars can change the _leftOverLeadByte state
                charsUsed = GetChars(bytes, count, chars, charCount, flush);
                bytesUsed = count;
                completed = _leftOverLeadByte == 0 && byteCount == count;
                return;
            }

            bytesUsed = 0;
            charsUsed = 0;
            completed = false;
        }

        // not IsLastByteALeadByte depends on the _leftOverLeadByte state
        private unsafe bool IsLastByteALeadByte(byte* bytes, int count)
        {
            if (!IsLeadByte(bytes[count - 1]))
                return false; // no need to process the buffer
            
            int index = 0;
            if (_leftOverLeadByte != 0)
                index++; // trail byte
            
            while (index < count)
            {
                if (IsLeadByte(bytes[index]))
                {
                    index++;
                    if (index >= count)
                        return true;
                }
                index++;
            }
            return false;
        }
    }
}

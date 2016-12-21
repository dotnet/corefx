// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace System.Text
{
    internal class OSEncoding : Encoding
    {
        private int _codePage;
        private string _encodingName;
        private string _webName;

        internal OSEncoding(int codePage) : base(codePage)
        {
            _codePage = codePage;
        }

        public override unsafe int GetByteCount(char[] chars, int index, int count)
        {
            if (chars == null)
                throw new ArgumentNullException(nameof(chars), SR.ArgumentNull_Array);

            if (index < 0 || count < 0)
                throw new ArgumentOutOfRangeException(index < 0 ? nameof(index) : nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);

            if (chars.Length - index < count)
                throw new ArgumentOutOfRangeException(nameof(chars), SR.ArgumentOutOfRange_IndexCountBuffer);

            if (count == 0)
                return 0;

            fixed (char* pChar = chars)
            {
                return WideCharToMultiByte(pChar+index, count, null, 0);
            }
        }

        public override unsafe int GetByteCount(String s)
        {
            // Validate input
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            if (s.Length == 0)
                return 0;

            fixed (char* pChars = s)
            {
                return WideCharToMultiByte(pChars, s.Length, null, 0);
            }
        }

        public override unsafe int GetBytes(String s, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            if (s == null || bytes == null)
                throw new ArgumentNullException(s == null ? nameof(s) : nameof(bytes), SR.ArgumentNull_Array);

            if (charIndex < 0 || charCount < 0)
                throw new ArgumentOutOfRangeException(charIndex < 0 ? nameof(charIndex) : nameof(charCount), SR.ArgumentOutOfRange_NeedNonNegNum);

            if (s.Length - charIndex < charCount)
                throw new ArgumentOutOfRangeException(nameof(s), SR.ArgumentOutOfRange_IndexCount);

            if (byteIndex < 0 || byteIndex > bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(byteIndex), SR.ArgumentOutOfRange_Index);

            if (charCount == 0)
                return 0;

            if (bytes.Length == 0)
            {
                throw new ArgumentOutOfRangeException(SR.Argument_EncodingConversionOverflowBytes);
            }
            
            fixed (char* pChars = s)
            fixed (byte *pBytes = bytes)
            {
                return WideCharToMultiByte(pChars+charIndex, charCount, pBytes+byteIndex, bytes.Length - byteIndex);
            }
        }

        public override unsafe int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            if (chars == null || bytes == null)
                throw new ArgumentNullException(chars == null ? nameof(chars) : nameof(bytes), SR.ArgumentNull_Array);

            if (charIndex < 0 || charCount < 0)
                throw new ArgumentOutOfRangeException(charIndex < 0 ? nameof(charIndex) : nameof(charCount), SR.ArgumentOutOfRange_NeedNonNegNum);

            if (chars.Length - charIndex < charCount)
                throw new ArgumentOutOfRangeException(nameof(chars), SR.ArgumentOutOfRange_IndexCountBuffer);

            if (byteIndex < 0 || byteIndex > bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(byteIndex), SR.ArgumentOutOfRange_Index);

            if (charCount == 0)
                return 0;

            if (bytes.Length == 0)
            {
                throw new ArgumentOutOfRangeException(SR.Argument_EncodingConversionOverflowBytes);
            }
            
            fixed (char* pChars = chars)
            fixed (byte *pBytes = bytes)
            {
                return WideCharToMultiByte(pChars+charIndex, charCount, pBytes+byteIndex, bytes.Length - byteIndex);
            }
        }

        public override unsafe int GetCharCount(byte[] bytes, int index, int count)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes), SR.ArgumentNull_Array);

            if (index < 0 || count < 0)
                throw new ArgumentOutOfRangeException(index < 0 ? nameof(index) : nameof(count), SR.ArgumentOutOfRange_NeedNonNegNum);

            if (bytes.Length - index < count)
                throw new ArgumentOutOfRangeException(nameof(bytes), SR.ArgumentOutOfRange_IndexCountBuffer);

            if (count == 0)
                return 0;

            fixed (byte* pBytes = bytes)
            {
                return MultiByteToWideChar(pBytes+index, count, null, 0);
            }
        }

        public override unsafe int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            if (bytes == null || chars == null)
                throw new ArgumentNullException(bytes == null ? nameof(bytes) : nameof(chars), SR.ArgumentNull_Array);

            if (byteIndex < 0 || byteCount < 0)
                throw new ArgumentOutOfRangeException(byteIndex < 0 ? nameof(byteIndex) : nameof(byteCount), SR.ArgumentOutOfRange_NeedNonNegNum);

            if (bytes.Length - byteIndex < byteCount)
                throw new ArgumentOutOfRangeException(nameof(bytes), SR.ArgumentOutOfRange_IndexCountBuffer);

            if (charIndex < 0 || charIndex > chars.Length)
                throw new ArgumentOutOfRangeException(nameof(charIndex), SR.ArgumentOutOfRange_Index);

            if (byteCount == 0)
                return 0;

            if (chars.Length == 0)
                throw new ArgumentOutOfRangeException(SR.Argument_EncodingConversionOverflowChars);

            fixed (byte* pBytes = bytes)
            fixed (char* pChars = chars)
            {
                return MultiByteToWideChar(pBytes+byteIndex, byteCount, pChars+charIndex, chars.Length - charIndex);
            }
        }

        public override int GetMaxByteCount(int charCount)
        {
            if (charCount < 0)
                throw new ArgumentOutOfRangeException(nameof(charCount), SR.ArgumentOutOfRange_NeedNonNegNum);

            long byteCount = (long)charCount * 14; // Max possible value for all encodings
            if (byteCount > 0x7fffffff)
                throw new ArgumentOutOfRangeException(nameof(charCount), SR.ArgumentOutOfRange_GetByteCountOverflow);

            return (int)byteCount;
        }

        public override int GetMaxCharCount(int byteCount)
        {
            if (byteCount < 0)
                throw new ArgumentOutOfRangeException(nameof(byteCount), SR.ArgumentOutOfRange_NeedNonNegNum);

            long charCount = byteCount * 4; // Max possible value for all encodings

            if (charCount > 0x7fffffff)
                throw new ArgumentOutOfRangeException(nameof(byteCount), SR.ArgumentOutOfRange_GetCharCountOverflow);

            return (int)charCount;
        }

        public override String EncodingName
        {
            get
            {
                if (_encodingName == null)
                {
                    _encodingName = EncodingTable.GetEnglishNameFromCodePage(_codePage);
                }
                return _encodingName;
            }
        }

        public override String WebName
        {
            get
            {
                if (_webName == null)
                {
                    _webName = EncodingTable.GetWebNameFromCodePage(_codePage);
                }
                return _webName;
            }
        }

        private unsafe int WideCharToMultiByte(char* pChars, int count, byte* pBytes, int byteCount)
        {
            int result = Interop.Kernel32.WideCharToMultiByte((uint)_codePage, 0, pChars, count, pBytes, byteCount, IntPtr.Zero, IntPtr.Zero);
            if (result <= 0)
                throw new ArgumentException(SR.Argument_InvalidCharSequenceNoIndex);
            return result;
        }

        private unsafe int MultiByteToWideChar(byte* pBytes, int byteCount, char* pChars, int count)
        {
            int result = Interop.Kernel32.MultiByteToWideChar((uint)_codePage, 0, pBytes, byteCount, pChars, count);
            if (result <= 0)
                throw new ArgumentException(SR.Argument_InvalidCharSequenceNoIndex);
            return result;
        }
    }
}